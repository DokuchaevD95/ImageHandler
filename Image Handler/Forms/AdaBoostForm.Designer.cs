namespace ImageHandler.Forms
{
    partial class AdaBoostForm
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
            this.trainButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadImageButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // trainButton
            // 
            this.trainButton.Location = new System.Drawing.Point(12, 12);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(142, 39);
            this.trainButton.TabIndex = 0;
            this.trainButton.Text = "Обучить";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.trainButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(12, 57);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(144, 39);
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "Загрузить дамп";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.Location = new System.Drawing.Point(204, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(353, 290);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // loadImageButton
            // 
            this.loadImageButton.Enabled = false;
            this.loadImageButton.Location = new System.Drawing.Point(204, 323);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(353, 42);
            this.loadImageButton.TabIndex = 3;
            this.loadImageButton.Text = "Загрузить изображение";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 102);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(144, 22);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // AdaBoostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 398);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.loadImageButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.trainButton);
            this.Name = "AdaBoostForm";
            this.Text = "AdaBoost";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button trainButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button loadImageButton;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}