using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicinesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MedicinesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicines()
        {
            var medicines = await _context.Medicines
                .OrderBy(m => m.MedicineName)
                .ToListAsync();

            return Ok(medicines);
        }

        // GET: api/MedicinesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicine>> GetMedicine(
            [FromRoute] int id)
        {
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medicine == null)
            {
                return NotFound(new
                {
                    message = "Medicine not found."
                });
            }

            return Ok(medicine);
        }

        // POST: api/MedicinesApi
        [HttpPost]
        public async Task<ActionResult<Medicine>> CreateMedicine(
            [FromBody] Medicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Medicines.Add(medicine);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMedicine),
                new { id = medicine.MedicineId },
                medicine);
        }

        // PUT: api/MedicinesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(
            [FromRoute] int id,
            [FromBody] Medicine medicine)
        {
            if (id != medicine.MedicineId)
            {
                return BadRequest(new
                {
                    message = "Medicine ID does not match."
                });
            }

            var existingMedicine = await _context.Medicines
                .FindAsync(id);

            if (existingMedicine == null)
            {
                return NotFound(new
                {
                    message = "Medicine not found."
                });
            }

            existingMedicine.MedicineName = medicine.MedicineName;
            existingMedicine.GenericName = medicine.GenericName;
            existingMedicine.Category = medicine.Category;
            existingMedicine.Dosage = medicine.Dosage;
            existingMedicine.Manufacturer = medicine.Manufacturer;
            existingMedicine.Price = medicine.Price;
            existingMedicine.StockQuantity = medicine.StockQuantity;
            existingMedicine.ExpiryDate = medicine.ExpiryDate;
            existingMedicine.Description = medicine.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MedicinesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(
            [FromRoute] int id)
        {
            var medicine = await _context.Medicines
                .FindAsync(id);

            if (medicine == null)
            {
                return NotFound(new
                {
                    message = "Medicine not found."
                });
            }

            _context.Medicines.Remove(medicine);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}