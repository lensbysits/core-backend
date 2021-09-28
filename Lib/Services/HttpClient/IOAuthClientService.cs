using System.Threading.Tasks;

namespace CoreLib.Services
{
    public interface IOAuthClientService
    {
        Task<string> GetBearerToken(string clientName);
    }
}