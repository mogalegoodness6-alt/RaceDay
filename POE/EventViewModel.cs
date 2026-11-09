using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models;

public class EventViewModel
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [StringLength(1000)]
    public string Description { get; set; } = "";

    [Required]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(7);

    [Required, StringLength(200)]
    public string Location { get; set; } = "";

    [Range(0.1, 1000)]
    public decimal Distance { get; set; } = 5;

    [Required]
    public string EventType { get; set; } = "Run";

    public string? BannerImageUrl { get; set; }
    public int EnrolmentCount { get; set; }
}
