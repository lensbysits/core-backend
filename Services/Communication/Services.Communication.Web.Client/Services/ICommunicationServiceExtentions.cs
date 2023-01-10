using Lens.Services.Communication.Web.Client.Builders;

namespace Lens.Services.Communication.Web.Client.Services;

public static class ICommunicationServiceExtentions
{
    public static CommunicationMessageBuilder CreateMessage(this ICommunicationService communicationService)
    {
        return new CommunicationMessageBuilder(communicationService);
    }
}
