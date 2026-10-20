using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientsApi
        // Admin, Doctor, and Receptionist can view all patients.
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            var patients = await _context.Patients
                .AsNoTracking()
                .OrderBy(p => p.PatientName)
                .ToListAsync();

            return Ok(patients);
        }

        // GET: api/PatientsApi/1
        // Admin, Doctor, and Receptionist can view one patient.
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            return Ok(patient);
        }

        // POST: api/PatientsApi
        // Admin and Receptionist can add patients.
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult<Patient>> CreatePatient(
            [FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // SQL Server generates the identity ID.
            patient.PatientId = 0;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPatient),
                new { id = patient.PatientId },
                patient);
        }

        // PUT: api/PatientsApi/1
        // Admin and Receptionist can edit patients.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> UpdatePatient(
            int id,
            [FromBody] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return BadRequest(new
                {
                    message = "Patient ID does not match."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var existingPatient = await _context.Patients.FindAsync(id);

            if (existingPatient == null)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            existingPatient.PatientName = patient.PatientName;
            existingPatient.DateOfBirth = patient.DateOfBirth;
            existingPatient.Gender = patient.Gender;
            existingPatient.Phone = patient.Phone;
            existingPatient.Email = patient.Email;
            existingPatient.Address = patient.Address;
            existingPatient.BloodGroup = patient.BloodGroup;
            existingPatient.EmergencyContact = patient.EmergencyContact;
            existingPatient.MedicalHistory = patient.MedicalHistory;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PatientsApi/1
        // Only Admin can delete patients.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}