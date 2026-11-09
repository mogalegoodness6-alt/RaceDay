
using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models;

public class ProfileViewModel
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = "";

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = "";

    public string Email { get; set; } = "";
}
