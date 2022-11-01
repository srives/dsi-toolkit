using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    public class PinSelectionFilter : Filter
    {
        /// <summary>
        /// The test function on which to filter the elements.
        /// </summary>
        /// <param name="elem">The element to be tested.</param>
        /// <returns>The result of the test.</returns>
        /// <remarks>
        /// This filter checks for the following:
        /// 1. That the element has a non-null category
        /// 2. The category name is "MEP Fabrication Hangers"
        /// 3. The element can be casted to a FabricationPart
        /// </remarks>
        private protected override bool Test(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (elem.Category != null
                && (elem.Category.Name == "MEP Fabrication Hangers"
                    || elem.Category.Name == "MEP Fabrication Pipework"
                    || elem.Category.Name == "MEP Fabrication Ductwork"))
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
