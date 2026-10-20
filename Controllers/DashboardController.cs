using HospitalManagementSystem.Data;
using HospitalManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                // Summary Cards
                TotalPatients = await _context.Patients.CountAsync(),

                TotalDoctors = await _context.Doctors.CountAsync(),

                TotalDepartments = await _context.Departments.CountAsync(),

                TotalAppointments = await _context.Appointments.CountAsync(),

                TotalMedicines = await _context.Medicines.CountAsync(),

                TotalPrescriptions = await _context.Prescriptions.CountAsync(),

                // Revenue
                TotalRevenue = await _context.Bills
                    .Where(b => b.PaymentStatus == "Paid")
                    .SumAsync(b => (decimal?)b.TotalAmount) ?? 0
            };

            // Appointment Status
            dashboard.ScheduledAppointments =
                await _context.Appointments
                    .CountAsync(a => a.Status == "Scheduled");

            dashboard.CompletedAppointments =
                await _context.Appointments
                    .CountAsync(a => a.Status == "Completed");

            dashboard.CancelledAppointments =
                await _context.Appointments
                    .CountAsync(a => a.Status == "Cancelled");

            // Monthly Revenue
            var currentYear = DateTime.Today.Year;

            var monthlyRevenue = await _context.Bills
                .Where(b =>
                    b.PaymentStatus == "Paid" &&
                    b.BillDate.Year == currentYear)
                .GroupBy(b => b.BillDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(b => b.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            dashboard.MonthlyRevenue = monthlyRevenue
                .Select(x => new MonthlyRevenueViewModel
                {
                    Month = new DateTime(
                        currentYear,
                        x.Month,
                        1).ToString("MMM"),

                    Revenue = x.Revenue
                })
                .ToList();

            return View(dashboard);
        }
    }
}