using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DSI.Commands.Helpers
{
    public static class CommandHelper
    {
        public static ElementArray GetUserSelectedElementsByFilter(UIApplication application, ISelectionFilter filter)
        {
            if (application == null)
            {
                throw new ArgumentNullException(paramName: nameof(application));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(paramName: nameof(filter));
            }

            UIDocument uidoc = application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            ICollection<ElementId> existingSelection = selection.GetElementIds();

            ElementArray typeList = new ElementArray();

            if (existingSelection.Count != 0)
            {
                foreach (ElementId elemId in existingSelection)
                {
                    Element elem = doc.GetElement(elemId);

                    if (filter.AllowElement(elem) == true)
                    {
                        typeList.Append(elem);
                    }
                }
            }
            else
            {
                IList<Reference> elementReference
                    = selection.PickObjects(ObjectType.Element, filter, "Select Model Elements");

                foreach (Reference reference in elementReference)
                {
                    typeList.Append(doc.GetElement(reference));
                }
            }

            return typeList;
        }


        public static string FeetToFractionString(double pFeet)
        {
            if (pFeet == -1)
            {
                return "-1";
            }
            else
            {
                // Initial data extraction
                int feet = (int)Math.Floor(pFeet);
                double remainder = pFeet - (int)Math.Floor(pFeet);

                // Running totals of fractional inches
                int inches = 0;
                int oneHalfs = 0;
                int oneFourths = 0;
                int oneEights = 0;
                int oneSixteenths = 0;

                // Conversion factors
                const double INCH_TO_FOOT = 0.08333333333;
                const double SIXTEENTH_INCH_TO_FOOT = INCH_TO_FOOT / 16;

                // Instead of allocating a bool to each running total, IF any
                // running totals are greater than 0 after the conversion then
                // their allocated bit is switched "on":
                //
                //  (1/2 in) [1/0]     (1/4 in) [1/0]     (1/8 in) [1/0]     (1/16 in) [1/0]
                //         +8                 +4                 +2                  +1
                //
                int bitwiseCheck = 0b_0000;

                // Calculates the decimal feet to sixteenth inches for further
                // calculations
                while (remainder >= SIXTEENTH_INCH_TO_FOOT)
                {
                    oneSixteenths++;
                    remainder -= SIXTEENTH_INCH_TO_FOOT;
                }

                // Convert sixteenth inches to eighth inches and check to see if
                // any sixteenth inches remain
                while (oneSixteenths >= 2) { oneEights++; oneSixteenths -= 2; }
                if (oneSixteenths > 0) bitwiseCheck += 0b_0001;

                // Convert eighth inches to fourth inches and check to see if
                // any eighth inches remain
                while (oneEights >= 2) { oneFourths++; oneEights -= 2; }
                if (oneEights > 0) bitwiseCheck += 0b_0010;

                // Convert fourth inches to half inches and check to see if
                // any fourth inches remain
                while (oneFourths >= 2) { oneHalfs++; oneFourths -= 2; }
                if (oneFourths > 0) bitwiseCheck += 0b_0100;

                // Convert half inches to inches and check to see if
                // any half inches remain
                while (oneHalfs >= 2) { inches++; oneHalfs -= 2; }
                if (oneHalfs > 0) bitwiseCheck += 0b_1000;

                // It's possible for another "foot" to be added to the human
                // readable measurement IF the precision loss generates 12
                // full inches 
                if (inches == 12) { feet++; inches = 0; }

                // Checks for the smallest existing inch measurement using
                // bitwise AND (&) and constructs the strings based on that
                if ((bitwiseCheck & 0b_0001) == 0b_0001)
                {
                    oneSixteenths +=
                        (oneEights * 2) + (oneFourths * 4) + (oneHalfs * 8);

                    return $"{feet}'{inches} {oneSixteenths}/16\"";
                }
                else if ((bitwiseCheck & 0b_0010) == 0b_0010)
                {
                    oneEights +=
                        (oneFourths * 2) + (oneHalfs * 4);

                    return $"{feet}'{inches} {oneEights}/8\"";
                }
                else if ((bitwiseCheck & 0b_0100) == 0b_0100)
                {
                    oneFourths +=
                        (oneHalfs * 2);

                    return $"{feet}'{inches} {oneFourths}/4\"";
                }
                else if ((bitwiseCheck & 0b_1000) == 0b_1000)
                {
                    return $"{feet}'{inches} {oneHalfs}/2\"";
                }
                else
                {
                    return $"{feet}'{inches}\"";
                }
            }
        }
    }
}
