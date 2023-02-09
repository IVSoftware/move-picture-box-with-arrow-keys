
namespace move_picture_box_with_arrow_keys
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
            this.pictureBoxGreen = new move_picture_box_with_arrow_keys.ArrowKeyPictureBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.pictureBoxBlue = new move_picture_box_with_arrow_keys.ArrowKeyPictureBox();
            this.pictureBoxPortal = new move_picture_box_with_arrow_keys.ArrowKeyPictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPortal)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxGreen
            // 
            this.pictureBoxGreen.BackColor = System.Drawing.Color.LightGreen;
            this.pictureBoxGreen.IsMoveTarget = false;
            this.pictureBoxGreen.Location = new System.Drawing.Point(24, 45);
            this.pictureBoxGreen.Name = "pictureBoxGreen";
            this.pictureBoxGreen.Size = new System.Drawing.Size(60, 60);
            this.pictureBoxGreen.TabIndex = 0;
            this.pictureBoxGreen.TabStop = false;
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.Location = new System.Drawing.Point(24, 184);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(525, 348);
            this.richTextBox.TabIndex = 1;
            this.richTextBox.Text = "";
            // 
            // pictureBoxBlue
            // 
            this.pictureBoxBlue.BackColor = System.Drawing.Color.LightBlue;
            this.pictureBoxBlue.IsMoveTarget = false;
            this.pictureBoxBlue.Location = new System.Drawing.Point(123, 12);
            this.pictureBoxBlue.Name = "pictureBoxBlue";
            this.pictureBoxBlue.Size = new System.Drawing.Size(60, 60);
            this.pictureBoxBlue.TabIndex = 0;
            this.pictureBoxBlue.TabStop = false;
            // 
            // pictureBoxPortal
            // 
            this.pictureBoxPortal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.pictureBoxPortal.IsMoveTarget = false;
            this.pictureBoxPortal.Location = new System.Drawing.Point(139, 98);
            this.pictureBoxPortal.Name = "pictureBoxPortal";
            this.pictureBoxPortal.Size = new System.Drawing.Size(60, 60);
            this.pictureBoxPortal.TabIndex = 0;
            this.pictureBoxPortal.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(217, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "<- The Portal";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 544);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.pictureBoxPortal);
            this.Controls.Add(this.pictureBoxBlue);
            this.Controls.Add(this.pictureBoxGreen);
            this.Name = "MainForm";
            this.Text = "Main Form";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPortal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArrowKeyPictureBox pictureBoxGreen;
        private System.Windows.Forms.RichTextBox richTextBox;
        private ArrowKeyPictureBox pictureBoxBlue;
        private ArrowKeyPictureBox pictureBoxPortal;
        private System.Windows.Forms.Label label1;
    }
}

