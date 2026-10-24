using System.ComponentModel.DataAnnotations;

namespace MyFirstApp.Models
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; }

        public ICollection<Student> Students { get; set; }
            = new List<Student>();
    }
}
