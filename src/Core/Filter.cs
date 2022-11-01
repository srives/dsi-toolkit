using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace DSI.Core
{
    public class Filter : ISelectionFilter
    {
        /// <summary>
        /// Defines the conditions on how elements should be filtered.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns>The result of the filter.</returns>
        public bool AllowElement(Element elem)
        {
            try
            {
                return Test(elem);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Definies the conditions on how element references should be filtered.
        /// </summary>
        /// <param name="reference">The element reference.</param>
        /// <param name="position">The position object.</param>
        /// <returns>The result of the filter.</returns>
        /// <remarks>
        /// This seems to be a special case of filtering that, currently, 
        /// is not used in the toolkit.
        /// </remarks>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }

        /// <summary>
        /// The test function on which to filter the elements.
        /// </summary>
        /// <param name="elem">The element to be tested.</param>
        /// <returns>The result of the test.</returns>
        /// <remarks>This should be overridden by inheriting classes.</remarks>
        private protected virtual bool Test(Element elem)
        {
            throw new NotImplementedException();
        }
    }
}
