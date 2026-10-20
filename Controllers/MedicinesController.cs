using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medicines
        // Search medicines by name and filter by category.
        public async Task<IActionResult> Index(
            string? searchString,
            string? category)
        {
            var query = _context.Medicines
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                query = query.Where(m =>
                    m.MedicineName.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(m => m.Category == category);
            }

            // Keep the selected search/filter values.
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCategory = category;


            // Populate category dropdown from existing medicine records.
            ViewBag.Categories = await _context.Medicines
                .AsNoTracking()
                .Where(m => m.Category != null && m.Category != "")
                .Select(m => m.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var medicines = await query
                .OrderBy(m => m.MedicineName)
                .ToListAsync();

            return View(medicines);
        }

        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var medicine = await _context.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            if (!ModelState.IsValid)
                return View(medicine);

            _context.Medicines.Add(medicine);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medicine added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Medicine medicine)
        {
            if (id != medicine.MedicineId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(medicine);

            var existingMedicine = await _context.Medicines
                .FindAsync(id);

            if (existingMedicine == null)
                return NotFound();

            try
            {
                // Update fields rather than attaching an entirely new entity.
                existingMedicine.MedicineName = medicine.MedicineName;
                existingMedicine.Category = medicine.Category;

                // Add assignments for other properties in your Medicine
                // model, such as price, stock or expiry date, if present.

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicineExists(id))
                    return NotFound();

                throw;
            }

            TempData["SuccessMessage"] = "Medicine updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var medicine = await _context.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medicine deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(int id)
        {
            return _context.Medicines
                .Any(m => m.MedicineId == id);
        }
    }
}
