using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// An activity. Format from a POST request.
/// </summary>
public class ActivityCreateDto
{
    /// <summary>
    /// Name of the activity.
    /// </summary>
    [Required]
    [MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string Name { get; set; }

    /// <summary>
    /// Description of the activity.
    /// </summary>
    [MaxLength(GlobalConstants.LONG_TEXT_MAX_CHAR)]
    public string Description { get; set; }

    /// <summary>
    /// The date the activity was started.
    /// </summary>
    /// <remarks>
    /// NOTE: if you have type DateTime instead of DateTime?,
    ///       this will return as DateTime.MinValue even if
    ///       it isn't specified in the request unless you set default value
    /// </remarks>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// The deadline the user set for the activity.
    /// </summary>
    public DateTime? DueDateUtc { get; set; }

    /// <summary>
    /// The date the user manual set the activity as complete.
    /// </summary>
    public DateTime? CompletedDateUtc { get; set; }

    /// <summary>
    /// The custom color of the activity for rendering.
    /// </summary>
    [MaxLength(7)]
    public string ColorHex { get; set; }

    /// <summary>
    /// A list of tags associated with the activity.
    /// Used for sorting and filtering.
    /// </summary>
    public IList<string>? Tags { get; set; }
}