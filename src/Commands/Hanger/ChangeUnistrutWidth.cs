using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using DSI.UI;
using System;
using System.Windows.Forms;
using DSI.Core;

namespace DSI.Commands.Hanger
{
    [Transaction(TransactionMode.Manual)]
    class ChangeUnistrutWidth : Command
    {
        /// <summary>
        /// Allows the user to change the width of a single or group of unistrut hangers.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the operation.</returns>
        /// <remarks>Running this command WILL decouple a hanger from it's duct.</remarks>
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null) throw new ArgumentNullException(paramName: nameof(commandData));

            using (var filteredElements = GetUserSelectedElementsByFilter(commandData.Application, new TrapezeHangerSelectionFilter()))
            using (var wiw = new WidthInputWindow(filteredElements))
            {
                if (wiw.ShowDialog() == DialogResult.OK)
                {
                    using (var t = new Transaction(commandData.Application.ActiveUIDocument.Document, "Change Unistrut Width"))
                    {
                        t.Start();
                        foreach (var elem in filteredElements) SetUnistrutWidth(elem as FabricationPart, wiw.UserInputAsDouble, commandData);
                        t.Commit();
                    }
                }
                else throw new OperationCanceledException();
            }

            return Result.Succeeded;
        }


        /// <summary>
        /// Sets a single unistrut hanger's width.
        /// </summary>
        /// <param name="elem">The unistrut hanger.</param>
        /// <param name="width">The desired new width of the hanger.</param>
        /// <param name="commandData">The command data passed by the Application.</param>
        private static void SetUnistrutWidth(Element elem, double width, ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            var fp = elem as FabricationPart;
            var dims = fp.GetDimensions();
            var hasDuctWidthBeenSet = false;

            FabricationDimensionDefinition ductWidth = null;

            foreach (var dim in dims)
            {
                if (dim.Name == "Duct Width" || dim.Name == "Width")
                {
                    if (hasDuctWidthBeenSet == false)
                    {
                        ductWidth = dim;
                        hasDuctWidthBeenSet = true;
                    }
                }
            }

            fp.GetHostedInfo().DisconnectFromHost();
            fp.SetDimensionValue(ductWidth, width);
        }
    }
}
