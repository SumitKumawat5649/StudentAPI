using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Repository;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamRepository _examRepository;

        // Constructor Dependency Injection
        public ExamsController(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        // 1. 📝 Create Exam: Naya exam banane ke liye
        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] Exam exam)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdExam = await _examRepository.CreateExamAsync(exam);
            return Ok(createdExam);
        }

        // 2. 📊 Submit Marks: Students ke marks entry karne ke liye
        [HttpPost("submit-marks")]
        public async Task<IActionResult> AddMarks([FromBody] Mark mark)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var addedMark = await _examRepository.AddMarksAsync(mark);
            return Ok(addedMark);
        }

        // 3. 🔍 Course-wise Exams: Kisi specific course ke saare exams dekhne ke liye (Where Concept)
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetExamsByCourse(int courseId)
        {
            var exams = await _examRepository.GetExamsByCourseAsync(courseId);
            return Ok(exams);
        }

        // 4. 📈 Exam Statistics: Aggregations dekhne ke liye (Average, Max, Min, Sum)
        [HttpGet("{examId}/statistics")]
        public async Task<IActionResult> GetExamStats(int examId)
        {
            var stats = await _examRepository.GetExamStatisticsAsync(examId);
            return Ok(stats);
        }

        // 5. 🎯 Exam Results: Individual students ka result aur Grades nikalne ke liye (Where + Select)
        [HttpGet("{examId}/results")]
        public async Task<IActionResult> GetExamResults(int examId, [FromQuery] int passingMarks = 40)
        {
            var results = await _examRepository.GetExamResultsAsync(examId, passingMarks);
            return Ok(results);
        }

        // 6. 📊 Result Summary: Pass/Fail ka chart data nikalne ke liye (🔥 Advanced GroupBy)
        [HttpGet("{examId}/result-summary")]
        public async Task<IActionResult> GetResultSummary(int examId, [FromQuery] int passingMarks = 40)
        {
            var results = await _examRepository.GetExamResultsAsync(examId, passingMarks);

            // Dynamic conversion for LINQ GroupBy processing
            var castedResults = results.Select(r => (dynamic)r);

            // GroupBy Status (Pass/Fail)
            var summary = castedResults.GroupBy(r => r.Status)
                .Select(g => new
                {
                    Status = g.Key,             // "Pass" ya "Fail"
                    Count = g.Count(),          // Kitne students hain
                    AverageMarks = g.Average(r => (double)r.MarksObtained) 
                });

            return Ok(summary);
        }

        // 7. 🏆 Ranking Endpoint: Top performing students dekhne ke liye (OrderByDescending + Take)
        [HttpGet("{examId}/ranking")]
        public async Task<IActionResult> GetTopStudents(int examId, [FromQuery] int top = 5)
        {
            var ranking = await _examRepository.GetTopStudentsByExamAsync(examId, top);
            return Ok(ranking);
        }
    }
}