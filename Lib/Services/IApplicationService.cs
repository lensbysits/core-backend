using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lens.Core.Lib.Services
{
    public interface IApplicationService<TLogger>
    {
        ILogger<TLogger> Logger { get; }
        IMapper Mapper { get; }
        IMediator Mediator { get; }
        IUserContext UserContext { get; }
    }

    public interface IApplicationService<TLogger, TSettings> : IApplicationService<TLogger> where TSettings : class
    {
        public TSettings Settings { get; }
    }
}
