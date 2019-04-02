namespace ImageHandler.Forms
{
    partial class StartForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.downloadFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // downloadFileButton
            // 
            this.downloadFileButton.Location = new System.Drawing.Point(42, 27);
            this.downloadFileButton.Name = "downloadFileButton";
            this.downloadFileButton.Size = new System.Drawing.Size(164, 66);
            this.downloadFileButton.TabIndex = 0;
            this.downloadFileButton.Text = "download";
            this.downloadFileButton.UseVisualStyleBackColor = true;
            this.downloadFileButton.Click += new System.EventHandler(this.downloadFileButtton_Click);
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 159);
            this.Controls.Add(this.downloadFileButton);
            this.Name = "StartForm";
            this.Text = "Стартовое окно";
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button downloadFileButton;
    }
}

