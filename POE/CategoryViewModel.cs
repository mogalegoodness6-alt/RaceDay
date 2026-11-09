
namespace RaceDayMVC.Models;

public class CategoryViewModel
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = "";

    public decimal DistanceKM { get; set; }

    public string ActivityType { get; set; } = "";

    public int MinimumAge { get; set; }

    public int MaximumAge { get; set; }
}
