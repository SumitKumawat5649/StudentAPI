using Microsoft.AspNetCore.Mvc;
using StudentAPI.Models;
using StudentAPI.Repository;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        // 🧠 Constructor Injection: Interface ko controller mein inject kiya
        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // 🌐 1. GET Endpoint: api/Courses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return Ok(courses); // 
        }

        // 🌐 2. POST Endpoint: api/Courses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            var createdCourse = await _courseRepository.CreateCourseAsync(course);

          
            return Ok(createdCourse);
        }
    }
}