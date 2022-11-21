using Lens.Services.Communication.Models;
using System.Threading.Tasks;

namespace Lens.Services.Communication;

public interface ISmsSenderService
{
    Task Send(SendSmsBM smsInfo);
}