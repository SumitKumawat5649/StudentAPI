using StudentAPI.Models;

namespace StudentAPI.Repository
{
    public interface IExamRepository
    {
        Task<Exam> CreateExamAsync(Exam exam);
        Task<Mark> AddMarksAsync(Mark mark);
        Task<IEnumerable<Exam>> GetExamsByCourseAsync(int courseId);
        Task<object> GetExamStatisticsAsync(int examId);
        Task<IEnumerable<object>> GetExamResultsAsync(int examId, int passingMarks);
        Task<IEnumerable<object>> GetTopStudentsByExamAsync(int examId, int topCount);
    }
}