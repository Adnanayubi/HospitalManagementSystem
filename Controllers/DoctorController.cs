using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Doctor
        // Display, search and filter doctors
        public async Task<IActionResult> Index(
            string? searchString,
            int? departmentId,
            string? availabilityStatus)
        {
            var query = _context.Doctors
                .Include(d => d.Department)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                query = query.Where(d =>
                    d.DoctorName.Contains(searchString) ||
                    d.Specialization.Contains(searchString));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(
                    d => d.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(availabilityStatus))
            {
                query = query.Where(
                    d => d.AvailabilityStatus == availabilityStatus);
            }

            ViewBag.SearchString = searchString;
            ViewBag.SelectedDepartmentId = departmentId;
            ViewBag.SelectedAvailability = availabilityStatus;

            ViewBag.Departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            var doctors = await query
                .OrderBy(d => d.DoctorName)
                .ToListAsync();

            return View("~/Views/Doctors/Index.cshtml", doctors);
        }

        // GET: /Doctor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
                return NotFound();

            return View("~/Views/Doctors/Details.cshtml", doctor);
        }

        // GET: /Doctor/Create
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            return View("~/Views/Doctors/Create.cshtml");
        }

        // POST: /Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            // Confirm that the selected department exists.
            if (!await _context.Departments.AnyAsync(
                d => d.DepartmentId == doctor.DepartmentId))
            {
                ModelState.AddModelError(
                    nameof(doctor.DepartmentId),
                    "Please select a valid department.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(doctor.DepartmentId);

                return View("~/Views/Doctors/Create.cshtml", doctor);
            }

            _context.Doctors.Add(doctor);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Doctor added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Doctor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == id.Value);

            if (doctor == null)
                return NotFound();

            await LoadDepartmentsAsync(doctor.DepartmentId);

            return View("~/Views/Doctors/Edit.cshtml", doctor);
        }

        // POST: /Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Doctor doctor)
        {
            if (id != doctor.DoctorId)
                return NotFound();

            var existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (existingDoctor == null)
                return NotFound();

            if (!await _context.Departments.AnyAsync(
                d => d.DepartmentId == doctor.DepartmentId))
            {
                ModelState.AddModelError(
                    nameof(doctor.DepartmentId),
                    "Please select a valid department.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(doctor.DepartmentId);

                return View("~/Views/Doctors/Edit.cshtml", doctor);
            }

            existingDoctor.DoctorName = doctor.DoctorName;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.Phone = doctor.Phone;
            existingDoctor.Email = doctor.Email;
            existingDoctor.DepartmentId = doctor.DepartmentId;
            existingDoctor.AvailabilityStatus =
                doctor.AvailabilityStatus;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Doctor updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Doctor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DoctorId == id.Value);

            if (doctor == null)
                return NotFound();

            return View("~/Views/Doctors/Delete.cshtml", doctor);
        }

        // POST: /Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Doctor deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Populate Department dropdown for Create and Edit.
        private async Task LoadDepartmentsAsync(
            int? selectedDepartmentId = null)
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            ViewBag.Departments = new SelectList(
                departments,
                nameof(Department.DepartmentId),
                nameof(Department.DepartmentName),
                selectedDepartmentId);
        }
    }
}
