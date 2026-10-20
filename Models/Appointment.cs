using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

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


        // Appointment Date
        [Required(ErrorMessage = "Please select an appointment date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }


        // Appointment Time
        [Required(ErrorMessage = "Please enter the appointment time.")]
        [Display(Name = "Appointment Time")]
        public TimeSpan AppointmentTime { get; set; }


        // Reason
        [StringLength(500)]
        [Display(Name = "Reason for Visit")]
        public string? Reason { get; set; }


        // Status
        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Scheduled";


        // Additional Notes
        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}