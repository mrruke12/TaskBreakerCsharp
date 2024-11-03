using System.ComponentModel.DataAnnotations;

namespace TaskBreakerApi.Models {
    public class UserRegistration : User {
        [Required]
        public string PasswordConfirm { get; set; }
    }
}
