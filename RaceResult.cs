namespace RaceDayAPI.Models;
public class RaceResult { public int ResultID { get; set; } public int EnrollmentID { get; set; } public TimeSpan? FinishTime { get; set; } public int? Position { get; set; } public string ResultStatus { get; set; } = "Pending"; }
