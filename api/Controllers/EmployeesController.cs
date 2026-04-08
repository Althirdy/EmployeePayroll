using EmployeePayroll.Data;
using EmployeePayroll.Dtos;
using EmployeePayroll.Helpers;
using EmployeePayroll.Mappers;
using EmployeePayroll.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePayroll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(AppDbContext appDbContext, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
           var employees = await _employeeService.GetAllEmployeesAsync();
           return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeByID([FromRoute] int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            if (employeeRequest == null) return BadRequest();

            var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeRequest);
            return CreatedAtAction(nameof(GetEmployeeByID), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateRequest employeeUpdateRequest, [FromRoute] int id)
        {
            if (employeeUpdateRequest == null) return BadRequest();

            try
            {
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeUpdateRequest);
                return Ok(updatedEmployee);

            }
            catch (ArgumentException ex) { 
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {

            try
            {
                bool isDeleted = await _employeeService.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/compute")]
        public async Task<IActionResult> ComputeTakeHomePay([FromBody] ComputeRequest computeRequest, [FromRoute] int id)
        {
            try
            {
                var totalPay = await _employeeService.ComputeTakeHomePayAsync(id, computeRequest.StartDate, computeRequest.EndDate);
                return Ok(totalPay);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
