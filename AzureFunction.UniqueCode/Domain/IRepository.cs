using AzureFunction.UniqueCode.Entities;
using AzureFunction.UniqueCode.Entities.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Domain
{
    public interface IRepository
    {
        Task<IEnumerable<Box>> GetDataEventBRU(EventBRU bru);
    }
}
