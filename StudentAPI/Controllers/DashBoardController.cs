using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public DashboardController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // 1. LINQ Count
            var totalStudents = await context.Students.CountAsync();
            var totalCourses = await context.Course.CountAsync();
            var totalExams = await context.Exam.CountAsync();

            // 2. LINQ Average
            double overallAverage = 0;
            if (await context.Mark.AnyAsync())
            {
                overallAverage = await context.Mark.AverageAsync(m => m.MarksObtained);
            }

            // 3. 🎯 LINQ GroupBy on Age (Umar ke hisab se summary)
            var ageWiseDistribution = await context.Students
                .GroupBy(s => s.Age)
                .Select(g => new
                {
                    Age = g.Key,
                    TotalCount = g.Count()
                }).ToListAsync();

            var dashboardSummary = new
            {
                TotalStudents = totalStudents,
                TotalCourses = totalCourses,
                TotalExams = totalExams,
                CollegeAverageMarks = Math.Round(overallAverage, 2),
                AgeStats = ageWiseDistribution
            };

            return Ok(dashboardSummary);
        }
    }
}