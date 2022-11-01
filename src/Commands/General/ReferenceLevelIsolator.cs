using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Core;
using DSI.UI;
using System;
using System.Windows.Forms;

namespace DSI.Commands.General
{
    [Transaction(TransactionMode.Manual)]
    public class ReferenceLevelIsolator : Command
    {
        /// <summary>
        /// Isolates parts in the model by ONLY their Reference Level parameter.
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
            using (var iw = new IsolatorWindow(commandData, filteredElements, "Reference Level"))
            {
                if (iw.ShowDialog() == DialogResult.OK)
                {
                    return Result.Succeeded;
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }
    }
}