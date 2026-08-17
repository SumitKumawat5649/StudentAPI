using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;   
using StudentAPI.Models; 

namespace StudentAPI.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _dbContext;

      
        public CourseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
          
            return await _dbContext.Course.ToListAsync();
        }

      
        public async Task<Course> CreateCourseAsync(Course course)
        {
           
            await _dbContext.Course.AddAsync(course);

         
            await _dbContext.SaveChangesAsync();

            return course;
        }
    }
}