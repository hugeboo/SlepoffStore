namespace SlepoffStore.Controls
{
    partial class SheetAlarmControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.imageLabel = new System.Windows.Forms.Label();
            this.textLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imageLabel
            // 
            this.imageLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.imageLabel.Image = global::SlepoffStore.Properties.Resources.icons8_будильник_24_black;
            this.imageLabel.Location = new System.Drawing.Point(0, 0);
            this.imageLabel.Name = "imageLabel";
            this.imageLabel.Size = new System.Drawing.Size(30, 71);
            this.imageLabel.TabIndex = 0;
            // 
            // textLabel
            // 
            this.textLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.textLabel.Location = new System.Drawing.Point(30, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(325, 71);
            this.textLabel.TabIndex = 1;
            this.textLabel.Text = "label2";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SheetAlarmControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textLabel);
            this.Controls.Add(this.imageLabel);
            this.Name = "SheetAlarmControl";
            this.Size = new System.Drawing.Size(355, 71);
            this.ResumeLayout(false);

        }

        #endregion

        private Label imageLabel;
        private Label textLabel;
    }
}
