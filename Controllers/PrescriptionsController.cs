using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }


        
        // =====================================================
       // GET: Prescriptions (with Search)
      // =====================================================
        public async Task<IActionResult> Index(string? searchString)
        {
            // Preserve the search keyword in the view
            ViewBag.SearchString = searchString;

            // Load prescriptions with related data
            IQueryable<Prescription> prescriptions =
                _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.Doctor)
                    .Include(p => p.Medicine);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string search = searchString.Trim();

                prescriptions = prescriptions.Where(p =>
                    p.PrescriptionId.ToString().Contains(search) ||

                    (p.Patient != null &&
                     p.Patient.PatientName != null &&
                     p.Patient.PatientName.Contains(search)) ||

                    (p.Doctor != null &&
                     p.Doctor.DoctorName != null &&
                     p.Doctor.DoctorName.Contains(search)) ||

                    (p.Medicine != null &&
                     p.Medicine.MedicineName != null &&
                     p.Medicine.MedicineName.Contains(search))
                );
            }

            // Show latest prescriptions first
            var result = await prescriptions
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            return View(result);
        }



        // =====================================================
        // GET: Prescriptions/Details/5
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(
                    p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }


        // =====================================================
        // GET: Prescriptions/Create
        // =====================================================
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            return View();
        }


        // =====================================================
        // POST: Prescriptions/Create
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Prescriptions.Add(prescription);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if validation fails
            await LoadDropdowns(
                prescription.PatientId,
                prescription.DoctorId,
                prescription.MedicineId);

            return View(prescription);
        }


        // =====================================================
        // GET: Prescriptions/Edit/5
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .FindAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            await LoadDropdowns(
                prescription.PatientId,
                prescription.DoctorId,
                prescription.MedicineId);

            return View(prescription);
        }


        // =====================================================
        // POST: Prescriptions/Edit/5
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Prescription prescription)
        {
            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(
                        prescription.PrescriptionId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if validation fails
            await LoadDropdowns(
                prescription.PatientId,
                prescription.DoctorId,
                prescription.MedicineId);

            return View(prescription);
        }


        // =====================================================
        // GET: Prescriptions/Delete/5
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(
                    p => p.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }


        // =====================================================
        // POST: Prescriptions/Delete/5
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var prescription = await _context.Prescriptions
                .FindAsync(id);

            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // Load Patient, Doctor and Medicine dropdowns
        // =====================================================
        private async Task LoadDropdowns(
            int? selectedPatientId = null,
            int? selectedDoctorId = null,
            int? selectedMedicineId = null)
        {
            // Patients
            var patients = await _context.Patients
                .OrderBy(p => p.PatientName)
                .ToListAsync();

            // Doctors
            var doctors = await _context.Doctors
                .OrderBy(d => d.DoctorName)
                .ToListAsync();

            // Medicines
            var medicines = await _context.Medicines
                .OrderBy(m => m.MedicineName)
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


            ViewBag.Medicines = new SelectList(
                medicines,
                "MedicineId",
                "MedicineName",
                selectedMedicineId);
        }


        // =====================================================
        // Check if prescription exists
        // =====================================================
        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions
                .Any(p => p.PrescriptionId == id);
        }
    }
}