using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    class PipeworkSleeveFilter : Filter
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
        /// 3. The element can be casted to a FamilyInstance
        /// 4. If the FamilyInstance symbol is one of the following:
        ///     a. "DSI Round Floor Sleeve"
        ///     b. "DSI Round Wall Sleeve"
        ///     c. "Rectangular Floor Sleeve"
        ///     d. "eM_SV_Rnd_Flr_Sleeve" -> added 8.27.20
        /// </remarks>
        private protected override bool Test(Element elem)
        {
            var pipeAccessoriesCheck
                = elem.Category != null
                    && elem.Category.Name == "Pipe Accessories"
                    && elem is FamilyInstance fi1
                    && (fi1.Symbol.FamilyName == "DSI Round Floor Sleeve"
                        || fi1.Symbol.FamilyName == "DSI Round Wall Sleeve"
                        || fi1.Symbol.FamilyName == "Rectangular Floor Sleeve"
                        || fi1.Symbol.FamilyName == "Rectangular Wall Sleeve"
                        || fi1.Symbol.FamilyName == "Round Floor Sleeve"
                        || fi1.Symbol.FamilyName == "Round Wall Sleeve");

            var pipeFittingsCheck
                = elem.Category != null
                    && elem.Category.Name == "Pipe Fittings"
                    && elem is FamilyInstance fi2
                    && (fi2.Symbol.FamilyName == "eM_SV_Rnd_Flr_Sleeve");

            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (pipeAccessoriesCheck || pipeFittingsCheck)
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
