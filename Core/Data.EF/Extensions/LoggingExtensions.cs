﻿using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

namespace Lens.Core.Data.EF.Extensions;

public static class LoggingExtensions
{
    public static LoggerConfiguration AddEFCoreLogging(this LoggerConfiguration loggerConfiguration, bool isBootstrap = false)
    {
        if (isBootstrap)
        {
            return loggerConfiguration;
        }

        return loggerConfiguration
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                                                .WithDefaultDestructurers()
                                                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
                                        );
    }
}
