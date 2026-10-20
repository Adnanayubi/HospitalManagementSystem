using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: api/AppointmentsApi
        // Get all appointments with Patient and Doctor information
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return Ok(appointments);
        }

        // =========================================================
        // GET: api/AppointmentsApi/1
        // Get one appointment
        // =========================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound(new
                {
                    message = "Appointment not found."
                });
            }

            return Ok(appointment);
        }

        // =========================================================
        // POST: api/AppointmentsApi
        // Create a new appointment
        // =========================================================
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(
            Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check Patient
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == appointment.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            // Check Doctor
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.DoctorId == appointment.DoctorId);

            if (!doctorExists)
            {
                return BadRequest(new
                {
                    message = "The selected doctor does not exist."
                });
            }

            // Check that appointment status is valid
            var validStatuses = new[]
            {
                "Scheduled",
                "Completed",
                "Cancelled"
            };

            if (!validStatuses.Contains(
                appointment.Status,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid appointment status. Use Scheduled, Completed or Cancelled."
                });
            }

            // Store normalized status
            appointment.Status = appointment.Status.Trim();

            _context.Appointments.Add(appointment);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAppointment),
                new { id = appointment.AppointmentId },
                appointment);
        }

        // =========================================================
        // PUT: api/AppointmentsApi/1
        // Update an appointment
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(
            int id,
            Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest(new
                {
                    message = "Appointment ID does not match."
                });
            }

            var existingAppointment = await _context.Appointments
                .FindAsync(id);

            if (existingAppointment == null)
            {
                return NotFound(new
                {
                    message = "Appointment not found."
                });
            }

            // Check Patient
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == appointment.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            // Check Doctor
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.DoctorId == appointment.DoctorId);

            if (!doctorExists)
            {
                return BadRequest(new
                {
                    message = "The selected doctor does not exist."
                });
            }

            // Check Status
            var validStatuses = new[]
            {
                "Scheduled",
                "Completed",
                "Cancelled"
            };

            if (!validStatuses.Contains(
                appointment.Status,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid appointment status. Use Scheduled, Completed or Cancelled."
                });
            }

            // Update fields
            existingAppointment.PatientId =
                appointment.PatientId;

            existingAppointment.DoctorId =
                appointment.DoctorId;

            existingAppointment.AppointmentDate =
                appointment.AppointmentDate;

            existingAppointment.AppointmentTime =
                appointment.AppointmentTime;

            existingAppointment.Reason =
                appointment.Reason;

            existingAppointment.Status =
                appointment.Status.Trim();

            existingAppointment.Notes =
                appointment.Notes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // DELETE: api/AppointmentsApi/1
        // Delete an appointment
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments
                .FindAsync(id);

            if (appointment == null)
            {
                return NotFound(new
                {
                    message = "Appointment not found."
                });
            }

            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}   