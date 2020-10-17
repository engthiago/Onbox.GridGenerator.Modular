using Autodesk.Revit.DB;
using Onbox.GridGenerator.Abstractions;

namespace Onbox.GridGenerator.Services
{
    public class RevitLengthConverter : ILengthConverter
    {
        public double ConvertInternalToMillimeters(double feet)
        {
            double millimeters = UnitUtils.ConvertFromInternalUnits(feet, DisplayUnitType.DUT_MILLIMETERS);
            return millimeters;
        }

        public double ConvertMillimetersToInternal(double millimiters)
        {
            double feet = UnitUtils.ConvertToInternalUnits(millimiters, DisplayUnitType.DUT_MILLIMETERS);
            return feet;
        }
    }
}
