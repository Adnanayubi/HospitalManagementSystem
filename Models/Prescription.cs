using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }

        // Patient
        [Required(ErrorMessage = "Please select a patient.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }


        // Doctor
        [Required(ErrorMessage = "Please select a doctor.")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }


        // Medicine
        [Required(ErrorMessage = "Please select a medicine.")]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine? Medicine { get; set; }


        // Prescription Date
        [Required(ErrorMessage = "Please select the prescription date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Prescription Date")]
        public DateTime PrescriptionDate { get; set; }


        // Dosage
        [Required(ErrorMessage = "Please enter the dosage.")]
        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;


        // Frequency
        [Required(ErrorMessage = "Please enter the frequency.")]
        [StringLength(100)]
        public string Frequency { get; set; } = string.Empty;


        // Duration
        [Required(ErrorMessage = "Please enter the duration.")]
        [StringLength(100)]
        public string Duration { get; set; } = string.Empty;


        // Instructions
        [StringLength(500)]
        public string? Instructions { get; set; }
    }
}