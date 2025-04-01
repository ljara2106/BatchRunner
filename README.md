# BatchRunner

A web-based application for executing and managing batch operations through a simple web interface. This tool allows you to configure and run batch files with logging capabilities.

## Features

- XML-based configuration for batch operations
- Web interface for executing batch files
- Execution logging with timestamps and client IP tracking
- Configurable UI elements through XML
- Real-time execution output display
- Error handling and logging

## Configuration

The application uses an XML configuration file located at `Config/batchConfig.xml` with the following structure:

```xml
<BatchConfigurations>
  <UIConfig>
    <Title>Your Title</Title>
    <UIDescription>Your Description</UIDescription>
  </UIConfig>
  <BatchConfig>
    <Name>Operation Name</Name>
    <Description>Operation Description</Description>
    <BatchFilePath>Path to batch file</BatchFilePath>
  </BatchConfig>
</BatchConfigurations>
```

## Technical Details

- Built with ASP.NET Core 8.0
- Uses XML serialization for configuration management
- Implements logging system with detailed execution tracking
- Supports multiple batch operations configuration

## Getting Started

1. Clone the repository
2. Configure your batch operations in `Config/batchConfig.xml`
3. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

## Logs

Execution logs are automatically created in the `Logs` directory with the format:
- `batch_execution_YYYYMMDD.log`

## Project Structure

- `/Config` - Configuration files
- `/Models` - Data models for batch configurations
- `/Services` - Core services including BatchService
- `/Logs` - Execution logs (auto-generated)

## Requirements

- .NET 8.0
- Modern web browser
- Windows environment (for batch file execution)
