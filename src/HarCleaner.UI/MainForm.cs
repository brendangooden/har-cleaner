using HarCleaner.Filters;
using HarCleaner.Models;
using HarCleaner.Services;

namespace HarCleaner.UI;

public partial class MainForm : Form
{
	private FilterOptions _filterOptions = new();
	private string? _inputFilePath;
	private string? _outputContent;

	public MainForm()
	{
		InitializeComponent();
		InitializeFormSettings();
	}

	private void InitializeFormSettings()
	{
		Text = "HAR Cleaner - GUI";
		Size = new Size(1000, 700);
		MinimumSize = new Size(800, 600);
		StartPosition = FormStartPosition.CenterScreen;

		// Enable drag and drop
		AllowDrop = true;
		DragEnter += MainForm_DragEnter;
		DragDrop += MainForm_DragDrop;
	}

	private void MainForm_DragEnter(object? sender, DragEventArgs e)
	{
		if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
		{
			var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
			if (files?.Length > 0 && files[0].EndsWith(".har", StringComparison.OrdinalIgnoreCase))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
		}
		e.Effect = DragDropEffects.None;
	}

	private void MainForm_DragDrop(object? sender, DragEventArgs e)
	{
		if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
		{
			var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
			if (files?.Length > 0 && files[0].EndsWith(".har", StringComparison.OrdinalIgnoreCase))
			{
				LoadInputFile(files[0]);
			}
		}
	}

	private void LoadInputFile(string filePath)
	{
		_inputFilePath = filePath;
		txtInputFile.Text = filePath;

		// Auto-generate output filename
		var directory = Path.GetDirectoryName(filePath) ?? "";
		var filenameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
		var extension = string.Equals(cmbOutputType.SelectedItem?.ToString(), "ml-ingest", StringComparison.OrdinalIgnoreCase) ? "json" : "har";
		var outputPath = Path.Combine(directory, $"{filenameWithoutExt}-cleaned.{extension}");
		txtOutputFile.Text = outputPath;

		btnProcess.Enabled = true;
		UpdateStatus($"Loaded: {Path.GetFileName(filePath)}");
	}

	private void btnBrowseInput_Click(object sender, EventArgs e)
	{
		using var dialog = new OpenFileDialog
		{
			Filter = "HAR Files (*.har)|*.har|All Files (*.*)|*.*",
			Title = "Select HAR File"
		};

		if (dialog.ShowDialog() == DialogResult.OK)
		{
			LoadInputFile(dialog.FileName);
		}
	}

	private void btnBrowseOutput_Click(object sender, EventArgs e)
	{
		var extension = string.Equals(cmbOutputType.SelectedItem?.ToString()?.ToLower(), "ml-ingest", StringComparison.OrdinalIgnoreCase) ? "json" : "har";
		using var dialog = new SaveFileDialog
		{
			Filter = $"{extension.ToUpper()} Files (*.{extension})|*.{extension}|All Files (*.*)|*.*",
			Title = "Save Cleaned File As"
		};

		if (dialog.ShowDialog() == DialogResult.OK)
		{
			txtOutputFile.Text = dialog.FileName;
		}
	}

	private void cmbOutputType_SelectedIndexChanged(object sender, EventArgs e)
	{
		// Update output file extension when output type changes
		if (!string.IsNullOrEmpty(txtOutputFile.Text))
		{
			var directory = Path.GetDirectoryName(txtOutputFile.Text) ?? "";
			var filenameWithoutExt = Path.GetFileNameWithoutExtension(txtOutputFile.Text);
			var extension = string.Equals(cmbOutputType.SelectedItem?.ToString()?.ToLower(), "ml-ingest", StringComparison.OrdinalIgnoreCase) ? "json" : "har";
			txtOutputFile.Text = Path.Combine(directory, $"{filenameWithoutExt}.{extension}");
		}
	}

	private async void btnProcess_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(_inputFilePath) || string.IsNullOrEmpty(txtOutputFile.Text))
		{
			MessageBox.Show("Please select both input and output files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		try
		{
			btnProcess.Enabled = false;
			UpdateStatus("Processing...");
			progressBar.Style = ProgressBarStyle.Marquee;

			// Collect filter options from UI
			CollectFilterOptions();

			// Process the HAR file
			await ProcessHarFileAsync();

			UpdateStatus("Processing completed successfully!");
			MessageBox.Show("HAR file processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		catch (Exception ex)
		{
			UpdateStatus($"Error: {ex.Message}");
			MessageBox.Show($"Error processing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			btnProcess.Enabled = true;
			progressBar.Style = ProgressBarStyle.Blocks;
			progressBar.Value = 0;
		}
	}

	private void CollectFilterOptions()
	{
		_filterOptions = new FilterOptions
		{
			InputFile = _inputFilePath!,
			OutputFile = txtOutputFile.Text,
			OutputType = cmbOutputType.SelectedItem?.ToString()?.ToLower() ?? "har",

			// Type filters
			ExcludeTypes = string.IsNullOrWhiteSpace(txtExcludeTypes.Text) ? null : txtExcludeTypes.Text,
			IncludeTypes = string.IsNullOrWhiteSpace(txtIncludeTypes.Text) ? null : txtIncludeTypes.Text,
			XhrOnly = chkXhrOnly.Checked,

			// URL filters
			IncludeUrlPatterns = string.IsNullOrWhiteSpace(txtIncludeUrl.Text) ? null : txtIncludeUrl.Text,
			ExcludeUrlPatterns = string.IsNullOrWhiteSpace(txtExcludeUrl.Text) ? null : txtExcludeUrl.Text,

			// Header filters
			IncludeHeaders = string.IsNullOrWhiteSpace(txtIncludeHeaders.Text) ? null : txtIncludeHeaders.Text,
			ExcludeHeaders = string.IsNullOrWhiteSpace(txtExcludeHeaders.Text) ? null : txtExcludeHeaders.Text,

			// Cookie filters
			IncludeCookies = string.IsNullOrWhiteSpace(txtIncludeCookies.Text) ? null : txtIncludeCookies.Text,
			ExcludeCookies = string.IsNullOrWhiteSpace(txtExcludeCookies.Text) ? null : txtExcludeCookies.Text,

			// Method filters
			IncludeMethods = string.IsNullOrWhiteSpace(txtIncludeMethods.Text) ? null : txtIncludeMethods.Text,
			ExcludeMethods = string.IsNullOrWhiteSpace(txtExcludeMethods.Text) ? null : txtExcludeMethods.Text,

			// Size filters
			MinSize = string.IsNullOrWhiteSpace(txtMinSize.Text) ? null : int.Parse(txtMinSize.Text),
			MaxSize = string.IsNullOrWhiteSpace(txtMaxSize.Text) ? null : int.Parse(txtMaxSize.Text),

			// Status filters
			IncludeStatusCodes = string.IsNullOrWhiteSpace(txtIncludeStatus.Text) ? null : txtIncludeStatus.Text,
			ExcludeStatusCodes = string.IsNullOrWhiteSpace(txtExcludeStatus.Text) ? null : txtExcludeStatus.Text,

			// Privacy options
			RemoveCookies = chkRemoveCookies.Checked,
			RemoveAuthTokens = chkRemoveAuth.Checked,
			RemovePersonalIdentifiers = chkRemovePersonal.Checked,
			RemoveTrackingHeaders = chkRemoveTracking.Checked,
			RemoveResponseContent = chkRemoveResponseContent.Checked,
			RemoveRequestContent = chkRemoveRequestContent.Checked,
			RemoveBase64Content = chkRemoveBase64.Checked,
			RemoveChromeData = chkRemoveChromeData.Checked,

			// Content options
			MaxContentSize = string.IsNullOrWhiteSpace(txtMaxContentSize.Text) ? null : long.Parse(txtMaxContentSize.Text),
			ExcludeContentTypes = string.IsNullOrWhiteSpace(txtExcludeContentTypes.Text) ? null : txtExcludeContentTypes.Text,

			Verbose = chkVerbose.Checked,
			DryRun = chkDryRun.Checked
		};
	}

	private async Task ProcessHarFileAsync()
	{
		await Task.Run(async () =>
		{
			var harLoader = new HarLoader();
			var harFile = await harLoader.LoadAsync(_filterOptions.InputFile);

			var harCleaner = new HarCleanerService();

			// Add request type filter
			if (_filterOptions.ExcludeTypesList.Length > 0 || _filterOptions.IncludeTypesList.Length > 0)
			{
				harCleaner.AddFilter(new RequestTypeFilter(_filterOptions.IncludeTypesList, _filterOptions.ExcludeTypesList));
			}

			// Add request method filter
			if (_filterOptions.XhrOnly || _filterOptions.IncludeMethodsList.Length > 0 || _filterOptions.ExcludeMethodsList.Length > 0)
			{
				harCleaner.AddFilter(new RequestMethodFilter(_filterOptions.XhrOnly, _filterOptions.IncludeMethodsList, _filterOptions.ExcludeMethodsList));
			}

			// Add URL filter
			if (_filterOptions.IncludeUrlPatternsList.Length > 0 || _filterOptions.ExcludeUrlPatternsList.Length > 0)
			{
				harCleaner.AddFilter(new UrlFilter(_filterOptions.IncludeUrlPatternsList, _filterOptions.ExcludeUrlPatternsList));
			}

			// Add header filter
			if (_filterOptions.IncludeHeadersList.Length > 0 || _filterOptions.ExcludeHeadersList.Length > 0)
			{
				harCleaner.AddFilter(new HeaderFilter(_filterOptions.IncludeHeadersList, _filterOptions.ExcludeHeadersList));
			}

			// Add status code filter
			if (_filterOptions.IncludeStatusCodesList.Length > 0 || _filterOptions.ExcludeStatusCodesList.Length > 0)
			{
				harCleaner.AddFilter(new StatusCodeFilter(_filterOptions.IncludeStatusCodesList, _filterOptions.ExcludeStatusCodesList));
			}

			// Add size filter
			if (_filterOptions.MinSize.HasValue || _filterOptions.MaxSize.HasValue)
			{
				harCleaner.AddFilter(new SizeFilter(_filterOptions.MinSize, _filterOptions.MaxSize));
			}

			// Add privacy filter
			if (_filterOptions.RemoveCookies || _filterOptions.RemoveAuthTokens || _filterOptions.RemovePersonalIdentifiers || _filterOptions.RemoveTrackingHeaders)
			{
				harCleaner.AddFilter(new PrivacyFilter(
					_filterOptions.RemoveCookies,
					_filterOptions.RemoveAuthTokens,
					_filterOptions.RemovePersonalIdentifiers,
					_filterOptions.RemoveTrackingHeaders));
			}

			// Add content filter
			if (_filterOptions.RemoveResponseContent || _filterOptions.RemoveRequestContent || _filterOptions.RemoveBase64Content ||
				_filterOptions.MaxContentSize.HasValue || _filterOptions.ExcludeContentTypesList.Length > 0)
			{
				harCleaner.AddFilter(new ContentFilter(
					_filterOptions.RemoveResponseContent,
					_filterOptions.RemoveRequestContent,
					_filterOptions.RemoveBase64Content,
					_filterOptions.MaxContentSize,
					_filterOptions.ExcludeContentTypesList));
			}

			// Add Chrome data filter
			if (_filterOptions.RemoveChromeData)
			{
				harCleaner.AddFilter(new ChromeDataFilter(true, true, true, true, true, true));
			}

			var result = harCleaner.Clean(harFile, _filterOptions.Verbose);

			// Save the result
			if (string.Equals(_filterOptions.OutputType?.ToLower(), "ml-ingest", StringComparison.OrdinalIgnoreCase))
			{
				var mlExporter = new MlIngestExporter();
				await mlExporter.SaveAsync(result.CleanedHarFile, _filterOptions.OutputFile);
			}
			else
			{
				var harExporter = new HarExporter();
				await harExporter.SaveAsync(result.CleanedHarFile, _filterOptions.OutputFile);
			}

			// Load content for preview
			_outputContent = await File.ReadAllTextAsync(_filterOptions.OutputFile);

			// Update UI on main thread
			await InvokeAsync(() =>
			{
				txtOutput.Text = _outputContent;

				lblStats.Text = $"Original entries: {result.OriginalCount}, " +
							   $"Filtered entries: {result.FilteredCount}, " +
							   $"Removed: {result.RemovedCount} ({result.RemovalPercentage:F1}%)";
			});
		});
	}

	private void btnSaveOutput_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(_outputContent))
		{
			MessageBox.Show("No output to save. Please process a file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		var extension = string.Equals(_filterOptions.OutputType?.ToLower(), "ml-ingest", StringComparison.OrdinalIgnoreCase) ? "json" : "har";
		using var dialog = new SaveFileDialog
		{
			Filter = $"{extension.ToUpper()} Files (*.{extension})|*.{extension}|All Files (*.*)|*.*",
			Title = "Save Output As",
			FileName = $"output.{extension}"
		};

		if (dialog.ShowDialog() == DialogResult.OK)
		{
			try
			{
				File.WriteAllText(dialog.FileName, _outputContent);
				MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}

	private void UpdateStatus(string message)
	{
		if (InvokeRequired)
		{
			Invoke(() => lblStatus.Text = message);
		}
		else
		{
			lblStatus.Text = message;
		}
	}
}
