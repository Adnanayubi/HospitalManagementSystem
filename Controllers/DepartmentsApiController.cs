using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DepartmentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return Ok(departments);
        }

        // GET: api/DepartmentsApi/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return NotFound(new
                {
                    message = "Department not found."
                });
            }

            return Ok(department);
        }

        // POST: api/DepartmentsApi
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment(
            Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Departments.Add(department);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDepartment),
                new { id = department.DepartmentId },
                department);
        }

        // PUT: api/DepartmentsApi/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(
            int id,
            Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest(new
                {
                    message = "Department ID does not match."
                });
            }

            var existingDepartment = await _context.Departments
                .FindAsync(id);

            if (existingDepartment == null)
            {
                return NotFound(new
                {
                    message = "Department not found."
                });
            }

            existingDepartment.DepartmentName =
                department.DepartmentName;

            existingDepartment.Description =
                department.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DepartmentsApi/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments
                .FindAsync(id);

            if (department == null)
            {
                return NotFound(new
                {
                    message = "Department not found."
                });
            }

            // Check whether doctors are using this department
            var hasDoctors = await _context.Doctors
                .AnyAsync(d => d.DepartmentId == id);

            if (hasDoctors)
            {
                return BadRequest(new
                {
                    message =
                        "This department cannot be deleted because doctors are assigned to it."
                });
            }

            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}