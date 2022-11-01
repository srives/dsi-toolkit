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
    public class HangerBOM : Command
    {
        /// <summary>
        /// Conversion factor for feet to inches.
        /// </summary>
        private const int inchesInFoot = 12;

        /// <summary>
        /// The number of decimal places to round to.
        /// </summary>
        private const int precision = 2;


        /// <summary>
        /// Generates a BOM from generic Clevis and Trapeze Hangers.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the command.</returns>
        private protected override Result Main(ExternalCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(paramName: nameof(commandData));
            }

            using (var filteredElements = GetUserSelectedElementsByFilter(commandData.Application, new HangerSelectionFilter()))
            using (var clevisHangers = new ElementArray())
            using (var trapezeHangers = new ElementArray())
            {
                if (filteredElements.Size != 0)
                {
                    var hangers = new List<HangerData>();
                    foreach (Element elem in filteredElements)
                    {
                        var fp = elem as FabricationPart;

                        if (fp.GetRodInfo().RodCount == 2)
                        {
                            trapezeHangers.Append(elem);
                        }
                        else
                        {
                            clevisHangers.Append(elem);
                        }
                    }

                    using (var t = new Transaction(commandData.Application.ActiveUIDocument.Document, "Set Fabrication Notes for Hangers"))
                    {
                        t.Start();

                        if (!clevisHangers.IsEmpty)
                        {
                            foreach (Element elem in clevisHangers)
                            {
                                hangers.Add(ProcessClevisHanger(elem));
                            }
                        }

                        if (!trapezeHangers.IsEmpty)
                        {
                            foreach (Element elem in trapezeHangers)
                            {
                                hangers.Add(ProcessTrapezeHanger(elem));
                            }
                        }

                        t.Commit();
                    }

                    var ew = new ExcelWriter(
                        templatePath: @"\\budacad\cad\Office_Templates\Excel\CSV_BOM_Revit_Hanger.xlsm",
                        defaultFileName: @"CSV_BOM_Revit_Hanger",
                        commandLog: log);

                    ExportData(
                        ew, hangers,
                        worksheetWriteRow: 5,
                        worksheetIndex: 1);

                    ew.Close();
                }
            }

            return Result.Succeeded;
        }


        // TODO: create a wrapper method for get_Parameter to check for null values since this breaks on some models
        /// <summary>
        /// Processes the data specific to Clevis Hangers.
        /// </summary>
        /// <param name="elem">The current hanger.</param>
        /// <returns>The partially constructed HangerData.</returns>
        private HangerData ProcessClevisHanger(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            var clevisHanger = new HangerData();

            var productEntry = elem?.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY)?.AsString() ?? "";
            elem.get_Parameter(BuiltInParameter.FABRICATION_PART_NOTES).Set(productEntry);
            clevisHanger.FabricationNotes = '\'' + productEntry;

            return ProcessHanger(clevisHanger, elem);
        }


        /// <summary>
        /// Processes the data specific to Trapeze Hangers.
        /// </summary>
        /// <param name="elem">The current hanger.</param>
        /// <returns>The partially constructed HangerData.</returns>
        private HangerData ProcessTrapezeHanger(Element elem)
        {
            var fp = elem as FabricationPart;
            var trapezeHanger = new HangerData();
            double totalWidthInInches;

            var ductWidth = GetDimensionValueByName(fp, "Duct Width", new string[1] { "Width" });

            var angleExtension = GetDimensionValueByName(fp, "Angle Extension");
            var bearerExtension = GetDimensionValueByName(fp, "Bearer Extn");
            var strutExtension = GetDimensionValueByName(fp, "Strut Extension");
            double extension;

            if (angleExtension != NumericOperationResult.FAILED)
            {
                extension = angleExtension;
            }
            else if (bearerExtension != NumericOperationResult.FAILED)
            {
                extension = bearerExtension;
            }
            else if (strutExtension != NumericOperationResult.FAILED)
            {
                extension = strutExtension;
            }
            else
            {
                extension = NumericOperationResult.FAILED;
            }

            var ductWidthInInches = ductWidth * inchesInFoot;
            var extensionInInches = extension * inchesInFoot;

            if (ductWidth           != NumericOperationResult.FAILED
                && extension        != NumericOperationResult.FAILED)
            {
                totalWidthInInches = ductWidthInInches + (extensionInInches * 2);
                totalWidthInInches = Math.Round(totalWidthInInches, precision, MidpointRounding.AwayFromZero);
            }
            else
            {
                log.Logger.Warning($"a term composing the sum {nameof(totalWidthInInches)} resulted in a numeric operation failure"
                                   + $" which has caused {nameof(totalWidthInInches)} to fail as well");
                totalWidthInInches = NumericOperationResult.FAILED;
            }

            var totalWidthInInchesAsString = totalWidthInInches.ToString(CultureInfo.InvariantCulture);
            elem?.get_Parameter(BuiltInParameter.FABRICATION_PART_NOTES)?.Set(totalWidthInInches);
            trapezeHanger.FabricationNotes = totalWidthInInchesAsString;

            return ProcessHanger(trapezeHanger, elem);
        }


        /// <summary>
        /// Processes the data which isn't specific to any Hanger.
        /// </summary>
        /// <param name="hanger">The partially constructed HangerData.</param>
        /// <param name="elem">The current hanger.</param>
        /// <returns>The fully constructed HangerData.</returns>
        private HangerData ProcessHanger(HangerData hanger, Element elem)
        {
            var fp                      = elem as FabricationPart;
            hanger.Family               = elem?.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM)?.AsValueString() ?? "";
            hanger.ItemNumber           = elem?.get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER)?.AsString() ?? "";
            hanger.ServiceAbbreviation  = fp?.ServiceAbbreviation ?? "";
            hanger.Offset               = elem?.get_Parameter(BuiltInParameter.FABRICATION_OFFSET_PARAM).AsValueString() ?? "";

            var supportRodValue = GetAncillaryUsageWidthOrDiameter(fp, FabricationAncillaryType.SupportRod);
            hanger.SupportRod
                = supportRodValue != NumericOperationResult.FAILED
                ? supportRodValue * inchesInFoot
                : supportRodValue;

            var lengthAValue = GetDimensionValueByName(fp, "Length A");
            hanger.LengthA
                = lengthAValue != NumericOperationResult.FAILED
                ? Math.Ceiling(lengthAValue * inchesInFoot)
                : lengthAValue;

            var lengthBValue = GetDimensionValueByName(fp, "Length B");
            hanger.LengthB
                = lengthBValue != NumericOperationResult.FAILED
                ? Math.Ceiling(lengthBValue * inchesInFoot)
                : lengthBValue;

            return hanger;
        }


        /// <summary>
        /// Exports the data to the ExcelWriter to be written to an Excel file.
        /// </summary>
        /// <param name="ew">The ExcelWriter instance to write the data to.</param>
        /// <param name="data">The data to write to the ExcelWriter.</param>

        /// <param name="worksheetWriteRow">The first row that the worksheet is intended to have data on. (1-based index).</param>
        /// <param name="worksheetIndex">The worksheet index that is intended to have data on. (1-based index).</param>
        private static void ExportData(ExcelWriter ew, List<HangerData> hangers, int worksheetWriteRow, int worksheetIndex)
        {
            string[,] firstRegion = new string[hangers.Count, 1];
            for (int r = 0; r < hangers.Count; r++)
            {
                firstRegion[r, 0] = hangers[r].Family;
            }

            int[,] secondRegion = new int[hangers.Count, 1];
            for (int r = 0; r < hangers.Count; r++)
            {
                secondRegion[r, 0] = HangerData.Quantity;
            }

            string[,] thirdRegion = new string[hangers.Count, 1];
            for (int r = 0; r < hangers.Count; r++)
            {
                thirdRegion[r, 0] = hangers[r].FabricationNotes;
            }

            double[,] fourthRegion = new double[hangers.Count, 3];
            for (int r = 0; r < hangers.Count; r++)
            {
                fourthRegion[r, 0] = hangers[r].SupportRod;
                fourthRegion[r, 1] = hangers[r].LengthA;
                fourthRegion[r, 2] = hangers[r].LengthB;
            }

            string[,] fifthRegion = new string[hangers.Count, 3];
            for (int r = 0; r < hangers.Count; r++)
            {
                fifthRegion[r, 0] = hangers[r].ItemNumber;
                fifthRegion[r, 1] = hangers[r].ServiceAbbreviation;
                fifthRegion[r, 2] = hangers[r].Offset;
            }

            ew.WriteRange(firstRegion, hangers.Count, 1, worksheetWriteRow, 1, worksheetIndex);
            ew.WriteRange(secondRegion, hangers.Count, 1, worksheetWriteRow, 2, worksheetIndex);
            ew.WriteRange(thirdRegion, hangers.Count, 1, worksheetWriteRow, 3, worksheetIndex);
            ew.WriteRange(fourthRegion, hangers.Count, 3, worksheetWriteRow, 4, worksheetIndex);
            ew.WriteRange(fifthRegion, hangers.Count, 3, worksheetWriteRow, 7, worksheetIndex);
        }


        private class HangerData
        {
            /// <summary>
            /// An Element's ELEM_FAMILY_PARAM parameter.
            /// </summary>
            public string Family { get; set; }


            /// <summary>
            /// A static value of 1 used for reporting.
            /// </summary>
            public static int Quantity { get; } = 1;


            /// <summary>
            /// An Element's FABRICATION_PART_NOTES built-in parameter. During hanger processing, this value
            /// is set before it is reported and is different between each hanger type.
            ///     Clevis Hanger:
            ///         1. Get the element's FABRICATION_PRODUCT_ENTRY parameter.
            ///         2. Set FABRICATION_PART_NOTES = FABRICATION_PRODUCT_ENTRY.
            ///         3. Report FABRICATION_PART_NOTES with a leading backslash.
            ///     Trapeze Hanger:
            ///         1. Get the rod offsets and duct width dimension values and convert them from feet to inches.
            ///         2. Sum those values, round up the sum, then truncate.
            ///         3. Set FABRICATION_PART_NOTES to the truncated sum.
            ///         4. Report FABRICATION_PART_NOTES.
            /// </summary>
            /// <remarks>This can result in a NumericOperationResult.FAILED.</remarks>
            public string FabricationNotes { get; set; }


            /// <summary>
            /// A FabricationPart's SupportRod ancillary useage in inches.
            /// </summary>
            /// <remarks>This can result in a NumericOperationResult.FAILED.</remarks>
            public double SupportRod { get; set; }


            /// <summary>
            /// A FabricationPart's LengthA dimension in inches.
            /// </summary>
            /// <remarks>This can result in a NumericOperationResult.FAILED.</remarks>
            public double LengthA { get; set; }


            /// <summary>
            /// A FabricationPart's LengthB dimension in inches.
            /// </summary>
            /// <remarks>This can result in a NumericOperationResult.FAILED.</remarks>
            public double LengthB { get; set; }


            /// <summary>
            /// An Element's FABRICATION_PART_ITEM_NUMBER parameter.
            /// </summary>
            public string ItemNumber { get; set; }


            /// <summary>
            /// A FabricationPart's ServiceAbbreviation property.
            /// </summary>
            public string ServiceAbbreviation { get; set; }


            /// <summary>
            /// An Element's FABRICATION_OFFSET_PARAM parameter.
            /// </summary>
            public string Offset { get; set; }
        }
    }
}
