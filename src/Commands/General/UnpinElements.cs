using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Core;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSI.Commands.General
{
    [Transaction(TransactionMode.Manual)]
    class UnpinElements : Command
    {
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            using (var filteredElements = GetUserSelectedElementsByFilter(commandData.Application, new PinSelectionFilter()))
            using (var t = new Transaction(commandData.Application.ActiveUIDocument.Document, "Unpin MEP Fabrication Elements"))
            {
                t.Start();
                foreach (Element elem in filteredElements) elem.Pinned = false;
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
