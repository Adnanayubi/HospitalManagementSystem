using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Please enter the medicine name.")]
        [StringLength(100)]
        [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GenericName { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(50)]
        public string? Dosage { get; set; }

        [StringLength(50)]
        public string? Manufacturer { get; set; }

        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}