using System.ComponentModel.DataAnnotations.Schema;
namespace StudentAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
            
        public int Age { get; set; }

        public int CourseId {  get; set; }

        [ForeignKey("CourseId")]
        public Course? CourseDetails { get; set; }
    }
}
