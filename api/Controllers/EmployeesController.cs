using EmployeePayroll.Data;
using EmployeePayroll.Dtos;
using EmployeePayroll.Helpers;
using EmployeePayroll.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePayroll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees.Select(e => e.ToEmployeeResponse()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeByID([FromRoute] int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee.ToEmployeeResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            if (employeeRequest == null)
            {
                return BadRequest();
            }


            var employeeModel = employeeRequest.ToEmployeeFromEmployeeRequest();
            employeeModel.EmployeeNumber = EmployeeNumberGenerator.Generate(
                employeeModel.LastName,
                employeeModel.DateOfBirth
                );

            _context.Employees.Add(employeeModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeByID), new { id = employeeModel.Id }, employeeModel.ToEmployeeResponse());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateRequest employeeUpdateRequest, [FromRoute] int id)
        {
            if (employeeUpdateRequest == null) return BadRequest();

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            employee.LastName = employeeUpdateRequest.LastName;
            employee.FirstName = employeeUpdateRequest.FirstName;
            employee.MiddleName = employeeUpdateRequest.MiddleName;
            employee.DateOfBirth = employeeUpdateRequest.DateOfBirth;
            employee.DailyRate = employeeUpdateRequest.DailyRate;
            employee.WorkingDays = employeeUpdateRequest.WorkingDays;

            await _context.SaveChangesAsync();

            return Ok(employee.ToEmployeeResponse());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            
            if (employee == null) return NotFound();
            _context.Employees.Remove(employee);
     
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
