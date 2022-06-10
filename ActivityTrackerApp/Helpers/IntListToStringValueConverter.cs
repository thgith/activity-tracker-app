using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ActivityTrackerApp.Helpers
{
    public class IntListToStringValueConverter : ValueConverter<IList<int>, string>
    {
        public IntListToStringValueConverter() : base(le => ListToString(le), (s => StringToList(s)))
        {

        }
        public static string ListToString(IList<int> list)
        {
            if (list == null || list.Count() == 0)
            {
                return null;
            }

            return string.Join(",", list);
        }

        public static IList<int> StringToList(string strList)
        {
            if (strList == null || strList == string.Empty)
            {
                return null;
            }

            return (IList<int>)strList.Split(',').Select(i => Convert.ToInt32(i));
        }
    }
}