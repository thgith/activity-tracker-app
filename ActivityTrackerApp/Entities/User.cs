using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Entities
{
    /// <summary>
    /// User info.
    /// </summary>
    [Index(nameof(Email))]
    public class User : BaseEntity
    {
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
        [MinLength(8), MaxLength(256)]
        public string PasswordHash { get; set; }
        
        [Required]
        public DateTime DateJoined { get; set; }

        public DateTime? DateDeleted { get; set; }

        /// <summary>
        /// The IDs of the user's activities.
        /// </summary>
        public IList<int> ActivityIds { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }
    }
}
