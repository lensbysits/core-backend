using Lens.Core.Lib.Services;
using Lens.Services.Communication.Models;
using Microsoft.Extensions.Logging;

namespace Lens.Services.Communication;

public class SmsSenderService : BaseService<SmsSenderService>, ISmsSenderService
{
    private readonly HttpClient _httpClient;

    public SmsSenderService(
        IApplicationService<SmsSenderService> applicationService,
        HttpClient httpClient) : base(applicationService)
    {
        _httpClient = httpClient;
    }

    public async Task Send(SendSmsBM smsInfo)
    {
        var response = await _httpClient.GetAsync($"?PhoneNumber={smsInfo.PhoneNumber}&Text={smsInfo.Text}");
        var resonseContent = await response.Content.ReadAsStringAsync();
        if(!response.IsSuccessStatusCode)
        {
            ApplicationService.Logger.LogWarning("Sending Sms failed. Resonse: {responseContent}", resonseContent);
        }
    }
}
