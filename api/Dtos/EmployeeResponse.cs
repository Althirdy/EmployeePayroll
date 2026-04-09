namespace EmployeePayroll.Dtos
{
    public class EmployeeResponse
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string DateOfBirthValue { get; set; } = string.Empty;

        public Decimal DailyRate { get; set; }

        public string WorkingDays { get; set; } = string.Empty;
    }
}
