namespace app_configuration_dynamic_reload.Models;

public class PageSettings
{
    public string BackgroundColor { get; set; } = null!;
    public long FontSize { get; set; }
    public string FontColor { get; set; } = null!;
    public string Message { get; set; } = null!;
}
