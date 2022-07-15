using ActivityTrackerApp.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ActivityTrackerApp.Helpers;

/// <summary>
/// Converts a list of strings to a string separated by commas and vice versa.
/// Mainly needed to convert the <see cref="Activity.Tags"/> field 
/// for the corresponding DB column.
/// </summary>
public class StringListToStringValueConverter : ValueConverter<IList<string>, string>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public StringListToStringValueConverter() : base(le => ListToString(le), (s => StringToList(s)))
    {

    }

    /// <summary>
    /// Converts the list of strings to a comma separated string.
    /// </summary>
    /// <param name="value">The list to convert.</param>
    public static string ListToString(IList<string> value)
    {
        if (value == null || value.Count() == 0)
        {
            return null;
        }

        return string.Join(",", value);
    }

    /// <summary>
    /// Converts the comma-separated string to a list of strings.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    public static IList<string> StringToList(string value)
    {
        if (value == null || value == string.Empty)
        {
            return null;
        }

        return value.Split(','); ;
    }
}