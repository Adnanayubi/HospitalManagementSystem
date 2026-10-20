using System;

namespace HospitalManagementSystem.Models
{
    public class DashboardViewModel
    {
        // Summary Cards
        public int TotalPatients { get; set; }

        public int TotalDoctors { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalAppointments { get; set; }

        public int TotalMedicines { get; set; }

        public int TotalPrescriptions { get; set; }

        public decimal TotalRevenue { get; set; }

        // Appointment Chart
        public int ScheduledAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        // Monthly Revenue Chart
        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; }
            = new List<MonthlyRevenueViewModel>();
    }

    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;

        public decimal Revenue { get; set; }
    }
}