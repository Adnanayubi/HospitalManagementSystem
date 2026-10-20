
namespace HospitalManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        // Summary cards
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalPrescriptions { get; set; }

        // Revenue
        public decimal TotalRevenue { get; set; }

        // Appointment status counts
        public int ScheduledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        // Monthly revenue chart
        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; }
            = new List<MonthlyRevenueViewModel>();
    }

    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;

        public decimal Revenue { get; set; }
    }
}
