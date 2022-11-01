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
    class Overkill : Command
    {
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            var doc = commandData.Application.ActiveUIDocument.Document;

            using (var elements = GetUserSelectedElementsByFilter(commandData.Application, new OverkillFilter()))
            {
                using (var ts = new Transaction(doc, "Overkill"))
                {
                    ts.Start();
                    foreach (var o in elements)
                    {
                        var e = (Element)o;
                        var fcollect = new FilteredElementCollector(doc, doc.ActiveView.Id)
                            .OfCategory((BuiltInCategory)e.Category.Id.IntegerValue)
                            .Where(fname =>
                                fname.Name.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                !fname.UniqueId.Equals(e.UniqueId))
                            .ToList();
                        foreach (var elem in fcollect)
                        {
                            var sharedp = elem.LookupParameter("DSI_BOM");

                            if (sharedp.AsString() == "Duplicate")
                            {
                                sharedp.Set("");
                            }

                            XYZ eLoc = (e.get_BoundingBox(doc.ActiveView).Max + e.get_BoundingBox(doc.ActiveView).Min) / 2;
                            var elemLoc = (elem.get_BoundingBox(doc.ActiveView).Max +
                                           elem.get_BoundingBox(doc.ActiveView).Min) / 2;
                            if (!eLoc.IsAlmostEqualTo(elemLoc))
                                continue;
                            sharedp.Set("Duplicate");
                        }
                    }

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
