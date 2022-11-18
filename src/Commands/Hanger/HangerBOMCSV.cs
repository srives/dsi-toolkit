using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using DSI.Core;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DSI.Commands.Hanger
{
    [Transaction(TransactionMode.Manual)]
    public class HangerBOMCSV : Command
    {
        private protected override Result Main(ExternalCommandData commandData)
        {
            HangerBOM bom = new HangerBOM();
            return bom.Csv(commandData);

        }
    }
}
