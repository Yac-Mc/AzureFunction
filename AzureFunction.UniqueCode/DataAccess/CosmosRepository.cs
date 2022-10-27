using AutoMapper;
using AzureFunction.UniqueCode.Domain;
using AzureFunction.UniqueCode.Entities.Queries;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.DataAccess
{
    public class CosmosRepository<T> : ICosmosRepository<T> where T : class
    {
        private readonly IMapper _mapper;
        private readonly ICommandText _commandText;
        private static readonly string _endpointUrl = Environment.GetEnvironmentVariable("CosmosDBEndPointUrl");
        private static readonly string _authorizationKey = Environment.GetEnvironmentVariable("CosmosDBAuthorizationKey");
        private static readonly string _databaseName = Environment.GetEnvironmentVariable("CosmosDataBase");
        private readonly int _cosmosDBMaxRegister = Convert.ToInt16(Environment.GetEnvironmentVariable("CosmosDBMaxRegister"));
        private readonly string _document;
        private string _partitionKeyName;
        private List<ArgumentException> _listException;

        private Container _container;
        private static CosmosClientOptions cosmosOptions = new CosmosClientOptions() { AllowBulkExecution = true };
        CosmosClient _client = new CosmosClient(_endpointUrl, _authorizationKey, cosmosOptions);        

        public CosmosRepository(IMapper mapper, ICommandText commandText)
        {
            _mapper = mapper;
            _commandText = commandText;
            _document = GetDocumentName(typeof(T));
            _container = _client.GetContainer(_databaseName, _document);
        }

        #region Methods privates
        private protected static string GetDocumentName(Type typeModel) => ((dynamic)typeModel.GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "JsonObjectAttribute")).Title;

        private protected static string GetPartitionKeyName(T t) => t.GetType().GetProperties().First(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any()).Name;

        private protected static string GetValueProperty(T item, string propertyName) => (string)typeof(T).GetProperty(propertyName).GetValue(item, null);

        private string GenerateListOfProperties() => string.Join(",", typeof(T).GetProperties().Select(p => p.GetCustomAttribute<JsonPropertyAttribute>()).Select(jp => $"{_document}.{jp.PropertyName}"));

        private string GenerateColumnNames(List<string> listColumns) => (listColumns != null && listColumns.Count() > 0) ? string.Join(",", listColumns) : GenerateListOfProperties();

        private void AddToListException(AggregateException aggregateException, string item)
        {
            _listException.Add(new ArgumentException(aggregateException.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException
                ? $"CosmosException: {cosmosException.StatusCode} -- {cosmosException.Message}. -- {item}"
                : $"Exception: {aggregateException.InnerExceptions.FirstOrDefault()}. -- {item}"));
        }

        private static IEnumerable<IEnumerable<T>> SplitList(IEnumerable<T> source, int divider)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / divider)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        #endregion

        #region Methods publics

        #region Get Data
        public async Task<IEnumerable<T>> GetAllAsync(bool withColums = true, List<string> listColumns = null)
        {
            var rQuery = _container.GetItemQueryIterator<T>(new QueryDefinition(_commandText.SelectAll((withColums) ? GenerateColumnNames(listColumns) : "*", _document)));
            List<T> results = new List<T>();
            while (rQuery.HasMoreResults)
            {
                var response = await rQuery.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetAllByFilterAsync(List<Filter> filters, bool withColums = true, List<string> listColumns = null)
        {
            var rQuery = _container.GetItemQueryIterator<T>(new QueryDefinition(_commandText.SelectAll((withColums) ? GenerateColumnNames(listColumns) : "*", _document, filters)));
            List<T> results = new List<T>();
            while (rQuery.HasMoreResults)
            {
                var response = await rQuery.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetAllCustomQueryAsync(string query)
        {
            List<T> results = new List<T>();
            if (!string.IsNullOrEmpty(query))
            {
                var rQuery = _container.GetItemQueryIterator<T>(new QueryDefinition(query));
                while (rQuery.HasMoreResults)
                {
                    var response = await rQuery.ReadNextAsync();
                    results.AddRange(response.ToList());
                }
            }
            return results;
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        #endregion Get Data

        #region ADD, UPDATE or DELETE Data
        public async Task AddItemAsync(string id, T t)
        {
            await _container.CreateItemAsync<T>(t, new PartitionKey(id));
        }

        public async Task UpdateItemAsync(string id, T t)
        {
            await _container.UpsertItemAsync<T>(t, new PartitionKey(id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<ArgumentException>> InsertBulkAsync(IEnumerable<T> list)
        {
            _listException = new();
            _partitionKeyName = GetPartitionKeyName(list.FirstOrDefault());
            var splitList = SplitList(list, _cosmosDBMaxRegister);
            foreach (var items in splitList)
            {
                List<Task> tasks = new List<Task>(items.Count());
                foreach (var item in items)
                {
                    string partitionKeyValue = GetValueProperty(item, _partitionKeyName);
                    tasks.Add(_container.CreateItemAsync(item, new PartitionKey(partitionKeyValue))
                        .ContinueWith(itemResponse =>
                        {
                            if (!itemResponse.IsCompletedSuccessfully)
                                AddToListException(itemResponse.Exception.Flatten(), $"partitionKeyValue = {partitionKeyValue}");
                        }));
                }
                await Task.WhenAll(tasks);
            }

            return _listException.AsEnumerable();
        }

        public async Task<IEnumerable<ArgumentException>> DeleteBulkAsync(IEnumerable<T> list)
        {
            _listException = new();
            _partitionKeyName = GetPartitionKeyName(list.FirstOrDefault());
            var splitList = SplitList(list, _cosmosDBMaxRegister);
            foreach (var items in splitList)
            {
                List<Task> tasks = new List<Task>(items.Count());
                foreach (var item in items)
                {
                    string partitionKeyValue = GetValueProperty(item, _partitionKeyName);
                    tasks.Add(_container.DeleteItemAsync<T>(partitionKeyValue, new PartitionKey(partitionKeyValue))
                        .ContinueWith(itemResponse =>
                        {
                            if (!itemResponse.IsCompletedSuccessfully)
                                AddToListException(itemResponse.Exception.Flatten(), $"partitionKeyValue = {partitionKeyValue}");
                        }));
                }
                await Task.WhenAll(tasks);
            }

            return _listException.AsEnumerable();
        }

        #endregion ADD, UPDATE or DELETE Data

        #endregion Methods publics
    }
}
