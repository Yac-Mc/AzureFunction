using AzureFunction.UniqueCode.Domain;
using AzureFunction.UniqueCode.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Functions
{
    public class SaveBoxInfo
    {
        private readonly ILogger<SaveBoxInfo> _logger;
        private readonly IDomain _domain;

        public SaveBoxInfo(ILogger<SaveBoxInfo> log, IDomain domain)
        {
            _logger = log;
            _domain = domain;
        }

        [FunctionName("SaveBoxInfo")]
        public async Task Run([ServiceBusTrigger("%TopicName%", "%TopicSubscription%", Connection = "ServiceBusConnString")] string request)
        {
            try
            {
                await _domain.ManagementSaveBoxInfo(JObject.Parse(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (!string.IsNullOrEmpty(((ArgumentException)ex).ParamName) && !EnumTypeExecptions.GetListCheckedExceptions().Contains(((ArgumentException)ex).ParamName))
                {
                    throw;
                }
            }
        }
    }
}
