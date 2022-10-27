using AutoMapper;
using AzureFunction.UniqueCode.Entities;
using FluentValidation;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using AzureFunction.UniqueCode.Entities.FluentValidations;
using AzureFunction.UniqueCode.Entities.Queries;

namespace AzureFunction.UniqueCode.Domain
{
    public class Domain : IDomain
    {
        private readonly IValidator<Event> _validatorEvent;
        private readonly IValidator<EventBRU> _validatorEventBRU;
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private readonly ICosmosRepository<Box> _cosmosRepositoryBox;
        private List<ArgumentException> _listExceptions;

        public Domain(IValidator<Event> validatorEvent, IValidator<EventBRU> validatorEventBRU, IMapper mapper, IRepository repository, ICosmosRepository<Box> cosmosRepositoryBox)
        {
            _validatorEvent = validatorEvent;
            _validatorEventBRU = validatorEventBRU;
            _mapper = mapper;
            _repository = repository;
            _cosmosRepositoryBox = cosmosRepositoryBox;
        }

        public async Task ManagementSaveBoxInfo(JObject jObject)
        {
            _listExceptions = new();
            try
            {
                Event _event = _mapper.Map<Event>(jObject);
                if (await FluentValidationResponse.ResultFluentValidation(_validatorEvent.Validate(_event)))
                {
                    if (_event.Code == EnumUniqueCodeEventType.BRU)
                        await ManagementBRU(jObject);

                    if (_listExceptions.Count > 0)
                    {
                        string msgException = string.Join("//", _listExceptions.Select(x => x.Message));
                        string paramName = _listExceptions.Select(x => x.ParamName).FirstOrDefault();
                        _listExceptions = new();
                        throw new ArgumentException(msgException, (!string.IsNullOrEmpty(paramName) && paramName == "noData") ? paramName : null);
                    }
                }
            }
            catch (Exception ex)
            {
                string msgException = _listExceptions.Count > 0 ? string.Join("//", _listExceptions.Select(x => x.Message)) + "// " + ex.Message : ex.Message;
                throw new ArgumentException(msgException, ((ArgumentException)ex).ParamName);
            }

        }

        private async Task ManagementBRU(JObject jObject)
        {
            EventBRU eventBRU = _mapper.Map<EventBRU>(jObject);
            if (await FluentValidationResponse.ResultFluentValidation(_validatorEventBRU.Validate(eventBRU)))
            {
                IEnumerable<Box> data = Enumerable.Empty<Box>();
                if (eventBRU.Type == EnumEventBRU.SERIALIZE)
                {
                    data = await _repository.GetDataEventBRU(eventBRU);
                    _listExceptions.AddRange(data.Any() 
                        ? await _cosmosRepositoryBox.InsertBulkAsync(data) 
                        : new List<ArgumentException> { new ArgumentException($"No existen datos en TV para {eventBRU}", EnumTypeExecptions.NODATA) });
                }
                if (eventBRU.Type == EnumEventBRU.DESERIALIZE)
                {
                    data = await _cosmosRepositoryBox.GetAllByFilterAsync(BuildListFilterDeserialize(eventBRU));
                    IEnumerable<Box> resultValidate = data.Where(x => x.Events.Any() && x.Events.Any(y => y.Code != EnumUniqueCodeEventType.BRU));
                    if (resultValidate.Any())
                    {
                        data = data.Except(resultValidate);
                    }
                    _listExceptions.AddRange(data.Any() 
                        ? await _cosmosRepositoryBox.DeleteBulkAsync(data) 
                        : new List<ArgumentException> { new ArgumentException($"No existen datos en CosmosDB para {eventBRU}", EnumTypeExecptions.NODATA) });
                }
            }
        }

        private List<Filter> BuildListFilterDeserialize(EventBRU eventBRU) => new List<Filter>()
        {
            new Filter(){
                Column = eventBRU.SerializationType == EnumEventBRU.HAWB ? "dispatchId" : "masterGuideId",
                Operator = FilterOperator.Equals,
                Value = Convert.ToInt32(eventBRU.Parameter1),
                NextCondition = FilterNextCondition.AND
            },
            new Filter(){
                Column = eventBRU.SerializationType == EnumEventBRU.HAWB ? "grower" : "awbId",
                Operator = FilterOperator.Equals,
                Value = eventBRU.Parameter2,
                Order = 2
            }
        };
    }
}
