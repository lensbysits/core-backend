namespace Lens.Services.Communication.Settings;

public class SendEmailSettings
{
    public string? SenderName { get; set; }
    public string? SenderAddress { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? OnlySendTo { get; set; }
    public string? AlwaysBccTo { get; set; }
    public string? SubjectPrefix { get; set; }
    public bool ActuallySendEmails { get; set; }
    public bool NoAuthentication { get; set; }
    public bool NoSecurity { get; set; }
}
