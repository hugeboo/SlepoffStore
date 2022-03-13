namespace SlepoffStore
{
    partial class SheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorsToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.captionToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox.Location = new System.Drawing.Point(11, 40);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(389, 147);
            this.textBox.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorsToolStripComboBox,
            this.captionToolStripTextBox,
            this.toolStripSeparator,
            this.closeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip.Size = new System.Drawing.Size(411, 116);
            this.contextMenuStrip.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip_Closing);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // colorsToolStripComboBox
            // 
            this.colorsToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorsToolStripComboBox.Items.AddRange(new object[] {
            "Black",
            "Blue",
            "Green",
            "Cyan",
            "Red",
            "Magenta",
            "Yellow",
            "White"});
            this.colorsToolStripComboBox.Name = "colorsToolStripComboBox";
            this.colorsToolStripComboBox.Size = new System.Drawing.Size(350, 33);
            this.colorsToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.colorsToolStripComboBox_SelectedIndexChanged);
            // 
            // captionToolStripTextBox
            // 
            this.captionToolStripTextBox.Name = "captionToolStripTextBox";
            this.captionToolStripTextBox.Size = new System.Drawing.Size(350, 31);
            this.captionToolStripTextBox.ToolTipText = "Change caption";
            this.captionToolStripTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.captionToolStripTextBox_KeyDown);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(407, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(410, 32);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // SheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 197);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.ControlBox = false;
            this.Controls.Add(this.textBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HeaderHeight = 40;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SheetForm";
            this.Padding = new System.Windows.Forms.Padding(11, 40, 11, 10);
            this.ResizeHandleSize = 6;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SheetForm";
            this.Activated += new System.EventHandler(this.SheetForm_Activated);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox;
        private System.Windows.Forms.Timer timer;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripTextBox captionToolStripTextBox;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripComboBox colorsToolStripComboBox;
    }
}