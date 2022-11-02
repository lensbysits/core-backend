using System.ComponentModel.DataAnnotations;

namespace Lens.Services.Communication.Models;

public class EmailAddressBM
{
    public string? Name { get; set; }

    [Required(ErrorMessage = "Required")]
    [StringLength(320, ErrorMessage = "Max 320 (64@255) characters")]
    [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    public static implicit operator string?(EmailAddressBM mailaddess) => mailaddess.Email;
    public static implicit operator EmailAddressBM(string mailaddess) => new() { Email = mailaddess };
}
