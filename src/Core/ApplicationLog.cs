using Serilog;
using System;
using System.IO;

namespace DSI.Core
{
    public class ApplicationLog
    {
        /// <summary>
        /// ApplicationLog constructor. 
        /// </summary>
        /// <param name="appData">The directory for the application data.</param>
        /// <param name="context">The fully qualified name of the log's executing context.</param>
        public ApplicationLog(DirectoryInfo appData, string context)
        {
            try
            {
                if (appData == null)
                {
                    throw new ArgumentNullException(paramName: nameof(appData));
                }

                LogDirectoryRoot = new DirectoryInfo($"{appData.FullName}\\logs");
                LogDirectoryDebug = new DirectoryInfo($"{LogDirectoryRoot.FullName}\\debug");

                if (!LogDirectoryRoot.Exists)
                {
                    CreateLogDirectory(LogDirectoryRoot);
                }

                if (!LogDirectoryDebug.Exists)
                {
                    CreateLogDirectory(LogDirectoryDebug);
                }

                string template = "{Timestamp:yyyy-MM-dd} | {Timestamp:HH:mm:ss} [{Level:w3} - " + context + "] {Exception}: {Message:lj}{NewLine}";

                Logger = new LoggerConfiguration()
                    // default level for entire pipeline is set to debug to catch debug logs to their own files
                    .MinimumLevel.Debug().WriteTo.File(
                        path: $"{LogDirectoryDebug}\\debug-log-.txt",
                        outputTemplate: template,
                        rollingInterval: RollingInterval.Day,
                        shared: true)
                    // the level for end-user logs are brought up to infomation to make logging less verbose
                    .WriteTo.File(
                        path: $"{LogDirectoryRoot.FullName}\\log-.txt",
                        outputTemplate: template,
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                        shared: true)
                    .CreateLogger();
            }
            catch (ArgumentNullException)
            {
                LogDirectoryRoot = null;
                Logger = null;
            }
        }


        /// <summary>
        /// Directory information for the logging directory. 
        /// </summary>
        public DirectoryInfo LogDirectoryRoot { get; }


        /// <summary>
        /// Directory information for the debug logging directory.
        /// </summary>
        public DirectoryInfo LogDirectoryDebug { get; }


        /// <summary>
        /// The Serilog logger instance created by the configuration defined in the constructor.
        /// </summary>
        public Serilog.Core.Logger Logger { get; }


        /// <summary>
        /// Creates the logging directory.
        /// </summary>
        /// <param name="logDirectory">The directory information that represents the logging directory.</param>
        /// <remarks>This is only called in the constructor if the log directory doesn't already exist.</remarks>
        private static void CreateLogDirectory(DirectoryInfo logDirectory)
        {
            logDirectory.Create();
        }
    }
}
