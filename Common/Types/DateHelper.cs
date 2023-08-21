using System;
using System.Globalization;

namespace Common.Types
{
    public class DateHelper
    {
        public static string ConvertToGregorian(string dateString)
        {
            var currentThreadCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-SA");
            DateTime now = new DateTime();
            var enCul = new CultureInfo("en-US");
            try
            {

                DateTime tempDate = DateTime.Parse(dateString);
                //  var result = tempDate.ToString("dd/MM/yyyy", enCul.DateTimeFormat);
                System.Threading.Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                return tempDate.ToString("yyyy-MM-dd", enCul.DateTimeFormat); ;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string ConvertDateToHijri(string input)
        {
            try
            {
                var arSA = CultureInfo.CreateSpecificCulture("ar-SA");
                var date = DateTime.ParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return date.ToString("yyyy-MM-dd", arSA);
            }
            catch
            {
                return input;
            }
        }
        public static string ToHijriString(DateTime date)
        {
            return ToHijriString(date, "dd-MM-yyyy");
        }
        public static string ToHijriString(DateTime date, string format)
        {
            return date.ToString(format, new CultureInfo("ar-SA"));
        }
    }
}
