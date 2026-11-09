using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class OrganiserController : Controller
{
    private readonly RaceDayApiClient _api;
    public OrganiserController(RaceDayApiClient api) => _api = api;

    private bool IsOrganiser() => HttpContext.Session.GetString("Role")?
        .Equals("Organiser", StringComparison.OrdinalIgnoreCase) == true;

    public async Task<IActionResult> Index()
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Account");
        return View(await _api.GetEventsAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Account");
        return View("Edit", new EventViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Account");
        var model = await _api.GetEventAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(EventViewModel model, IFormFile? bannerImage)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid) return View("Edit", model);
        var result = await _api.SaveEventAsync(model, bannerImage);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View("Edit", model);
        }
        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Account");
        var result = await _api.DeleteEventAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }


    // CATEGORY MANAGEMENT PAGE
    [HttpGet]
    public async Task<IActionResult> Categories()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var categories = await _api.GetCategoriesAsync();
        ViewBag.Events = await _api.GetEventsAsync();

        return View(categories);
    }

    // CREATE OR UPDATE CATEGORY
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCategory(CategoryViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var result = model.CategoryID == 0
            ? await _api.CreateCategoryAsync(model)
            : await _api.UpdateCategoryAsync(model);

        TempData[result.Success ? "Success" : "Error"] = result.Message;

        return RedirectToAction(nameof(Categories));
    }

    // DELETE CATEGORY
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var result = await _api.DeleteCategoryAsync(id);

        TempData[result.Success ? "Success" : "Error"] = result.Message;

        return RedirectToAction(nameof(Categories));
    }

    // LINK CATEGORY TO EVENT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkCategory(int eventId, int categoryId)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var result = await _api.LinkCategoryToEventAsync(
            eventId, categoryId);

        TempData[result.Success ? "Success" : "Error"] = result.Message;

        return RedirectToAction(nameof(Categories));
    }

}
