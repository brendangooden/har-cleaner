# HAR Cleaner

A .NET CLI tool for cleaning and filtering HAR (HTTP Archive) files based on various criteria.

## Features

- **File Type Filtering**: Exclude/include requests based on file extensions or MIME types
- **Request Type Filtering**: Filter for XHR/AJAX requests vs regular page loads
- **URL Pattern Filtering**: Include/exclude based on URL contains/not contains patterns
- **HTTP Method Filtering**: Filter by GET, POST, etc.
- **Status Code Filtering**: Filter by response status codes
- **Size-based Filtering**: Filter by request/response size thresholds
- **Header Filtering**: Include/exclude specific headers from requests and responses (preserves entries)
- **Cookie Filtering**: Include/exclude specific cookies from requests and responses (preserves entries)
- **Privacy & Security Cleaning**: Remove sensitive data like cookies, auth tokens, personal identifiers
- **Content Filtering**: Remove or limit response/request content
- **Chrome DevTools Data**: Remove browser-specific debugging fields
- **ML-Ingest Export**: Export to structured JSON format optimized for machine learning analysis
- **Verbose Output**: See detailed information about what was filtered
- **Dry Run Mode**: Preview changes without saving

## Understanding Filter Types

HAR Cleaner provides two distinct types of filtering that work differently:

### Entry Filters (Remove Entire Requests)
These filters **completely remove** HTTP requests/responses from the HAR file. When a request matches these criteria, the entire entry is deleted from the output.

**Available Entry Filters:**
- `--exclude-types` / `--include-types`: Remove requests based on file extensions or MIME types
- `--include-url` / `--exclude-url`: Remove requests based on URL patterns  
- `--include-methods` / `--exclude-methods`: Remove requests based on HTTP methods
- `--include-status` / `--exclude-status`: Remove requests based on status codes
- `--min-size` / `--max-size`: Remove requests based on response size
- `--xhr-only`: Only keep XHR/AJAX requests, remove all others

**Example**: `--exclude-types js,css,png` removes all JavaScript, CSS, and image requests entirely from the HAR file.

### Content Filters (Keep Requests, Remove Content Body)
These filters **keep the HTTP request/response entries** but remove or modify the actual content body. The request metadata (headers, timing, URLs, etc.) is preserved.

**Available Content Filters:**
- `--exclude-content-types`: Keep requests but remove content body for specified MIME types
- `--max-content-size`: Keep requests but remove content body if larger than specified size
- `--remove-response-content`: Remove all response content bodies
- `--remove-request-content`: Remove all request content bodies (POST data)
- `--remove-base64`: Remove base64 encoded content
- Privacy options (`--remove-cookies`, `--remove-auth`, etc.): Remove sensitive data while keeping requests

**Example**: `--exclude-content-types image,video` keeps image/video requests in the HAR file with all their metadata (headers, timing, size), but replaces the actual content with `[CONTENT REMOVED - Type: image/png]`.

### Metadata Filters (Keep Requests, Modify Headers/Cookies)
These filters **keep the HTTP request/response entries** but selectively include or exclude specific headers and cookies from the metadata. All other request information (URL, timing, content, etc.) is preserved.

**Available Metadata Filters:**
- `--include-headers`: Only keep headers matching the specified patterns (others are removed)
- `--exclude-headers`: Remove headers matching the specified patterns (others are kept)
- `--include-cookies`: Only keep cookies matching the specified patterns (others are removed)
- `--exclude-cookies`: Remove cookies matching the specified patterns (others are kept)

**Examples**: 
- `--exclude-headers "user-agent,accept-language"` removes browser identification headers while keeping all other headers
- `--include-cookies "session,auth"` only keeps authentication-related cookies, removing tracking and analytics cookies
- `--exclude-cookies "tracking,analytics"` removes privacy-invasive cookies while preserving functional cookies

### When to Use Which Filter Type

**Use Entry Filters When:**
- You want to reduce HAR file size by removing unnecessary requests
- You want to focus analysis on specific types of requests  
- You don't need the metadata for filtered requests
- Examples: Remove all static assets to focus on API calls, only include POST requests for security analysis

**Use Content Filters When:**
- You want to preserve request metadata but remove sensitive/large content
- You need timing and header information but not the actual content
- You want to sanitize data while keeping the request structure
- Examples: Remove image content but keep timing data for performance analysis, sanitize responses while preserving request patterns

**Use Metadata Filters When:**
- You want to preserve requests and content but remove/filter specific headers or cookies
- You need to sanitize privacy-sensitive metadata while keeping functional data
- You want to focus analysis on specific headers/cookies without losing request context
- Examples: Remove tracking headers for privacy analysis, keep only authentication cookies for security testing, filter out browser fingerprinting headers

**Combining Filter Types:**
Entry filters are applied first (removing entire requests), then content filters are applied (modifying content), then metadata filters are applied (filtering headers/cookies). This allows powerful combinations like keeping only API requests AND removing large response bodies AND filtering sensitive headers.

## Installation

```bash
dotnet build
```

## Usage

### Basic Usage
```bash
# Clean a HAR file (copies all entries if no filters specified)
HarCleaner -i input.har -o output.har

# Export to ML-ingest format for machine learning analysis
HarCleaner -i input.har -o output.json --output-type ml-ingest
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

### Header Filtering
```bash
# Remove tracking and fingerprinting headers
HarCleaner -i input.har -o output.har --exclude-headers "user-agent,accept-language,accept-encoding"

# Keep only essential headers
HarCleaner -i input.har -o output.har --include-headers "authorization,content-type,content-length"

# Remove multiple header patterns
HarCleaner -i input.har -o output.har --exclude-headers "x-forwarded,x-real-ip,cf-"
```

### Cookie Filtering
```bash
# Remove tracking and analytics cookies
HarCleaner -i input.har -o output.har --exclude-cookies "tracking,analytics,_ga,_gid,fbp"

# Keep only session and authentication cookies
HarCleaner -i input.har -o output.har --include-cookies "session,auth,token,login"

# Remove advertising and social media cookies
HarCleaner -i input.har -o output.har --exclude-cookies "doubleclick,facebook,twitter,linkedin"
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

### ML-Ingest Export Format
```bash
# Export to structured JSON for machine learning analysis
HarCleaner -i input.har -o ml-data.json --output-type ml-ingest

# Combine with filtering for focused ML datasets
HarCleaner -i input.har -o api-ml-data.json \
  --output-type ml-ingest \
  --xhr-only \
  --include-url "api/"

# Privacy-cleaned ML export
HarCleaner -i input.har -o clean-ml-data.json \
  --output-type ml-ingest \
  --remove-cookies \
  --remove-auth \
  --remove-personal
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

### Header Filtering
```bash
# Exclude specific headers from requests and responses
HarCleaner -i input.har -o output.har --exclude-headers "user-agent,accept-language"

# Only include certain headers (remove all others)
HarCleaner -i input.har -o output.har --include-headers "authorization,content-type"
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
| `-o, --output` | Output file path (required) |
| `--output-type` | Output format: 'har' (default) or 'ml-ingest' |
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
| `--include-headers` | Comma-separated list of header names or patterns to include in requests and responses (others will be removed) |
| `--exclude-headers` | Comma-separated list of header names or patterns to exclude from requests and responses |
| `--include-cookies` | Comma-separated list of cookie names or patterns to include in requests and responses (others will be removed) |
| `--exclude-cookies` | Comma-separated list of cookie names or patterns to exclude from requests and responses |
| `--remove-chrome-data` | Remove Chrome DevTools specific fields |

## ML-Ingest Output Format

The ML-ingest export format produces structured JSON optimized for machine learning analysis. Each HTTP transaction is represented as a nested object with clearly separated request and response data.

### Structure
```json
[
  {
    "timestamp": "2025-08-26T23:21:45.197Z",
    "response_time_ms": 593.57,
    "request": {
      "method": "GET",
      "url": "https://example.com/api/users",
      "domain": "example.com",
      "path": "/api/users",
      "query_params": "page=1&limit=10",
      "headers": "accept: application/json; user-agent: Chrome/139.0.0.0",
      "cookies": "session_id=abc123; user_pref=dark_mode",
      "body": null,
      "size": 1024,
      "user_agent_category": "chrome",
      "has_auth": true
    },
    "response": {
      "status_code": 200,
      "headers": "content-type: application/json; cache-control: no-cache",
      "cookies": "new_session=xyz789; expires=Wed, 03 Sep 2025 23:21:46 GMT",
      "body": null,
      "size": 2048,
      "content_type": "application/json",
      "mime_type": "application/json",
      "cache_status": "no-cache"
    },
    "request_type": "xhr",
    "resource_type": "xhr"
  }
]
```

### Request Object Fields
- `method`: HTTP method (GET, POST, PUT, DELETE, etc.)
- `url`: Complete request URL
- `domain`: Extracted domain name
- `path`: URL path component
- `query_params`: Query string parameters
- `headers`: Request headers (concatenated string)
- `cookies`: Request cookies (concatenated string)
- `body`: Request body content (null if none)
- `size`: Request size in bytes
- `user_agent_category`: Detected browser type (chrome, firefox, safari, etc.)
- `has_auth`: Boolean indicating presence of authentication data

### Response Object Fields
- `status_code`: HTTP response status code
- `headers`: Response headers (concatenated string)
- `cookies`: Response Set-Cookie headers (concatenated string)
- `body`: Response body content (null if removed/filtered)
- `size`: Response size in bytes
- `content_type`: Primary content type
- `mime_type`: Full MIME type
- `cache_status`: Cache control information

### Top-Level Fields
- `timestamp`: ISO 8601 timestamp of when request was made
- `response_time_ms`: Time taken for response in milliseconds
- `request`: Complete request object (see above)
- `response`: Complete response object (see above)
- `request_type`: Classified request type (document, xhr, etc.)
- `resource_type`: Resource classification (document, script, image, etc.)

### Use Cases for ML-Ingest Format
- **Performance Analysis**: Analyze response times, sizes, and caching patterns
- **Security Analysis**: Identify authentication patterns and security headers
- **API Behavior**: Study request/response patterns for API endpoints
- **User Journey Analysis**: Track user interactions through web applications
- **Anomaly Detection**: Identify unusual request patterns or responses
- **Traffic Classification**: Categorize different types of web traffic

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

### Machine Learning Dataset Creation
Export API calls to structured format for ML analysis:
```bash
# Export all API calls with authentication patterns
HarCleaner -i production-traffic.har -o api-dataset.json \
  --output-type ml-ingest \
  --include-url "api/" \
  --verbose

# Create privacy-safe training dataset
HarCleaner -i user-sessions.har -o training-data.json \
  --output-type ml-ingest \
  --remove-personal \
  --remove-auth \
  --max-content-size 1024
```

## Build from Source

```bash
git clone <repository-url>
cd har-cleaner
dotnet build
dotnet run --project src/HarCleaner -- --help
```

## Testing

The project includes a comprehensive test suite with 107+ tests covering all components:

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
- **Filter Tests**: All 11 filter types with various scenarios (50+ tests)
- **Service Tests**: HAR loading, cleaning service, and export functionality (15+ tests)
- **Integration Tests**: End-to-end scenarios with real HAR data (25+ tests)
- **Edge Cases**: Error handling, empty files, malformed data (17+ tests)

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
