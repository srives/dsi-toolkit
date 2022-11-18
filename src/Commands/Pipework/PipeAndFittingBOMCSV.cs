using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using DSI.Core;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    public class PipeAndFittingBOMCSV : Command
    {
        private protected override Result Main(ExternalCommandData commandData)
        {
            var bom = new PipeAndFittingBOM();
            return bom.Csv(commandData);
        }
    }
}
