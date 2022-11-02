using Lens.Services.Communication.Models;

namespace Lens.Services.Communication;

public interface ISenderService
{
    Task Send(SendBM sendInfo);
}