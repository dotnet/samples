using app_configuration_dynamic_reload.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace app_configuration_dynamic_reload.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PageSettings _pageSettings;

    public IndexModel(ILogger<IndexModel> logger, IOptionsSnapshot<PageSettings> pageSettings)
    {
        _logger = logger;
        _pageSettings = pageSettings.Value;
    }

    public void OnGet()
    {
        ViewData["BackgroundColor"] = _pageSettings.BackgroundColor;
        ViewData["FontSize"] = _pageSettings.FontSize;
        ViewData["FontColor"] = _pageSettings.FontColor;
        ViewData["Message"] = _pageSettings.Message;
    }
}
