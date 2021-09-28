using AutoMapper;
using CoreLib.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CoreApp.Services
{
    public class ApplicationService<TLogger> : IApplicationService<TLogger>
    {
        protected readonly IServiceProvider _serviceProvider;
        public ApplicationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private ILogger<TLogger> logger;
        public ILogger<TLogger> Logger { get => logger ??= _serviceProvider.GetRequiredService<ILogger<TLogger>>(); }

        private IMapper mapper;
        public IMapper Mapper { get => mapper ??= _serviceProvider.GetRequiredService<IMapper>(); }

        private IMediator mediator;
        public IMediator Mediator { get => mediator ??= _serviceProvider.GetRequiredService<IMediator>(); }
    }

    public class ApplicationService<TLogger, TSettings> : ApplicationService<TLogger>, IApplicationService<TLogger, TSettings> where TSettings : class
    {
        public ApplicationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private TSettings settings;
        public TSettings Settings { get => settings ??= _serviceProvider.GetRequiredService<IOptions<TSettings>>().Value; }
    }
}
