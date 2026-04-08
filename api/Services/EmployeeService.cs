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

            employee.LastName = request.LastName;
            employee.FirstName = request.FirstName;
            employee.MiddleName = request.MiddleName;
            employee.DateOfBirth = request.DateOfBirth;
            employee.DailyRate = request.DailyRate;
            employee.WorkingDays = request.WorkingDays;

            await _context.SaveChangesAsync();

            return employee.ToEmployeeResponse();
        }
    }
}
