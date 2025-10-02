using BigShotCore.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BigShotCore.Data.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string ApiKey { get; set; } = Guid.NewGuid().ToString(); // generate API key

        public int RoleId { get; set; }
        public AppRole Role { get; set; } = null!;

        public List<Order> Orders { get; set; } = new();
    }


    
}
