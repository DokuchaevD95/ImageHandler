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
            this.AdaBoost = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Cascade = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AdaBoost
            // 
            this.AdaBoost.Location = new System.Drawing.Point(12, 12);
            this.AdaBoost.Name = "AdaBoost";
            this.AdaBoost.Size = new System.Drawing.Size(125, 49);
            this.AdaBoost.TabIndex = 0;
            this.AdaBoost.Text = "AdaBoost";
            this.AdaBoost.UseVisualStyleBackColor = true;
            this.AdaBoost.Click += new System.EventHandler(this.AdaBoost_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(284, 69);
            this.button2.TabIndex = 1;
            this.button2.Text = "Нескоько примеров обучающей выборки";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Cascade
            // 
            this.Cascade.Location = new System.Drawing.Point(171, 12);
            this.Cascade.Name = "Cascade";
            this.Cascade.Size = new System.Drawing.Size(125, 49);
            this.Cascade.TabIndex = 2;
            this.Cascade.Text = "Cascade";
            this.Cascade.UseVisualStyleBackColor = true;
            this.Cascade.Click += new System.EventHandler(this.Cascade_Click);
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 179);
            this.Controls.Add(this.Cascade);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.AdaBoost);
            this.Name = "StartForm";
            this.Text = "Стартовое окно";
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button AdaBoost;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Cascade;
    }
}

