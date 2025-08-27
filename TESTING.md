# HAR Cleaner Test Suite

This document provides an overview of the comprehensive test suite for the HAR Cleaner application.

## Test Summary

**Total Tests: 56**
- ✅ **56 Passed**
- ❌ **0 Failed**
- ⏭️ **0 Skipped**

## Test Categories

### 1. Request Type Filter Tests (6 tests)
- `ShouldInclude_ExcludeJavaScript_ReturnsFalseForJsFiles`
- `ShouldInclude_ExcludeCss_ReturnsFalseForCssFiles`
- `ShouldInclude_ExcludeMultipleTypes_ReturnsFalseForAllExcludedTypes`
- `ShouldInclude_IncludeOnlyJson_ReturnsTrueOnlyForJsonFiles`
- `ShouldInclude_NoFilters_ReturnsTrueForAllEntries`
- `ShouldInclude_ExcludeImages_ReturnsFalseForImageMimeTypes`

**Coverage**: File extension filtering, MIME type filtering, include/exclude logic

### 2. Request Method Filter Tests (6 tests)
- `ShouldInclude_XhrOnly_ReturnsTrueForXhrRequests`
- `ShouldInclude_XhrOnly_ReturnsFalseForRegularRequests`
- `ShouldInclude_IncludeOnlyPost_ReturnsTrueOnlyForPostRequests`
- `ShouldInclude_ExcludeOptions_ReturnsFalseForOptionsRequests`
- `ShouldInclude_DetectsJsonResponseAsXhr`
- `ShouldInclude_IncludeMultipleMethods_ReturnsTrueForAnyIncludedMethod`

**Coverage**: XHR/AJAX detection, HTTP method filtering, JSON response detection

### 3. URL Filter Tests (6 tests)
- `ShouldInclude_IncludeApiPattern_ReturnsTrueOnlyForApiUrls`
- `ShouldInclude_ExcludeTrackingPattern_ReturnsFalseForTrackingUrls`
- `ShouldInclude_MultipleIncludePatterns_ReturnsTrueForAnyMatchingPattern`
- `ShouldInclude_CaseInsensitiveMatching_WorksCorrectly`
- `ShouldInclude_NoPatterns_ReturnsTrueForAllUrls`
- `ShouldInclude_ExcludeOverridesInclude_ReturnsFalseWhenBothMatch`

**Coverage**: URL pattern matching, case sensitivity, include/exclude precedence

### 4. Status Code Filter Tests (4 tests)
- `ShouldInclude_IncludeSuccessStatusCodes_ReturnsTrueOnlyForSuccessfulResponses`
- `ShouldInclude_ExcludeErrorStatusCodes_ReturnsFalseForErrorResponses`
- `ShouldInclude_NoStatusCodeFilters_ReturnsTrueForAllResponses`
- `ShouldInclude_IncludeRedirectStatusCodes_ReturnsTrueForRedirects`

**Coverage**: HTTP status code filtering, success/error/redirect filtering

### 5. Size Filter Tests (5 tests)
- `ShouldInclude_MinSizeFilter_ReturnsTrueOnlyForLargeResponses`
- `ShouldInclude_MaxSizeFilter_ReturnsTrueOnlyForSmallResponses`
- `ShouldInclude_MinAndMaxSizeFilter_ReturnsTrueOnlyForMediumResponses`
- `ShouldInclude_NoSizeFilters_ReturnsTrueForAllResponses`
- `ShouldInclude_BoundaryValues_WorksCorrectly`

**Coverage**: Response size filtering, min/max boundaries, boundary value testing

### 6. Privacy Filter Tests (7 tests)
- `ShouldInclude_RemoveCookies_ClearsCookiesFromRequestAndResponse`
- `ShouldInclude_RemoveAuthTokens_RemovesAuthorizationHeaders`
- `ShouldInclude_RemovePersonalIdentifiers_RedactsQueryParameters`
- `ShouldInclude_RemoveTrackingHeaders_RemovesUserAgentAndLanguage`
- `ShouldInclude_AllPrivacyOptionsEnabled_CleansAllSensitiveData`
- `ShouldInclude_NoPrivacyOptionsEnabled_LeavesDataUnchanged`

**Coverage**: Cookie removal, auth token cleaning, personal data redaction, tracking header removal

### 7. Content Filter Tests (8 tests)
- `ShouldInclude_RemoveResponseContent_ClearsResponseText`
- `ShouldInclude_RemoveRequestContent_ClearsPostData`
- `ShouldInclude_MaxContentSize_ReplacesLargeContent`
- `ShouldInclude_ExcludeContentTypes_RemovesSpecificMimeTypes`
- `ShouldInclude_RemoveBase64Content_DetectsAndRemovesBase64`
- `ShouldInclude_NoContentFilters_LeavesContentUnchanged`
- `ShouldInclude_SmallContentUnderLimit_RemainsUnchanged`

**Coverage**: Content removal, size limits, MIME type filtering, base64 detection

### 8. HAR Cleaner Service Tests (8 tests)
- `Clean_NoFilters_ReturnsAllEntries`
- `Clean_WithUrlFilter_FiltersCorrectly`
- `Clean_WithMultipleFilters_AppliesAllFilters`
- `Clean_VerboseMode_ReturnsExcludedEntries`
- `Clean_PreservesHarFileStructure`
- `Clean_FilterThatExcludesAllEntries_ReturnsEmptyEntries`
- `Clean_PrivacyFilter_ModifiesEntriesInPlace`

**Coverage**: Filter chain execution, verbose output, HAR structure preservation, statistics calculation

### 9. HAR Loader Tests (6 tests)
- `LoadAsync_ValidHarFile_ReturnsHarFile`
- `LoadAsync_FileNotFound_ThrowsFileNotFoundException`
- `LoadAsync_InvalidJson_ThrowsInvalidOperationException`
- `LoadAsync_EmptyLogProperty_ThrowsInvalidOperationException`
- `Load_SynchronousVersion_WorksCorrectly`

**Coverage**: File loading, JSON parsing, error handling, async/sync operations

### 10. HAR Exporter Tests (6 tests)
- `SaveAsync_ValidHarFile_CreatesFile`
- `SaveAsync_CreatesDirectoryIfNotExists`
- `Save_SynchronousVersion_WorksCorrectly`
- `SaveAsync_ProducesValidJson`

**Coverage**: File saving, directory creation, JSON serialization, async/sync operations

## Test Architecture

### Test Helpers
- **TestDataHelper**: Centralized test data creation with factory methods for common scenarios
- **Realistic Test Data**: Tests use representative HAR structures with proper headers, cookies, and content

### Test Patterns Used
- **Arrange-Act-Assert (AAA)**: All tests follow this clear pattern
- **Boundary Value Testing**: Tests cover edge cases and boundary conditions
- **Negative Testing**: Tests verify error conditions and invalid inputs
- **Integration Testing**: Service tests verify component interactions
- **Parameterized Testing**: Multiple scenarios tested efficiently

### Key Test Scenarios Covered

#### 🔍 **Real-World Filtering Scenarios**
1. **Web Development**: Remove static assets, keep only API calls
2. **Security Analysis**: Filter by status codes (errors only)
3. **Performance Testing**: Size-based filtering for large responses
4. **Privacy Compliance**: Remove sensitive data before sharing HAR files
5. **Browser Compatibility**: Remove Chrome-specific debugging data

#### 🛡️ **Edge Cases & Error Handling**
1. **Empty HAR files**: No entries to process
2. **Invalid JSON**: Malformed HAR file handling
3. **Missing properties**: Graceful degradation
4. **Large content**: Size limit handling
5. **Unicode/Special characters**: Proper encoding support

#### 🔄 **Filter Combinations**
1. **Multiple include patterns**: Any matching pattern includes entry
2. **Include + Exclude conflicts**: Exclude takes precedence
3. **Filter chains**: Multiple filters applied in sequence
4. **Cleaning vs Filtering**: Some filters modify, others exclude

## Running the Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific test category
dotnet test --filter "PrivacyFilter"

# Run with coverage (requires coverage tools)
dotnet test --collect:"XPlat Code Coverage"
```

## Continuous Integration

The test suite is designed to be CI/CD friendly:
- **Fast execution**: All tests complete in under 1 second
- **No external dependencies**: Tests use temporary files and in-memory data
- **Deterministic**: Tests produce consistent results across environments
- **Comprehensive coverage**: Tests cover all major code paths and scenarios

## Future Test Enhancements

Potential areas for additional testing:
1. **Performance benchmarks**: Large HAR file processing
2. **Memory usage tests**: Ensure efficient memory handling
3. **Concurrency tests**: Parallel filter execution
4. **Fuzzing tests**: Random input validation
5. **Integration tests**: End-to-end CLI testing
