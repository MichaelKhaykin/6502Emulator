namespace _6502Emulator
{
    partial class Form1
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.codeTextBox = new System.Windows.Forms.RichTextBox();
            this.BuildButton = new System.Windows.Forms.Button();
            this.BuildFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(80, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(703, 65);
            this.dataGridView1.TabIndex = 4;
            // 
            // codeTextBox
            // 
            this.codeTextBox.Location = new System.Drawing.Point(801, 2);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(405, 548);
            this.codeTextBox.TabIndex = 5;
            this.codeTextBox.Text = "";
            // 
            // BuildButton
            // 
            this.BuildButton.Location = new System.Drawing.Point(801, 557);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(405, 37);
            this.BuildButton.TabIndex = 6;
            this.BuildButton.Text = "Quick Build";
            this.BuildButton.UseVisualStyleBackColor = true;
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // BuildFile
            // 
            this.BuildFile.Location = new System.Drawing.Point(801, 600);
            this.BuildFile.Name = "BuildFile";
            this.BuildFile.Size = new System.Drawing.Size(405, 43);
            this.BuildFile.TabIndex = 7;
            this.BuildFile.Text = "Build From File";
            this.BuildFile.UseVisualStyleBackColor = true;
            this.BuildFile.Click += new System.EventHandler(this.BuildFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 722);
            this.Controls.Add(this.BuildFile);
            this.Controls.Add(this.BuildButton);
            this.Controls.Add(this.codeTextBox);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.RichTextBox codeTextBox;
        private System.Windows.Forms.Button BuildButton;
        private System.Windows.Forms.Button BuildFile;
    }
}

