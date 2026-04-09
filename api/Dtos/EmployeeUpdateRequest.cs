namespace EmployeePayroll.Dtos
{
    public class EmployeeUpdateRequest
    {
        public decimal DailyRate { get; set; }
        public string WorkingDays { get; set; } = string.Empty;
    }
}
