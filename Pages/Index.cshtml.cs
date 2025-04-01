using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BatchRunner.Models;
using BatchRunner.Services;

namespace BatchRunner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BatchService _batchService;
        private readonly ILogger<IndexModel> _logger;

        public List<BatchConfig> BatchConfigs { get; set; }
        public UIConfig? UIConfig { get; set; }
        public string ExecutionResult { get; set; }
        public bool ExecutionSuccess { get; set; }

        public IndexModel(BatchService batchService, ILogger<IndexModel> logger)
        {
            _batchService = batchService;
            _logger = logger;
            BatchConfigs = new List<BatchConfig>();
            ExecutionResult = string.Empty; // Initialize with a default value
        }

        public void OnGet()
        {
            try
            {
                var configs = _batchService.LoadConfigurations();
                BatchConfigs = configs.Configs;
                UIConfig = _batchService.GetUIConfig();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading batch configurations");
                ExecutionResult = "Error loading batch configurations. Please contact the administrator.";
                ExecutionSuccess = false;
            }
        }

        public async Task<IActionResult> OnPostAsync(string selectedBatch)
        {
            if (string.IsNullOrEmpty(selectedBatch))
            {
                ExecutionResult = "Please select a batch operation.";
                ExecutionSuccess = false;
                return Page();
            }

            try
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var (success, output) = await _batchService.ExecuteBatchFile(selectedBatch, clientIp);
                
                ExecutionSuccess = success;
                ExecutionResult = output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing batch file");
                ExecutionResult = "An error occurred while executing the batch file.";
                ExecutionSuccess = false;
            }

            // Reload configurations for the dropdown
            var configs = _batchService.LoadConfigurations();
            BatchConfigs = configs.Configs;
            UIConfig = configs.UIConfig; // Add this line to reload UIConfig

            return Page();
        }
    }
}
