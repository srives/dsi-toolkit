using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using System.Linq;
using System.CodeDom;
using System.Windows;

namespace DSI.Core
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        /// <summary>
        /// The execution context for the addin.
        /// </summary>
        private protected ApplicationContext context;


        /// <summary>
        /// Provides access to the application logging interface.
        /// </summary>
        private protected ApplicationLog log;


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                context = new ApplicationContext(commandData);
                log = new ApplicationLog(context.InstallDirectory, GetType().FullName);
                return Main(commandData);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                log.Logger.Debug("operation was cancelled via Revit dialog");
                return Result.Cancelled;
            }
            catch (OperationCanceledException)
            {
                log.Logger.Debug("operation was cancelled via Windows Forms dialog");
                return Result.Cancelled;
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show($"{e.ParamName} was passed as a null value", "DSI ToolKit Error");
                log.Logger.Debug(e, $"{e.ParamName} was passed as a null value - command did not execute");
                return Result.Failed;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "DSI ToolKit Error");
                log.Logger.Debug(e, "an exception occured");
                return Result.Failed;
            }
        }


        /// <summary>
        /// The main logic for a command.
        /// </summary>
        /// <param name="commandData">The command data passed by the Application.</param>
        /// <returns>The result of the command.</returns>
        private protected virtual Result Main(ExternalCommandData commandData)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// A more explicit redefinition of the File.Copy(string, string, bool) method.
        /// </summary>
        /// <param name="from">The source file path.</param>
        /// <param name="to">The path to the destination.</param>
        private void CopyFileToLocationWithOverwrite(string from, string to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(paramName: nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(paramName: nameof(to));
            }

            try
            {
                File.Copy(from, to, true);
            }
            catch (UnauthorizedAccessException)
            {
                log.Logger.Error($"the user does not permission to write to {to}");
            }
            catch (DirectoryNotFoundException e)
            {
                log.Logger.Error(e, "the specified directory was not found");
            }
            catch (FileNotFoundException e)
            {
                log.Logger.Error(e, "the specified file was not found");
            }
            catch (Exception e)
            {
                log.Logger.Error(e, "an exception occured");
            }
        }


        /// <summary>
        /// Gets a specific FabricationAncillaryUsage on a FabricationPart.
        /// </summary>
        /// <param name="fp">The FabricationPart on which the FabricationAncillaryUsage is hosted.</param>
        /// <param name="type">The FabricationAnciallaryType to search for.</param>
        /// <returns>The desired FabricationAncillaryUsage, null if it isn't found.</returns>
        protected static FabricationAncillaryUsage GetAncillaryUsage(FabricationPart fp, FabricationAncillaryType type)
        {
            if (fp == null)
            {
                throw new ArgumentNullException(paramName: nameof(fp));
            }

            FabricationAncillaryUsage result = null;
            var usages = fp.GetPartAncillaryUsage();

            foreach (var usage in usages)
            {
                if (usage.Type == type)
                {
                    result = usage;
                    break;
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the width or diameter of a specific FabricationAncillaryUsage on a FabricationPart.
        /// </summary>
        /// <param name="fp">The FabricationPart on which the FabricationAncillaryUsage is hosted.</param>
        /// <param name="type">The FabricationAnciallaryType to search for.</param>
        /// <returns>The width or diameter of the FabricationAncillaryUsage, or a NumericOperationResult.FAILED if it is not found.</returns>
        protected double GetAncillaryUsageWidthOrDiameter(FabricationPart fp, FabricationAncillaryType type)
        {
            if (fp == null)
            {
                throw new ArgumentNullException(paramName: nameof(fp));
            }

            FabricationAncillaryUsage useage = GetAncillaryUsage(fp, type);

            if (useage != null)
            {
                return useage.AncillaryWidthOrDiameter;
            }
            else
            {
                log.Logger.Warning($"the usage of type {type} could not be found for FabricationPart {fp.Id}");
                return NumericOperationResult.FAILED;
            }
        }


        /// <summary>
        /// Gets a FabricationPart's dimension by name.
        /// </summary>
        /// <param name="fp">The FabricationPart where the dimension is hosted.</param>
        /// <param name="name">The name of the dimension.</param>
        /// <returns>The found FabricationDimensionDefinition, or null if one is not found.</returns>
        protected FabricationDimensionDefinition GetDimensionByName(FabricationPart fp, string name)
        {
            if (fp == null)
            {
                log.Logger.Debug(
                    exception: new ArgumentNullException(paramName: nameof(fp)),
                    messageTemplate: "the passed FabricationPart was null");

                return null;
            }

            if (name == null)
            {
                return null;
            }

            var dims = fp.GetDimensions();
            FabricationDimensionDefinition result = null;

            foreach (var dim in dims)
            {
                if (dim.Name == name)
                {
                    result = dim;
                    break;
                }
            }

            return result;
        }


        /// <summary>
        /// Gets a FabricationPart's dimension by name or a set of fallback names.
        /// </summary>
        /// <param name="fp">The FabricationPart where the dimension is hosted.</param>
        /// <param name="name">The name of the dimension.</param>
        /// <param name="fallbacks">
        /// Any alternate names for the dimensions. These should be added to the fallback array in order of priority.
        /// </param>
        /// <returns>The found FabricationDimensionDefinition, or null if one is not found.</returns>
        protected FabricationDimensionDefinition GetDimensionByName(FabricationPart fp, string name, string[] fallbacks)
        {
            if (fp == null)
            {
                log.Logger.Debug(
                    exception: new ArgumentNullException(paramName: nameof(fp)),
                    messageTemplate: "the passed FabricationPart was null");

                return null;
            }

            if (name == null
                && (fallbacks == null || fallbacks.Length == 0))
            {
                return null;
            }

            var dims = fp.GetDimensions();
            FabricationDimensionDefinition result = null;

            if (name != null)
            {
                foreach (var dim in dims)
                {
                    if (dim.Name == name)
                    {
                        result = dim;
                    }
                }
            }

            if (result == null && fallbacks != null)
            {
                foreach (var fallback in fallbacks)
                {
                    result = GetDimensionByName(fp, fallback);

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Wrapper for FabricationPart's GetDeimensionValue(FabricationDimensionDefinition) method. 
        /// </summary>
        /// <param name="fp">The FabricationPart where the dimension is hosted.</param>
        /// <param name="def">The dimension definition used to lookup the dimension value.</param>
        /// <returns>The value of the FabricationPart's dimension.</returns>
        protected double GetDimensionValueByDefinition(FabricationPart fp, FabricationDimensionDefinition def)
        {
            if (fp == null)
            {
                throw new ArgumentNullException(paramName: nameof(fp));
            }

            if (def == null)
            {
                log.Logger.Warning($"the unnamed dimension could not be found for FabricationPart {fp.Id}");
                return NumericOperationResult.FAILED;
            }

            return fp.GetDimensionValue(def);
        }


        /// <summary>
        /// Get's the value of a FabricationDimensionDefinition, found by name.
        /// </summary>
        /// <param name="fp">The FabricationPart where the dimension is hosted.</param>
        /// <param name="name">The name of the dimension.</param>
        /// <returns>The found FabricationDimensionDefinition, or a NumericOperationResult.FAILED if one is not found.</returns>
        protected double GetDimensionValueByName(FabricationPart fp, string name)
        {
            if (fp == null)
            {
                throw new ArgumentNullException(paramName: nameof(fp));
            }

            var dim = GetDimensionByName(fp, name);

            if (dim != null)
            {
                return fp.GetDimensionValue(dim);
            }
            else
            {
                log.Logger.Warning($"the dimension named {name} could not be found for FabricationPart {fp.Id}");
                return NumericOperationResult.FAILED;
            }
        }


        /// <summary>
        /// Get's the value of a FabricationDimensionDefinition, found by name or a set of fallback names.
        /// </summary>
        /// <param name="fp">The FabricationPart where the dimension is hosted.</param>
        /// <param name="name">The name of the dimension.</param>
        /// <param name="fallbacks">
        /// Any alternate names for the dimensions. These should be added to the fallback array in order of priority.
        /// </param>
        /// <returns>The found FabricationDimensionDefinition, or a NumericOperationResult.FAILED if one is not found.</returns>
        protected double GetDimensionValueByName(FabricationPart fp, string name, string[] fallbacks)
        {
            if (fp == null)
            {
                throw new ArgumentNullException(paramName: nameof(fp));
            }

            var dim = GetDimensionByName(fp, name, fallbacks);

            if (dim != null)
            {
                return fp.GetDimensionValue(dim);
            }
            else
            {
                log.Logger.Warning($"the dimension named {name} + its fallback values could not be found for FabricationPart {fp.Id}");
                return NumericOperationResult.FAILED;
            }
        }


        /// <summary>
        /// Gets all elements selected by the user.
        /// If the user has any elements already selected when this method is called,
        /// then the user will NOT be prompted to select any elements and this method
        /// will return the ElementArray of the current selection.
        /// </summary>
        /// <param name="application">The Revit UI instance.</param>
        /// <returns>All elements selected by the user.</returns>
        protected static ElementArray GetUserSelectedElements(UIApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            var uidoc = application.ActiveUIDocument;
            var doc = uidoc.Document;
            var selection = uidoc.Selection;
            var existingSelection = selection.GetElementIds();
            var typeList = new ElementArray();

            if (existingSelection.Count != 0)
            {
                foreach (var elemId in existingSelection)
                {
                    typeList.Append(doc.GetElement(elemId));
                }
            }
            else
            {
                var elementReference = selection.PickObjects(ObjectType.Element, "Select Model Elements");
                foreach (var reference in elementReference)
                {
                    typeList.Append(doc.GetElement(reference));
                }
            }

            return typeList;
        }


        /// <summary>
        /// Gets all elements selected by the user.
        /// If the user has any elements already selected when this method is called,
        /// then the user will NOT be prompted to select any elements and this method
        /// will return the ElementArray of the current selection.
        /// </summary>
        /// <param name="application">The Revit UI instance.</param>
        /// <param name="filter">The filter used to select the elements.</param>
        /// <returns>The filtered element array.</returns>
        protected static ElementArray GetUserSelectedElementsByFilter(UIApplication application, ISelectionFilter filter)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(paramName: nameof(filter));
            }

            var uidoc               = application.ActiveUIDocument;
            var doc                 = uidoc.Document;
            var selection           = uidoc.Selection;
            var existingSelection   = selection.GetElementIds();
            var typeList            = new ElementArray();

            if (existingSelection.Count != 0)
            {
                foreach (var elemId in existingSelection)
                {
                    var elem = doc.GetElement(elemId);
                    if (filter.AllowElement(elem) == true)
                    {
                        typeList.Append(elem);
                    }
                }
            }
            else
            {
                var elementReference = selection.PickObjects(ObjectType.Element, filter, "Select Model Elements");
                foreach (var reference in elementReference)
                {
                    typeList.Append(doc.GetElement(reference));
                }
            }

            return typeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="quantityFieldName"></param>
        /// <param name="excludedProperties"></param>
        /// <returns></returns>
        private protected List<T> CountAndReturnUniques<T> (List<T> data, string quantityFieldName, string[] excludedProperties)
        {
            if (data.Count != 0)
            {
                var working = data;
                var uniques = new List<T>();
                var master = working[0];
                var masterProps = master.GetType().GetProperties();
                var quantity = 0;

                for (var i = data.Count - 1; i >= 0; i--)
                {
                    var apprentice = working[i];
                    var apprenticeProps = apprentice.GetType().GetProperties();
                    var unique = false;
                    var results = new bool[apprenticeProps.Length - excludedProperties.Length - 1];
                    var iter = 0;
                    var resultsIter = 0;

                    while (iter < apprenticeProps.Length)
                    {
                        var mProp = masterProps[iter];
                        var aProp = apprenticeProps[iter];
                        var inExcluded = excludedProperties.Contains(aProp.Name);
                        var isQuantity = aProp.Name == quantityFieldName;

                        if (!inExcluded && !isQuantity)
                        {
                            var aValue = aProp.GetValue(apprentice);
                            var mValue = mProp.GetValue(master);
                            results[resultsIter] = aValue.Equals(mValue);
                            resultsIter++;
                        }

                        iter++;
                    }

                    if (results.Contains(false))
                    {
                        unique = true;
                    }

                    if (!unique)
                    {
                        quantity++;
                        working.RemoveAt(i);
                    }
                }

                var masterQuantity = master.GetType().GetProperty(quantityFieldName);
                masterQuantity.SetValue(master, quantity);
                uniques.Add(master);

                if (working.Count > 0)
                {
                    return uniques.Concat(CountAndReturnUniques(working, quantityFieldName, excludedProperties)).ToList();
                }
                else
                {
                    return uniques;
                }
            }
            else
            {
                return data;
            }
        }
    }

    public static class NumericOperationResult
    {
        public const double FAILED = -1;
        public const double SUCCEDED = 1;
    }
}
