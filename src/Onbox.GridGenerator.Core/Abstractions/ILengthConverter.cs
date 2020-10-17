namespace Onbox.GridGenerator.Abstractions
{
    public interface ILengthConverter
    {
        double ConvertInternalToMillimeters(double feet);
        double ConvertMillimetersToInternal(double millimiters);
    }
}
