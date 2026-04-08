namespace EmployeePayroll.Helpers
{
    public class WorkingDayHelper
    {
        public static bool IsWorkingDay(DateTime date, string workingDays)
        {
            if (string.IsNullOrEmpty(workingDays)) return false;

            workingDays = workingDays.ToUpper();

            switch(date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return workingDays.Contains("M");
                case DayOfWeek.Tuesday:
                    return workingDays.Contains("T");
                case DayOfWeek.Wednesday:
                    return workingDays.Contains("W");
                case DayOfWeek.Thursday:
                    return workingDays.Contains("TH");
                case DayOfWeek.Friday:
                    return workingDays.Contains("F");
                case DayOfWeek.Saturday:
                    return workingDays.Contains("S");
                default:
                    return false;
            }
        }
    }
}
