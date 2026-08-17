using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Repository;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        // 🧠 Constructor Injection: Dependency Injection ke zariye repository mangwayi
        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        // 🌐 1. GET: api/Students (LINQ Join / Include ke sath data dikhayega)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentRepository.GetAllStudentAsync();
            return Ok(students); // 200 Success status code aur sath mein pure relational list return karega
        }

        // 🌐 2. GET: api/Students/{id} (Single student uski course detail ke sath)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = $"Student with ID {id} not found" });
            }
            return Ok(student);
        }

        // 🌐 3. POST: api/Students (Naya student insert karega jisme ab CourseId jayegi)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            var createdStudent = await _studentRepository.AddStudentAsync(student);

            // CreatedAtAction standard REST convention hai jo response header mein dynamic path deta hai
            return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, createdStudent);
        }

        // 🌐 4. PUT: api/Students/{id} (Student data update karne ke liye)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Student student)
        {
         
            if (id != student.Id)
            {
                return BadRequest(new { Message = "ID in URL and Body do not match" });
            }

            var updatedStudent = await _studentRepository.UpdateStudentAsync(student);
            if (updatedStudent == null)
            {
                return NotFound(new { Message = $"Student with ID {id} not found to update" });
            }

            return Ok(updatedStudent);
        }

        // 🌐 5. DELETE: api/Students/{id} (Student record clear karne ke liye)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleteResult = await _studentRepository.DeleteStudentAsync(id);
            if (!deleteResult)
            {
                return NotFound(new { Message = $"Student with ID {id} not found to delete" });
            }

            return Ok(new { Message = "Student record deleted successfully" });
        }
    }
}