namespace EmguTest
{
    partial class VideoForm
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
            this.capRightPictureBox = new System.Windows.Forms.PictureBox();
            this.capLeftPictureBox = new System.Windows.Forms.PictureBox();
            this.stuffPictureBox1 = new System.Windows.Forms.PictureBox();
            this.stuffPictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.capRightPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.capLeftPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stuffPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stuffPictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // capRightPictureBox
            // 
            this.capRightPictureBox.Location = new System.Drawing.Point(437, 36);
            this.capRightPictureBox.Name = "capRightPictureBox";
            this.capRightPictureBox.Size = new System.Drawing.Size(400, 300);
            this.capRightPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.capRightPictureBox.TabIndex = 16;
            this.capRightPictureBox.TabStop = false;
            // 
            // capLeftPictureBox
            // 
            this.capLeftPictureBox.Location = new System.Drawing.Point(10, 36);
            this.capLeftPictureBox.Name = "capLeftPictureBox";
            this.capLeftPictureBox.Size = new System.Drawing.Size(400, 300);
            this.capLeftPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.capLeftPictureBox.TabIndex = 15;
            this.capLeftPictureBox.TabStop = false;
            // 
            // stuffPictureBox1
            // 
            this.stuffPictureBox1.Location = new System.Drawing.Point(12, 354);
            this.stuffPictureBox1.Name = "stuffPictureBox1";
            this.stuffPictureBox1.Size = new System.Drawing.Size(400, 300);
            this.stuffPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.stuffPictureBox1.TabIndex = 17;
            this.stuffPictureBox1.TabStop = false;
            // 
            // stuffPictureBox2
            // 
            this.stuffPictureBox2.Location = new System.Drawing.Point(438, 354);
            this.stuffPictureBox2.Name = "stuffPictureBox2";
            this.stuffPictureBox2.Size = new System.Drawing.Size(400, 300);
            this.stuffPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.stuffPictureBox2.TabIndex = 18;
            this.stuffPictureBox2.TabStop = false;
            // 
            // VideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 686);
            this.Controls.Add(this.stuffPictureBox2);
            this.Controls.Add(this.stuffPictureBox1);
            this.Controls.Add(this.capRightPictureBox);
            this.Controls.Add(this.capLeftPictureBox);
            this.Name = "VideoForm";
            this.Text = "VideoForm";
            this.Load += new System.EventHandler(this.VideoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.capRightPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.capLeftPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stuffPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stuffPictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox capRightPictureBox;
        private System.Windows.Forms.PictureBox capLeftPictureBox;
        private System.Windows.Forms.PictureBox stuffPictureBox1;
        private System.Windows.Forms.PictureBox stuffPictureBox2;
    }
}