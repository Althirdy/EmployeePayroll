namespace EmployeePayroll.Dtos
{
    public class EmployeeUpdateRequest
    {
        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal DailyRate { get; set; }
        public string WorkingDays { get; set; } = string.Empty;
    }
}
