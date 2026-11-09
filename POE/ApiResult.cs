namespace RaceDayMVC.Models;

public record ApiResult(bool Success, string Message, string? Payload = null);
