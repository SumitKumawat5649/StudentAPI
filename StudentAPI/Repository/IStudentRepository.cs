using StudentAPI.Models;

namespace StudentAPI.Repository
{
    public interface IStudentRepository
    {
       Task<IEnumerable<Student>> GetAllStudentAsync();
        Task<Student?> GetStudentByIdAsync(int id);

        Task<Student> AddStudentAsync(Student student);

        Task<Student?> UpdateStudentAsync(Student student);

        Task<bool> DeleteStudentAsync(int id);
    }
}
