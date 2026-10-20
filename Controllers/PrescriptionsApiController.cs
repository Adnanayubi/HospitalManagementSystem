using HospitalManagementSystem.Data;
using HospitalManagementSystem.DTOs;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PrescriptionsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Department)
                .Include(p => p.Medicine)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return Ok(prescriptions);
        }

        // GET: api/PrescriptionsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Department)
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(
                    p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound(new
                {
                    message = "Prescription not found."
                });
            }

            return Ok(prescription);
        }

        // POST: api/PrescriptionsApi
        [HttpPost]
        public async Task<ActionResult<Prescription>> CreatePrescription(
    [FromBody] PrescriptionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == dto.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.DoctorId == dto.DoctorId);

            if (!doctorExists)
            {
                return BadRequest(new
                {
                    message = "The selected doctor does not exist."
                });
            }

            var medicineExists = await _context.Medicines
                .AnyAsync(m => m.MedicineId == dto.MedicineId);

            if (!medicineExists)
            {
                return BadRequest(new
                {
                    message = "The selected medicine does not exist."
                });
            }

            var prescription = new Prescription
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                MedicineId = dto.MedicineId,
                PrescriptionDate = dto.PrescriptionDate,
                Dosage = dto.Dosage,
                Frequency = dto.Frequency,
                Duration = dto.Duration,
                Instructions = dto.Instructions
            };

            _context.Prescriptions.Add(prescription);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPrescription),
                new { id = prescription.PrescriptionId },
                prescription);
        }

        // PUT: api/PrescriptionsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(
    int id,
    [FromBody] PrescriptionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPrescription =
                await _context.Prescriptions.FindAsync(id);

            if (existingPrescription == null)
            {
                return NotFound(new
                {
                    message = "Prescription not found."
                });
            }

            // Check Patient
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == dto.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            // Check Doctor
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.DoctorId == dto.DoctorId);

            if (!doctorExists)
            {
                return BadRequest(new
                {
                    message = "The selected doctor does not exist."
                });
            }

            // Check Medicine
            var medicineExists = await _context.Medicines
                .AnyAsync(m => m.MedicineId == dto.MedicineId);

            if (!medicineExists)
            {
                return BadRequest(new
                {
                    message = "The selected medicine does not exist."
                });
            }

            // Update prescription
            existingPrescription.PatientId =
                dto.PatientId;

            existingPrescription.DoctorId =
                dto.DoctorId;

            existingPrescription.MedicineId =
                dto.MedicineId;

            existingPrescription.PrescriptionDate =
                dto.PrescriptionDate;

            existingPrescription.Dosage =
                dto.Dosage;

            existingPrescription.Frequency =
                dto.Frequency;

            existingPrescription.Duration =
                dto.Duration;

            existingPrescription.Instructions =
                dto.Instructions;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PrescriptionsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription =
                await _context.Prescriptions.FindAsync(id);

            if (prescription == null)
            {
                return NotFound(new
                {
                    message = "Prescription not found."
                });
            }

            _context.Prescriptions.Remove(prescription);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}