using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Core;
using System;
using System.Globalization;

namespace DSI.Commands.General
{
    [Transaction(TransactionMode.Manual)]
    public class SetCIDAndServiceType : Command
    {
        /// <summary>
        /// Sets a part's DSI_CID and Service Type.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the operation.</returns>
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            using (var filteredElements = GetUserSelectedElements(commandData.Application))
            {
                SetElementsCidAndServiceType(commandData.Application, filteredElements);
            }
            
            return Result.Succeeded;
        }


        /// <summary>
        /// Sets the DSI_CID and Service Type DSI_CID and Service Type for each element in the element array.
        /// </summary>
        /// <param name="application">Used to find the currently active document in the application.</param>
        /// <param name="elements">The set of elements to set CID and Service Type on.</param>
        private static void SetElementsCidAndServiceType(UIApplication application, ElementArray elements)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            if (elements == null)
            {
                throw new ArgumentNullException(paramName: nameof(elements));
            }

            if (!elements.IsEmpty)
            {
                using (var t = new Transaction(application.ActiveUIDocument.Document, "Set DSI_CID and DSI_ServiceType"))
                {
                    t.Start();

                    foreach (Element elem in elements)
                    {
                        if (elem is FabricationPart fp)
                        {
                            var cid = fp.ItemCustomId;
                            var serviceType = fp.ServiceType.ToString(CultureInfo.InvariantCulture);

                            elem.get_Parameter(new Guid(Properties.Resources.DSI_CID_GUID)).Set(cid);
                            elem.get_Parameter(new Guid(Properties.Resources.DSI_SERVICE_TYPE_GUID)).Set(serviceType);
                        }
                    }

                    t.Commit();
                }
            }
        }
    }
}
