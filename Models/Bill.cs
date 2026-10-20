using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Bill
    {
        public int BillId { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Bill Date")]
        public DateTime BillDate { get; set; } = DateTime.Today;

        [Range(0, 1000000)]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Medicine Charges")]
        public decimal MedicineCharges { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Lab Charges")]
        public decimal LabCharges { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Room Charges")]
        public decimal RoomCharges { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Other Charges")]
        public decimal OtherCharges { get; set; }

        [Range(0, 1000000)]
        public decimal Discount { get; set; }

        [Range(0, 1000000)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending";

        [StringLength(30)]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}