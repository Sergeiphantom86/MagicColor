using System;

public static class NumberFormatter
{
    private static readonly string[] _suffixes = { "", "K", "M", "B", "T", "Q" };

    private const double ScalingFactor = 1000.0;
    private const int ScalingThreshold = 100000;
    private const int MaxFractionDigits = 1;
    private const double RoundingEpsilon = 0.0001;

    public static string FormatNumber(long number)
    {
        if (number == 0) return "0";

        bool isNegative = number < 0;

        double absNumber = Math.Abs(number);

        if (ShouldUseDirectFormatting(absNumber))
        {
            return FormatDirect(absNumber, isNegative);
        }

        return FormatWithSuffix(absNumber, isNegative);
    }

    private static bool ShouldUseDirectFormatting(double absNumber)
    {
        return absNumber < ScalingThreshold;
    }

    private static string FormatDirect(double absNumber, bool isNegative)
    {
        return isNegative ? $"-{absNumber:0}" : absNumber.ToString("0");
    }

    private static string FormatWithSuffix(double absNumber, bool isNegative)
    {
        (int suffixIndex, double scaledValue) = ScaleNumber(absNumber);

        scaledValue = Math.Round(scaledValue, MaxFractionDigits);

        return AddSignAndSuffix(FormatRoundedValue(scaledValue), suffixIndex, isNegative);
    }

    private static (int suffixIndex, double scaledValue) ScaleNumber(double absNumber)
    {
        int suffixIndex = 0;
        double scaledValue = absNumber;

        while (ShouldContinueScaling(scaledValue, suffixIndex))
        {
            scaledValue /= ScalingFactor;
            suffixIndex++;
        }

        return (suffixIndex, scaledValue);
    }

    private static bool ShouldContinueScaling(double value, int suffixIndex)
    {
        return value >= ScalingFactor &&
               suffixIndex < _suffixes.Length - 1;
    }

    private static string FormatRoundedValue(double value)
    {
        return Math.Abs(value - Math.Round(value, 0)) < RoundingEpsilon ?
            value.ToString("0") :
            value.ToString("0.0");
    }

    private static string AddSignAndSuffix(string value, int suffixIndex, bool isNegative)
    {
        return isNegative ?
            $"-{value}{_suffixes[suffixIndex]}" :
            $"{value}{_suffixes[suffixIndex]}";
    }
}