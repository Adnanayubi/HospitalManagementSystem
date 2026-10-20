using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bills

        // GET: Bills
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.Bills
                .Include(b => b.Patient)
                .AsQueryable();

            // Search bills
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();

                if (int.TryParse(term, out int billId))
                {
                    // Search by exact Bill ID
                    query = query.Where(b => b.BillId == billId);
                }
                else
                {
                    // Search by patient name, payment status,
                    // or payment method
                    query = query.Where(b =>
                        (b.Patient != null &&
                         b.Patient.PatientName != null &&
                         b.Patient.PatientName.Contains(term)) ||

                        (b.PaymentStatus != null &&
                         b.PaymentStatus.Contains(term)) ||

                        (b.PaymentMethod != null &&
                         b.PaymentMethod.Contains(term))
                    );
                }
            }

            // Preserve the search text in the textbox
            ViewBag.SearchString = searchString;

            // Keep the newest bills first
            var bills = await query
                .OrderByDescending(b => b.BillDate)
                .ThenByDescending(b => b.BillId)
                .ToListAsync();

            return View(bills);
        }


        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bills/Create
        public async Task<IActionResult> Create()
        {
            await LoadPatients();

            return View();
        }

        // POST: Bills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bill bill)
        {
            if (!ModelState.IsValid)
            {
                await LoadPatients(bill.PatientId);
                return View(bill);
            }

            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == bill.PatientId);

            if (!patientExists)
            {
                ModelState.AddModelError(
                    "PatientId",
                    "The selected patient does not exist.");

                await LoadPatients(bill.PatientId);
                return View(bill);
            }

            CalculateTotal(bill);

            _context.Bills.Add(bill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Bill created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .FindAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            await LoadPatients(bill.PatientId);

            return View(bill);
        }

        // POST: Bills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadPatients(bill.PatientId);
                return View(bill);
            }

            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == bill.PatientId);

            if (!patientExists)
            {
                ModelState.AddModelError(
                    "PatientId",
                    "The selected patient does not exist.");

                await LoadPatients(bill.PatientId);
                return View(bill);
            }

            var existingBill = await _context.Bills
                .FindAsync(id);

            if (existingBill == null)
            {
                return NotFound();
            }

            existingBill.PatientId = bill.PatientId;
            existingBill.BillDate = bill.BillDate;
            existingBill.ConsultationFee = bill.ConsultationFee;
            existingBill.MedicineCharges = bill.MedicineCharges;
            existingBill.LabCharges = bill.LabCharges;
            existingBill.RoomCharges = bill.RoomCharges;
            existingBill.OtherCharges = bill.OtherCharges;
            existingBill.Discount = bill.Discount;
            existingBill.PaymentStatus = bill.PaymentStatus;
            existingBill.PaymentMethod = bill.PaymentMethod;
            existingBill.Notes = bill.Notes;

            CalculateTotal(existingBill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Bill updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bills
                .FindAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Bill deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Load patients for dropdown
        private async Task LoadPatients(int? selectedPatientId = null)
        {
            var patients = await _context.Patients
                .OrderBy(p => p.PatientName)
                .ToListAsync();

            ViewBag.PatientId = new SelectList(
                patients,
                "PatientId",
                "PatientName",
                selectedPatientId);
        }

        // Calculate bill total
        private void CalculateTotal(Bill bill)
        {
            var subtotal =
                bill.ConsultationFee +
                bill.MedicineCharges +
                bill.LabCharges +
                bill.RoomCharges +
                bill.OtherCharges;

            bill.TotalAmount =
                subtotal - bill.Discount;

            if (bill.TotalAmount < 0)
            {
                bill.TotalAmount = 0;
            }
        }
    }
}