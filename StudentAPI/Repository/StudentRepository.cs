using StudentAPI.Data;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;


namespace StudentAPI.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext context;

        public StudentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentAsync()
        {
           return await context.Students.Include(s => s.CourseDetails).ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
           return await context.Students.Include(s => s.CourseDetails).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            await context.Students.AddAsync(student);
            await context.SaveChangesAsync();  
            return student;
        }

        public async Task<Student?> UpdateStudentAsync(Student student)
        {
             context.Students.Update(student);
            await context.SaveChangesAsync();
            return student;
            
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
           var student = await context.Students.FindAsync(id);
            if(student != null)
            {
                context.Students.Remove(student);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
            
        }
    }
}
