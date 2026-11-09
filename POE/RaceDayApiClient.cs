
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using RaceDayMVC.Models;

namespace RaceDayMVC.Services;

public class RaceDayApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;

    private readonly JsonSerializerOptions _json =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    public RaceDayApiClient(
        HttpClient http,
        IHttpContextAccessor context)
    {
        _http = http;
        _context = context;
    }

    private void AddBearerToken()
    {
        var token = _context.HttpContext?.Session.GetString("ApiToken");

        _http.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
    }

    // LOGIN
    public async Task<(bool Success, string Message)> LoginAsync(
        LoginViewModel model)
    {
        using var response = await _http.PostAsJsonAsync(
            "api/Auth/login",
            new
            {
                email = model.Email,
                password = model.Password
            });

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return (false, ExtractMessage(
                body, "Login failed. Check your login details."));

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var token = FindString(root, "token", "accessToken", "jwt");
            var role = FindString(root, "role", "userRole") ?? "Participant";
            var name = FindString(root, "fullName", "name", "username")
                       ?? model.Email;

            if (string.IsNullOrWhiteSpace(token))
                return (false, "Login response did not contain an authentication token.");

            var session = _context.HttpContext!.Session;
            session.SetString("ApiToken", token);
            session.SetString("Role", role);
            session.SetString("Name", name);
            session.SetString("Email", model.Email);

            return (true, "Login successful.");
        }
        catch (JsonException)
        {
            return (false, "The API login response was not valid JSON.");
        }
    }

    // REGISTER
    public async Task<(bool Success, string Message)> RegisterAsync(
        RegisterViewModel model)
    {
        using var response = await _http.PostAsJsonAsync(
            "api/Auth/register",
            new
            {
                fullName = model.FullName,
                name = model.FullName,
                email = model.Email,
                password = model.Password,
                role = model.Role
            });

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Registration successful. You can now log in.")
            : (false, ExtractMessage(
                body, "Registration failed. Check the API requirements."));
    }

    // MAP API EVENT JSON TO MVC MODEL
    private static EventViewModel MapEvent(JsonElement item, int fallbackId = 0)
    {
        string ReadString(params string[] names)
        {
            foreach (var name in names)
            {
                if (item.TryGetProperty(name, out var value) &&
                    value.ValueKind == JsonValueKind.String)
                    return value.GetString() ?? "";
            }

            return "";
        }

        int ReadInt(params string[] names)
        {
            foreach (var name in names)
            {
                if (item.TryGetProperty(name, out var value) &&
                    value.TryGetInt32(out var number))
                    return number;
            }

            return fallbackId;
        }

        DateTime.TryParse(
            ReadString("eventDate", "date"),
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var eventDate);

        decimal distance = 5m;
        var distanceText = ReadString("distance");

        if (decimal.TryParse(
            distanceText,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var parsedDistance))
        {
            distance = parsedDistance;
        }
        else if (item.TryGetProperty("distance", out var distanceValue) &&
                 distanceValue.ValueKind == JsonValueKind.Number &&
                 distanceValue.TryGetDecimal(out parsedDistance))
        {
            distance = parsedDistance;
        }

        var eventType = ReadString("eventType", "type");

        return new EventViewModel
        {
            Id = ReadInt("eventID", "eventId", "id"),
            Name = ReadString("eventName", "name"),
            Description = ReadString("description"),
            Date = eventDate == default ? DateTime.Today : eventDate,
            Location = ReadString("location"),
            Distance = distance,
            EventType = string.IsNullOrWhiteSpace(eventType) ? "Run" : eventType,
            BannerImageUrl = ReadString("bannerImageUrl"),
            EnrolmentCount = 0
        };
    }

    // GET ALL EVENTS
    public async Task<List<EventViewModel>> GetEventsAsync()
    {
        AddBearerToken();

        try
        {
            using var response = await _http.GetAsync("api/Events");

            if (!response.IsSuccessStatusCode)
                return new();

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            if (document.RootElement.ValueKind != JsonValueKind.Array)
                return new();

            return document.RootElement
                .EnumerateArray()
                .Select(item => MapEvent(item))
                .ToList();
        }
        catch (HttpRequestException)
        {
            return new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    // GET ONE EVENT
    public async Task<EventViewModel?> GetEventAsync(int id)
    {
        AddBearerToken();

        try
        {
            using var response = await _http.GetAsync($"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return null;

            return MapEvent(document.RootElement, id);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // GET ENROLMENTS
    public async Task<List<EnrollmentViewModel>> GetEnrolmentsAsync()
    {
        AddBearerToken();

        try
        {
            using var response = await _http.GetAsync("api/enrolments");

            if (!response.IsSuccessStatusCode)
                return new();

            return await response.Content
                .ReadFromJsonAsync<List<EnrollmentViewModel>>(_json) ?? new();
        }
        catch (HttpRequestException)
        {
            return new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    // GET MY RESULTS
    public async Task<List<RaceResultViewModel>> GetMyResultsAsync()
    {
        AddBearerToken();

        using var response = await _http.GetAsync("api/results/mine");
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                ExtractMessage(body,
                    $"Could not load results. HTTP {(int)response.StatusCode}."));

        return JsonSerializer.Deserialize<List<RaceResultViewModel>>(
            body, _json) ?? new();
    }

    // ENROL IN AN EVENT WITH A SELECTED CATEGORY
    public async Task<(bool Success, string Message)> CreateEnrolmentAsync(
        int eventId,
        int categoryId,
        int? raceNumber = null)
    {
        AddBearerToken();

        using var response = await _http.PostAsJsonAsync(
            "api/enrolments",
            new
            {
                EventID = eventId,
                CategoryID = categoryId,
                RaceNumber = raceNumber
            });

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "You have successfully enrolled in the event.")
            : (false, ExtractMessage(
                body,
                $"Could not enrol. HTTP {(int)response.StatusCode}."));
    }

    // CANCEL ENROLMENT
    public async Task<(bool Success, string Message)> DeleteEnrolmentAsync(int id)
    {
        AddBearerToken();

        using var response = await _http.DeleteAsync($"api/enrolments/{id}");
        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Enrolment cancelled successfully.")
            : (false, ExtractMessage(
                body, $"Could not cancel enrolment. HTTP {(int)response.StatusCode}."));
    }

    // CREATE OR UPDATE EVENT AND UPLOAD OPTIONAL BANNER
    public async Task<(bool Success, string Message)> SaveEventAsync(
        EventViewModel model, IFormFile? banner)
    {
        AddBearerToken();

        var eventRequest = new
        {
            EventName = model.Name,
            Description = model.Description,
            EventDate = model.Date,
            Location = model.Location,
            Status = "Upcoming",
            Distance = model.Distance,
            EventType = model.EventType,
            BannerImageUrl = model.BannerImageUrl
        };

        using HttpResponseMessage response = model.Id == 0
            ? await _http.PostAsJsonAsync("api/Events", eventRequest)
            : await _http.PutAsJsonAsync(
                $"api/Events/{model.Id}", eventRequest);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return (false, ExtractMessage(
                body, $"Could not save event. HTTP {(int)response.StatusCode}."));
        }

        if (banner == null || banner.Length == 0)
            return (true, "Event saved successfully.");

        int eventId = model.Id;

        if (eventId == 0)
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                eventId = FindInt(document.RootElement,
                    "eventID", "eventId", "id");
            }
            catch (JsonException)
            {
                eventId = 0;
            }

            if (eventId == 0)
            {
                var events = await GetEventsAsync();

                var createdEvent = events
                    .Where(e =>
                        e.Name == model.Name &&
                        e.Location == model.Location)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefault();

                eventId = createdEvent?.Id ?? 0;
            }
        }

        if (eventId == 0)
        {
            return (true,
                "Event saved, but its image could not be uploaded because the event ID was not returned.");
        }

        var uploadResult = await UploadEventBannerAsync(eventId, banner);

        return uploadResult.Success
            ? (true, "Event and banner image saved successfully.")
            : (true, $"Event saved, but banner upload failed: {uploadResult.Message}");
    }

    // UPLOAD EVENT BANNER TO THE API
    private async Task<(bool Success, string Message)> UploadEventBannerAsync(
        int eventId, IFormFile banner)
    {
        AddBearerToken();

        using var content = new MultipartFormDataContent();
        using var fileStream = banner.OpenReadStream();
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(banner.ContentType)
                    ? "application/octet-stream"
                    : banner.ContentType);

        content.Add(
            fileContent,
            "image",
            Path.GetFileName(banner.FileName));

        using var response = await _http.PostAsync(
            $"api/Events/{eventId}/banner",
            content);

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Banner uploaded.")
            : (false, ExtractMessage(
                body, $"Image upload failed. HTTP {(int)response.StatusCode}."));
    }

    // FIND AN INTEGER VALUE IN JSON
    private static int FindInt(JsonElement root, params string[] names)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (names.Any(n => string.Equals(
                        n, property.Name, StringComparison.OrdinalIgnoreCase))
                    && property.Value.TryGetInt32(out var number))
                    return number;
            }

            foreach (var property in root.EnumerateObject())
            {
                var found = FindInt(property.Value, names);
                if (found != 0)
                    return found;
            }
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                var found = FindInt(item, names);
                if (found != 0)
                    return found;
            }
        }

        return 0;
    }

    // DELETE EVENT
    public async Task<(bool Success, string Message)> DeleteEventAsync(int id)
    {
        AddBearerToken();

        using var response = await _http.DeleteAsync($"api/Events/{id}");
        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Event deleted.")
            : (false, ExtractMessage(body, "Could not delete event."));
    }

    // GET PROFILE
    public async Task<ProfileViewModel?> GetProfileAsync()
    {
        AddBearerToken();

        using var response = await _http.GetAsync("api/Profile");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<ProfileViewModel>(_json);
    }

    // UPDATE PROFILE
    public async Task<(bool Success, string Message)> UpdateProfileAsync(
        ProfileViewModel model)
    {
        AddBearerToken();

        using var response = await _http.PutAsJsonAsync(
            "api/Profile",
            new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                phone = model.Phone
            });

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Profile updated successfully.")
            : (false, ExtractMessage(
                body, $"Could not update profile. HTTP {(int)response.StatusCode}."));
    }

    // GET ALL CATEGORIES
    public async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        AddBearerToken();

        using var response = await _http.GetAsync("api/categories");

        if (!response.IsSuccessStatusCode)
            return new();

        return await response.Content
            .ReadFromJsonAsync<List<CategoryViewModel>>(_json) ?? new();
    }

    // CREATE CATEGORY
    public async Task<(bool Success, string Message)> CreateCategoryAsync(
        CategoryViewModel model)
    {
        AddBearerToken();

        var request = new
        {
            model.CategoryName,
            model.DistanceKM,
            model.ActivityType,
            model.MinimumAge,
            model.MaximumAge
        };

        using var response = await _http.PostAsJsonAsync(
            "api/categories", request);

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Category created successfully.")
            : (false, ExtractMessage(body, "Could not create category."));
    }

    // UPDATE CATEGORY
    public async Task<(bool Success, string Message)> UpdateCategoryAsync(
        CategoryViewModel model)
    {
        AddBearerToken();

        var request = new
        {
            model.CategoryName,
            model.DistanceKM,
            model.ActivityType,
            model.MinimumAge,
            model.MaximumAge
        };

        using var response = await _http.PutAsJsonAsync(
            $"api/categories/{model.CategoryID}", request);

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Category updated successfully.")
            : (false, ExtractMessage(body, "Could not update category."));
    }

    // DELETE CATEGORY
    public async Task<(bool Success, string Message)> DeleteCategoryAsync(
        int id)
    {
        AddBearerToken();

        using var response = await _http.DeleteAsync(
            $"api/categories/{id}");

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Category deleted successfully.")
            : (false, ExtractMessage(body, "Could not delete category."));
    }

    // GET CATEGORIES LINKED TO AN EVENT
    public async Task<List<CategoryViewModel>> GetEventCategoriesAsync(
        int eventId)
    {
        AddBearerToken();

        using var response = await _http.GetAsync(
            $"api/events/{eventId}/categories");

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Could not load event categories. HTTP {(int)response.StatusCode}: {body}");
        }

        return JsonSerializer.Deserialize<List<CategoryViewModel>>(
            body, _json) ?? new List<CategoryViewModel>();
    }

    // LINK A CATEGORY TO AN EVENT
    public async Task<(bool Success, string Message)> LinkCategoryToEventAsync(
        int eventId, int categoryId)
    {
        AddBearerToken();

        using var response = await _http.PostAsJsonAsync(
            $"api/events/{eventId}/categories",
            new { CategoryID = categoryId });

        var body = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? (true, "Category linked to event successfully.")
            : (false, ExtractMessage(body, "Could not link category to event."));
    }

    // FIND A STRING VALUE IN JSON
    private static string? FindString(JsonElement root, params string[] names)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (names.Any(n => string.Equals(
                        n, property.Name, StringComparison.OrdinalIgnoreCase))
                    && property.Value.ValueKind == JsonValueKind.String)
                    return property.Value.GetString();
            }

            foreach (var property in root.EnumerateObject())
            {
                var found = FindString(property.Value, names);
                if (!string.IsNullOrWhiteSpace(found))
                    return found;
            }
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                var found = FindString(item, names);
                if (!string.IsNullOrWhiteSpace(found))
                    return found;
            }
        }

        return null;
    }

    // EXTRACT API ERROR MESSAGE
    private static string ExtractMessage(string body, string fallback)
    {
        if (string.IsNullOrWhiteSpace(body))
            return fallback;

        try
        {
            using var doc = JsonDocument.Parse(body);
            return FindString(doc.RootElement, "message", "title", "error")
                   ?? fallback;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }
}
