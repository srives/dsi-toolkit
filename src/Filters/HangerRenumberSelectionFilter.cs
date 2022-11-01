using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSIToolkit.Commands.Filters
{
    public class HangerRenumberSelectionFilter : ISelectionFilter
    {
        /// <summary>
        /// A list of hanger CIDs to filter by.
        /// </summary>
        private readonly List<int> hangerCids = new List<int>() 
        { 
            838, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool AllowElement(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (
                elem.Category != null &&
                elem.Category.Name == "MEP Fabrication Hangers" &&
                elem is FabricationPart fp &&
                hangerCids.Exists(x => fp.ItemCustomId == x ?  true : false))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
