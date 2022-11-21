using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSI.Core;
using System.IO;
using System.Windows;

namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    class SleeveBOM : Command
    {
        string ExcelRoot { 
            get 
            {
                var ret = @"\\budacad\cad\Office_Templates\Excel\";
                // This was added for debugging off the local path for GTP developers
                if (!Directory.Exists(ret))
                {
                    if (Directory.Exists(@"C:\budacad\cad\Office_Templates\Excel\"))
                    {
                        ret = @"C:\budacad\cad\Office_Templates\Excel\";
                    }
                }
                return ret;
            } 
        }

        private const int precision = 2;
        private bool csv = false;

        public Result Csv(ExternalCommandData commandData)
        {
            csv = true;
            return Main(commandData);
        }

        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            if (csv == false && !Directory.Exists(ExcelRoot))
            {
                MessageBox.Show("Could not find " + ExcelRoot, "Missing Excel Templates");
            }
            
            var data = new List<Sleeve>();

            using (var filteredElements = GetUserSelectedElementsByFilter(commandData.Application, new PipeworkSleeveFilter()))
            {
                if (filteredElements.Size != 0)
                {
                    foreach (Element elem in filteredElements)
                    {
                        var fi = elem as FamilyInstance;

                        if (fi.Symbol.FamilyName == "DSI Round Floor Sleeve"  // Round
                            || fi.Symbol.FamilyName == "DSI Round Wall Sleeve"
                            || fi.Symbol.FamilyName == "eM_SV_Rnd_Flr_Sleeve"
                            || fi.Symbol.FamilyName == "eM_SV_Rnd_Flr_Sleeve V2" // NEW
                            || fi.Symbol.FamilyName == "eM_SV_Rnd_Wall_Sleeve V2" // NEW
                            || fi.Symbol.FamilyName == "Round Floor Sleeve"
                            || fi.Symbol.FamilyName == "Round Wall Sleeve"
                            )
                        {
                            data.Add(ProcessRoundSleeve(elem));
                        }
                        else if (fi.Symbol.FamilyName == "Rectangular Floor Sleeve" 
                                 || fi.Symbol.FamilyName == "Rectangular Wall Sleeve"
                                 || fi.Symbol.FamilyName == "eM_SV_Rec_Wall_Sleeve V2" // NEW
                                 || fi.Symbol.FamilyName == "eM_SV_Rec_Flr_Sleeve V2") // NEW
                        {
                            data.Add(ProcessRectangularSleeve(elem));
                        }
                    }

                    data = CountAndReturnUniques(data, "Quantity", Array.Empty<string>());

                    if (!csv)
                    {
                        var ew = new ExcelWriter(
                            templatePath: $"{ExcelRoot}CSV_BOM_Revit_Sleeves.xlsm",
                            defaultFileName: @"CSV_BOM_Revit_Sleves",
                            commandLog: log);
                        ExportData(
                            ew, data,
                            worksheetWriteRow: 3,
                            worksheetIndex: 1);
                        ew.Close(csv);
                    }
                    else
                    {
                        var ew = new ExcelWriter(
                            columnHeaders: new string[,] { { "Size", "Description", "QTY", "Length" } },
                            //templatePath: @"\\budacad\cad\Office_Templates\Excel\CSV_BOM_Revit_Sleeves.xlsm", 
                            defaultFileName: @"CSV_BOM_Revit_Sleves",
                            commandLog: log);
                        ExportDataCSV(
                            ew, data,
                            worksheetWriteRow: 3);
                        ew.Close(csv);
                    }
                }
            }

            return Result.Succeeded;
        }

        static private Sleeve ProcessRoundSleeve(Element elem)
        {
            var fi = elem as FamilyInstance;

            Sleeve sleeve;
            if (fi.Symbol.FamilyName == "eM_SV_Rnd_Flr_Sleeve"
                || fi.Symbol.FamilyName == "Round Floor Sleeve"
                || fi.Symbol.FamilyName == "Round Wall Sleeve"
                || fi.Symbol.FamilyName == "Rectangular Floor Sleeve"
                || fi.Symbol.FamilyName == "eM_SV_Rnd_Flr_Sleeve V2" // NEW
                || fi.Symbol.FamilyName == "eM_SV_Rnd_Wall_Sleeve V2" // NEW
                || fi.Symbol.FamilyName == "eM_SV_Rec_Wall_Sleeve V2" // NEW
                || fi.Symbol.FamilyName == "eM_SV_Rec_Flr_Sleeve V2" // NEW
                || fi.Symbol.FamilyName == "Rectangular Wall Sleeve")
            {
                sleeve = new Sleeve
                {
                    Size = elem?.get_Parameter(BuiltInParameter.RBS_REFERENCE_OVERALLSIZE)?.AsString() ?? "",
                    Description = elem?.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsValueString() ?? ""
                };
            }
            else
            {
                sleeve = new Sleeve
                {
                    Size = elem?.get_Parameter(BuiltInParameter.RBS_REFERENCE_OVERALLSIZE)?.AsString() ?? "",
                    Description = elem?.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ?? ""
                };
            }

            // TODO: make a generic method on the Command class to localize this routine
            IList<Parameter> lengthParams = elem.GetParameters("Sleeve Length");
            if (lengthParams.Count == 1)
            {
                sleeve.Length = lengthParams[0].AsDouble() * 12;                                                 // default here is feet, converted to inches
            }
            else
            {
                sleeve.Length = -1;
            }

            return sleeve;
        }

        static private Sleeve ProcessRectangularSleeve(Element elem)
        {
            Sleeve sleeve = new Sleeve();

            string length = null;
            string width = null;

            IList<Parameter> lengthParams = elem.GetParameters("Sleeve Length");
            if (lengthParams.Count == 1)
            {
                double rawLength = lengthParams[0].AsDouble() * 12;
                rawLength = Math.Round(rawLength, precision, MidpointRounding.AwayFromZero);
                length = rawLength.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                length = "";
            }

            IList<Parameter> widthParam = elem.GetParameters("Sleeve Width");
            if (widthParam.Count == 1)
            {
                double rawWidth = widthParam[0].AsDouble() * 12;
                rawWidth = Math.Round(rawWidth, precision, MidpointRounding.AwayFromZero);
                width = rawWidth.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                width = "";
            }

            sleeve.Size =
                length + "x" + width;
            sleeve.Description
                = elem.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString();

            IList<Parameter> depthParams = elem.GetParameters("Sleeve Depth");
            if (depthParams.Count == 1)
            {
                sleeve.Length = depthParams[0].AsDouble() * 12;                                                 // default here is feet, converted to inches
            }
            else
            {
                sleeve.Length = -1;
            }

            return sleeve;
        }

       private static void ExportData(
            ExcelWriter ew,
            List<Sleeve> data,
            int worksheetWriteRow,
            int worksheetIndex)
        {
            string[,] firstRegion = new string[data.Count, 2];
            for (int r = 0; r < data.Count; r++)
            {
                firstRegion[r, 0] = data[r].Size;
                firstRegion[r, 1] = data[r].Description;
            }

            int[,] secondRegion = new int[data.Count, 1];
            for (int r = 0; r < data.Count; r++)
            {
                secondRegion[r, 0] = data[r].Quantity;
            }

            double[,] thirdRegion = new double[data.Count, 1];
            for (int r = 0; r < data.Count; r++)
            {
                thirdRegion[r, 0] = data[r].Length;
            }

            ew.WriteRange(firstRegion, data.Count, 2, worksheetWriteRow, 1, worksheetIndex);
            ew.WriteRange(secondRegion, data.Count, 1, worksheetWriteRow, 3, worksheetIndex);
            ew.WriteRange(thirdRegion, data.Count, 1, worksheetWriteRow, 4, worksheetIndex);
        }

        private static void ExportDataCSV(
            ExcelWriter ew, 
            List<Sleeve> data, 
            int worksheetWriteRow)
        {
            string[,] firstRegion = new string[data.Count, 5];
            for (int r = 0; r < data.Count; r++)
            {
                firstRegion[r, 0] = data[r].Size;
                firstRegion[r, 1] = data[r].Description;
            }

            int[,] secondRegion = new int[data.Count, 1];
            for (int r = 0; r < data.Count; r++)
            {
                firstRegion[r, 2] = data[r].Quantity.ToString();
            }

            double[,] thirdRegion = new double[data.Count, 1];
            for (int r = 0; r < data.Count; r++)
            {
                firstRegion[r, 3] = data[r].Length.ToString();
            }

            ew.WriteRangeCSV(firstRegion);
        }

        private class Sleeve
        {
            public string Size { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
            public double Length { get; set; }
        }
    }
}
