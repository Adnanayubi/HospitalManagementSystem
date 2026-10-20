using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please enter the patient's name.")]
        [StringLength(100)]
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(20)]
        [Display(Name = "Blood Group")]
        public string? BloodGroup { get; set; }

        [StringLength(20)]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [StringLength(1000)]
        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }
    }
}