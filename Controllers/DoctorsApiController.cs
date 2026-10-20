using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DoctorsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .OrderBy(d => d.DoctorName)
                .ToListAsync();

            return Ok(doctors);
        }

        // GET: api/DoctorsApi/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound(new
                {
                    message = "Doctor not found."
                });
            }

            return Ok(doctor);
        }

        // POST: api/DoctorsApi
        [HttpPost]
        public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.DepartmentId == doctor.DepartmentId);

            if (!departmentExists)
            {
                return BadRequest(new
                {
                    message = "The selected department does not exist."
                });
            }

            _context.Doctors.Add(doctor);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDoctor),
                new { id = doctor.DoctorId },
                doctor);
        }

        // PUT: api/DoctorsApi/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(
            int id,
            Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return BadRequest(new
                {
                    message = "Doctor ID does not match."
                });
            }

            var existingDoctor = await _context.Doctors
                .FindAsync(id);

            if (existingDoctor == null)
            {
                return NotFound(new
                {
                    message = "Doctor not found."
                });
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.DepartmentId == doctor.DepartmentId);

            if (!departmentExists)
            {
                return BadRequest(new
                {
                    message = "The selected department does not exist."
                });
            }

            existingDoctor.DoctorName = doctor.DoctorName;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.Phone = doctor.Phone;
            existingDoctor.Email = doctor.Email;
            existingDoctor.DepartmentId = doctor.DepartmentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/DoctorsApi/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors
                .FindAsync(id);

            if (doctor == null)
            {
                return NotFound(new
                {
                    message = "Doctor not found."
                });
            }

            _context.Doctors.Remove(doctor);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}