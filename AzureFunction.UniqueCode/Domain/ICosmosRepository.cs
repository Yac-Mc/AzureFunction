using AzureFunction.UniqueCode.Entities.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Domain
{
    public interface ICosmosRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(bool withColums = true, List<string> listColumns = null);
        Task<IEnumerable<T>> GetAllByFilterAsync(List<Filter> filters, bool withColums = true, List<string> listColumns = null);
        Task<IEnumerable<T>> GetAllCustomQueryAsync(string query);
        Task<T> GetItemAsync(string id);
        Task AddItemAsync(string id, T t);
        Task<IEnumerable<ArgumentException>> InsertBulkAsync(IEnumerable<T> list);
        Task UpdateItemAsync(string id, T t);
        Task DeleteItemAsync(string id);
        Task<IEnumerable<ArgumentException>> DeleteBulkAsync(IEnumerable<T> list);
    }
}
