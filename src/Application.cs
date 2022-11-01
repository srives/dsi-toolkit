using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Core;
using DSI.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace DSI
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        /// <summary>
        /// Provides access to the application logging interface.
        /// </summary>
        private ApplicationLog log;


        /// Handles any necessary cleanup tasks for the toolkit on Revit shutdown.
        /// </summary>
        /// <param name="application">The UIControlledApplication for the currently running Revit instance.</param>
        /// <returns>The Result status of the function.</returns>

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        /// <summary>
        /// Handles any startup tasks for the toolkit on Revit startup.
        /// </summary>
        /// <param name="application">The UIControlledApplication for the currently running Revit instance.</param>
        /// <returns>The Result status of the function.</returns>
        public Result OnStartup(UIControlledApplication application)
        {

            var context = new ApplicationContext(application);
            log = new ApplicationLog(context.InstallDirectory, GetType().FullName);

            CheckForUpdates(context);

            var ribbonTabName   = CreateRibbonTab(application, "DSI Toolkit");
            var generalPanel    = CreateRibbonPanel(application, ribbonTabName, "General");
            var pipeworkPanel   = CreateRibbonPanel(application, ribbonTabName, "Pipework");
            var hangerPanel     = CreateRibbonPanel(application, ribbonTabName, "Hangers");

            #region **General Buttons**

            var selectAndIsolate = AddPulldownButtonToPanel(
                generalPanel,
                "Select and Isolate",
                "Select and Isolate",
                "Asks the user to select elemens in the model and allows the user to select, isolate, or select and isolate those elements in the view.",
                "A database transaction will be opened to hide the elements in the view.",
                Properties.Resources.filter);

            AddPushButtoToPulldownButton(
                selectAndIsolate,
                "S&I by Service Type",
                "by Service Type",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.ServiceNameIsolator",
                "Selects and isolates by Service Name.",
                "A database transaction will be opened to hide the elements in the view.");

            AddPushButtoToPulldownButton(
                selectAndIsolate,
                "S&I by Reference Level",
                "by Reference Level",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.ReferenceLevelIsolator",
                "Selects and isolates by Reference Level.",
                "A database transaction will be opened to hide the elements in the view.");

            AddPushButtonToPanel(
                generalPanel,
                "Set CID and Service Type",
                "Set CID & \nService Type",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.SetCIDAndServiceType",
                "Asks the user to select pipework in the model and updates those elements' DSI_CID and DSI_ServiceType.",
                "A database transaction will be opened and the DSI_CID and DSI_Service Type will be updated in the model.",
                Properties.Resources.refresh);

            AddPushButtonToPanel(
                generalPanel,
                "Tag Duplicate Elements",
                "Tag Duplicate\nElements",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.Overkill",
                "Tags all duplicate (overkill) elements.",
                "Sets a part's DSI_BOM parameter as `Duplicate` if found to be duplicated.",
                Properties.Resources.delete);

            /*
            AddPushButtonToPanel(
                generalPanel,
                "Pin Elements",
                "Pin MEP \nFabrications",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.PinElements",
                "Pins selected elements in the model.",
                "Only pins elements with category MEP Fabrication Pipework, MEP Fabrication Hangers, MEP Fabrication Ductwork",
                Properties.Resources.pin);

            AddPushButtonToPanel(
                generalPanel,
                "Unpin Elements",
                "Unpin MEP \nFabrications",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.General.UnpinElements",
                "Unpin selected elements in the model.",
                "Only unpins elements with category MEP Fabrication Pipework, MEP Fabrication Hangers, MEP Fabrication Ductwork",
                Properties.Resources.unpin);
            */

            #endregion

            #region **Pipework Buttons**

            var tigerstop = AddPulldownButtonToPanel(
                pipeworkPanel,
                "TigerStop",
                "Send to TigerStop",
                "Asks the users to select copper pipework in the model and sends those elments to a desired TigerStop for cutting.",
                "During selection, parts without the Category property set to MEP Fabrication Pipework and ItemCustomId fabrication part property set to 2041 will be automatically filtered out.",
                Properties.Resources.next);

            AddPushButtoToPulldownButton(
                tigerstop,
                "Buda TigerStop",
                "Buda TigerStop",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Pipework.TigerstopBuda",
                "Send selected elements to the Buda TigerStop.",
                "The file name for the exported CSV will be the entered package name.");

            AddPushButtoToPulldownButton(
                tigerstop,
                "Dallas TigerStop",
                "Dallas TigerStop",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Pipework.TigerstopDallas",
                "Send selected elements to the Dallas TigerStop.",
                "The file name for the exported CSV will be the entered package name.");

            AddPushButtoToPulldownButton(
                tigerstop,
                "Houston TigerStop",
                "Houston TigerStop",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Pipework.TigerstopHouston",
                "Send selected elements to the Houston TigerStop.",
                "The file name for the exported CSV will be the entered package name.");

            var exportBom = AddPulldownButtonToPanel(
                pipeworkPanel,
                "Export BOM",
                "Export BOM",
                "Various BOM export tools for pipework.",
                "Hover over the options in the list for more information on the specific exporter.",
                Properties.Resources.excelbw);

            AddPushButtoToPulldownButton(
                exportBom,
                "Export Pipework BOM to Excel",
                "Pipe and Fitting BOM",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Pipework.PipeAndFittingBOM",
                "Asks the users to select pipework and fittings in the model and exports the required information to a BOM.",
                "During selection, parts without the Category property set to MEP Fabrication Pipework AND parts with a Service Type of Weld (57), Joint (58), or Gasket (62) will be automatically filtered out.");

            AddPushButtoToPulldownButton(
                exportBom,
                "Export Pipe Sleve BOM to Excel",
                "Pipe Sleeve BOM",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Pipework.SleeveBOM",
                "Asks the user to select pipe sleeves in the model and exports the required intormation to a BOM.",
                "During selection, parts without a Family Name name of either 'DSI Round Floor Sleeve' or 'Rectangular Floor Sleeve' will be automatically filtered out.");

            #endregion

            #region **Hanger Buttons**

            /*
            AddPushButtonToPanel(
                hangerPanel,
                "Duct Hanger Renumber",
                "Duct Hanger Renumber",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Hanger.HangerRenumber",
                "[placeholder]",
                "[placeholder]",
                Properties.Resources.hashtag);
            */

            AddPushButtonToPanel(
                hangerPanel,
                "Export Hanger BOM to Excel",
                "Hanger BOM",
                Assembly.GetExecutingAssembly().Location,
                "DSI.Commands.Hanger.HangerBOM",
                "Asks the users to select hangers in the model and exports the required information to a BOM. This will only export the data from Trapeze and Clevis hangers.",
                "During selection, parts without the Category property set to MEP Fabrication Hangers will be automatically filtered out. A database transaction will be opened and the selected element's Product Entry will be updated.",
                Properties.Resources.excelbw);

            #endregion

            return Result.Succeeded;
        }

        /// <summary>
        /// Creates a new pulldown button and attaches it to a panel.
        /// </summary>
        /// <param name="panel"> The panel to attach the button to. </param>
        /// <param name="btnName"> The internal name of the button. </param>
        /// <param name="btnText"> The text displayed on the button. </param>
        /// <param name="toolTip"> The text displayed on button hover. </param>
        /// <param name="longDescription"> The text displayed on extended button hover. </param>
        /// <param name="image"> The image displayed on the button. </param>
        /// <returns> Returns the newly created button to attach push buttons to. </returns>
        private static PulldownButton AddPulldownButtonToPanel(
            RibbonPanel panel,
            string btnName,
            string btnText,
            string toolTip,
            string longDescription,
            Image image)
        {
            if (panel == null)
            {
                throw new ArgumentNullException(paramName: nameof(panel));
            }

            var btn = new PulldownButtonData(btnName, btnText)
            {
                ToolTip = toolTip,
                LongDescription = longDescription
            };

            if (image != null)
            {
                var imageSource = GetImageSource(image);
                btn.Image = imageSource;
                btn.LargeImage = imageSource;
            }

            return panel.AddItem(btn) as PulldownButton;
        }


        /// <summary>
        /// Crates a new push button and attaches it to a panel.
        /// </summary>
        /// <param name="panel"> The panel to attach the button to. </param>
        /// <param name="btnName"> The internal name of the button. </param>
        /// <param name="btnText"> The text displayed on the button. </param>
        /// <param name="assemblyName"> The name of the assembly where the targeted command exists. </param>
        /// <param name="className"> The fully qualifed name of the targeted command. </param>
        /// <param name="toolTip"> The text displayed on button hover. </param>
        /// <param name="longDescription"> The text displayed on extended button hover. </param>
        /// <param name="image"> The image to display on the button. </param>
        private static void AddPushButtonToPanel(
            RibbonPanel panel,
            string btnName,
            string btnText,
            string assemblyName,
            string className,
            string toolTip,
            string longDescription,
            Image image)
        {
            if (panel == null)
            {
                throw new ArgumentNullException(paramName: nameof(panel));
            }

            var btn = new PushButtonData(btnName, btnText, assemblyName, className)
            {
                ToolTip = toolTip,
                LongDescription = longDescription
            };

            if (image != null)
            {
                var imageSource = GetImageSource(image);
                btn.Image = imageSource;
                btn.LargeImage = imageSource;
            }

            panel.AddItem(btn);
        }


        /// <summary>
        /// Creates a new push button and attaches it to a pulldown button.
        /// </summary>
        /// <param name="pulldown"> The pulldown button to attach the push buttons to. </param>
        /// <param name="btnName"> The internal name of the button. </param>
        /// <param name="btnText"> The text displayed on the button. </param>
        /// <param name="assemblyName"> The name of the assembly where the targeted command exists. </param>
        /// <param name="className"> The fully qualifed name of the targeted command. </param>
        /// <param name="toolTip"> The text displayed on button hover. </param>
        /// <param name="longDescription"> The text displayed on extended button hover. </param>
        private static void AddPushButtoToPulldownButton(
            PulldownButton pulldown,
            string btnName,
            string btnText,
            string assemblyName,
            string className,
            string toolTip,
            string longDescription)
        {
            if (pulldown == null)
            {
                throw new ArgumentNullException(paramName: nameof(pulldown));
            }

            var btn = new PushButtonData(btnName, btnText, assemblyName, className)
            {
                ToolTip = toolTip,
                LongDescription = longDescription
            };

            pulldown.AddPushButton(btn);
        }


        /// <summary>
        /// Checks for toolkit updates and launches the installer if one is found.
        /// </summary>
        /// <param name="context">The context object for the application.</param>
        private void CheckForUpdates(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(paramName: nameof(context));
            }

            if (context.LoadingStatus == ContextLoadingStatus.SUCCESS)
            {
                log.Logger.Debug("checking for updates");
                if (!context.IsUpToDate || context.Config.Debug)
                {
                    log.Logger.Information($"an update was found for version {context.RemoteAssemblyFileVersion}, prompting the user for update");
                    using (var cw = new ConfirmationWindow("Update Found", "An update for the DSI Toolkit has been found. \nWould you like to install the update?"))
                    {
                        if (cw.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DownloadInstaller(context);
                            var localInstallerPath = $"{context.InstallDirectory.FullName}\\DSIToolkitAddinInstaller.exe";

                            try
                            {
                                Process.Start(localInstallerPath, $"--launched-from-addin --verbose");
                                Process.GetCurrentProcess().Kill();
                            }
                            catch (Win32Exception e)
                            {
                                log.Logger.Error(e, "an unknown error occured when opening the installer or the installer file name could not be found");
                            }
                            catch (ObjectDisposedException e)
                            {
                                log.Logger.Error(e, "the installer process object has already been disposed");
                            }
                            catch (FileNotFoundException e)
                            {
                                log.Logger.Error(e, "the PATH environment variable has a string containing quotes");
                            }
                        }
                        else
                        {
                            log.Logger.Information("user aborted the update process");
                        }
                    }
                }
                else
                {
                    log.Logger.Debug("no update was found");
                }
            }
            else if (context.LoadingStatus == ContextLoadingStatus.ERROR_ENCOUNTERED)
            {
                log.Logger.Warning("an error was encountered when loading the application context; the update process will be skipped");
            }
        }


        /// <summary>
        /// Creates a new panel and attaches it to a ribbon tab.
        /// </summary>
        /// <param name="application"> The Revit application instantce. </param>
        /// <param name="tabName"> The tab name to which to attach the panel to. </param>
        /// <param name="panelName">The name of the panel that is displayed in the ribbon tab. </param>
        /// <returns> Returns the panel to attach UI elements to. </returns>
        private static RibbonPanel CreateRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            RibbonPanel panel = null;
            var panels = application.GetRibbonPanels(tabName);

            foreach (var p in panels)
            {
                if (p.Name == panelName)
                {
                    panel = p;
                    break;
                }
            }

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tabName, panelName);
            }

            return panel;
        }


        /// <summary>
        /// Creates a new ribbon tab and attaches it to the client.
        /// </summary>
        /// <param name="application"> The Revit application instance. </param>
        /// <param name="tabName"> The tab name that is displayed in the revit client. </param>
        /// <returns> Returns the tab name to attach UI elements to. </returns>
        private static string CreateRibbonTab(UIControlledApplication application, string tabName)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            application.CreateRibbonTab(tabName);

            return tabName;
        }


        /// <summary>
        /// Downloads the newest version of the installer to the local toolkit install directory.
        /// </summary>
        /// <param name="context">The context object for the application.</param>
        private static void DownloadInstaller(ApplicationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(paramName: nameof(context));
            }

            var remoteInstaller = new FileInfo(Properties.Resources.REMOTE_INSTALLER_PATH);

            if (remoteInstaller.Exists)
            {
                remoteInstaller.CopyTo($"{context.InstallDirectory.FullName}\\DSIToolkitAddinInstaller.exe", true);
            }
        }


        /// <summary>
        /// Recursively copys all files and subdirectories to a target directory.
        /// </summary>
        /// <param name="sourceDirName">The source directory.</param>
        /// <param name="destDirName">The target directory.</param>
        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("source directory does not exist or could not be found: " + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }


        /// <summary>
        /// Creates a new bitmap of a given image.
        /// </summary>
        /// <param name="img"> The image to create a bitmap of. </param>
        /// <returns> Returns the generated bitmap. </returns>
        private static BitmapSource GetImageSource(Image img)
        {
            var bmp = new BitmapImage();

            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }

            return bmp;
        }
    }
}
