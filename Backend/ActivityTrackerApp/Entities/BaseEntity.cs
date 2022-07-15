using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Entities;

/// <summary>
/// Base entity that other entities extend.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Identifier of the entity.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}