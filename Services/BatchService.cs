using System.Diagnostics;
using System.Xml.Serialization;
using BatchRunner.Models;

namespace BatchRunner.Services
{
    public class BatchService
    {
        private readonly ILogger<BatchService> _logger;
        private readonly string _configPath;
        private readonly string _logPath;
        private BatchConfigurations? _cachedConfigs;

        public BatchService(ILogger<BatchService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "batchConfig.xml");
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public BatchConfigurations LoadConfigurations()
        {
            if (_cachedConfigs != null)
                return _cachedConfigs;

            try
            {
                using var stream = File.OpenRead(_configPath);
                var serializer = new XmlSerializer(typeof(BatchConfigurations));
                var result = serializer.Deserialize(stream) as BatchConfigurations;
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize batch configurations.");
                }
                _cachedConfigs = result;
                return _cachedConfigs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading batch configurations");
                throw;
            }
        }

        public UIConfig? GetUIConfig()
        {
            var configs = LoadConfigurations();
            return configs.UIConfig;
        }

        public async Task<(bool success, string output)> ExecuteBatchFile(string batchFilePath, string clientIp)
        {
            var logFile = Path.Combine(_logPath, $"batch_execution_{DateTime.Now:yyyyMMdd}.log");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {batchFilePath}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                var logMessage = $"[{timestamp}] IP: {clientIp} | Batch: {batchFilePath} | Exit Code: {process.ExitCode}\n";
                if (!string.IsNullOrEmpty(output))
                    logMessage += $"Output: {output}\n";
                if (!string.IsNullOrEmpty(error))
                    logMessage += $"Error: {error}\n";
                logMessage += "----------------------------------------\n";

                await File.AppendAllTextAsync(logFile, logMessage);

                return (process.ExitCode == 0, string.IsNullOrEmpty(error) ? output : error);
            }
            catch (Exception ex)
            {
                var errorMessage = $"[{timestamp}] IP: {clientIp} | Batch: {batchFilePath} | Error: {ex.Message}\n";
                await File.AppendAllTextAsync(logFile, errorMessage);
                return (false, ex.Message);
            }
        }
    }
}