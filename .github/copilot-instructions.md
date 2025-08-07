# Zebra QR Code Generator - Project Instructions

## Project Overview

**Zebra** is a .NET 9.0 console application that processes text files containing URLs and generates QR codes for each URL, then creates a beautiful HTML report displaying the URLs alongside their corresponding QR codes.

## Key Features

- **URL Processing**: Reads URLs from text files and validates them
- **QR Code Generation**: Creates QR codes using the QRCoder library
- **HTML Report Generation**: Creates styled HTML reports with two approaches:
  - Traditional string concatenation ([`HtmlService`](Services/HtmlService.cs))
  - Razor templating engine ([`RazorHtmlService`](Services/RazorHtmlService.cs))
- **Error Handling**: Gracefully handles invalid URLs and displays errors in the report
- **Testing**: Built-in test commands and demo functionality

## Technology Stack

- **Framework**: .NET 9.0
- **Language**: C# with nullable reference types enabled
- **Dependencies**:
  - `QRCoder` (v1.6.0) - QR code generation
  - `RazorLight` (v2.3.1) - Razor templating engine
- **Output**: HTML files with embedded CSS and base64-encoded QR code images

## Project Structure


### Key Components

#### Services
- **[`ZebraService`](Services/ZebraService.cs)**: Main orchestration service that coordinates the entire process
- **[`FileService`](Services/FileService.cs)**: Handles reading URL files and writing HTML output
- **[`QrCodeService`](Services/QrCodeService.cs)**: Generates QR codes and validates URLs
- **[`HtmlService`](Services/HtmlService.cs)**: Creates HTML reports using string concatenation
- **[`RazorHtmlService`](Services/RazorHtmlService.cs)**: Creates HTML reports using Razor templates

#### Models
- **[`UrlQrPair`](Models/UrlQrPair.cs)**: Represents a URL with its generated QR code and validation status
- **[`QrCodeReportModel`](Models/QrCodeReportModel.cs)**: View model for Razor templates
- **[`ProcessResult`](Models/ProcessResult.cs)**: Tracks processing results and metrics

#### Configuration
- **[`ZebraConfig`](Configuration/ZebraConfig.cs)**: Application settings and configuration options

## Usage Examples

```bash
# Process URLs from file
zebra urls.txt output.html

# Run built-in tests
zebra --test

# Run demo
zebra --demo
```


### Testing Strategy

#### Unit Tests
- **Service Tests**: Mock dependencies, test individual service methods
- **Model Tests**: Validate model behavior, validation logic
- **Configuration Tests**: Test configuration loading and validation

#### Integration Tests
- **End-to-End**: Full workflow from input file to HTML output
- **File Processing**: Real file I/O operations
- **HTML Generation**: Template rendering with real data

#### Test Utilities
- **Test Data Builders**: Fluent builders for creating test models
- **File Helpers**: Temporary file management for tests
- **Custom Assertions**: Domain-specific assertion methods

### Testing Dependencies (Desired)
- `Microsoft.NET.Test.Sdk`
- `xUnit` or `NUnit` for test framework
- `FluentAssertions` for readable assertions
- `Moq` or `NSubstitute` for mocking
- `Microsoft.AspNetCore.Mvc.Testing` for integration tests

## Development Guidelines

1. **Follow SOLID principles** in service design
2. **Use dependency injection** for service composition
3. **Implement proper error handling** with meaningful messages
4. **Write comprehensive tests** for all public APIs
5. **Use async/await** for I/O operations
6. **Validate inputs** early and provide clear error messages
7. **Keep templates modular** using Razor partials
8. **Generate accessible HTML** with proper semantic markup

## CLI Commands

- `zebra <input-file> [output-file]` - Process URLs and generate HTML
- `zebra --test` or `zebra -t` - Run built-in tests
- `zebra --demo` or `zebra -d` - Run demo with sample data
- `zebra --help` or `zebra -h` - Show usage information

## Future Enhancements

1. **Configuration file support** (JSON/XML)
2. **Multiple output formats** (PDF, CSV)
3. **Batch processing** of multiple input files
4. **Custom QR code styling** options
5. **Web API interface** for remote processing
6. **Docker containerization** for deployment
7. **Performance metrics** and logging
8. **Plugin architecture** for extensibility