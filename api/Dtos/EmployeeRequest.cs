namespace EmployeePayroll.Dtos
{
    public class EmployeeRequest
    {
        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public Decimal DailyRate { get; set; }

        public string WorkingDays { get; set; } = string.Empty;
    }
}
