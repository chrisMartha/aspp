using System;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public interface ILogger
    {
        void Log(LogRequest request);
        Boolean IsEnabled(LogLevel level);
    }
}
