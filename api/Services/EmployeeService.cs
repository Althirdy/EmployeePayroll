using EmployeePayroll.Data;
using EmployeePayroll.Dtos;
using EmployeePayroll.Helpers;
using EmployeePayroll.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EmployeePayroll.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeResponse> CreateEmployeeAsync(EmployeeRequest request)
        {
            var employeeModel = request.ToEmployeeFromEmployeeRequest();

            if(employeeModel.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth cannot be in the future");
            }

            employeeModel.WorkingDays = employeeModel.WorkingDays.Trim().ToUpper();
            if(employeeModel.WorkingDays != "MWF" && employeeModel.WorkingDays != "TTHS")
            {
                throw new ArgumentException("Invalid working days");
            }

            employeeModel.EmployeeNumber = EmployeeNumberGenerator.Generate(
                employeeModel.LastName,
                employeeModel.DateOfBirth
                );

            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();

            return employeeModel.ToEmployeeResponse();
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id) ?? throw new ArgumentException("Employee not found");

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.ToListAsync();

            return employees.Select(e => e.ToEmployeeResponse());
        }

        public async Task<EmployeeResponse> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id) ?? throw new ArgumentException("Employee not found");

            return employee.ToEmployeeResponse();
        }

        public async Task<EmployeeResponse> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id) ?? throw new ArgumentException("Employee not found");

            if (!request.WorkingDays.Equals("MWF", StringComparison.OrdinalIgnoreCase) && !request.WorkingDays.Equals("TTH", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid working days");
            }

            employee.LastName = request.LastName;
            employee.FirstName = request.FirstName;
            employee.MiddleName = request.MiddleName;
            employee.DateOfBirth = request.DateOfBirth;
            employee.DailyRate = request.DailyRate;
            employee.WorkingDays = request.WorkingDays;

            await _context.SaveChangesAsync();

            return employee.ToEmployeeResponse();
        }

        public async Task<EmployeeTakeHomePayResponse> ComputeTakeHomePayAsync(int id, DateTime startDate, DateTime endDate)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id) ?? throw new ArgumentException("Employee not found");
            
            if(endDate < startDate)
            {
                throw new ArgumentException("End date must be greater than or equal to start date");
            }

            decimal totalPay = 0;
            var currentDate = startDate;
            
            while(currentDate <= endDate)
            {
                bool isBirthDay = currentDate.Month == employee.DateOfBirth.Month && currentDate.Day == employee.DateOfBirth.Day;
                if (isBirthDay) totalPay += employee.DailyRate;

                bool isWorkingDay = WorkingDayHelper.IsWorkingDay(currentDate,employee.WorkingDays);
                if (isWorkingDay) totalPay += employee.DailyRate * 2;

                //Console.WriteLine($"Current Day: {currentDate.DayOfWeek}");
                //Console.WriteLine($"Current totalPay: {totalPay}");

                currentDate = currentDate.AddDays(1);
            }

            return new EmployeeTakeHomePayResponse
            {
                EmployeeNumber = employee.EmployeeNumber,
                StartingDate = startDate.ToString("MMMM dd yyyy"),
                EndDate = endDate.ToString("MMMM dd yyyy"),
                TakeHomePay = totalPay
            };
        }
    }
}
