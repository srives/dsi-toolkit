﻿using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    public class TrapezeHangerSelectionFilter : Filter
    {
        /// <summary>
        /// The test function on which to filter the elements.
        /// </summary>
        /// <param name="elem">The element to be tested.</param>
        /// <returns>The result of the test.</returns>
        /// <remarks>
        /// This filter checks for the following:
        ///     1. That the element has a non-null category
        ///     2. The category name is "MEP Fabrication Hangers"
        ///     3. The element can be casted to a FabricationPart
        ///     4. The FabricationPart's rod count is 2
        /// </remarks>
        private protected override bool Test(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (elem.Category != null
                && elem.Category.Name == "MEP Fabrication Hangers" 
                && elem is FabricationPart fp 
                && fp.GetRodInfo().RodCount == 2)
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
