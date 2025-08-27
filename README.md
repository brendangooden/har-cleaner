# HAR Cleaner

A .NET CLI tool for cleaning and filtering HAR (HTTP Archive) files based on various criteria.

## Features

- **File Type Filtering**: Exclude/include requests based on file extensions or MIME types
- **Request Type Filtering**: Filter for XHR/AJAX requests vs regular page loads
- **URL Pattern Filtering**: Include/exclude based on URL contains/not contains patterns
- **HTTP Method Filtering**: Filter by GET, POST, etc.
- **Status Code Filtering**: Filter by response status codes
- **Size-based Filtering**: Filter by request/response size thresholds
- **Privacy & Security Cleaning**: Remove sensitive data like cookies, auth tokens, personal identifiers
- **Content Filtering**: Remove or limit response/request content
- **Chrome DevTools Data**: Remove browser-specific debugging fields
- **Verbose Output**: See detailed information about what was filtered
- **Dry Run Mode**: Preview changes without saving

## Installation

```bash
dotnet build
```

## Usage

### Basic Usage
```bash
# Clean a HAR file (copies all entries if no filters specified)
HarCleaner -i input.har -o output.har
```

### File Type Filtering
```bash
# Exclude JavaScript, CSS, and image files
HarCleaner -i input.har -o output.har --exclude-types js,css,png,jpg,gif

# Include only JSON and XML responses
HarCleaner -i input.har -o output.har --include-types json,xml
```

### XHR/AJAX Filtering
```bash
# Include only XHR/AJAX requests
HarCleaner -i input.har -o output.har --xhr-only
```

### URL Pattern Filtering
```bash
# Include only URLs containing "api/"
HarCleaner -i input.har -o output.har --include-url "api/"

# Exclude tracking and analytics URLs
HarCleaner -i input.har -o output.har --exclude-url "tracking,analytics,google-analytics"

# Combine include and exclude patterns
HarCleaner -i input.har -o output.har --include-url "api/" --exclude-url "tracking"
```

### HTTP Method Filtering
```bash
# Include only GET requests
HarCleaner -i input.har -o output.har --include-methods GET

# Exclude OPTIONS requests
HarCleaner -i input.har -o output.har --exclude-methods OPTIONS
```

### Status Code Filtering
```bash
# Include only successful responses (200-299)
HarCleaner -i input.har -o output.har --include-status "200,201,202,204"

# Exclude error responses
HarCleaner -i input.har -o output.har --exclude-status "404,500,502,503"
```

### Size Filtering
```bash
# Include only responses larger than 1KB
HarCleaner -i input.har -o output.har --min-size 1024

# Exclude large responses (> 1MB)
HarCleaner -i input.har -o output.har --max-size 1048576
```

### Privacy and Security Filtering
```bash
# Remove all cookies and authentication data
HarCleaner -i input.har -o privacy-cleaned.har \
  --remove-cookies \
  --remove-auth \
  --remove-personal

# Remove tracking headers (user-agent, accept-language, etc.)
HarCleaner -i input.har -o tracking-cleaned.har --remove-tracking

# Remove all response content (useful for focusing on metadata)
HarCleaner -i input.har -o headers-only.har --remove-response-content

# Remove Chrome DevTools debugging data
HarCleaner -i input.har -o standard.har --remove-chrome-data
```

### Content Filtering
```bash
# Remove large response content
HarCleaner -i input.har -o lightweight.har --max-content-size 10240

# Remove specific content types
HarCleaner -i input.har -o no-media.har --exclude-content-types "image,video,audio"

# Remove base64 encoded content
HarCleaner -i input.har -o no-base64.har --remove-base64
```

### Combined Filtering
```bash
# Complex filtering: XHR only, API calls, exclude tracking, successful responses only
HarCleaner -i input.har -o output.har \
  --xhr-only \
  --include-url "api/" \
  --exclude-url "tracking,analytics" \
  --include-status "200,201,202"

# Privacy-focused cleaning for sharing HAR files
HarCleaner -i input.har -o shareable.har \
  --remove-cookies \
  --remove-auth \
  --remove-personal \
  --remove-tracking \
  --remove-response-content \
  --max-content-size 1024
```

### Verbose and Dry Run
```bash
# Preview changes without saving (dry run)
HarCleaner -i input.har -o output.har --xhr-only --dry-run

# Verbose output showing what was filtered
HarCleaner -i input.har -o output.har --xhr-only --verbose
```

## Command Line Options

| Option | Description |
|--------|-------------|
| `-i, --input` | Input HAR file path (required) |
| `-o, --output` | Output HAR file path (required) |
| `--exclude-types` | Comma-separated list of file types to exclude |
| `--include-types` | Comma-separated list of file types to include only |
| `--xhr-only` | Include only XHR/AJAX requests |
| `--include-url` | Comma-separated list of URL patterns that must be present |
| `--exclude-url` | Comma-separated list of URL patterns to exclude |
| `--include-methods` | Comma-separated list of HTTP methods to include |
| `--exclude-methods` | Comma-separated list of HTTP methods to exclude |
| `--min-size` | Minimum response size in bytes |
| `--max-size` | Maximum response size in bytes |
| `--include-status` | Comma-separated list of status codes to include |
| `--exclude-status` | Comma-separated list of status codes to exclude |
| `-v, --verbose` | Enable verbose output |
| `--dry-run` | Preview changes without saving |
| `--remove-cookies` | Remove all cookies from requests and responses |
| `--remove-auth` | Remove authentication tokens and headers |
| `--remove-personal` | Remove personal identifiers (sessions, user info) |
| `--remove-tracking` | Remove tracking headers (user-agent, accept-language, etc.) |
| `--remove-response-content` | Remove response body content |
| `--remove-request-content` | Remove request body content (POST data) |
| `--remove-base64` | Remove base64 encoded content |
| `--max-content-size` | Maximum content size in bytes (larger content will be removed) |
| `--exclude-content-types` | Comma-separated list of content types to remove content from |
| `--remove-chrome-data` | Remove Chrome DevTools specific fields |

## Examples

### Cleaning a Development HAR
Remove all static assets and keep only API calls:
```bash
HarCleaner -i dev-session.har -o api-only.har \
  --exclude-types js,css,png,jpg,gif,svg,ico,woff,ttf \
  --include-url "api/"
```

### Debugging Failed Requests
Keep only failed requests for debugging:
```bash
HarCleaner -i full-session.har -o errors-only.har \
  --include-status "400,401,403,404,500,502,503" \
  --verbose
```

### Privacy-Focused Cleaning
Prepare HAR files for sharing by removing sensitive data:
```bash
HarCleaner -i production-debug.har -o shareable.har \
  --remove-cookies \
  --remove-auth \
  --remove-personal \
  --remove-response-content \
  --exclude-types js,css,png,jpg \
  --verbose
```

### Performance Analysis
Keep only large responses that might impact performance:
```bash
HarCleaner -i session.har -o large-responses.har \
  --min-size 100000 \
  --verbose
```

## Build from Source

```bash
git clone <repository-url>
cd har-cleaner
dotnet build
dotnet run --project src/HarCleaner -- --help
```

## Testing

The project includes a comprehensive test suite with 56+ tests covering all components:

### Running Tests
```bash
# Run all tests from the solution root
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests from the test project directory
cd src/HarCleaner.Tests
dotnet test
```

### Test Coverage
The test suite covers:
- **Filter Tests**: All 9 filter types with various scenarios (28+ tests)
- **Service Tests**: HAR loading, cleaning service, and export functionality (15+ tests)
- **Integration Tests**: End-to-end scenarios with real HAR data (10+ tests)
- **Edge Cases**: Error handling, empty files, malformed data (8+ tests)

### Test Categories
- **Unit Tests**: Individual filter and service component testing
- **Integration Tests**: Full workflow testing with sample HAR files
- **Privacy Tests**: Verification of sensitive data removal
- **Performance Tests**: Large file handling and filter efficiency
- **Error Handling**: Invalid input and edge case scenarios

### Test Data
Tests use both generated test data and real-world HAR file samples to ensure comprehensive coverage of actual use cases.

## Requirements

- .NET 9.0 or later

## License

MIT License
