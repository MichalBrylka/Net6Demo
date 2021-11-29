using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotReloadDemo.Pages;

public class Utils<T> {
        public static int GetProcessId(){
            return System.Environment.ProcessId;
        }
    }
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public int? ProcessId => Utils<int>.GetProcessId();
    public readonly string WelcomeText = "Welcome from";

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation($"{WelcomeText} {ProcessId}!");
    }
}
