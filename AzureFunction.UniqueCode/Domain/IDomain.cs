using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AzureFunction.UniqueCode.Domain
{
    public interface IDomain
    {
        Task ManagementSaveBoxInfo(JObject jObject);
    }
}