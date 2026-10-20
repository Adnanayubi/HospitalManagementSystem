using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.DTOs
{
    public class PrescriptionUpdateDto
    {
        [Required(ErrorMessage = "Please select a patient.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a medicine.")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Please select the prescription date.")]
        [DataType(DataType.Date)]
        public DateTime PrescriptionDate { get; set; }

        [Required(ErrorMessage = "Please enter the dosage.")]
        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the frequency.")]
        [StringLength(100)]
        public string Frequency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the duration.")]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Instructions { get; set; }
    }
}