using System.ComponentModel.DataAnnotations;

namespace BigShotCore.Data.Models
{
    public class AppRole
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!; // Admin / Customer

        public List<AppUser> Users { get; set; } = new();
    }
}
