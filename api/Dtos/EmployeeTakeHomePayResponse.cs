namespace EmployeePayroll.Dtos
{
    public class EmployeeTakeHomePayResponse
    {
        public string EmployeeNumber { get; set; } = string.Empty;

        public string StartingDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public decimal TakeHomePay { get; set; }
    }
}
