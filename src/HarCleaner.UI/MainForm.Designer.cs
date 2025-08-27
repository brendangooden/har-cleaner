namespace HarCleaner.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Main layout
            var mainTable = new TableLayoutPanel();
            var leftPanel = new Panel();
            var rightPanel = new Panel();

            // File selection
            var grpFiles = new GroupBox();
            var lblInputFile = new Label();
            txtInputFile = new TextBox();
            btnBrowseInput = new Button();
            var lblOutputFile = new Label();
            txtOutputFile = new TextBox();
            btnBrowseOutput = new Button();
            var lblOutputType = new Label();
            cmbOutputType = new ComboBox();

            // Filter options
            var grpFilters = new GroupBox();
            var filterTable = new TableLayoutPanel();

            // Type filters
            var lblExcludeTypes = new Label();
            txtExcludeTypes = new TextBox();
            var lblIncludeTypes = new Label();
            txtIncludeTypes = new TextBox();
            chkXhrOnly = new CheckBox();

            // URL filters
            var lblIncludeUrl = new Label();
            txtIncludeUrl = new TextBox();
            var lblExcludeUrl = new Label();
            txtExcludeUrl = new TextBox();

            // Header filters
            var lblIncludeHeaders = new Label();
            txtIncludeHeaders = new TextBox();
            var lblExcludeHeaders = new Label();
            txtExcludeHeaders = new TextBox();

            // Cookie filters
            var lblIncludeCookies = new Label();
            txtIncludeCookies = new TextBox();
            var lblExcludeCookies = new Label();
            txtExcludeCookies = new TextBox();

            // Method filters
            var lblIncludeMethods = new Label();
            txtIncludeMethods = new TextBox();
            var lblExcludeMethods = new Label();
            txtExcludeMethods = new TextBox();

            // Size filters
            var lblMinSize = new Label();
            txtMinSize = new TextBox();
            var lblMaxSize = new Label();
            txtMaxSize = new TextBox();

            // Status filters
            var lblIncludeStatus = new Label();
            txtIncludeStatus = new TextBox();
            var lblExcludeStatus = new Label();
            txtExcludeStatus = new TextBox();

            // Privacy options
            var grpPrivacy = new GroupBox();
            chkRemoveCookies = new CheckBox();
            chkRemoveAuth = new CheckBox();
            chkRemovePersonal = new CheckBox();
            chkRemoveTracking = new CheckBox();
            chkRemoveResponseContent = new CheckBox();
            chkRemoveRequestContent = new CheckBox();
            chkRemoveBase64 = new CheckBox();
            chkRemoveChromeData = new CheckBox();

            // Content options
            var grpContent = new GroupBox();
            var lblMaxContentSize = new Label();
            txtMaxContentSize = new TextBox();
            var lblExcludeContentTypes = new Label();
            txtExcludeContentTypes = new TextBox();

            // Processing options
            var grpProcessing = new GroupBox();
            chkVerbose = new CheckBox();
            chkDryRun = new CheckBox();
            btnProcess = new Button();

            // Output display
            var grpOutput = new GroupBox();
            txtOutput = new TextBox();
            btnSaveOutput = new Button();

            // Status bar
            var statusPanel = new Panel();
            lblStatus = new Label();
            lblStats = new Label();
            progressBar = new ProgressBar();

            // Suspend layout
            this.SuspendLayout();
            mainTable.SuspendLayout();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            grpFiles.SuspendLayout();
            grpFilters.SuspendLayout();
            filterTable.SuspendLayout();
            grpPrivacy.SuspendLayout();
            grpContent.SuspendLayout();
            grpProcessing.SuspendLayout();
            grpOutput.SuspendLayout();
            statusPanel.SuspendLayout();

            //
            // mainTable
            //
            mainTable.ColumnCount = 2;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainTable.Controls.Add(leftPanel, 0, 0);
            mainTable.Controls.Add(rightPanel, 1, 0);
            mainTable.Dock = DockStyle.Fill;
            mainTable.Location = new Point(0, 0);
            mainTable.Name = "mainTable";
            mainTable.RowCount = 1;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.Size = new Size(1000, 640);
            mainTable.TabIndex = 0;

            //
            // leftPanel
            //
            leftPanel.AutoScroll = true;
            leftPanel.Controls.Add(grpFiles);
            leftPanel.Controls.Add(grpFilters);
            leftPanel.Controls.Add(grpPrivacy);
            leftPanel.Controls.Add(grpContent);
            leftPanel.Controls.Add(grpProcessing);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(3, 3);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(5);
            leftPanel.Size = new Size(394, 634);
            leftPanel.TabIndex = 0;

            //
            // rightPanel
            //
            rightPanel.Controls.Add(grpOutput);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(403, 3);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(5);
            rightPanel.Size = new Size(594, 634);
            rightPanel.TabIndex = 1;

            //
            // grpFiles
            //
            grpFiles.Controls.Add(lblInputFile);
            grpFiles.Controls.Add(txtInputFile);
            grpFiles.Controls.Add(btnBrowseInput);
            grpFiles.Controls.Add(lblOutputFile);
            grpFiles.Controls.Add(txtOutputFile);
            grpFiles.Controls.Add(btnBrowseOutput);
            grpFiles.Controls.Add(lblOutputType);
            grpFiles.Controls.Add(cmbOutputType);
            grpFiles.Dock = DockStyle.Top;
            grpFiles.Location = new Point(5, 5);
            grpFiles.Name = "grpFiles";
            grpFiles.Padding = new Padding(8);
            grpFiles.Size = new Size(384, 130);
            grpFiles.TabIndex = 0;
            grpFiles.TabStop = false;
            grpFiles.Text = "Files (Drag & Drop HAR files here)";

            //
            // lblInputFile
            //
            lblInputFile.AutoSize = true;
            lblInputFile.Location = new Point(8, 20);
            lblInputFile.Name = "lblInputFile";
            lblInputFile.Size = new Size(70, 15);
            lblInputFile.TabIndex = 0;
            lblInputFile.Text = "Input File:";

            //
            // txtInputFile
            //
            txtInputFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInputFile.Location = new Point(8, 38);
            txtInputFile.Name = "txtInputFile";
            txtInputFile.ReadOnly = true;
            txtInputFile.Size = new Size(290, 23);
            txtInputFile.TabIndex = 1;

            //
            // btnBrowseInput
            //
            btnBrowseInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseInput.Location = new Point(305, 37);
            btnBrowseInput.Name = "btnBrowseInput";
            btnBrowseInput.Size = new Size(70, 25);
            btnBrowseInput.TabIndex = 2;
            btnBrowseInput.Text = "Browse...";
            btnBrowseInput.UseVisualStyleBackColor = true;
            btnBrowseInput.Click += btnBrowseInput_Click;

            //
            // lblOutputFile
            //
            lblOutputFile.AutoSize = true;
            lblOutputFile.Location = new Point(8, 70);
            lblOutputFile.Name = "lblOutputFile";
            lblOutputFile.Size = new Size(74, 15);
            lblOutputFile.TabIndex = 3;
            lblOutputFile.Text = "Output File:";

            //
            // txtOutputFile
            //
            txtOutputFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtOutputFile.Location = new Point(8, 88);
            txtOutputFile.Name = "txtOutputFile";
            txtOutputFile.Size = new Size(290, 23);
            txtOutputFile.TabIndex = 4;

            //
            // btnBrowseOutput
            //
            btnBrowseOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseOutput.Location = new Point(305, 87);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(70, 25);
            btnBrowseOutput.TabIndex = 5;
            btnBrowseOutput.Text = "Browse...";
            btnBrowseOutput.UseVisualStyleBackColor = true;
            btnBrowseOutput.Click += btnBrowseOutput_Click;

            //
            // lblOutputType
            //
            lblOutputType.AutoSize = true;
            lblOutputType.Location = new Point(200, 20);
            lblOutputType.Name = "lblOutputType";
            lblOutputType.Size = new Size(79, 15);
            lblOutputType.TabIndex = 6;
            lblOutputType.Text = "Output Type:";

            //
            // cmbOutputType
            //
            cmbOutputType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbOutputType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOutputType.FormattingEnabled = true;
            cmbOutputType.Items.AddRange(new object[] { "har", "ml-ingest" });
            cmbOutputType.Location = new Point(285, 17);
            cmbOutputType.Name = "cmbOutputType";
            cmbOutputType.Size = new Size(90, 23);
            cmbOutputType.TabIndex = 7;
            cmbOutputType.SelectedIndex = 0;
            cmbOutputType.SelectedIndexChanged += cmbOutputType_SelectedIndexChanged;

            // Configure remaining controls...
            SetupFilterControls(grpFilters, filterTable, lblExcludeTypes, txtExcludeTypes, lblIncludeTypes, txtIncludeTypes, chkXhrOnly,
                              lblIncludeUrl, txtIncludeUrl, lblExcludeUrl, txtExcludeUrl, lblIncludeHeaders, txtIncludeHeaders,
                              lblExcludeHeaders, txtExcludeHeaders, lblIncludeCookies, txtIncludeCookies,
                              lblExcludeCookies, txtExcludeCookies, lblIncludeMethods, txtIncludeMethods,
                              lblExcludeMethods, txtExcludeMethods, lblMinSize, txtMinSize, lblMaxSize, txtMaxSize,
                              lblIncludeStatus, txtIncludeStatus, lblExcludeStatus, txtExcludeStatus);

            SetupPrivacyControls(grpPrivacy, chkRemoveCookies, chkRemoveAuth, chkRemovePersonal, chkRemoveTracking,
                               chkRemoveResponseContent, chkRemoveRequestContent, chkRemoveBase64, chkRemoveChromeData);

            SetupContentControls(grpContent, lblMaxContentSize, txtMaxContentSize, lblExcludeContentTypes, txtExcludeContentTypes);

            SetupProcessingControls(grpProcessing, chkVerbose, chkDryRun, btnProcess);

            SetupOutputControls(grpOutput, txtOutput, btnSaveOutput);

            //
            // statusPanel
            //
            statusPanel.Controls.Add(lblStatus);
            statusPanel.Controls.Add(lblStats);
            statusPanel.Controls.Add(progressBar);
            statusPanel.Dock = DockStyle.Bottom;
            statusPanel.Location = new Point(0, 640);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(1000, 30);
            statusPanel.TabIndex = 1;

            //
            // lblStatus
            //
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(8, 8);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(100, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Ready to process";

            //
            // lblStats
            //
            lblStats.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStats.AutoSize = true;
            lblStats.Location = new Point(250, 8);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(0, 15);
            lblStats.TabIndex = 1;

            //
            // progressBar
            //
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            progressBar.Location = new Point(800, 5);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(190, 20);
            progressBar.TabIndex = 2;

            //
            // MainForm
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 670);
            this.Controls.Add(mainTable);
            this.Controls.Add(statusPanel);
            this.Name = "MainForm";
            this.Text = "HAR Cleaner - GUI";

            // Resume layout
            this.ResumeLayout(false);
            mainTable.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            grpFiles.ResumeLayout(false);
            grpFiles.PerformLayout();
            grpFilters.ResumeLayout(false);
            filterTable.ResumeLayout(false);
            filterTable.PerformLayout();
            grpPrivacy.ResumeLayout(false);
            grpPrivacy.PerformLayout();
            grpContent.ResumeLayout(false);
            grpContent.PerformLayout();
            grpProcessing.ResumeLayout(false);
            grpProcessing.PerformLayout();
            grpOutput.ResumeLayout(false);
            grpOutput.PerformLayout();
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
        }

        private void SetupFilterControls(GroupBox grpFilters, TableLayoutPanel filterTable, Label lblExcludeTypes, TextBox txtExcludeTypes,
            Label lblIncludeTypes, TextBox txtIncludeTypes, CheckBox chkXhrOnly, Label lblIncludeUrl, TextBox txtIncludeUrl,
            Label lblExcludeUrl, TextBox txtExcludeUrl, Label lblIncludeHeaders, TextBox txtIncludeHeaders,
            Label lblExcludeHeaders, TextBox txtExcludeHeaders, Label lblIncludeCookies, TextBox txtIncludeCookies,
            Label lblExcludeCookies, TextBox txtExcludeCookies, Label lblIncludeMethods, TextBox txtIncludeMethods,
            Label lblExcludeMethods, TextBox txtExcludeMethods, Label lblMinSize, TextBox txtMinSize, Label lblMaxSize,
            TextBox txtMaxSize, Label lblIncludeStatus, TextBox txtIncludeStatus, Label lblExcludeStatus, TextBox txtExcludeStatus)
        {
            grpFilters.Controls.Add(filterTable);
            grpFilters.Dock = DockStyle.Top;
            grpFilters.Location = new Point(5, 140);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(8);
            grpFilters.Size = new Size(384, 340); // Increased height for new header filters
            grpFilters.TabIndex = 1;
            grpFilters.TabStop = false;
            grpFilters.Text = "Entry Filters (Remove Entire Requests)";

            // Add help text
            var helpLabel = new Label
            {
                Text = "These filters completely remove HTTP requests from the HAR file.",
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Italic),
                AutoSize = false,
                Size = new Size(360, 20),
                Location = new Point(8, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };
            grpFilters.Controls.Add(helpLabel);

            filterTable.ColumnCount = 2;
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filterTable.Dock = DockStyle.Fill;
            filterTable.Location = new Point(8, 40); // Moved down for help text
            filterTable.Name = "filterTable";
            filterTable.RowCount = 14;
            for (int i = 0; i < 14; i++)
                filterTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            filterTable.Size = new Size(368, 292); // Adjusted for help text and new rows
            filterTable.TabIndex = 0;

            int row = 0;

            // Type filters - Remove entire entries
            lblExcludeTypes.Text = "Exclude Types:";
            lblExcludeTypes.Anchor = AnchorStyles.Left;
            txtExcludeTypes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeTypes.PlaceholderText = "js,css,png";
            filterTable.Controls.Add(lblExcludeTypes, 0, row);
            filterTable.Controls.Add(txtExcludeTypes, 1, row++);

            lblIncludeTypes.Text = "Include Types:";
            lblIncludeTypes.Anchor = AnchorStyles.Left;
            txtIncludeTypes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeTypes.PlaceholderText = "json,html";
            filterTable.Controls.Add(lblIncludeTypes, 0, row);
            filterTable.Controls.Add(txtIncludeTypes, 1, row++);

            chkXhrOnly.Text = "XHR Only";
            chkXhrOnly.Anchor = AnchorStyles.Left;
            filterTable.SetColumnSpan(chkXhrOnly, 2);
            filterTable.Controls.Add(chkXhrOnly, 0, row++);

            // URL filters
            lblIncludeUrl.Text = "Include URLs:";
            lblIncludeUrl.Anchor = AnchorStyles.Left;
            txtIncludeUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeUrl.PlaceholderText = "api,login";
            filterTable.Controls.Add(lblIncludeUrl, 0, row);
            filterTable.Controls.Add(txtIncludeUrl, 1, row++);

            lblExcludeUrl.Text = "Exclude URLs:";
            lblExcludeUrl.Anchor = AnchorStyles.Left;
            txtExcludeUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeUrl.PlaceholderText = "static,cdn";
            filterTable.Controls.Add(lblExcludeUrl, 0, row);
            filterTable.Controls.Add(txtExcludeUrl, 1, row++);

            // Header filters
            lblIncludeHeaders.Text = "Include Headers:";
            lblIncludeHeaders.Anchor = AnchorStyles.Left;
            txtIncludeHeaders.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeHeaders.PlaceholderText = "authorization,content-type";
            filterTable.Controls.Add(lblIncludeHeaders, 0, row);
            filterTable.Controls.Add(txtIncludeHeaders, 1, row++);

            lblExcludeHeaders.Text = "Exclude Headers:";
            lblExcludeHeaders.Anchor = AnchorStyles.Left;
            txtExcludeHeaders.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeHeaders.PlaceholderText = "user-agent,accept-language";
            filterTable.Controls.Add(lblExcludeHeaders, 0, row);
            filterTable.Controls.Add(txtExcludeHeaders, 1, row++);

            // Cookie filters
            lblIncludeCookies.Text = "Include Cookies:";
            lblIncludeCookies.Anchor = AnchorStyles.Left;
            txtIncludeCookies.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeCookies.PlaceholderText = "session,auth";
            filterTable.Controls.Add(lblIncludeCookies, 0, row);
            filterTable.Controls.Add(txtIncludeCookies, 1, row++);

            lblExcludeCookies.Text = "Exclude Cookies:";
            lblExcludeCookies.Anchor = AnchorStyles.Left;
            txtExcludeCookies.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeCookies.PlaceholderText = "tracking,analytics";
            filterTable.Controls.Add(lblExcludeCookies, 0, row);
            filterTable.Controls.Add(txtExcludeCookies, 1, row++);

            // Method filters
            lblIncludeMethods.Text = "Include Methods:";
            lblIncludeMethods.Anchor = AnchorStyles.Left;
            txtIncludeMethods.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeMethods.PlaceholderText = "GET,POST";
            filterTable.Controls.Add(lblIncludeMethods, 0, row);
            filterTable.Controls.Add(txtIncludeMethods, 1, row++);

            lblExcludeMethods.Text = "Exclude Methods:";
            lblExcludeMethods.Anchor = AnchorStyles.Left;
            txtExcludeMethods.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeMethods.PlaceholderText = "OPTIONS";
            filterTable.Controls.Add(lblExcludeMethods, 0, row);
            filterTable.Controls.Add(txtExcludeMethods, 1, row++);

            // Size filters
            lblMinSize.Text = "Min Size (bytes):";
            lblMinSize.Anchor = AnchorStyles.Left;
            txtMinSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtMinSize.PlaceholderText = "1024";
            filterTable.Controls.Add(lblMinSize, 0, row);
            filterTable.Controls.Add(txtMinSize, 1, row++);

            lblMaxSize.Text = "Max Size (bytes):";
            lblMaxSize.Anchor = AnchorStyles.Left;
            txtMaxSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtMaxSize.PlaceholderText = "1048576";
            filterTable.Controls.Add(lblMaxSize, 0, row);
            filterTable.Controls.Add(txtMaxSize, 1, row++);

            // Status filters
            lblIncludeStatus.Text = "Include Status:";
            lblIncludeStatus.Anchor = AnchorStyles.Left;
            txtIncludeStatus.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtIncludeStatus.PlaceholderText = "200,201";
            filterTable.Controls.Add(lblIncludeStatus, 0, row);
            filterTable.Controls.Add(txtIncludeStatus, 1, row++);

            lblExcludeStatus.Text = "Exclude Status:";
            lblExcludeStatus.Anchor = AnchorStyles.Left;
            txtExcludeStatus.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeStatus.PlaceholderText = "404,500";
            filterTable.Controls.Add(lblExcludeStatus, 0, row);
            filterTable.Controls.Add(txtExcludeStatus, 1, row++);
        }

        private void SetupPrivacyControls(GroupBox grpPrivacy, CheckBox chkRemoveCookies, CheckBox chkRemoveAuth,
            CheckBox chkRemovePersonal, CheckBox chkRemoveTracking, CheckBox chkRemoveResponseContent,
            CheckBox chkRemoveRequestContent, CheckBox chkRemoveBase64, CheckBox chkRemoveChromeData)
        {
            grpPrivacy.Dock = DockStyle.Top;
            grpPrivacy.Location = new Point(5, 485); // Adjusted for increased filter group height with header filters
            grpPrivacy.Name = "grpPrivacy";
            grpPrivacy.Padding = new Padding(8);
            grpPrivacy.Size = new Size(384, 120);
            grpPrivacy.TabIndex = 2;
            grpPrivacy.TabStop = false;
            grpPrivacy.Text = "Privacy Options";

            var privacyFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Location = new Point(8, 24),
                Name = "privacyFlow",
                Size = new Size(368, 88),
                TabIndex = 0
            };

            chkRemoveCookies.Text = "Remove Cookies";
            chkRemoveAuth.Text = "Remove Authentication";
            chkRemovePersonal.Text = "Remove Personal Data";
            chkRemoveTracking.Text = "Remove Tracking Headers";
            chkRemoveResponseContent.Text = "Remove Response Content";
            chkRemoveRequestContent.Text = "Remove Request Content";
            chkRemoveBase64.Text = "Remove Base64 Content";
            chkRemoveChromeData.Text = "Remove Chrome DevTools Data";

            privacyFlow.Controls.AddRange(new Control[] {
                chkRemoveCookies, chkRemoveAuth, chkRemovePersonal, chkRemoveTracking,
                chkRemoveResponseContent, chkRemoveRequestContent, chkRemoveBase64, chkRemoveChromeData
            });

            grpPrivacy.Controls.Add(privacyFlow);
        }

        private void SetupContentControls(GroupBox grpContent, Label lblMaxContentSize, TextBox txtMaxContentSize,
            Label lblExcludeContentTypes, TextBox txtExcludeContentTypes)
        {
            grpContent.Dock = DockStyle.Top;
            grpContent.Location = new Point(5, 610); // Adjusted for increased filter group height with header filters
            grpContent.Name = "grpContent";
            grpContent.Padding = new Padding(8);
            grpContent.Size = new Size(384, 100); // Increased height for help text
            grpContent.TabIndex = 3;
            grpContent.TabStop = false;
            grpContent.Text = "Content Filters (Keep Requests, Remove Content Body)";

            // Add help text
            var helpLabel = new Label
            {
                Text = "These options keep HTTP requests but remove or modify the content body.",
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Italic),
                AutoSize = false,
                Size = new Size(360, 20),
                Location = new Point(8, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };
            grpContent.Controls.Add(helpLabel);

            lblMaxContentSize.AutoSize = true;
            lblMaxContentSize.Location = new Point(8, 40); // Moved down for help text
            lblMaxContentSize.Name = "lblMaxContentSize";
            lblMaxContentSize.Size = new Size(120, 15);
            lblMaxContentSize.Text = "Max Content Size:";

            txtMaxContentSize.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMaxContentSize.Location = new Point(135, 37); // Moved down for help text
            txtMaxContentSize.Name = "txtMaxContentSize";
            txtMaxContentSize.PlaceholderText = "10000";
            txtMaxContentSize.Size = new Size(240, 23);

            lblExcludeContentTypes.AutoSize = true;
            lblExcludeContentTypes.Location = new Point(8, 70); // Moved down for help text
            lblExcludeContentTypes.Name = "lblExcludeContentTypes";
            lblExcludeContentTypes.Size = new Size(120, 15);
            lblExcludeContentTypes.Text = "Exclude Content Types:";

            txtExcludeContentTypes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExcludeContentTypes.Location = new Point(135, 67); // Moved down for help text
            txtExcludeContentTypes.Name = "txtExcludeContentTypes";
            txtExcludeContentTypes.PlaceholderText = "image,video";
            txtExcludeContentTypes.Size = new Size(240, 23);

            grpContent.Controls.AddRange(new Control[] {
                lblMaxContentSize, txtMaxContentSize, lblExcludeContentTypes, txtExcludeContentTypes
            });
        }

        private void SetupProcessingControls(GroupBox grpProcessing, CheckBox chkVerbose, CheckBox chkDryRun, Button btnProcess)
        {
            grpProcessing.Dock = DockStyle.Top;
            grpProcessing.Location = new Point(5, 715); // Adjusted for increased content group height with header filters
            grpProcessing.Name = "grpProcessing";
            grpProcessing.Padding = new Padding(8);
            grpProcessing.Size = new Size(384, 80);
            grpProcessing.TabIndex = 4;
            grpProcessing.TabStop = false;
            grpProcessing.Text = "Processing";

            chkVerbose.AutoSize = true;
            chkVerbose.Location = new Point(8, 25);
            chkVerbose.Name = "chkVerbose";
            chkVerbose.Size = new Size(67, 19);
            chkVerbose.Text = "Verbose";
            chkVerbose.UseVisualStyleBackColor = true;

            chkDryRun.AutoSize = true;
            chkDryRun.Location = new Point(8, 50);
            chkDryRun.Name = "chkDryRun";
            chkDryRun.Size = new Size(70, 19);
            chkDryRun.Text = "Dry Run";
            chkDryRun.UseVisualStyleBackColor = true;

            btnProcess.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnProcess.Enabled = false;
            btnProcess.Location = new Point(280, 25);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(95, 35);
            btnProcess.Text = "Process HAR";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;

            grpProcessing.Controls.AddRange(new Control[] { chkVerbose, chkDryRun, btnProcess });
        }

        private void SetupOutputControls(GroupBox grpOutput, TextBox txtOutput, Button btnSaveOutput)
        {
            grpOutput.Dock = DockStyle.Fill;
            grpOutput.Location = new Point(5, 5);
            grpOutput.Name = "grpOutput";
            grpOutput.Padding = new Padding(8);
            grpOutput.Size = new Size(584, 624);
            grpOutput.TabIndex = 0;
            grpOutput.TabStop = false;
            grpOutput.Text = "Output Preview";

            txtOutput.Dock = DockStyle.Fill;
            txtOutput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtOutput.Location = new Point(8, 24);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.Size = new Size(568, 565);
            txtOutput.WordWrap = false;

            btnSaveOutput.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveOutput.Location = new Point(480, 595);
            btnSaveOutput.Name = "btnSaveOutput";
            btnSaveOutput.Size = new Size(95, 25);
            btnSaveOutput.Text = "Save As...";
            btnSaveOutput.UseVisualStyleBackColor = true;
            btnSaveOutput.Click += btnSaveOutput_Click;

            grpOutput.Controls.AddRange(new Control[] { txtOutput, btnSaveOutput });
        }

        #endregion

        // Control declarations
        private TextBox txtInputFile = null!;
        private Button btnBrowseInput = null!;
        private TextBox txtOutputFile = null!;
        private Button btnBrowseOutput = null!;
        private ComboBox cmbOutputType = null!;

        private TextBox txtExcludeTypes = null!;
        private TextBox txtIncludeTypes = null!;
        private CheckBox chkXhrOnly = null!;
        private TextBox txtIncludeUrl = null!;
        private TextBox txtExcludeUrl = null!;
        private TextBox txtIncludeHeaders = null!;
        private TextBox txtExcludeHeaders = null!;
        private TextBox txtIncludeCookies = null!;
        private TextBox txtExcludeCookies = null!;
        private TextBox txtIncludeMethods = null!;
        private TextBox txtExcludeMethods = null!;
        private TextBox txtMinSize = null!;
        private TextBox txtMaxSize = null!;
        private TextBox txtIncludeStatus = null!;
        private TextBox txtExcludeStatus = null!;

        private CheckBox chkRemoveCookies = null!;
        private CheckBox chkRemoveAuth = null!;
        private CheckBox chkRemovePersonal = null!;
        private CheckBox chkRemoveTracking = null!;
        private CheckBox chkRemoveResponseContent = null!;
        private CheckBox chkRemoveRequestContent = null!;
        private CheckBox chkRemoveBase64 = null!;
        private CheckBox chkRemoveChromeData = null!;

        private TextBox txtMaxContentSize = null!;
        private TextBox txtExcludeContentTypes = null!;

        private CheckBox chkVerbose = null!;
        private CheckBox chkDryRun = null!;
        private Button btnProcess = null!;

        private TextBox txtOutput = null!;
        private Button btnSaveOutput = null!;

        private Label lblStatus = null!;
        private Label lblStats = null!;
        private ProgressBar progressBar = null!;
    }
}
