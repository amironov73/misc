
namespace NitroApp
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this._infoLabel = new System.Windows.Forms.Label();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._nameLabel = new System.Windows.Forms.Label();
            this._remainingLabel = new System.Windows.Forms.Label();
            this._listBox = new System.Windows.Forms.ListBox();
            this._startupTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._infoLabel);
            this.panel1.Controls.Add(this._progressBar);
            this.panel1.Controls.Add(this._nameLabel);
            this.panel1.Controls.Add(this._remainingLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 103);
            this.panel1.TabIndex = 0;
            // 
            // _infoLabel
            // 
            this._infoLabel.AllowDrop = true;
            this._infoLabel.AutoEllipsis = true;
            this._infoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._infoLabel.Location = new System.Drawing.Point(0, 70);
            this._infoLabel.Name = "_infoLabel";
            this._infoLabel.Size = new System.Drawing.Size(375, 33);
            this._infoLabel.TabIndex = 2;
            this._infoLabel.Text = "Information";
            this._infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._infoLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this._infoLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // _progressBar
            // 
            this._progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._progressBar.Location = new System.Drawing.Point(0, 47);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(375, 23);
            this._progressBar.TabIndex = 1;
            // 
            // _nameLabel
            // 
            this._nameLabel.AllowDrop = true;
            this._nameLabel.AutoEllipsis = true;
            this._nameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._nameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._nameLabel.Location = new System.Drawing.Point(0, 23);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(375, 24);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "File name";
            this._nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._nameLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this._nameLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // _remainingLabel
            // 
            this._remainingLabel.AllowDrop = true;
            this._remainingLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._remainingLabel.Location = new System.Drawing.Point(0, 0);
            this._remainingLabel.Name = "_remainingLabel";
            this._remainingLabel.Size = new System.Drawing.Size(375, 23);
            this._remainingLabel.TabIndex = 3;
            this._remainingLabel.Text = "Remaining";
            this._remainingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._remainingLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this._remainingLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // _listBox
            // 
            this._listBox.AllowDrop = true;
            this._listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listBox.FormattingEnabled = true;
            this._listBox.ItemHeight = 15;
            this._listBox.Location = new System.Drawing.Point(0, 103);
            this._listBox.Name = "_listBox";
            this._listBox.ScrollAlwaysVisible = true;
            this._listBox.Size = new System.Drawing.Size(375, 91);
            this._listBox.TabIndex = 1;
            this._listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this._listBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // _startupTimer
            // 
            this._startupTimer.Enabled = true;
            this._startupTimer.Tick += new System.EventHandler(this._startupTimer_Tick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 194);
            this.Controls.Add(this._listBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "NitroFlare downloader";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _nameLabel;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _infoLabel;
        private System.Windows.Forms.ListBox _listBox;
        private System.Windows.Forms.Label _remainingLabel;
        private System.Windows.Forms.Timer _startupTimer;
    }
}

