
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please enter the doctor's name.")]
        [StringLength(100)]
        [Display(Name = "Doctor Name")]
        public string DoctorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the specialization.")]
        [StringLength(100)]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please select a department.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        // NEW: Doctor availability status
        [Required]
        [StringLength(20)]
        [RegularExpression(
            "^(Available|Unavailable|On Leave)$",
            ErrorMessage = "Please select a valid availability status.")]
        [Display(Name = "Availability Status")]
        public string AvailabilityStatus { get; set; } = "Available";

        // Navigation property
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
    }
}
