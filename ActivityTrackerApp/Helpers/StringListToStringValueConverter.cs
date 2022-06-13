using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ActivityTrackerApp.Helpers;

/// <summary>
/// Converts a list of strings to a string separated by commas and vice versa.
/// Mainly needed to convert the <see cref="Activity.Tags"/> field 
/// for the corresponding DB column.
/// </summary>
public class StringListToStringValueConverter : ValueConverter<IList<string>, string>
{
    public StringListToStringValueConverter() : base(le => ListToString(le), (s => StringToList(s)))
    {

    }

    public static string ListToString(IList<string> value)
    {
        if (value == null || value.Count() == 0)
        {
            return null;
        }

        return string.Join(",", value);
    }

    public static IList<string> StringToList(string value)
    {
        if (value == null || value == string.Empty)
        {
            return null;
        }

        return value.Split(','); ;
    }
}