using System;

public class NumberFormatter
{
    private const double ScalingFactor = 1000.0;
    private const int ScalingThreshold = 100000;
    private const int MaxFractionDigits = 1;
    private const double RoundingEpsilon = 0.0001;

    private readonly string[] _suffixesEN = { string.Empty, "K", "M", "B", "T", "Q" };
    private readonly string[] _suffixesRU = { string.Empty, "Ò", "Ì", "Ìð", "Ò", "Ê" };
    private readonly string[] _suffixesTR = { string.Empty, "B", "M", "Mr", "Tr", "Kn" };

    private readonly IProgressSaver _progressSaver = new ProgressSaver();

    public string FormatNumber(long number)
    {
        if (number == 0)
            return "0";

        bool isNegative = number < 0;
        double absNumber = Math.Abs(number);

        string[] suffixes = GetSuffixesForCurrentLanguage();

        if (ShouldUseDirectFormatting(absNumber))
        {
            return FormatDirect(absNumber, isNegative);
        }

        return FormatWithSuffix(absNumber, isNegative, suffixes);
    }

    private string[] GetSuffixesForCurrentLanguage()
    {
        return _progressSaver.Saves.CurrentLanguage switch
        {
            "ru" => _suffixesRU,
            "tr" => _suffixesTR,
            _ => _suffixesEN,
        };
    }

    private bool ShouldUseDirectFormatting(double absNumber)
    {
        return absNumber < ScalingThreshold;
    }

    private string FormatDirect(double absNumber, bool isNegative)
    {
        return isNegative ? $"-{absNumber:0}" : absNumber.ToString("0");
    }

    private string FormatWithSuffix(double absNumber, bool isNegative, string[] suffixes)
    {
        (int suffixIndex, double scaledValue) = ScaleNumber(absNumber, suffixes);

        scaledValue = Math.Round(scaledValue, MaxFractionDigits);

        return AddSignAndSuffix(FormatRoundedValue(scaledValue), suffixIndex, isNegative, suffixes);
    }

    private (int suffixIndex, double scaledValue) ScaleNumber(double absNumber, string[] suffixes)
    {
        int suffixIndex = 0;
        double scaledValue = absNumber;

        while (ShouldContinueScaling(scaledValue, suffixIndex, suffixes))
        {
            scaledValue /= ScalingFactor;
            suffixIndex++;
        }

        return (suffixIndex, scaledValue);
    }

    private bool ShouldContinueScaling(double value, int suffixIndex, string[] suffixes)
    {
        return value >= ScalingFactor &&
               suffixIndex < suffixes.Length - 1;
    }

    private string FormatRoundedValue(double value)
    {
        return Math.Abs(value - Math.Round(value, 0)) < RoundingEpsilon ? value.ToString("0") : value.ToString("0.0");
    }

    private string AddSignAndSuffix(string value, int suffixIndex, bool isNegative, string[] suffixes)
    {
        return isNegative ? $"-{value} {suffixes[suffixIndex]}" : $"{value} {suffixes[suffixIndex]}";
    }
}