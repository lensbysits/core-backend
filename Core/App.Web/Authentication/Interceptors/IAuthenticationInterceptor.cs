﻿using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Lens.Core.App.Web.Authentication;

public interface IAuthenticationInterceptor
{
    Task OnAuthenticationFailed(AuthenticationFailedContext context);

    Task OnForbidden(ForbiddenContext context);

    Task OnMessageReceived(MessageReceivedContext context);

    Task OnTokenValidated(TokenValidatedContext context);

    Task OnChallenge(JwtBearerChallengeContext context);
}
