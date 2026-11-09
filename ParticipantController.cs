
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _api;

    public ParticipantController(RaceDayApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        return View(await _api.GetEventsAsync());
    }

    public async Task<IActionResult> MyEnrolments()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var enrolments = await _api.GetEnrolmentsAsync();
            return View(enrolments);
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(new List<EnrollmentViewModel>());
        }
    }

    public async Task<IActionResult> MyResults()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var results = await _api.GetMyResultsAsync();
            return View(results);
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(new List<RaceResultViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        try
        {
            var profile = await _api.GetProfileAsync();

            if (profile == null)
            {
                ViewBag.Error =
                    "Could not load your profile. Please check your login and try again.";

                profile = new ProfileViewModel();
            }
            else
            {
                profile.Email = HttpContext.Session.GetString("Email") ?? "";
            }

            return View(profile);
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Error = ex.Message;

            return View(new ProfileViewModel
            {
                Email = HttpContext.Session.GetString("Email") ?? ""
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        model.Email = HttpContext.Session.GetString("Email") ?? "";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _api.UpdateProfileAsync(model);

            if (result.Success)
            {
                TempData["Success"] = result.Message;

                HttpContext.Session.SetString(
                    "Name",
                    $"{model.FirstName} {model.LastName}".Trim());

                return RedirectToAction(nameof(Profile));
            }

            ViewBag.Error = result.Message;
            return View(model);
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Error =
                $"Could not contact the RaceDay API: {ex.Message}";

            return View(model);
        }
    }

    // ENROL IN AN EVENT USING A SELECTED CATEGORY
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        if (eventId <= 0)
        {
            TempData["Error"] =
                "The selected event ID is invalid.";

            return RedirectToAction("Index", "Home");
        }

        if (categoryId <= 0)
        {
            TempData["Error"] =
                "Please select an event category before enrolling.";

            return RedirectToAction("Index");
        }

        try
        {
            var result = await _api.CreateEnrolmentAsync(
                eventId,
                categoryId);

            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] =
                $"Could not contact the RaceDay API: {ex.Message}";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                $"Something went wrong: {ex.Message}";
        }

        return RedirectToAction(nameof(MyEnrolments));
    }

    // CANCEL AN ENROLMENT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        if (id <= 0)
        {
            TempData["Error"] = "The enrolment ID is invalid.";
            return RedirectToAction(nameof(MyEnrolments));
        }

        try
        {
            var result = await _api.DeleteEnrolmentAsync(id);

            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] =
                $"Could not contact the RaceDay API: {ex.Message}";
        }

        return RedirectToAction(nameof(MyEnrolments));
    }

    // CHECK WHETHER THE LOGGED-IN USER IS A PARTICIPANT
    private bool IsParticipant()
    {
        return string.Equals(
            HttpContext.Session.GetString("Role"),
            "Participant",
            StringComparison.OrdinalIgnoreCase);
    }
}
