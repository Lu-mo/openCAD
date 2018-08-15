namespace Canvas.Options
{
	partial class GridPage
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
            this.m_gridHeight = new CommonTools.FloatEditor();
            this.m_gridBinding = new CommonTools.MyBindingSource();
            this.m_gridWidth = new CommonTools.FloatEditor();
            this.m_gridColor = new CommonTools.ColorPickerCombobox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.m_lines = new CommonTools.MyRadioButton();
            this.m_dots = new CommonTools.MyRadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.colorPickerCombobox1 = new CommonTools.ColorPickerCombobox();
            this.m_backgroundBinding = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBinding)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_backgroundBinding)).BeginInit();
            this.SuspendLayout();
            // 
            // m_gridHeight
            // 
            this.m_gridHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.m_gridBinding, "Spacing.Height", true));
            this.m_gridHeight.Location = new System.Drawing.Point(181, 143);
            this.m_gridHeight.Name = "m_gridHeight";
            this.m_gridHeight.Size = new System.Drawing.Size(123, 21);
            this.m_gridHeight.TabIndex = 3;
            this.m_gridHeight.Text = "0.1";
            this.m_gridHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.m_gridHeight.Value = 0.1F;
            // 
            // m_gridBinding
            // 
            this.m_gridBinding.DataSource = typeof(Canvas.Options.OptionsGrid);
            // 
            // m_gridWidth
            // 
            this.m_gridWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.m_gridBinding, "Spacing.Width", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.m_gridWidth.Location = new System.Drawing.Point(181, 119);
            this.m_gridWidth.Name = "m_gridWidth";
            this.m_gridWidth.Size = new System.Drawing.Size(123, 21);
            this.m_gridWidth.TabIndex = 2;
            this.m_gridWidth.Text = "0.1";
            this.m_gridWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.m_gridWidth.Value = 0.1F;
            // 
            // m_gridColor
            // 
            this.m_gridColor.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.m_gridBinding, "Color", true));
            this.m_gridColor.Location = new System.Drawing.Point(181, 92);
            this.m_gridColor.Name = "m_gridColor";
            this.m_gridColor.SelectedItem = System.Drawing.Color.Wheat;
            this.m_gridColor.Size = new System.Drawing.Size(123, 21);
            this.m_gridColor.TabIndex = 1;
            this.m_gridColor.Text = "m_gridColor";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.m_gridBinding, "Enabled", true));
            this.checkBox1.Location = new System.Drawing.Point(34, 25);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(66, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Enabled";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // m_lines
            // 
            this.m_lines.AutoSize = true;
            this.m_lines.CheckedValue = null;
            this.m_lines.Location = new System.Drawing.Point(34, 45);
            this.m_lines.Name = "m_lines";
            this.m_lines.Size = new System.Drawing.Size(101, 16);
            this.m_lines.TabIndex = 4;
            this.m_lines.TabStop = true;
            this.m_lines.Text = "Style - Lines";
            this.m_lines.UseVisualStyleBackColor = true;
            // 
            // m_dots
            // 
            this.m_dots.AutoSize = true;
            this.m_dots.CheckedValue = null;
            this.m_dots.Location = new System.Drawing.Point(34, 66);
            this.m_dots.Name = "m_dots";
            this.m_dots.Size = new System.Drawing.Size(83, 16);
            this.m_dots.TabIndex = 4;
            this.m_dots.TabStop = true;
            this.m_dots.Text = "Style Dots";
            this.m_dots.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.m_gridHeight);
            this.groupBox1.Controls.Add(this.m_dots);
            this.groupBox1.Controls.Add(this.m_gridColor);
            this.groupBox1.Controls.Add(this.m_lines);
            this.groupBox1.Controls.Add(this.m_gridWidth);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 186);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Grid Spacing Vertically";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Grid Spacing Horizontal";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(310, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "(units)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "(units)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Color";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.colorPickerCombobox1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(12, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 59);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Background";
            // 
            // colorPickerCombobox1
            // 
            this.colorPickerCombobox1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.m_backgroundBinding, "Color", true));
            this.colorPickerCombobox1.Location = new System.Drawing.Point(181, 20);
            this.colorPickerCombobox1.Name = "colorPickerCombobox1";
            this.colorPickerCombobox1.SelectedItem = System.Drawing.Color.Wheat;
            this.colorPickerCombobox1.Size = new System.Drawing.Size(123, 21);
            this.colorPickerCombobox1.TabIndex = 1;
            this.colorPickerCombobox1.Text = "m_gridColor";
            // 
            // m_backgroundBinding
            // 
            this.m_backgroundBinding.DataSource = typeof(Canvas.Options.OptionsBackground);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Color";
            // 
            // GridPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 261);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "GridPage";
            this.Text = "Grid settings";
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBinding)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_backgroundBinding)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private CommonTools.ColorPickerCombobox m_gridColor;
		private CommonTools.MyBindingSource m_gridBinding;
		private CommonTools.FloatEditor m_gridWidth;
		private CommonTools.FloatEditor m_gridHeight;
		private System.Windows.Forms.CheckBox checkBox1;
		private CommonTools.MyRadioButton m_lines;
		private CommonTools.MyRadioButton m_dots;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private CommonTools.ColorPickerCombobox colorPickerCombobox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.BindingSource m_backgroundBinding;
	}
}