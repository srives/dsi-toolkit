using Autodesk.Revit.DB;
using DSI.Core;
using System;

namespace DSI.Filters
{
    public class OverkillFilter : Filter
    {
        private protected override bool Test(Element elem)
        {
            if (elem == null)
            {
                throw new ArgumentNullException(paramName: nameof(elem));
            }

            if (elem.Category != null 
                && (elem.Category.Name == "MEP Fabrication Hangers"
                    || elem.Category.Name == "MEP Fabrication Pipework"
                    || elem.Category.Name == "MEP Fabrication Ductwork"
                    || elem.Category.Name == "Pipe Accessories"
                    || elem.Category.Name == "Pipe Fittings")
                && elem is FabricationPart)
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
