using AutoMapper;
using AzureFunction.UniqueCode.Domain;
using AzureFunction.UniqueCode.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebFlowers.Infra.Common;
using WebFlowers.Infra.Common.NetCore;
using Parameter = GloboStudio.Core.Runtime.Parameter;
using ParameterDirection = GloboStudio.Core.Runtime.ParameterDirection;

namespace AzureFunction.UniqueCode.DataAccess
{
    public class Repository : IRepository
    {
        private readonly IMapper _mapper;
        private static readonly string leAPI = Environment.GetEnvironmentVariable("LEApiEndpoint").TrimEnd('/');
        private static readonly string WfWinWebService = "/WinWebServiceCommand.svc/ExecuteCommandStream";
        private readonly ServiceAccess serviceAccess = new();

        public Repository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Box>> GetDataEventBRU(EventBRU bru)
        {
            return await Task.Run(() =>
            {
                ServiceHelper serviceHelper = new ServiceHelper();
                Parameter[] parameters = {
                        serviceHelper.BuildParameter(bru.SerializationType, "SerializationType", ParameterDirection.Input),
                        serviceHelper.BuildParameter(bru.Parameter1, "Parameter1", ParameterDirection.Input),
                        serviceHelper.BuildParameter(bru.Parameter2, "Parameter2", ParameterDirection.Input)
                };

                Parameter[] GeneralParameters = {
                         serviceHelper.BuildParameter(serviceHelper.BuildStoreProcedureParameter("[dbo].[GetDataUniqueCodeBRU]", parameters), "args", ParameterDirection.Input)
                };

                object requestPayload = serviceHelper.GetRequestPayload("ExecuteProcedure", "BaseDataAccess", GeneralParameters);
                DataTable dtReult = serviceAccess.InvokeService($"{leAPI}{WfWinWebService}", JsonConvert.SerializeObject(requestPayload), true);
                DataColumn newColumnArea = new ("area", typeof(string));
                DataColumn newColumnObsv = new ("obsv", typeof(string));
                DataColumn newColumnMasterGuideId = new("masterGuideId", typeof(string));
                DataColumn newColumnAWBId = new("awbId", typeof(string));
                newColumnArea.DefaultValue = bru.Event.Area;
                newColumnObsv.DefaultValue = bru.Event.Obsv;
                newColumnMasterGuideId.DefaultValue = bru.SerializationType == EnumEventBRU.AWB ? bru.Parameter1 : null;
                newColumnAWBId.DefaultValue = bru.SerializationType == EnumEventBRU.AWB ? bru.Parameter2 : null;
                DataColumn[] newColumns = new DataColumn[4] { newColumnArea, newColumnObsv, newColumnMasterGuideId, newColumnAWBId };
                dtReult.Columns.AddRange(newColumns);

                return _mapper.Map<List<DataRow>, IEnumerable<Box>>(dtReult.AsEnumerable().ToList());
            });
        }
    }
}
