using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Models
{
    /// <summary>
    /// User info.
    /// </summary>
    [Table(nameof(User))]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(1), MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
        
        [Required]
        public DateTime DateJoined { get; set; }

        public DateTime DateDeleted { get; set; }
    }
}
