using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Models
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}