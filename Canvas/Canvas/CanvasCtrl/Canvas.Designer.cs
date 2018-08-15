namespace Canvas
{
	partial class CanvasCtrl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.（释放资源）
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>如果托管资源应该被处理，则为true；否则，为false
        protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Component Designer generated code组件设计器生成代码

        /// <summary> 
        /// Required method for Designer support - do not modify 设计器支持所需的方法-不修改
        /// the contents of this method with the code editor.使用代码编辑器的方法的内容
        /// </summary>
        private void InitializeComponent()//初始化
		{
			this.SuspendLayout();//临时挂起控件的布局逻辑
			// 
			// Canvas
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);//控件尺寸
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;//自动缩放
			this.Name = "Canvas";//设置控件名
			this.Size = new System.Drawing.Size(309, 186);//设置高度宽度。
            this.ResumeLayout(false);//恢复控件布局逻辑

		}

		#endregion
	}
}
