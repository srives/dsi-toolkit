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
    public class PipeAndFittingBOM : Command
    {
        /// <summary>
        /// The number of decimal places to round to.
        /// </summary>
        private const int precision = 2;

        /// <summary>
        /// Generates a BOM from a model's pipes and pipe fittings.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the command.</returns>
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            var fittings        = new List<Element>();
            var pipeworkData    = new List<PipeOrFitting>();

            using (var filteredElements = GetUserSelectedElementsByFilter(commandData.Application, new PipeworkAndFittingSelectionFilter()))
            using (var pipework = new ElementArray())
            {
                if (filteredElements.Size != 0)
                {
                    foreach (Element elem in filteredElements)
                    {
                        var fp = elem as FabricationPart;

                        if (fp.ItemCustomId == 2041)
                        {
                            pipework.Append(elem);
                        }
                        else
                        {
                            fittings.Add(elem);
                        }
                    }

                    foreach (Element pipe in pipework)
                    {
                        pipeworkData.Add(ProcessPipework(pipe));
                    }

                    var fittingData = ProcessFittings(fittings);

                    var ew = new ExcelWriter(
                        templatePath: @"\\budacad\cad\Office_Templates\Excel\CSV_BOM_REVIT_By_Service.xlsm",
                        defaultFileName: @"CSV_BOM_REVIT_By_Service",
                        commandLog: log);
                    ExportData(
                        ew, pipeworkData,
                        previousWriteSize: 0,
                        worksheetWriteRow: 2,
                        worksheetIndex: 3);
                    ExportData(
                        ew, fittingData,
                        previousWriteSize: pipeworkData.Count,
                        worksheetWriteRow: 2,
                        worksheetIndex: 3);

                    ew.Close();
                }
            }

            return Result.Succeeded;
        }


        /// <summary>
        /// Extracts the data from a list of fittings.
        /// </summary>
        /// <param name="fittings">The list of fitting Elements to process.</param>
        /// <returns>A list of unique fittings with populated data fields.</returns>
        private List<PipeOrFitting> ProcessFittings(List<Element> fittings)
        {
            var fittingData = new List<PipeOrFitting>();

            foreach (Element fitting in fittings)
            {
                var fp      = fitting as FabricationPart;
                var data    = new PipeOrFitting()
                {
                    ServiceName     = fp?.ServiceName ?? "",
                    ProductEntry    = fitting?.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY)?.AsString() ?? "",
                    Family          = fitting?.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ?? "",
                    Specification   = fitting?.get_Parameter(BuiltInParameter.FABRICATION_SPECIFICATION)?.AsValueString() ?? "",
                    PartMaterial    = fitting?.get_Parameter(BuiltInParameter.FABRICATION_PART_MATERIAL)?.AsValueString() ?? "",
                    Quantity        = 1,
                    Length          = 0,
                    Elem            = fitting ?? null
                };

                // trim the bracketed data to clean up the BOM
                data.Specification = TrimBracketedData(data.Specification);
                data.PartMaterial = TrimBracketedData(data.PartMaterial);

                fittingData.Add(data);
            }

            return CountAndReturnUniques(fittingData, "Quantity", new string[1] { "Elem" });
        }


        /// <summary>
        /// Extracts the data from a single pipe.
        /// </summary>
        /// <param name="pipe">The pipe element to process.</param>
        /// <returns>A pipe with populated data fields.</returns>
        private PipeOrFitting ProcessPipework(Element pipe)
        {
            var fp      = pipe as FabricationPart;
            var data    = new PipeOrFitting()
            {
                ServiceName         = fp?.ServiceName ?? "",
                ProductEntry        = pipe?.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY).AsString() ?? "",
                Family              = pipe?.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() ?? "",
                Specification       = pipe?.get_Parameter(BuiltInParameter.FABRICATION_SPECIFICATION).AsValueString() ?? "",
                PartMaterial        = pipe?.get_Parameter(BuiltInParameter.FABRICATION_PART_MATERIAL).AsValueString() ?? "",
                Quantity            = 0,
                Elem                = pipe ?? null
            };


            double lengthValue = GetDimensionValueByName(fp, "Length");
            data.Length = lengthValue != NumericOperationResult.FAILED
                        ? Math.Round(lengthValue, precision, MidpointRounding.AwayFromZero)
                        : lengthValue;

            // trim the bracketed data to clean up the BOM
            data.Specification = TrimBracketedData(data.Specification);
            data.PartMaterial = TrimBracketedData(data.PartMaterial);

            return data;
        }


        /// <summary>
        /// Exports the data to the ExcelWriter to be written to an Excel file.
        /// </summary>
        /// <param name="ew">The ExcelWriter instance to write the data to.</param>
        /// <param name="data">The data to write to the ExcelWriter.</param>
        /// <param name="previousWriteSize">
        /// If a two part write is needed, then previousWriteSize will allow the next write to start off where the last left off.
        /// </param>
        /// <param name="worksheetWriteRow">The first row that the worksheet is intended to have data on. (1-based index).</param>
        /// <param name="worksheetIndex">The worksheet index that is intended to have data on. (1-based index).</param>
        private static void ExportData(
            ExcelWriter ew, 
            List<PipeOrFitting> data, 
            int previousWriteSize, 
            int worksheetWriteRow, 
            int worksheetIndex)
        {
            string[,] firstRegion = new string[data.Count, 5];
            for (int r = 0; r < data.Count; r++)
            {
                firstRegion[r, 0] = data[r].ServiceName;
                firstRegion[r, 1] = data[r].ProductEntry;
                firstRegion[r, 2] = data[r].Family;
                firstRegion[r, 3] = data[r].Specification;
                firstRegion[r, 4] = data[r].PartMaterial;
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

            ew.WriteRange(firstRegion, data.Count, 5, worksheetWriteRow + previousWriteSize, 1, worksheetIndex);
            ew.WriteRange(secondRegion, data.Count, 1, worksheetWriteRow + previousWriteSize, 6, worksheetIndex);
            ew.WriteRange(thirdRegion, data.Count, 1, worksheetWriteRow + previousWriteSize, 7, worksheetIndex);
        }


        /// <summary>
        /// Trims the excess data from the Specification and PartMaterial data if it exists.
        /// </summary>
        /// <param name="data">The string candidate to be trimmed.</param>
        /// <returns>The string, trimmed or untrimmed.</returns>
        private static string TrimBracketedData(string data)
        {
            if (data.Contains("{"))
            {
                return data.Substring(0, data.IndexOf("{", StringComparison.InvariantCulture) - 1);
            }
            else
            {
                return data;
            }
        }


        private class PipeOrFitting
        {
            public string ServiceName { get; set; }
            public string ProductEntry { get; set; }
            public string Family { get; set; }
            public string Specification { get; set; }
            public string PartMaterial { get; set; }
            public int Quantity { get; set; }
            public double Length { get; set; }
            public Element Elem { get; set; }
        }
    }
}
