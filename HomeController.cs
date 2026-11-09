
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class HomeController : Controller
{
    private readonly RaceDayApiClient _api;

    public HomeController(RaceDayApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index(string? search, string? eventType)
    {
        var events = await _api.GetEventsAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            events = events.Where(e =>
                e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(eventType) && eventType != "All")
        {
            events = events.Where(e =>
                e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewBag.ApiUnavailable = events.Count == 0;
        ViewBag.Search = search;
        ViewBag.EventType = eventType ?? "All";

        return View(events.OrderBy(e => e.Date).ToList());
    }

    // DISPLAY EVENT DETAILS AND ITS AVAILABLE CATEGORIES
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var item = await _api.GetEventAsync(id);

        if (item is null)
            return NotFound();

        try
        {
            var categories = await _api.GetEventCategoriesAsync(id);

            ViewBag.Categories = categories;
        }
        catch (Exception)
        {
            ViewBag.Categories = new List<RaceDayMVC.Models.CategoryViewModel>();
            ViewBag.CategoryError =
                "Could not load categories. Please try again later.";
        }

        return View(item);
    }

    public IActionResult Error()
    {
        return View();
    }
}
