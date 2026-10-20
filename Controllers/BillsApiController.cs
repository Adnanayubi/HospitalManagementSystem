using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BillsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
            var bills = await _context.Bills
                .Include(b => b.Patient)
                .OrderByDescending(b => b.BillDate)
                .ThenByDescending(b => b.BillId)
                .ToListAsync();

            return Ok(bills);
        }

        // GET: api/BillsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(
            [FromRoute] int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound(new
                {
                    message = "Bill not found."
                });
            }

            return Ok(bill);
        }

        // POST: api/BillsApi
        [HttpPost]
        public async Task<ActionResult<Bill>> CreateBill(
            [FromBody] Bill bill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check patient
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == bill.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            // Validate payment status
            var validPaymentStatuses = new[]
            {
                "Pending",
                "Paid",
                "Partially Paid"
            };

            if (!validPaymentStatuses.Contains(
                bill.PaymentStatus,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid payment status. Use Pending, Paid or Partially Paid."
                });
            }

            // Validate payment method
            if (!string.IsNullOrWhiteSpace(bill.PaymentMethod))
            {
                var validPaymentMethods = new[]
                {
                    "Cash",
                    "Card",
                    "UPI",
                    "Bank Transfer"
                };

                if (!validPaymentMethods.Contains(
                    bill.PaymentMethod,
                    StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        message =
                            "Invalid payment method. Use Cash, Card, UPI or Bank Transfer."
                    });
                }

                bill.PaymentMethod = bill.PaymentMethod.Trim();
            }

            // Calculate total
            CalculateTotal(bill);

            _context.Bills.Add(bill);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBill),
                new { id = bill.BillId },
                bill);
        }

        // PUT: api/BillsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(
            [FromRoute] int id,
            [FromBody] Bill bill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check ID
            if (id != bill.BillId)
            {
                return BadRequest(new
                {
                    message = "Bill ID does not match."
                });
            }

            // Find existing bill
            var existingBill = await _context.Bills
                .FindAsync(id);

            if (existingBill == null)
            {
                return NotFound(new
                {
                    message = "Bill not found."
                });
            }

            // Check patient
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == bill.PatientId);

            if (!patientExists)
            {
                return BadRequest(new
                {
                    message = "The selected patient does not exist."
                });
            }

            // Validate payment status
            var validPaymentStatuses = new[]
            {
                "Pending",
                "Paid",
                "Partially Paid"
            };

            if (!validPaymentStatuses.Contains(
                bill.PaymentStatus,
                StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid payment status. Use Pending, Paid or Partially Paid."
                });
            }

            // Validate payment method
            if (!string.IsNullOrWhiteSpace(bill.PaymentMethod))
            {
                var validPaymentMethods = new[]
                {
                    "Cash",
                    "Card",
                    "UPI",
                    "Bank Transfer"
                };

                if (!validPaymentMethods.Contains(
                    bill.PaymentMethod,
                    StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        message =
                            "Invalid payment method. Use Cash, Card, UPI or Bank Transfer."
                    });
                }
            }

            // Update bill
            existingBill.PatientId =
                bill.PatientId;

            existingBill.BillDate =
                bill.BillDate;

            existingBill.ConsultationFee =
                bill.ConsultationFee;

            existingBill.MedicineCharges =
                bill.MedicineCharges;

            existingBill.LabCharges =
                bill.LabCharges;

            existingBill.RoomCharges =
                bill.RoomCharges;

            existingBill.OtherCharges =
                bill.OtherCharges;

            existingBill.Discount =
                bill.Discount;

            existingBill.PaymentStatus =
                bill.PaymentStatus.Trim();

            existingBill.PaymentMethod =
                string.IsNullOrWhiteSpace(bill.PaymentMethod)
                    ? null
                    : bill.PaymentMethod.Trim();

            existingBill.Notes =
                bill.Notes;

            // Recalculate total
            CalculateTotal(existingBill);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/BillsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(
            [FromRoute] int id)
        {
            var bill = await _context.Bills
                .FindAsync(id);

            if (bill == null)
            {
                return NotFound(new
                {
                    message = "Bill not found."
                });
            }

            _context.Bills.Remove(bill);

            await _context.SaveChangesAsync();

            return NoContent();
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