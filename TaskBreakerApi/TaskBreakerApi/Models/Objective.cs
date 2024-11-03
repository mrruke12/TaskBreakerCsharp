using System.ComponentModel.DataAnnotations;

namespace TaskBreakerApi.Models {
    public class Objective {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Goal { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
