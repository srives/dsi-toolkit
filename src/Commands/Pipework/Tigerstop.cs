using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSI.Filters;
using DSI.Commands.Helpers;
using DSI.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace DSI.Commands.Pipework
{
    [Transaction(TransactionMode.Manual)]
    public abstract class Tigerstop : IExternalCommand
    {
        public const int InchesInFoot = 12;
        public const int LengthDimensionIndex = 1;

        private protected string PackageName = null;

        public abstract string LocationPath { get; }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (commandData == null)
                {
                    throw new ArgumentNullException(paramName: nameof(commandData));
                }

                List<Data> pipes = new List<Data>();

                using (InputBoxWindow ibw = new InputBoxWindow("Package Name", "Enter Package Name:"))
                {
                    if (ibw.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        PackageName = ibw.UserInput;
                        using (ElementArray filteredElements = CommandHelper.GetUserSelectedElementsByFilter(commandData.Application, new TigerstopSelectionFilter()))
                        {
                            foreach (Element elem in filteredElements)
                            {
                                pipes.Add(ProcessElement(elem, PackageName));
                            }
                        }

                        using (var writer = new StreamWriter(LocationPath))
                        {
                            writer.WriteLine("\"Job Name\",\"Item No\",\"QTY\",\"Description\",\"Cut Length\",\"Label Length (ft)\",\"Material\",\"Spool\"");
                            writer.Flush();

                            foreach (Data p in pipes)
                            {
                                writer.WriteLine($"{p.JobName},{p.ItemNumber},{Data.Quantity},{p.Description},{p.CutLength},{p.LabelLength},{p.Material},{p.Spool}");
                                writer.Flush();
                            }

                            writer.Close();
                        }
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }


        private static Data ProcessElement(Element elem, string packageName)
        {
            Data pipe = new Data();
            FabricationPart fp = elem as FabricationPart;

            double pipeLength;
            string material
                = elem.get_Parameter(BuiltInParameter.FABRICATION_PART_MATERIAL).AsValueString();
            int trimIndexBegin = material.IndexOf(":", StringComparison.InvariantCulture);
            int trimIndexEnd = material.IndexOf("{", StringComparison.InvariantCulture);

            IList<FabricationDimensionDefinition> dims = fp.GetDimensions();
            FabricationDimensionDefinition lengthDimension = null;
            foreach (FabricationDimensionDefinition dim in dims)
            {
                if (dim.Name == "Length")
                {
                    lengthDimension = dim;
                    break;
                }
            }

            if (lengthDimension != null)
            {
                pipeLength = fp.GetDimensionValue(lengthDimension);
            }
            else
            {
                pipeLength = -1;
            }

            if (trimIndexBegin == -1 && trimIndexEnd != -1)
            {
                int length = (trimIndexEnd - 1);
                if (length > 0)
                {
                    material = material.Substring(0, length);
                }
            }
            else if (trimIndexBegin != -1 && trimIndexEnd == -1)
            {
                int length = material.Length - (trimIndexBegin + 2);
                if (length > 0)
                {
                    material = material.Substring(trimIndexBegin + 2, length);
                }
            }
            else if (trimIndexBegin != -1 && trimIndexEnd != -1)
            {
                int length = (trimIndexEnd - 1) - (trimIndexBegin + 2);
                if (length > 0)
                {
                    material = material.Substring(trimIndexBegin + 2, length);
                }
            }

            pipe.JobName
                = $"\"{packageName}\"";
            pipe.ItemNumber
                = $"\"{fp.ItemNumber}\"";
            pipe.Description
                = $"\"{elem.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY).AsString()}\"";
            pipe.CutLength
                = $"\"{(pipeLength * InchesInFoot).ToString("N6", CultureInfo.InvariantCulture)}\"";
            pipe.LabelLength
                = $"\"{CommandHelper.FeetToFractionString(pipeLength)}\"";
            pipe.Material
                = $"\"{material}\"";
            pipe.Spool
                = $"\"\"";

            return pipe;
        }


        private class Data
        {
            public string JobName { get; set; }
            public string ItemNumber { get; set; }
            public static string Quantity { get => "\"1\""; }
            public string Description { get; set; }
            public string CutLength { get; set; }
            public string LabelLength { get; set; }
            public string Material { get; set; }
            public string Spool { get; set; }
        }
    }
}
