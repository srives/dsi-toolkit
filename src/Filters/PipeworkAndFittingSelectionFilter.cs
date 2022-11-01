using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    public class PipeworkAndFittingSelectionFilter : Filter
    {
        /// <summary>
        /// The test function on which to filter the elements.
        /// </summary>
        /// <param name="elem">The element to be tested.</param>
        /// <returns>The result of the test.</returns>
        /// <remarks>
        /// This filter checks for the following:
        /// 1. That the element has a non-null category
        /// 2. The category name is "MEP Fabrication Pipework"
        /// 3. The element can be casted to a FabricationPart
        /// 4. The FabricationPart doesn't have a service type of 57, 58, OR 62
        /// </remarks>
        private protected override bool Test(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (elem.Category != null 
                && elem.Category.Name == "MEP Fabrication Pipework" 
                && elem is FabricationPart fp
                && fp.ServiceType != 57 
                && fp.ServiceType != 58
                && fp.ServiceType != 62)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
