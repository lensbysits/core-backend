using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreLib.Services
{
    public interface IApplicationService<TLogger>
    {
        ILogger<TLogger> Logger { get; }
        IMapper Mapper { get; }
        IMediator Mediator { get; }
    }

    public interface IApplicationService<TLogger, TSettings> : IApplicationService<TLogger> where TSettings : class
    {
        public TSettings Settings { get; }
    }
}
