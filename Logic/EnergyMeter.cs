using System.Text.RegularExpressions;

namespace Reader.Logic;

public class EnergyMeter
{
    public string HighCounter { get; private set; }
    public string LowCounter { get; private set; }

    public bool IsValid { get => HighCounter != null && LowCounter != null; }

    public bool AddReading(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        raw = raw.Replace(" ", string.Empty);

        var numbersOnlyRegex = "^[0-9]*$";
        Match match = Regex.Match(raw, numbersOnlyRegex, RegexOptions.IgnoreCase);

        if (!match.Success)
            return false;

        if (raw.Length < 5)
            return false;

        if (raw.Length > 6)
            return false;

        if (raw.Length == 6)
            raw = raw.Substring(0, 5);

        if (HighCounter == null)
        {
            HighCounter = raw;
            return true;
        }
        else if (LowCounter == null)
        {
            LowCounter = raw;
            return true;
        }
        else
        {
            return false;
        }
    }
}