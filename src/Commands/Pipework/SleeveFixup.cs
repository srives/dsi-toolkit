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
using System.Windows;
using System.Xml.Linq;
using System.Security.Policy;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    class SleeveFixup : Command
    {
        private string _dash = "_";
        private string _evolveVersion = "V2";

        private protected override Result Main(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            var (sz1, b1) = RoundWall(uidoc);
            var (sz2, b2) = RoundFloor(uidoc);
            var (sz3, b3) = RectangularFloor(uidoc);
            var (sz4, b4) = RectangularWall(uidoc);

            var msg = string.Empty;
            if (!string.IsNullOrWhiteSpace(sz1))
                msg += $"{sz1} {Environment.NewLine}";
            if (!string.IsNullOrWhiteSpace(sz2))
                msg += $"{sz2} {Environment.NewLine}";
            if (!string.IsNullOrWhiteSpace(sz3))
                msg += $"{sz3} {Environment.NewLine}";
            if (!string.IsNullOrWhiteSpace(sz4))
                msg += $"{sz4} {Environment.NewLine}";

            var title = "Sleeve Update";
            var errors = !b1 && !b2 && !b3 && !b4;
            if (errors)
            {
                title += " w/Errors";
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = "Errors prevented completion.";
                }
            }
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "Nothing done.";
            }
            msg += _errorMessage;
            MessageBox.Show(msg, title);
            return Result.Succeeded;
        }

        private (string, bool) ReturnMessage(string what, int changedValues, int selectedElementCount, List<ElementId> badElements)
        {
            string message = string.Empty;
            bool success = false;
            if (changedValues > 0)
            {
                message = $"{what}: Updated {changedValues} items ({selectedElementCount} selected items)";
                success = true;
                if (badElements.Count > 0)
                {
                    message += $"; {badElements.Count} failed elements";
                }
            }
            else if (selectedElementCount == 0)
            {
                success = true;
                message = $"{what}: No selected values.";
            }
            else if (badElements.Count > 0)
            {
                success = false;
                message = $"{what}: No changes; {badElements.Count} failed elements.";
            }
            return (message, success);
        }

        private List<FamilyInstance> GetElementsByFamilyNameFromSelection(UIDocument uidoc, ICollection<ElementId> sels, string familyName)
        {
            var ret = new List<FamilyInstance>();
            Document doc = uidoc.Document;
            foreach (var eid in sels)
            {
                var e = doc.GetElement(eid);
                if (e is FamilyInstance)
                {
                    var famElem = e as FamilyInstance;
                    if (famElem.Symbol.FamilyName == familyName)
                    {
                        ret.Add(famElem);
                    }
                }
            }
            return ret;
        }

        public string GetPrefix(FamilyInstance e, Document doc)
        {
            string pfx = string.Empty;
            try
            {
                var subIds = e.GetSubComponentIds();
                if (subIds != null && subIds.Count > 0)
                {
                    var pntId = subIds.FirstOrDefault(id => doc.GetElement(id).Name.Contains("Point"));
                    var pnt = doc.GetElement(pntId);
                    Parameter oHosParameter = e.LookupParameter("eM_Service Name");
                    Parameter oNestedParameter = pnt.LookupParameter("eM_Service Name"); // WOULD LIKE TO USE IN FRONT OF PREFIX
                    //    if (string.IsNullOrEmpty(oNestedParameter))
                    //         oNestedParameter.Set(oHosParameter);
                }
            }
            catch
            {
            }
            return pfx;
        }

        private string _errorMessage = string.Empty;
        private void StoreErrorMessage(string context, Exception e, FamilyInstance fam)
        {
            _errorMessage += $"{Environment.NewLine}{context}: {e.Message}";
            if (fam != null)
              _errorMessage += $" on Element {fam.Id}:{fam.Name}";
        }

        private double GetDouble(FamilyInstance e, string parameter, double def = -999.999)
        {
            var answer = def;
            var p = e?.LookupParameter(parameter);
            try
            {
                answer = (p?.HasValue == true) ? p.AsDouble() : def;
            }
            catch
            {
            }
            return answer;
        }

        private string GetString(FamilyInstance e, string parameter, string def = "")
        {
            var answer = def;
            var p = e?.LookupParameter(parameter);
            try
            {
                answer = (p?.HasValue == true) ? p.AsValueString() : def;
            }
            catch
            {
            }
            return answer;
        }

        public (string, bool) RoundWall(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();

            var what = "Round Wall";
            int selectedElementCount = sels?.Count ?? 0;
            int changedValues = 0;
            List<ElementId> badElements = new List<ElementId>();

            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                var elements = GetElementsByFamilyNameFromSelection(uidoc, sels, $"eM_SV_Rnd_Wall_Sleeve {_evolveVersion}");
                foreach (var e in elements)
                {
                    try
                    {
                        Parameter pntdesc = e.LookupParameter("Point Description");
                        Parameter pntnum1 = e.LookupParameter("Point Number 1");
                        Parameter pntnum2 = e.LookupParameter("Point Number 2");
                        pntdesc.Set(GetDouble(e, "Sleeve_Nominal_Diameter",1) * 12 + "\" x " + GetDouble(e, "Sleeve_Length", 1) * 12 + "\"");
                        pntnum1.Set(GetString(e, "Point Prefix") + $"1{_dash}" + GetString(e, "Point Main Number"));
                        pntnum2.Set(GetString(e, "Point Prefix") + $"2{_dash}" + GetString(e, "Point Main Number"));
                        changedValues++;
                    }
                    catch (Exception ex)
                    {
                        StoreErrorMessage(what, ex, e);
                    }
                }
                if (changedValues > 0)
                {
                    ts.Commit();
                }
            }
            return ReturnMessage(what, changedValues, selectedElementCount, badElements);
        }

        public (string, bool) RoundFloor(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();

            var what = "Round Floor";
            int selectedElementCount = sels?.Count ?? 0;
            int changedValues = 0;
            List<ElementId> badElements = new List<ElementId>();

            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                var elements = GetElementsByFamilyNameFromSelection(uidoc, sels, $"eM_SV_Rnd_Flr_Sleeve {_evolveVersion}");
                foreach (var e in elements)
                {
                    try
                    {
                        Parameter pntdesc = e.LookupParameter("Point Description");
                        Parameter pntnum = e.LookupParameter("Point Number");
                        pntdesc.Set(GetDouble(e, "Sleeve_Nominal_Diameter", 1) * 12 + "\" x " + GetDouble(e, "Sleeve_Length", 1) * 12 + "\"");
                        var val = GetString(e, "Point Prefix") + $"{_dash}" + GetString(e, "Point Main Number");
                        pntnum.Set(val);
                        changedValues++;
                    }
                    catch (Exception ex)
                    {
                        StoreErrorMessage(what, ex, e);
                    }
                }
                if (changedValues > 0)
                {
                    ts.Commit();
                }
            }
            return ReturnMessage(what, changedValues, selectedElementCount, badElements);
        }

        public (string, bool) RectangularWall(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();

            var what = "Rectanular Wall";
            int selectedElementCount = sels?.Count ?? 0;
            int changedValues = 0;
            List<ElementId> badElements = new List<ElementId>();
            string where = "0";
            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                var elements = GetElementsByFamilyNameFromSelection(uidoc, sels, $"eM_SV_Rec_Wall_Sleeve {_evolveVersion}");
                foreach (var e in elements)
                {
                    try
                    {
                        var pfx = GetPrefix(e, doc);
                        Parameter pntdesc = e.LookupParameter("Point Description");
                        Parameter pntnum1 = e.LookupParameter("Point Number 1");
                        Parameter pntnum2 = e.LookupParameter("Point Number 2");
                        Parameter pntnum3 = e.LookupParameter("Point Number 3");
                        Parameter pntnum4 = e.LookupParameter("Point Number 4");
                        Parameter pntnum1a = e.LookupParameter("Point Number 1a");
                        Parameter pntnum2a = e.LookupParameter("Point Number 2a");
                        Parameter pntnum3a = e.LookupParameter("Point Number 3a");
                        Parameter pntnum4a = e.LookupParameter("Point Number 4a");

                        var val = GetDouble(e, "Sleeve Length", 1) * 12 + "\"x" + GetDouble(e, "Sleeve Width", 1) * 12 + "\"x" + GetDouble(e, "Sleeve Depth", 1) * 12 + "\"";
                        pntdesc.Set(val);
                        val = GetString(e, "Point Prefix") + $"1{_dash}" + GetString(e, "Point Main Number");
                        pntnum1.Set(val);
                        val = GetString(e, "Point Prefix") + $"2{_dash}" + GetString(e, "Point Main Number");
                        pntnum2.Set(val);
                        val = GetString(e, "Point Prefix") + $"3{_dash}" + GetString(e, "Point Main Number");
                        pntnum3.Set(val);
                        pntnum4.Set(GetString(e, "Point Prefix") + $"4{_dash}" + GetString(e, "Point Main Number"));
                        pntnum1a.Set(GetString(e, "Point Prefix") + $"1a{_dash}" + GetString(e, "Point Main Number"));
                        pntnum2a.Set(GetString(e, "Point Prefix") + $"2a{_dash}" + GetString(e, "Point Main Number"));
                        pntnum3a.Set(GetString(e, "Point Prefix") + $"3a{_dash}" + GetString(e, "Point Main Number"));
                        pntnum4a.Set(GetString(e, "Point Prefix") + $"4a{_dash}" + GetString(e, "Point Main Number"));
                        changedValues++;
                    }
                    catch (Exception ex)
                    {
                        StoreErrorMessage(what + " " + where, ex, e);
                    }
                }
                if (changedValues > 0)
                {
                    ts.Commit();
                }
            }
            return ReturnMessage(what, changedValues, selectedElementCount, badElements);
        }

        public (string, bool) RectangularFloor(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            var sels = selection.GetElementIds();

            var what = "Rectangular Floor";
            int selectedElementCount = sels?.Count ?? 0;
            int changedValues = 0;
            List<ElementId> badElements = new List<ElementId>();

            using (var ts = new Transaction(doc, "Sleeve Point Info"))
            {
                ts.Start();
                var elements = GetElementsByFamilyNameFromSelection(uidoc, sels, $"eM_SV_Rec_Flr_Sleeve {_evolveVersion}");
                foreach (var e in elements)
                {
                    try
                    {
                        var pfx = GetPrefix(e, doc);
                        Parameter pntdesc = e.LookupParameter("Point Description");
                        Parameter pntnum1 = e.LookupParameter("Point Number 1");
                        Parameter pntnum2 = e.LookupParameter("Point Number 2");
                        Parameter pntnum3 = e.LookupParameter("Point Number 3");
                        Parameter pntnum4 = e.LookupParameter("Point Number 4");

                        pntdesc.Set(GetDouble(e,"Sleeve Length", 1) * 12 + "\"x" + GetDouble(e, "Sleeve Width",1) * 12 + "\"x" + GetDouble(e, "Sleeve Depth", 1) * 12 + "\"");
                        pntnum1.Set(GetString(e, "Point Prefix") + $"1{_dash}" + GetString(e, "Point Main Number"));

                        var val = GetString(e, "Point Prefix") + $"2{_dash}" + GetString(e, "Point Main Number");
                        pntnum2.Set(val);

                        pntnum3.Set(GetString(e, "Point Prefix") + $"3{_dash}" + GetString(e, "Point Main Number"));
                        pntnum4.Set(GetString(e, "Point Prefix") + $"4{_dash}" + GetString(e, "Point Main Number"));
                        changedValues++;
                    }
                    catch (Exception ex)
                    {
                        StoreErrorMessage(what, ex, e);
                    }
                }
                if (changedValues > 0)
                {
                    ts.Commit();
                }
            }
            return ReturnMessage(what, changedValues, selectedElementCount, badElements);
        }
    }
}
