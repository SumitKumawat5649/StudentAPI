using StudentAPI.Models;

namespace StudentAPI.Repository
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();

        Task<Course> CreateCourseAsync(Course course);
    }
}
