using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        
// GET: Appointments
        public async Task<IActionResult> Index(string? searchString)
        {
            // Keep the search text in the view
            ViewBag.SearchString = searchString;

            // Load appointments with patient and doctor information
            IQueryable<Appointment> appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string search = searchString.Trim();

                appointments = appointments.Where(a =>
                    a.AppointmentId.ToString().Contains(search) ||
                    (a.Patient != null &&
                     a.Patient.PatientName != null &&
                     a.Patient.PatientName.Contains(search)) ||
                    (a.Doctor != null &&
                     a.Doctor.DoctorName != null &&
                     a.Doctor.DoctorName.Contains(search))
                );
            }

            // Show the latest appointments first
            var result = await appointments
                .OrderByDescending(a => a.AppointmentId)
                .ToListAsync();

            return View(result);
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(
                    a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(
                appointment.PatientId,
                appointment.DoctorId);

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            await LoadDropdowns(
                appointment.PatientId,
                appointment.DoctorId);

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(
                        appointment.AppointmentId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(
                appointment.PatientId,
                appointment.DoctorId);

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(
                    a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var appointment = await _context.Appointments
                .FindAsync(id);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Load Patient and Doctor dropdowns
        private async Task LoadDropdowns(
            int? selectedPatientId = null,
            int? selectedDoctorId = null)
        {
            var patients = await _context.Patients
                .OrderBy(p => p.PatientName)
                .ToListAsync();

            var doctors = await _context.Doctors
                .OrderBy(d => d.DoctorName)
                .ToListAsync();

            ViewBag.Patients = new SelectList(
                patients,
                "PatientId",
                "PatientName",
                selectedPatientId);

            ViewBag.Doctors = new SelectList(
                doctors,
                "DoctorId",
                "DoctorName",
                selectedDoctorId);
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments
                .Any(a => a.AppointmentId == id);
        }
    }
}