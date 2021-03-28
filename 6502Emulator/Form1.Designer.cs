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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.codeTextBox = new System.Windows.Forms.RichTextBox();
            this.BuildButton = new System.Windows.Forms.Button();
            this.DebugButton = new System.Windows.Forms.Button();
            this.fileComboBox = new System.Windows.Forms.ComboBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.ColorTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(71, 2);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(625, 52);
            this.dataGridView1.TabIndex = 4;
            // 
            // codeTextBox
            // 
            this.codeTextBox.Location = new System.Drawing.Point(712, 2);
            this.codeTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new System.Drawing.Size(582, 439);
            this.codeTextBox.TabIndex = 5;
            this.codeTextBox.Text = "";
            // 
            // BuildButton
            // 
            this.BuildButton.Location = new System.Drawing.Point(712, 446);
            this.BuildButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(360, 30);
            this.BuildButton.TabIndex = 6;
            this.BuildButton.Text = "Quick Build";
            this.BuildButton.UseVisualStyleBackColor = true;
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // DebugButton
            // 
            this.DebugButton.Location = new System.Drawing.Point(712, 511);
            this.DebugButton.Name = "DebugButton";
            this.DebugButton.Size = new System.Drawing.Size(360, 27);
            this.DebugButton.TabIndex = 9;
            this.DebugButton.Text = "Step Through";
            this.DebugButton.UseVisualStyleBackColor = true;
            this.DebugButton.Visible = false;
            this.DebugButton.Click += new System.EventHandler(this.DebugButton_Click);
            // 
            // fileComboBox
            // 
            this.fileComboBox.FormattingEnabled = true;
            this.fileComboBox.ItemHeight = 16;
            this.fileComboBox.Location = new System.Drawing.Point(712, 481);
            this.fileComboBox.Name = "fileComboBox";
            this.fileComboBox.Size = new System.Drawing.Size(360, 24);
            this.fileComboBox.TabIndex = 10;
            this.fileComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(712, 544);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(360, 27);
            this.RunButton.TabIndex = 11;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Visible = false;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // ColorTimer
            // 
            this.ColorTimer.Enabled = true;
            this.ColorTimer.Interval = 60;
            this.ColorTimer.Tick += new System.EventHandler(this.ColorTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1339, 778);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.fileComboBox);
            this.Controls.Add(this.DebugButton);
            this.Controls.Add(this.BuildButton);
            this.Controls.Add(this.codeTextBox);
            this.Controls.Add(this.dataGridView1);
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
        private System.Windows.Forms.Button DebugButton;
        private System.Windows.Forms.ComboBox fileComboBox;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.Timer ColorTimer;
    }
}

