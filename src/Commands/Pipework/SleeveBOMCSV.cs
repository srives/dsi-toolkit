using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSI.Core;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    class SleeveBOMCSV : Command
    {
        private protected override Result Main(ExternalCommandData commandData)
        {
            SleeveBOM bom = new SleeveBOM();
            return bom.Csv(commandData);
        }
    }
}
