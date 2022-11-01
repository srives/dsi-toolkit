
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Commands.Helpers;
using System;
using System.Collections.Generic;

namespace DSI.Commands.Hanger
{
    [Transaction(TransactionMode.Manual)]
    class HangerRenumber : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (commandData == null)
                {
                    throw new ArgumentNullException(paramName: nameof(commandData));
                }

                List<Data> hangers = new List<Data>();

                using (ElementArray filteredElements = CommandHelper.GetUserSelectedElementsByFilter(commandData.Application, new Filters.HangerSelectionFilter()))
                {
                    foreach (Element elem in filteredElements)
                    {
                        hangers.Add(ProcessHanger(elem));
                    }
                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        private Data ProcessHanger(Element elem)
        {
            Data d = new Data();

            d.Elem = elem;
            d.Type = GetCableType(elem);
            d.Color = GetHangerColor(elem as FabricationPart);
            d.CableSizeValue = GetCableSizeValue(elem, d.Type);
            d.CableSizePostfix = GetCableSizePostfix(d.Type);
            d.CableLength = GetCableLength(elem as FabricationPart);
            d.Model = ConstructModel(d);
            d.Mark = ConstructMark(d);

            return d;
        }

        private static string GetCableType(Element elem)
        {
            string family = elem.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString();
            int substrIndex = family.IndexOf(" ", StringComparison.InvariantCulture);

            return family.Substring(substrIndex + 1);
        }

        private static string GetHangerColor(FabricationPart fp)
        {
            string fpsn = fp.ServiceName;
            if (fpsn.Contains("Low") || fpsn.Contains("Medium") || fpsn.Contains("Outside"))
            {
                return "BL";
            }
            else if (fpsn.Contains("Exhaust") || fpsn.Contains("Return") || fpsn.Contains("Flue") || fpsn.Contains("Relief"))
            {
                return "YL";
            }
            else
            {
                return "BK";
            }
        }

        private static string GetCableSizeValue(Element elem, string type)
        {
            string sizeValue = null;
            double dim;
            int breakpoint;

            /* 
             ** DIM = Diameter
             ** BREAKPOINT = 25
             ** TRUE RESULT = "#2"
             ** FALSE RESULT = "#3"
             ** HANGERS: [A, A MDI, At, B, CL, E, F, I, V]
             */
            if (type == "A" || type == "A MDI" || type == "At" || type == "B" ||
                type == "CL" || type == "E" || type == "F" || type == "I" ||
                type == "V")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Diameter");
                breakpoint = 25;
                sizeValue = dim < breakpoint ? "#2" : "#3";
            }
            /*
             ** DIM = Diameter
             ** BREAKPOINT =  25
             ** TRUE RESULT = "#2"
             ** FALSE RESULT = "#3"
             ** HANGERS: [DL]
             ** NOTES: These hangers can't have diameter larger than 68", so we must test
             **        for a range between the breakpoint and 68".
             */
            else if (type == "DL")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Diameter");
                if (dim < 25)
                {
                    sizeValue = "#2";
                }
                else if (dim > 25 && dim < 68)
                {
                    sizeValue = "#3";
                }
            }
            /*
             ** DIM = Diameter
             ** BREAKPOINT = 25
             ** TRUE RESULT = "#2"
             ** FALSE RESULT = null
             ** HANGERS: [F C-Clip, G, V C-Clip]
             */
            else if (type == "F C-Clip" || type == "G" || type == "V C-Clip")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Diameter");
                breakpoint = 25;
                sizeValue = dim < breakpoint ? "#2" : null;
            }
            /*
             ** DIM = Diameter
             ** BREAKPOINT = 68
             ** TRUE RESULT = "#3"
             ** FALSE RESULT = null
             ** HANGERS: [CBB, DBB]
             */
            else if (type == "CBB" || type == "DBB")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Diameter");
                breakpoint = 68;
                sizeValue = dim < breakpoint ? "#3" : null;
            }
            /*
             ** DIM = Diameter
             ** BREAKPOINT =  25
             ** TRUE RESULT = "1/2"
             ** FALSE RESULT = "3/8"
             ** HANGERS: [Y]
             ** NOTES: These hangers can't have diameter larger than 97", so we must test
             **        for a range between the breakpoint and 97".
             */
            else if (type == "Y")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Diameter");
                if (dim < 85)
                {
                    sizeValue = "1/2";
                }
                else if (dim > 85 && dim < 97)
                {
                    sizeValue = "3/8";
                }
            }
            /*
             ** DIM = Duct Width + Duct Depth
             ** BREAKPOINT = 47
             ** TRUE RESULT = "#2"
             ** FALSE RESULT = "#3"
             ** HANGERS: [H, H MDI, Ht, J, L]
             */
            else if (type == "H" || type == "H MDI" || type == "Ht" || type == "J" ||
                     type == "L")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Duct Width")
                    + GetDimensionValue(elem as FabricationPart, "Duct Depth");
                breakpoint = 47;
                sizeValue = dim < breakpoint ? "#2" : "#3";
            }
            /*
             ** DIM = Support Rod Diameter
             ** BREAKPOINT = 96
             ** TRUE RESULT = "3/8"
             ** FALSE RESULT = "1/2"
             ** HANGERS: [K, M, N]
             */
            else if (type == "M" || type == "N")
            {
                dim = GetDimensionValue(elem as FabricationPart, "Duct Width")
                    + GetDimensionValue(elem as FabricationPart, "Duct Depth");
                breakpoint = 96;
                sizeValue = dim < breakpoint ? "3/8" : "1/2";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "#1STP"
             ** FALSE RESULT = ~
             ** HANGERS: [As, Es, O, O1, P, P1, S-Strap, Vs]
             */
            else if (type == "As" || type == "Es" || type == "O" || type == "O1" ||
                     type == "P" || type == "P1" || type == "S-Strap" || type == "Vs")
            {
                sizeValue = "#1STP";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "#2"
             ** FALSE RESULT = ~
             ** HANGERS: [R2, R C-Clip, R #2 Loop, R #2 MDI, R #2 TCA, R #2 Toggle]
             */
            else if (type == "R2" || type == "R C-Clip" || type == "R #2 Loop" ||
                     type == "R #2 MDI" || type == "R #2 TCA" ||
                     type == "R #2 Toggle")
            {
                sizeValue = "#2";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "#3"
             ** FALSE RESULT = ~
             ** HANGERS: [Q, Q #3 Loop, Q #3 MDI, Q #3 TCA, R3, R #3 Loop, R #3 MDI,
             *            R #3 TCA, R #3 Toggle, AG, BG, DG, EG, FG] 
             */
            else if (type == "Q" || type == "Q #3 Loop" || type == "Q #3 MDI" ||
                     type == "Q #3 TCA" || type == "R3" || type == "R #3 Loop" ||
                     type == "R #3 MDI" || type == "R #3 TCA" ||
                     type == "R #3 Toggle" || type == "AG" || type == "BG" ||
                     type == "DG" || type == "EG" || type == "FG")
            {
                sizeValue = "#3";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "3/8"
             ** FALSE RESULT = ~
             ** HANGERS: [S38]
             */
            else if (type == "S38")
            {
                sizeValue = "3/8";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "1/2"
             ** FALSE RESULT = ~
             ** HANGERS: [S12, S12b, Beam Clamp 1_2]
             */
            else if (type == "S12" || type == "S12b" || type == "Beam Clamp 1_2")
            {
                sizeValue = "1/2";
            }
            /*
             ** DIM = ~
             ** BREAKPOINT = ~
             ** TRUE RESULT = "3/4"
             ** FALSE RESULT = ~
             ** HANGERS: [Y34]
             */
            else if (type == "Y34")
            {
                sizeValue = "3/4";
            }

            return sizeValue;
        }

        private static string GetCableSizePostfix(string type)
        {
            string postfix = null;

            if (type == "A" || type == "A MDI" || type == "CBB" || type == "CL" ||
                type == "H" || type == "H MDI" || type == "J" || type == "Q" ||
                type == "R2" || type == "R3" || type == "AG")
            {
                postfix = "-SWL";
            }
            else if (type == "At" || type == "Ht")
            {
                postfix = "-TOG";
            }
            else if (type == "B" || type == "DL" || type == "I" || type == "Q #3 TCA" ||
                     type == "R #2 TCA" || type == "R #3 TCA" || type == "BG" ||
                     type == "DG")
            {
                postfix = "-3/8Stud";
            }
            else if (type == "F" || type == "E" || type == "L" || type == "Q #3 Loop" ||
                     type == "R #2 Loop" || type == "R #3 Loop" || type == "V" ||
                     type == "EG" || type == "FG")
            {
                postfix = "-Loop";
            }
            else if (type == "F C-Clip" || type == "R C-Clip" || type == "V C-Clip")
            {
                postfix = "-C-Clip";
            }
            else if (type == "G")
            {
                postfix = "Loop w/ C-Clip";
            }
            else if (type == "Q #3 MDI" || type == "R #2 MDI" || type == "R #3 MDI")
            {
                postfix = "-MDI";
            }
            else if (type == "R #2 Toggle" || type == "R #3 Toggle")
            {
                postfix = "-TG";
            }

            return postfix;
        }

        public string GetCableLength(FabricationPart fp)
        {
            const int slackInInches = 12;
            double dimInInches = GetDimensionValue(fp, "Length A") + slackInInches;
            double dimInFeet = dimInInches / 12;

            return (5 * (int)Math.Round(dimInFeet / 5.0)).ToString();
        }

        private string ConstructModel(Data d)
        {
            return $"{d.CableSizeValue}{d.CableSizePostfix}{d.CableLength}";
        }

        private string ConstructMark(Data d)
        {
            return $"{d.Type}{d.CableSizeValue.Substring(1)}-{d.CableLength}";
        }

        private static double GetDimensionValue(FabricationPart fp, string dimName)
        {
            IList<FabricationDimensionDefinition> dims = fp.GetDimensions();
            FabricationDimensionDefinition dim = null;

            foreach (FabricationDimensionDefinition d in dims)
            {
                if (d.Name == dimName)
                {
                    dim = d;
                }
            }

            return dim != null ? fp.GetDimensionValue(dim) : -1;
        }

        private class Data
        {
            public Element Elem { get; set; }
            public string Type { get; set; }
            public string Color { get; set; }
            public string CableSizeValue { get; set; }      // NULLABLE IS THE ERROR STATE FOR THIS VARIABLE
            public string CableSizePostfix { get; set; }    // NULLABLE IS A POSSIBLE VALUE FOR THIS VARIABLE <- THIS NEEDS TO BE CHECKED FOR IN FINAL CONSTRUCTION
            public string CableLength { get; set; }
            public string Top { get; set; }
            public string Model { get; set; }
            public string Mark { get; set; }
        }
    }
}
