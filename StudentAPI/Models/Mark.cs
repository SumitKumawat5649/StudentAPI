using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class Mark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        [Required]
        public int MarksObtained { get; set; }
    }
}