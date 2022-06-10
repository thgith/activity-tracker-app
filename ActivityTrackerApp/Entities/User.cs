using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Entities
{
    /// <summary>
    /// User info.
    /// </summary>
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(1), MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MinLength(1), MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [MinLength(6), MaxLength(200)]
        public string Email { get; set; }
        
        [Required]
        [MinLength(8), MaxLength(50)]
        public string Password { get; set; }
        
        [Required]
        public DateTime DateJoined { get; set; }

        public DateTime? DateDeleted { get; set; }

        /// <summary>
        /// The IDs of the user's activities.
        /// </summary>
        public IList<int> ActivityIds { get; set; }
    }
}
