using System.ComponentModel.DataAnnotations;

namespace MyFirstApp.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "School")]
        public int SchoolId { get; set; }

        public School? School { get; set; }
        public int Age { get; set; }
    }
}
