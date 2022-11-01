using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace DSI.Core
{
    public class ApplicationContext
    {
        /// <summary>
        /// Provides access to the application logging interface.
        /// </summary>
        private readonly ApplicationLog log;


        /// <summary>
        /// The UI Controlled Application object for the currently executing Revit instance.
        /// </summary>
        private readonly UIControlledApplication uiApplication;


        /// <summary>
        /// The External Command Hander object for the currently executing command.
        /// </summary>
        private readonly ExternalCommandData commandData;


        /// <summary>
        /// Flag to determine if an error was incountered on initializaton.
        /// </summary>
        private readonly bool wasErrorEncountered;


        /// <summary>
        /// ApplicationContext constructor.
        /// </summary>
        /// <param name="application">The UIControlledApplication representing the currently executing Revit instance.</param>
        public ApplicationContext(UIControlledApplication application)
        {
            LoadingStatus = ContextLoadingStatus.IN_PROGRESS;

            try
            {
                InstallDirectory = LocateInstallDirectory();
                log = new ApplicationLog(InstallDirectory, typeof(ApplicationContext).FullName);
                Config = new ApplicationConfiguration($"{InstallDirectory.FullName}\\config.json");
            }
            catch (DirectoryNotFoundException)
            {
                log = null;
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                InstallDirectory = null;
                Config = new ApplicationConfiguration();
            }

            try
            {
                uiApplication = application ?? throw new ArgumentNullException(paramName: nameof(application));
                RevitVersionName = uiApplication.ControlledApplication.VersionName;
                RevitVersionNumber = uiApplication.ControlledApplication.VersionNumber;
                RevitVersionBuild = uiApplication.ControlledApplication.VersionBuild;
            }
            catch (ArgumentNullException e)
            {
                if (log != null) log.Logger.Debug(e, "the passed in UIControlledApplication was null; unable to get revit version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                RevitVersionName = null;
                RevitVersionNumber = null;
                RevitVersionBuild = null;
            }

            try
            {
                LocalAssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                RemoteAssemblyFileVersion = FileVersionInfo.GetVersionInfo($"{Properties.Resources.REMOTE_INSTALL_DIRECTORY}\\{RevitVersionNumber}\\DSIRevitToolkit.dll").FileVersion;
                IsUpToDate = !IsUpdateAvaliable();
            }
            catch (FileNotFoundException e)
            {
                if (log != null) log.Logger.Warning(e, "either the local assembly or remote assembly was not found; unable to get assembly version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                LocalAssemblyFileVersion = "";
                RemoteAssemblyFileVersion = "";
                IsUpToDate = true;
            }
            catch (FormatException e)
            {
                if (log != null) log.Logger.Warning(e, "unable to get assembly version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                IsUpToDate = true;
            }

            if (!wasErrorEncountered)
            {
                log.Logger.Debug("loading context was successful");
                LoadingStatus = ContextLoadingStatus.SUCCESS;
            }
        }


        /// <summary>
        /// ApplicationContext constructor.
        /// </summary>
        /// <param name="command">The command data representing the currently executing addin command.</param>
        public ApplicationContext(ExternalCommandData command)
        {
            LoadingStatus = ContextLoadingStatus.IN_PROGRESS;

            try
            {
                InstallDirectory = LocateInstallDirectory();
                log = new ApplicationLog(InstallDirectory, typeof(ApplicationContext).FullName);
                Config = new ApplicationConfiguration($"{InstallDirectory.FullName}\\config.json");
            }
            catch (DirectoryNotFoundException)
            {
                log = null;
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                InstallDirectory = null;
                Config = new ApplicationConfiguration();
            }

            try
            {
                commandData = command ?? throw new ArgumentNullException(paramName: nameof(command));
                RevitVersionName = commandData.Application.Application.VersionName;
                RevitVersionNumber = commandData.Application.Application.VersionNumber;
                RevitVersionBuild = commandData.Application.Application.VersionBuild;
            }
            catch (ArgumentNullException e)
            {
                if (log != null) log.Logger.Debug(e, "the passed in UIControlledApplication was null; unable to get revit version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                RevitVersionName = null;
                RevitVersionNumber = null;
                RevitVersionBuild = null;
            }

            try
            {
                LocalAssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                RemoteAssemblyFileVersion = FileVersionInfo.GetVersionInfo($"{Properties.Resources.REMOTE_INSTALL_DIRECTORY}\\{RevitVersionNumber}\\DSIRevitToolkit.dll").FileVersion;
                IsUpToDate = !IsUpdateAvaliable();
            }
            catch (FileNotFoundException e)
            {
                if (log != null) log.Logger.Warning(e, "either the local assembly or remote assembly was not found; unable to get assembly version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                LocalAssemblyFileVersion = "";
                RemoteAssemblyFileVersion = "";
                IsUpToDate = true;
            }
            catch (FormatException e)
            {
                if (log != null) log.Logger.Warning(e, "unable to get assembly version information");
                wasErrorEncountered = true;
                LoadingStatus = ContextLoadingStatus.ERROR_ENCOUNTERED;
                IsUpToDate = true;
            }

            if (!wasErrorEncountered)
            {
                log.Logger.Debug("loading context was successful");
                LoadingStatus = ContextLoadingStatus.SUCCESS;
            }
        }


        /// <summary>
        /// Represents the current status of the context;
        /// </summary>
        public ContextLoadingStatus LoadingStatus { get; }


        /// <summary>
        /// Directory information for the installation folder.
        /// </summary>
        public DirectoryInfo InstallDirectory { get; }


        /// <summary>
        /// Loaded configuration for the user.
        /// </summary>
        public ApplicationConfiguration Config { get; }


        /// <summary>
        /// The file version of the remote, serverside assembly.
        /// </summary>
        /// <remarks>This should represent the most up to date version of the assembly.</remarks>
        public string LocalAssemblyFileVersion { get; }


        /// <summary>
        /// The file version of the local assembly.
        /// </summary>
        public string RemoteAssemblyFileVersion { get; }


        /// <summary>
        /// The current version name of the executing Revit instance.
        /// </summary>
        public string RevitVersionName { get; }


        /// <summary>
        /// The current version number of the executing Revit instance.
        /// </summary>
        public string RevitVersionNumber { get; }


        /// <summary>
        /// The current version build of the executing Revit instance.
        /// </summary>
        public string RevitVersionBuild { get; }


        /// <summary>
        /// The flag to check is the current application instance is up to date.
        /// </summary>
        public bool IsUpToDate { get; }


        /// <summary>
        /// Takes a file version string and returns a integer 4-Tuple representation of it.        
        /// </summary>
        /// <param name="version">The file version string.</param>
        /// <returns>The 4-Tuple representation of the file version string.</returns>
        /// <exception cref="System.FormatException">Thrown when the file version string couldn't be correctly parsed.</exception>
        public (int major, int minor, int patch, int subPatch) FileVersionToTuple(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(paramName: nameof(version));
            }

            var substrs = version.Split('.');
            (int major, int minor, int patch, int subPatch) versionTuple;

            try
            {
                versionTuple.major = ParseVersionSubstring(substrs[0]);
                versionTuple.minor = ParseVersionSubstring(substrs[1]);
                versionTuple.patch = ParseVersionSubstring(substrs[2]);
                versionTuple.subPatch = ParseVersionSubstring(substrs[3]);
            }
            catch (FormatException e)
            {
                log.Logger.Debug(e, "passed in version information was malformed");
                throw;
            }

            return versionTuple;
        }


        /// <summary>
        /// Locates the installation folder for the toolkit.
        /// </summary>
        /// <returns>The directory info for the located directory.</returns>
        /// <remarks>If the installation folder is not found, then the install folder is created.</remarks>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the %APPDATA%\Local directory is not found.</exception>
        private static DirectoryInfo LocateInstallDirectory()
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localAppDataDirectory = new DirectoryInfo(localAppDataPath);

            try
            {
                if (localAppDataDirectory.Exists)
                {
                    var revitToolkitInstallDirectory = new DirectoryInfo($"{localAppDataDirectory.FullName}\\{Properties.Resources.INSTALL_DIRECTORY}");

                    if (!revitToolkitInstallDirectory.Exists)
                    {
                        CreateAppDataDirectory(revitToolkitInstallDirectory);
                    }

                    return revitToolkitInstallDirectory;
                }
                else
                {
                    throw new DirectoryNotFoundException();
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
        }


        /// <summary>
        /// Creates the installation folder for the toolkit.
        /// </summary>
        /// <param name="directory">The DirectoryInfo which the new directory is created from.</param>
        private static void CreateAppDataDirectory(DirectoryInfo directory)
        {
            directory.Create();
        }


        /// <summary>
        /// Parses a version substring.
        /// </summary>
        /// <param name="substr">The version substring to be parsed.</param>
        /// <returns>The integer representation of the version substring.</returns>
        /// <exception cref="System.FormatException">Thrown when a version substring can not be parsed as an int.</exception>
        private static int ParseVersionSubstring(string substr)
        {
            try
            {
                return int.Parse(substr, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw;
            }
        }


        /// <summary>
        /// Compares the Local Assemblt File Version agains the Remote Assembly File Version so see if any updates are needed.
        /// </summary>
        /// <returns>The bool representation of the updated needed flag.</returns>
        private bool IsUpdateAvaliable()
        {
            var (lMajor, lMinor, lPatch, lSubPatch) = FileVersionToTuple(LocalAssemblyFileVersion);
            var (rMajor, rMinor, rPatch, rSubPatch) = FileVersionToTuple(RemoteAssemblyFileVersion);

            if (lMajor < rMajor) return true;
            else if (lMinor < rMinor) return true;
            else if (lPatch < rPatch) return true;
            else if (lSubPatch < rSubPatch) return true;
            else return false;
        }
    }


    /// <summary>
    /// The loading status of the context object. Used to determine if there were any error when fetching various properties.
    /// </summary>
    public enum ContextLoadingStatus
    {
        SUCCESS,
        IN_PROGRESS,
        ERROR_ENCOUNTERED
    }
}

