using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSI.Core;
using Autodesk.Revit.UI.Selection;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    class SleeveFixup : Command
    {        
        private protected override Result Main(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            RoundWall(uidoc);
            RoundFloor(uidoc);
            RectangularFloor(uidoc);
            RectangularWall(uidoc);
            return Result.Succeeded;
        }

        public void RoundWall(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();
            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                foreach (var eid in sels)
                {
                    var e = doc.GetElement(eid);
                    var pntId = ((FamilyInstance)e).GetSubComponentIds().FirstOrDefault(id => doc.GetElement(id).Name.Contains("Point"));
                    var pnt = doc.GetElement(pntId);
                    Parameter pntdesc = e.LookupParameter("Point Description");
                    Parameter pntnum1 = e.LookupParameter("Point Number 1");
                    Parameter pntnum2 = e.LookupParameter("Point Number 2");
                    pntdesc.Set(e.LookupParameter("Sleeve_Nominal_Diameter").AsDouble() * 12 + "\" x " + e.LookupParameter("Sleeve_Length").AsDouble() * 12 + "\"");
                    pntnum1.Set(e.LookupParameter("Point Prefix").AsString() + "1-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum2.Set(e.LookupParameter("Point Prefix").AsString() + "2-" + e.LookupParameter("Point Main Number").AsString());
                }
                ts.Commit();
            }
        }

        public void RoundFloor(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();
            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                foreach (var eid in sels)
                {
                    var e = doc.GetElement(eid);
                    var pntId = ((FamilyInstance)e).GetSubComponentIds().FirstOrDefault(id => doc.GetElement(id).Name.Contains("Point"));
                    var pnt = doc.GetElement(pntId);
                    Parameter pntdesc = e.LookupParameter("Point Description");
                    Parameter pntnum = e.LookupParameter("Point Number");
                    pntdesc.Set(e.LookupParameter("Sleeve_Nominal_Diameter").AsDouble() * 12 + "\" x " + e.LookupParameter("Sleeve_Length").AsDouble() * 12 + "\"");
                    pntnum.Set(e.LookupParameter("Point Prefix").AsString() + "-" + e.LookupParameter("Point Main Number").AsString());
                }
                ts.Commit();
            }
        }

        public void RectangularWall(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();
            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                foreach (var eid in sels)
                {
                    var e = doc.GetElement(eid);
                    var pntId = ((FamilyInstance)e).GetSubComponentIds().FirstOrDefault(id => doc.GetElement(id).Name.Contains("Point"));
                    var pnt = doc.GetElement(pntId);
                    Parameter oHosParameter = e.LookupParameter("eM_Service Name");
                    Parameter oNestedParameter = pnt.LookupParameter("eM_Service Name");
                    //    if (string.IsNullOrEmpty(oNestedParameter.AsString()))
                    //         oNestedParameter.Set(oHosParameter.AsString());
                    Parameter pntdesc = e.LookupParameter("Point Description");
                    Parameter pntnum1 = e.LookupParameter("Point Number 1");
                    Parameter pntnum2 = e.LookupParameter("Point Number 2");
                    Parameter pntnum3 = e.LookupParameter("Point Number 3");
                    Parameter pntnum4 = e.LookupParameter("Point Number 4");
                    Parameter pntnum1a = e.LookupParameter("Point Number 1a");
                    Parameter pntnum2a = e.LookupParameter("Point Number 2a");
                    Parameter pntnum3a = e.LookupParameter("Point Number 3a");
                    Parameter pntnum4a = e.LookupParameter("Point Number 4a");
                    pntdesc.Set(e.LookupParameter("Sleeve Length").AsDouble() * 12 + "\"x" + e.LookupParameter("Sleeve Width").AsDouble() * 12 + "\"x" + e.LookupParameter("Sleeve Depth").AsDouble() * 12 + "\"");
                    pntnum1.Set(e.LookupParameter("Point Prefix").AsString() + "1-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum2.Set(e.LookupParameter("Point Prefix").AsString() + "2-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum3.Set(e.LookupParameter("Point Prefix").AsString() + "3-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum4.Set(e.LookupParameter("Point Prefix").AsString() + "4-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum1a.Set(e.LookupParameter("Point Prefix").AsString() + "1a-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum2a.Set(e.LookupParameter("Point Prefix").AsString() + "2a-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum3a.Set(e.LookupParameter("Point Prefix").AsString() + "3a-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum4a.Set(e.LookupParameter("Point Prefix").AsString() + "4a-" + e.LookupParameter("Point Main Number").AsString());
                }
                ts.Commit();
            }
        }

        public void RectangularFloor(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();
            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                foreach (var eid in sels)
                {
                    var e = doc.GetElement(eid);
                    var pntId = ((FamilyInstance)e).GetSubComponentIds().FirstOrDefault(id => doc.GetElement(id).Name.Contains("Point"));
                    var pnt = doc.GetElement(pntId);
                    Parameter oHosParameter = e.LookupParameter("eM_Service Name");
                    Parameter oNestedParameter = pnt.LookupParameter("eM_Service Name");
                    //    if (string.IsNullOrEmpty(oNestedParameter.AsString()))
                    //         oNestedParameter.Set(oHosParameter.AsString());
                    Parameter pntdesc = e.LookupParameter("Point Description");
                    Parameter pntnum1 = e.LookupParameter("Point Number 1");
                    Parameter pntnum2 = e.LookupParameter("Point Number 2");
                    Parameter pntnum3 = e.LookupParameter("Point Number 3");
                    Parameter pntnum4 = e.LookupParameter("Point Number 4");
                    pntdesc.Set(e.LookupParameter("Sleeve Length").AsDouble() * 12 + "\"x" + e.LookupParameter("Sleeve Width").AsDouble() * 12 + "\"x" + e.LookupParameter("Sleeve Depth").AsDouble() * 12 + "\"");
                    pntnum1.Set(e.LookupParameter("Point Prefix").AsString() + "1-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum2.Set(e.LookupParameter("Point Prefix").AsString() + "2-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum3.Set(e.LookupParameter("Point Prefix").AsString() + "3-" + e.LookupParameter("Point Main Number").AsString());
                    pntnum4.Set(e.LookupParameter("Point Prefix").AsString() + "4-" + e.LookupParameter("Point Main Number").AsString());
                }
                ts.Commit();
            }
        }
    }
}
