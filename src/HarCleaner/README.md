# HAR Cleaner

A .NET CLI tool for cleaning and filtering HAR (HTTP Archive) files based on various criteria.

## Features

- **File Type Filtering**: Exclude/include requests based on file extensions or MIME types
- **Request Type Filtering**: Filter for XHR/AJAX requests vs regular page loads
- **URL Pattern Filtering**: Include/exclude based on URL contains/not contains patterns
- **HTTP Method Filtering**: Filter by GET, POST, etc.
- **Status Code Filtering**: Filter by response status codes
- **Size-based Filtering**: Filter by request/response size thresholds
- **Privacy Filtering**: Remove cookies, auth tokens, personal identifiers, and tracking headers
- **Content Filtering**: Remove request/response bodies and large content
- **Output Formats**: Export as standard HAR or simplified ML-ingest format
- **Verbose Output**: See detailed information about what was filtered
- **Dry Run Mode**: Preview changes without saving

## Output Formats

### Standard HAR Format (default)
The traditional HAR format maintains full compatibility with standard HAR file specifications.

### ML-Ingest Format  
A simplified, flattened JSON format optimized for machine learning data ingestion:
- Flattened structure (array of simplified objects)
- Combined cookies into single string
- Merged headers (excluding sensitive auth/cookie headers)
- Extracted query parameters
- Includes both request and response bodies (when not filtered out)
- **Intelligent request classification** using `request_type` field derived from Chrome DevTools resource types:
  - `xhr`, `fetch` - AJAX/API calls
  - `document` - HTML pages
  - `script` - JavaScript files
  - `stylesheet` - CSS files
  - `image` - Images (PNG, JPG, etc.)
  - `font` - Web fonts
  - `media` - Audio/video files
  - `api` - API calls (detected by URL patterns when DevTools data unavailable)
  - `other` - Everything else
- Enhanced authentication detection (`has_auth`)
- User agent categorization
- Domain/path separation
- Chrome DevTools resource type preservation

```bash
# Export as ML-ingest format
HarCleaner -i input.har -o output.json --output-type ml-ingest

# Export as standard HAR format (default)
HarCleaner -i input.har -o output.har --output-type har
```

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

### Combined Filtering
```bash
# Complex filtering: XHR only, API calls, exclude tracking, successful responses only
HarCleaner -i input.har -o output.har \
  --xhr-only \
  --include-url "api/" \
  --exclude-url "tracking,analytics" \
  --include-status "200,201,202"
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
| `--output-type` | Output format: 'har' (default) or 'ml-ingest' |
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
| `-v, --verbose` | Enable verbose output |
| `--dry-run` | Preview changes without saving |

## Examples

### Cleaning a Development HAR
Remove all static assets and keep only API calls:
```bash
HarCleaner -i dev-session.har -o api-only.har \
  --exclude-types js,css,png,jpg,gif,svg,ico,woff,ttf \
  --include-url "api/"
```

### ML-Ingest Format for Data Analysis
Export simplified format for machine learning analysis:
```bash
# Basic ML-ingest export
HarCleaner -i session.har -o ml-data.json --output-type ml-ingest

# ML-ingest with privacy filtering
HarCleaner -i session.har -o clean-ml-data.json \
  --output-type ml-ingest \
  --remove-cookies \
  --remove-auth \
  --remove-personal

# ML-ingest for API analysis only
HarCleaner -i session.har -o api-analysis.json \
  --output-type ml-ingest \
  --include-url "api/" \
  --include-status "200,201,400,401,403,404,500"
```

### Debugging Failed Requests
Keep only failed requests for debugging:
```bash
HarCleaner -i full-session.har -o errors-only.har \
  --include-status "400,401,403,404,500,502,503" \
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
cd har-cleaner/HarCleaner
dotnet build
dotnet run -- --help
```

## Requirements

- .NET 8.0 or later

## License

MIT License
