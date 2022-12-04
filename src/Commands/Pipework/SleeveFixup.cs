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
            _errorMessage += $"{Environment.NewLine}ERROR {context}: {e.Message}";
            if (fam != null)
              _errorMessage += $" on Element {fam.Id}:{fam.Name}";
        }
        private void StoreWarningMessage(string context, FamilyInstance fam)
        {
            _errorMessage += $"{Environment.NewLine}WARNING {context}";
            if (fam != null)
                _errorMessage += $" on Element {fam.Id}:{fam.Name}";
        }

        /// <returns>value, true if success, else false (in which case, value is def)</returns>
        private (double, bool) GetDouble(FamilyInstance e, string parameter, double def = -999.999)
        {
            bool success = false;
            var answer = def;
            var p = e?.LookupParameter(parameter);
            try
            {
                success = p?.HasValue == true;
                if (success)
                {
                    answer = p.AsDouble();
                }
                else
                {
                    StoreWarningMessage($"Getting Numeric Parameter {parameter}", e);
                }
            }
            catch (Exception ex)
            {
                StoreErrorMessage($"Getting Numeric Parameter {parameter}", ex, e);
                success = false;
            }
            return (answer, success);
        }

        /// <summary>
        /// Get the parameter value as a string
        /// Also retruns false if error
        /// </summary>
        private (string, bool) GetString(FamilyInstance e, string parameter, string def = "")
        {
            bool success = false;
            var answer = def;
            var p = e?.LookupParameter(parameter); // e.g., "Point Main Number"
            try
            {
                success = (p?.HasValue == true);
                if (p == null)
                {
                    success = false;
                    StoreWarningMessage($"Missing parameter. {parameter}", e);
                }
                else if (success)
                {                    
                    answer = p.AsString();
                }
                else
                {
                    StoreWarningMessage($"No VAL: {parameter}", e);
                }
            }
            catch (Exception ex)
            {
                StoreErrorMessage($"Error getting {parameter}", ex, e);
                success = false;
            }
            return (answer, success);
        }

        private bool SetDIAMxLEN(string paramName, FamilyInstance e)
        {
            Parameter p = e.LookupParameter(paramName);
            var (diam, hasd) = GetDouble(e, "Sleeve_Nominal_Diameter", 1);
            var (len, hasl) = GetDouble(e, "Sleeve_Length", 1);
            string val;
            if (hasd && hasl)
            {
                val = diam * 12 + "\" x " + len * 12 + "\"";
                p.Set(val);
            }
            else
            {
                var warning = "Missing mults DxWxL. Element has ";
                if (!hasd)
                    warning += " No Diameter, ";
                if (!hasl)
                    warning += " No Length";
                StoreWarningMessage(warning, e);

                // ask Frank what we should do if we get here (should we save the value?)
                val = diam * 12 + "\" x " + len * 12 + "\"";
                p.Set(val);
            }
            return (hasd && hasl);
        }

        private bool SetDxWxL(string paramName, FamilyInstance e)
        {
            Parameter p = e.LookupParameter(paramName);
            var (depth, hasd) = GetDouble(e, "Sleeve_Depth", 1);
            var (width, hasw) = GetDouble(e, "Sleeve_Width", 1);
            var (len, hasl) = GetDouble(e, "Sleeve_Length", 1);
            string val;
            if (hasd && hasw && hasl)
            {
                val = depth * 12 + "\"x" + width * 12 + "\"x" + len * 12 + "\"";
                p.Set(val);
            }
            else
            {
                var warning = "Missing mults DxWxL. Element has ";
                if (!hasd)
                    warning += " No Depth, ";
                if (!hasw)
                    warning += " No Width, ";
                if (!hasl)
                    warning += " No Length";
                StoreWarningMessage(warning, e);

                // ask Frank what we should do if we get here (should we save the value?)
                val = depth * 12 + "\"x" + width * 12 + "\"x" + len * 12 + "\"";
                p.Set(val);
            }
            return (hasd && hasw && hasl);
        }

        private bool SetPrefix_XDash_MainNum(FamilyInstance e, string paramName, string dash)
        {
            Parameter p = e.LookupParameter(paramName);
            var (pp, haspp) = GetString(e, "Point Prefix", "");
            var (pm, haspm) = GetString(e, "Point Main Number", "");
            string val;

            if (haspp && haspm)
            {
                val = pp + $"{dash}{_dash}" + pm;
                p.Set(val);
            }
            else
            {
                var warning = "Could not set Desc. Element has ";
                if (!haspp)
                    warning += " No Prefix, ";
                if (!haspm)
                    warning += " No Main Number.";
                StoreWarningMessage(warning, e);

                // ask Frank what we should do if we get here (should we save the value?)
                val = pp + $"{dash}{_dash}" + pm;
                p.Set(val);
            }
            return (haspp && haspm);
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
                        SetDIAMxLEN("Point Description", e);
                        SetPrefix_XDash_MainNum(e, "Point Number 1", "1");
                        SetPrefix_XDash_MainNum(e, "Point Number 2", "2");
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
                        SetDIAMxLEN("Point Description", e);
                        SetPrefix_XDash_MainNum(e, "Point Number", "");
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
                        SetDxWxL("Point Description", e);
                        SetPrefix_XDash_MainNum(e, "Point Number 1", "1");
                        SetPrefix_XDash_MainNum(e, "Point Number 2", "2");
                        SetPrefix_XDash_MainNum(e, "Point Number 3", "3");
                        SetPrefix_XDash_MainNum(e, "Point Number 4", "4");
                        SetPrefix_XDash_MainNum(e, "Point Number 1a", "1a");
                        SetPrefix_XDash_MainNum(e, "Point Number 2a", "2a");
                        SetPrefix_XDash_MainNum(e, "Point Number 3a", "3a");
                        SetPrefix_XDash_MainNum(e, "Point Number 4a", "4a");
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
                        SetDxWxL("Point Description", e);
                        SetPrefix_XDash_MainNum(e, "Point Number 1", "1");
                        SetPrefix_XDash_MainNum(e, "Point Number 2", "2");
                        SetPrefix_XDash_MainNum(e, "Point Number 3", "3");
                        SetPrefix_XDash_MainNum(e, "Point Number 4", "4");
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
