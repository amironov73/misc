
namespace Html2Markdown
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._htmlBox = new System.Windows.Forms.TextBox();
            this._markdownBox = new System.Windows.Forms.TextBox();
            this._convertButton = new System.Windows.Forms.Button();
            this._pasteButton = new System.Windows.Forms.Button();
            this._copyButton = new System.Windows.Forms.Button();
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _htmlBox
            // 
            this._htmlBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._htmlBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._htmlBox.Location = new System.Drawing.Point(0, 0);
            this._htmlBox.Multiline = true;
            this._htmlBox.Name = "_htmlBox";
            this._htmlBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._htmlBox.Size = new System.Drawing.Size(458, 168);
            this._htmlBox.TabIndex = 0;
            this._htmlBox.Text = resources.GetString("_htmlBox.Text");
            // 
            // _markdownBox
            // 
            this._markdownBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._markdownBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._markdownBox.Location = new System.Drawing.Point(0, 0);
            this._markdownBox.Multiline = true;
            this._markdownBox.Name = "_markdownBox";
            this._markdownBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._markdownBox.Size = new System.Drawing.Size(458, 165);
            this._markdownBox.TabIndex = 1;
            // 
            // _convertButton
            // 
            this._convertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._convertButton.Location = new System.Drawing.Point(476, 41);
            this._convertButton.Name = "_convertButton";
            this._convertButton.Size = new System.Drawing.Size(96, 23);
            this._convertButton.TabIndex = 3;
            this._convertButton.Text = "Convert";
            this._convertButton.UseVisualStyleBackColor = true;
            this._convertButton.Click += new System.EventHandler(this._convertButton_Click);
            // 
            // _pasteButton
            // 
            this._pasteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._pasteButton.Location = new System.Drawing.Point(476, 12);
            this._pasteButton.Name = "_pasteButton";
            this._pasteButton.Size = new System.Drawing.Size(96, 23);
            this._pasteButton.TabIndex = 2;
            this._pasteButton.Text = "Paste";
            this._pasteButton.UseVisualStyleBackColor = true;
            this._pasteButton.Click += new System.EventHandler(this._pasteButton_Click);
            // 
            // _copyButton
            // 
            this._copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._copyButton.Location = new System.Drawing.Point(476, 70);
            this._copyButton.Name = "_copyButton";
            this._copyButton.Size = new System.Drawing.Size(96, 23);
            this._copyButton.TabIndex = 4;
            this._copyButton.Text = "Copy";
            this._copyButton.UseVisualStyleBackColor = true;
            this._copyButton.Click += new System.EventHandler(this._copyButton_Click);
            // 
            // _splitContainer
            // 
            this._splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._splitContainer.Cursor = System.Windows.Forms.Cursors.VSplit;
            this._splitContainer.Location = new System.Drawing.Point(12, 12);
            this._splitContainer.Name = "_splitContainer";
            this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.Controls.Add(this._htmlBox);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.Controls.Add(this._markdownBox);
            this._splitContainer.Size = new System.Drawing.Size(458, 337);
            this._splitContainer.SplitterDistance = 168;
            this._splitContainer.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this._splitContainer);
            this.Controls.Add(this._copyButton);
            this.Controls.Add(this._pasteButton);
            this.Controls.Add(this._convertButton);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "MainForm";
            this.Text = "HTML to Markdown converter";
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel1.PerformLayout();
            this._splitContainer.Panel2.ResumeLayout(false);
            this._splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox _htmlBox;
        private System.Windows.Forms.TextBox _markdownBox;
        private System.Windows.Forms.Button _convertButton;
        private System.Windows.Forms.Button _pasteButton;
        private System.Windows.Forms.Button _copyButton;
        private System.Windows.Forms.SplitContainer _splitContainer;
    }
}

