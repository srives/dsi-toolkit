using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Core;
using DSI.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSI.Commands.General
{
    [Transaction(TransactionMode.Manual)]
    class RemoveEvolveParams : Command
    {
        /// <summary>
        /// Removes evolve project parameters.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the operation.</returns>
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            using (var cw = new ConfirmationWindow("Remove Evolve Parameters", "This will remove project parameters that begin with 'eM_'. \nWould you like to continue?"))
            using (var fec = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document))
            using (var ps = fec.WhereElementIsNotElementType().OfClass(typeof(ParameterElement)))
            {
                if (cw.ShowDialog() == DialogResult.OK)
                {
                    var parameters = new List<ParameterElement>();

                    foreach (ParameterElement param in ps)
                    {
                        if (param.GetDefinition().Name.StartsWith("eM_", StringComparison.InvariantCulture))
                        {
                            parameters.Add(param);
                        }
                    }

                    if (parameters != null)
                    {
                        using (var t = new Transaction(commandData.Application.ActiveUIDocument.Document, "Remove Evolve Project Parameters"))
                        {
                            t.Start();
                            foreach (var param in parameters)
                            {
                                commandData.Application.ActiveUIDocument.Document.Delete(param.Id);
                            }
                            t.Commit();
                        }
                    }

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
