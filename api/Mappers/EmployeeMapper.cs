using EmployeePayroll.Dtos;
using EmployeePayroll.Models;

namespace EmployeePayroll.Mappers
{
    public static class EmployeeMapper
    {

        public static EmployeeResponse ToEmployeeResponse(this Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                EmployeeName = $"{employee.LastName}, {employee.FirstName} {employee.MiddleName}".Trim(),
                DateOfBirth = employee.DateOfBirth.ToString("MMMM d, yyyy"),
                DailyRate = employee.DailyRate,
                WorkingDays = employee.WorkingDays
            };
        }

        public static Employee ToEmployeeFromEmployeeRequest(this EmployeeRequest employeeRequest)
        {
            return new Employee
            {
                LastName = employeeRequest.LastName,
                FirstName = employeeRequest.FirstName,
                MiddleName = employeeRequest.MiddleName,
                DateOfBirth = employeeRequest.DateOfBirth,
                DailyRate = employeeRequest.DailyRate,
                WorkingDays = employeeRequest.WorkingDays
            };
        }


    }
}
