using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.Models;

namespace StudentAPI.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext context;

        // Constructor
        public ExamRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        // 1. Naya Exam Create karne ke liye
        public async Task<Exam> CreateExamAsync(Exam exam)
        {
            context.Exam.Add(exam);
            await context.SaveChangesAsync();
            return exam;
        }

        // 2. Students ke Marks submit karne ke liye
        public async Task<Mark> AddMarksAsync(Mark mark)
        {
            context.Mark.Add(mark);
            await context.SaveChangesAsync();
            return mark;
        }

        // 3. Course ke mutabik saare Exams dekhne ke liye
        public async Task<IEnumerable<Exam>> GetExamsByCourseAsync(int courseId)
        {
            return await context.Exam
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        // 4. Statistics: Average, Max, Min, Sum
        public async Task<object> GetExamStatisticsAsync(int examId)
        {
            var marksQuery = context.Mark.Where(m => m.ExamId == examId);

            if (!await marksQuery.AnyAsync())
            {
                return new { Message = "Is exam ke liye abhi koi marks upload nahi kiye gaye hain." };
            }

            return new
            {
                ExamId = examId,
                TotalStudentsAppeared = await marksQuery.CountAsync(),
                AverageMarks = await marksQuery.AverageAsync(m => m.MarksObtained),
                HighestMarks = await marksQuery.MaxAsync(m => m.MarksObtained),
                LowestMarks = await marksQuery.MinAsync(m => m.MarksObtained),
                TotalMarksScored = await marksQuery.SumAsync(m => m.MarksObtained)
            };
        }

        // 5. Exam Results & Grades
        public async Task<IEnumerable<object>> GetExamResultsAsync(int examId, int passingMarks)
        {
            var data = await context.Mark
                .Where(m => m.ExamId == examId)
                .Include(m => m.Student)
                .Select(m => new
                {
                    StudentId = m.StudentId,
                    StudentName = m.Student!.Name,
                    MarksObtained = m.MarksObtained,
                    Status = m.MarksObtained >= passingMarks ? "Pass" : "Fail",
                    Grade = m.MarksObtained >= 90 ? "A+" :
                            m.MarksObtained >= 80 ? "A" :
                            m.MarksObtained >= 70 ? "B" :
                            m.MarksObtained >= 50 ? "C" : "F"
                })
                .ToListAsync();

            return data.Cast<object>();
        }

        // 6. Ranking System
        public async Task<IEnumerable<object>> GetTopStudentsByExamAsync(int examId, int topCount)
        {
            var topStudents = await context.Mark
                .Where(m => m.ExamId == examId)
                .Include(m => m.Student)
                .OrderByDescending(m => m.MarksObtained)
                .Take(topCount)
                .Select(m => new
                {
                    StudentId = m.StudentId,
                    StudentName = m.Student!.Name,
                    MarksObtained = m.MarksObtained
                })
                .ToListAsync();

            var rankedData = topStudents.Select((s, index) => new
            {
                Rank = index + 1,
                s.StudentId,
                s.StudentName,
                s.MarksObtained
            });

            return rankedData.Cast<object>();
        }
    }
}