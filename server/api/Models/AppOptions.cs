using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class AppOptions
{
    [Required] [MinLength(1)] public string DefaultConnection { get; set; } = null!;
    [Required] [MinLength(1)] public string JwtSecret { get; set; }
    public string FrontendUrl { get; set; } = "http://localhost:5173";
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public string? SmtpFromEmail { get; set; }

}