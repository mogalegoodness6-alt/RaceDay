namespace RaceDayAPI.Models;
public class RaceEvent { public int EventID { get; set; } public int OrganizerID { get; set; } public string EventName { get; set; } = ""; public DateTime EventDate { get; set; } public string Location { get; set; } = ""; public string Status { get; set; } = "Upcoming"; }
