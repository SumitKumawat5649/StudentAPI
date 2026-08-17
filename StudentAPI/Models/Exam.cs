using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamName { get; set; } = string.Empty; 

        [Required]
        public int TotalMarks { get; set; }

    
        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}