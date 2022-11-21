using Lens.Services.Communication.Models;

namespace Lens.Services.Communication.Web.Client.Services;

public interface ICommunicationService
{
    Task Send(SendBM sendInfo);
}