namespace ProgramStatus
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.程序态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出题态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.制作标准答案态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.学生答题态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查阅态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.程序态ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Tag = "";
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 程序态ToolStripMenuItem
            // 
            this.程序态ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.出题态ToolStripMenuItem,
            this.制作标准答案态ToolStripMenuItem,
            this.学生答题态ToolStripMenuItem,
            this.查阅态ToolStripMenuItem});
            this.程序态ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.程序态ToolStripMenuItem.Name = "程序态ToolStripMenuItem";
            this.程序态ToolStripMenuItem.RightToLeftAutoMirrorImage = true;
            this.程序态ToolStripMenuItem.Size = new System.Drawing.Size(84, 31);
            this.程序态ToolStripMenuItem.Text = "程序态";
            // 
            // 出题态ToolStripMenuItem
            // 
            this.出题态ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.出题态ToolStripMenuItem.Name = "出题态ToolStripMenuItem";
            this.出题态ToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.出题态ToolStripMenuItem.Text = "出题态";
            this.出题态ToolStripMenuItem.Click += new System.EventHandler(this.出题态ToolStripMenuItem_Click);
            // 
            // 制作标准答案态ToolStripMenuItem
            // 
            this.制作标准答案态ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.制作标准答案态ToolStripMenuItem.Name = "制作标准答案态ToolStripMenuItem";
            this.制作标准答案态ToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.制作标准答案态ToolStripMenuItem.Text = "制作标准答案态";
            this.制作标准答案态ToolStripMenuItem.Click += new System.EventHandler(this.制作标准答案态ToolStripMenuItem_Click);
            // 
            // 学生答题态ToolStripMenuItem
            // 
            this.学生答题态ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.学生答题态ToolStripMenuItem.Name = "学生答题态ToolStripMenuItem";
            this.学生答题态ToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.学生答题态ToolStripMenuItem.Text = "学生答题态";
            this.学生答题态ToolStripMenuItem.Click += new System.EventHandler(this.学生答题态ToolStripMenuItem_Click);
            // 
            // 查阅态ToolStripMenuItem
            // 
            this.查阅态ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.查阅态ToolStripMenuItem.Name = "查阅态ToolStripMenuItem";
            this.查阅态ToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.查阅态ToolStripMenuItem.Text = "查阅态";
            this.查阅态ToolStripMenuItem.Click += new System.EventHandler(this.查阅态ToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 程序态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 出题态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 制作标准答案态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 学生答题态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查阅态ToolStripMenuItem;
    }
}

