using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    class TigerstopSelectionFilter : Filter
    {
        private readonly int copperPipeCid = 2041;

        /// <summary>
        /// The test function on which to filter the elements.
        /// </summary>
        /// <param name="elem">The element to be tested.</param>
        /// <returns>The result of the test.</returns>
        /// <remarks>
        /// This filter checks for the following:
        /// 1. That the element has a non-null category
        /// 2. The category name is "MEP Fabrication Pipework"
        /// 3. The element's FABRICATION_PART_MATERIAL parameter contains the string "Copper"
        /// 4. The element can be casted to a FabricationPart
        /// 5. The FabricationPart's ItemCustomId (CID) is 2041
        /// </remarks>
        private protected override bool Test(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (elem.Category != null 
                && elem.Category.Name == "MEP Fabrication Pipework"
                && elem.get_Parameter(BuiltInParameter.FABRICATION_PART_MATERIAL).AsValueString().Contains("Copper")
                && elem is FabricationPart fp
                && fp.ItemCustomId == copperPipeCid)
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
