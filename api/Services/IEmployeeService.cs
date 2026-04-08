using EmployeePayroll.Dtos;

namespace EmployeePayroll.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync();
        Task<EmployeeResponse> GetEmployeeByIdAsync(int id);
        Task<EmployeeResponse> CreateEmployeeAsync(EmployeeRequest request);
        Task<EmployeeResponse> UpdateEmployeeAsync(int id,EmployeeUpdateRequest request);
        Task<bool> DeleteEmployeeAsync(int id);

    }
}
