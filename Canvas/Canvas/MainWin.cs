using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Data.SqlClient;
using System.IO;

namespace Canvas
{
    public partial class MainWin : Form
    {
        DocumentForm f;
        string examName = null;
        string testID = null;
        string status = null;
        string stuID = null;
        string stuName = null;
        string className = null;
        string school = null;
        public List<string> filePathList = new List<string>();
        string connectionString = @"server=DESKTOP-BLPK865\SQLEXPRESS; database = CAD-题组信息; uid = sa; pwd = 123456";
        MenuItemManager m_menuItems;
        public int flag=1;
   
        /// <summary>
        /// status为状态，答题时，testID绘图题第几题，id为卷名,stuID为姓名
        /// 出题时,testID为题库题目ID
        /// 标准答案时,testID为题库题目ID
        /// </summary>
        /// <param name="status"></param>
        /// <param name="examName"></param>
        /// <param name="testId"></param>
        public MainWin(string status,string examName,string testID,string stuID,string stuName,string className,string school)
        {
            this.examName = examName;
            this.testID = testID;
            this.status = status;
            this.stuID = stuID;
            this.stuName = stuName;
            this.className = className;
            this.school = school;
            ///p为三点中点
			UnitPoint p = HitUtil.CenterPointFrom3Points(new UnitPoint(0, 2), new UnitPoint(1.4142136f, 1.4142136f), new UnitPoint(2, 0));

            InitializeComponent();
            Text = Program.AppName;         //设置窗口名称
            string[] args = Environment.GetCommandLineArgs();

            //从某个文件打开时参数会有两个
            if (args.Length == 2)           // assume it points to a file
                OpenDocument(args[1], status);
            else
                OpenDocument(string.Empty, status);

            m_menuItems = new MenuItemManager(this);
            m_menuItems.SetupStripPanels(); //初始化上下左右四个工具面板
            SetupToolbars();                //安装三个工具栏

            Application.Idle += new EventHandler(OnIdle);//添加触发,不断刷新三个控件
            if (status.Equals("答题"))
            {
                string sql = "declare @num int " +
                    "exec @num = findOrderNum '"+examName+"',"+testID+" " +
                    "print @num " +
                    "select 作图题题目 from 作图题库 where id=@num";

                OnFileOpen(sql);
            }
            if (status.Equals("修改答案"))
            {
                string sql = "use [CAD__"+school+"] select 画图题答案"+testID+" from testScore_"+examName+" where 学号='"+this.stuID+"'";
                OnFileOpen(sql);
            }
            if (status.Equals("标准答案"))
            {
                string sql = "select 作图题题目 from 作图题库 where id=" + testID;
                OnFileOpen(sql);
                //OpenDocument("D://9.cadxml", status);
            }
        }

        /// <summary>
        /// 安装工具栏(上/左/下)
        /// </summary>
        void SetupToolbars()
        {
            //#region "文件"下拉菜单项通过MenuItemManager初始化
            //MenuItem mmitem = m_menuItems.GetItem("New");
            //mmitem.Text = "&New";
            //mmitem.Text = "&新建";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.NewDocument);
            //mmitem.Click += new EventHandler(OnFileNew);
            //mmitem.ToolTipText = "New document";

            //mmitem = m_menuItems.GetItem("Open");
            //mmitem.Text = "&Open";
            //mmitem.Text = "&打开";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.OpenDocument);
            //mmitem.Click += new EventHandler(OnFileOpen);
            //mmitem.ToolTipText = "Open document";

            //mmitem = m_menuItems.GetItem("Save");
            //mmitem.Text = "&Save";
            //mmitem.Text = "&保存";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.SaveDocument);
            //mmitem.Click += new EventHandler(OnFileSave);
            //mmitem.ToolTipText = "Save document";

            //mmitem = m_menuItems.GetItem("SaveAs");
            //mmitem.Text = "Save &As";
            //mmitem.Text = "另存为";
            //mmitem.Click += new EventHandler(OnFileSaveAs);

            //mmitem = m_menuItems.GetItem("Exit");
            //mmitem.Text = "E&xit";
            //mmitem.Text = "退出";
            //mmitem.Click += new EventHandler(OnFileExit);
            //#endregion

            //#region "文件"图片列工具栏构造
            //ToolStrip strip = m_menuItems.GetStrip("file");
            //strip.Items.Add(m_menuItems.GetItem("New").CreateButton());
            //strip.Items.Add(m_menuItems.GetItem("Open").CreateButton());
            //strip.Items.Add(m_menuItems.GetItem("Save").CreateButton());
            //#endregion

            //#region "文件"下拉菜单栏添加控件
            //ToolStripMenuItem menuitem = m_menuItems.GetMenuStrip("file");
            //menuitem.Text = "&File";
            //menuitem.Text = "文件";
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("New").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Open").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Save").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("SaveAs").CreateMenuItem());
            //menuitem.DropDownItems.Add(new ToolStripSeparator());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Exit").CreateMenuItem());
            //m_mainMenu.Items.Insert(0, menuitem);
            //#endregion

            #region 上左下菜单栏/状态栏设置控件
            ToolStripPanel panel = m_menuItems.GetStripPanel(DockStyle.Top);

            panel.Join(m_menuItems.GetStrip("layer"));
            panel.Join(m_menuItems.GetStrip("draw"));
            panel.Join(m_menuItems.GetStrip("edit"));
            //panel.Join(m_menuItems.GetStrip("file"));
            //panel.Join(m_mainMenu); //主菜单加入面板
            m_mainMenu.Visible = false; //隐身空白面板

            panel = m_menuItems.GetStripPanel(DockStyle.Left);
            panel.Join(m_menuItems.GetStrip("modify")); //左侧修改栏

            panel = m_menuItems.GetStripPanel(DockStyle.Bottom);
            panel.Join(m_menuItems.GetStatusStrip("status")); //底部状态栏
            #endregion
        }

        /// <summary>
        /// 如果文档存在,刷新撤回前进移动控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnIdle(object sender, EventArgs e)
        {
            //MessageBox.Show("OnIdle");
            m_activeDocument = this.ActiveMdiChild as DocumentForm;
            if (m_activeDocument != null)
                m_activeDocument.UpdateUI();

        }
        DocumentForm m_activeDocument = null;
        protected override void OnMdiChildActivate(EventArgs e)
        {
            DocumentForm olddocument = m_activeDocument;
            base.OnMdiChildActivate(e);
            m_activeDocument = this.ActiveMdiChild as DocumentForm;
            foreach (Control ctrl in Controls)
            {
                if (ctrl is ToolStripPanel)
                    ((ToolStripPanel)ctrl).SuspendLayout();
            }
            if (m_activeDocument != null)
            {
                ToolStripManager.RevertMerge(m_menuItems.GetStrip("edit"));
                ToolStripManager.RevertMerge(m_menuItems.GetStrip("draw"));
                ToolStripManager.RevertMerge(m_menuItems.GetStrip("layer"));
                ToolStripManager.RevertMerge(m_menuItems.GetStrip("status"));
                ToolStripManager.RevertMerge(m_menuItems.GetStrip("modify"));
                ToolStripManager.Merge(m_activeDocument.GetToolStrip("draw"), m_menuItems.GetStrip("draw"));
                ToolStripManager.Merge(m_activeDocument.GetToolStrip("edit"), m_menuItems.GetStrip("edit"));
                ToolStripManager.Merge(m_activeDocument.GetToolStrip("layer"), m_menuItems.GetStrip("layer"));
                ToolStripManager.Merge(m_activeDocument.GetToolStrip("status"), m_menuItems.GetStrip("status"));
                ToolStripManager.Merge(m_activeDocument.GetToolStrip("modify"), m_menuItems.GetStrip("modify"));
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl is ToolStripPanel)
                    ((ToolStripPanel)ctrl).ResumeLayout();
            }
        }
        /// <summary>
        /// 读取从数据库题目
        /// </summary>
        /// <param name="id"></param>   
        void OnFileOpen(string sql)
        {
            try
            {
                
                SqlConnection SqlCon = new SqlConnection(connectionString); //数据库连接
                SqlCon.Open();
                SqlCommand com = new SqlCommand(sql, SqlCon);
                SqlDataReader dr = com.ExecuteReader();
                byte[] xmlbytes=null;
                while (dr.Read())
                {
                    xmlbytes = (byte[])dr[0];
                }
                string filePath =  @"D:\" + testID + ".cadxml";
                byte2File(xmlbytes, filePath);
                OpenDocument(filePath,status);
                SqlCon.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }
        /// <summary>
        /// 将答案传输到数据库
        /// </summary>
        void sendAnswer(string stauts) {
            byte[] answer = File2byte(  @"D:\" + testID + ".cadxml");
            string sql;
            if (status.Equals("答题"))
            {
                sql = "use [CAD__"+school+"] begin try " +
                    "update testScore_" + this.examName + " set 画图题答案" + this.testID + "=@file where 学号='" + this.stuID + "'  end try begin catch insert into testScore_" + this.examName + " (学号,画图题答案" + this.testID + ",姓名,年级班级) values('" + this.stuID + "',@file,'" + stuName + "','" + className + "') end catch";
            }
            else if (status.Equals("修改答案")) {

                sql = "use [CAD__" + school + "] update testScore_" + this.examName + " set 画图题答案"+this.testID+"=@file where 学号='" + this.stuID + "'";
            }
            else if (status.Equals("出题"))
            {
                sql = "insert into 作图题库 (作图题题目) values(@file)";
            }
            else 
            {
                sql = "UPDATE 作图题库 set 作图题答案=@file where id=" + this.testID;
            }

            UpToSql(answer,sql);
        }
        /// <summary>
        /// 传输截图
        /// </summary>
        /// <param name="id"></param>
        void sendPic(string status)
        {
            byte[] pic = File2byte(  @"D:\" + testID + ".jpg");
            string sql;
            if (status.Equals("答题"))
            {
                sql = "use [CAD__" + school + "] UPDATE testScore_" + this.examName + " set 画图题答案图片"+ this.testID + "=@file where 学号=" + this.stuID;
            }
            else if (status.Equals("修改答案"))
            {
                sql = "use [CAD__" + school + "] UPDATE testScore_" + this.examName + " set 画图题答案图片"+this.testID+"=@file where 学号=" + this.stuID;
            }
            else if (status.Equals("出题"))
            {
                sql = "UPDATE  作图题库 set 作图题题目图片=@file where id=" + this.testID;
            }
            else
            {
                sql = "UPDATE 作图题库 set 作图题答案图片=@file where id=" + this.testID;
            }
            UpToSql(pic, sql);
            File.Delete(@"D:\" + testID + ".jpg");
            File.Delete(@"D:\" + testID + ".cadxml");
        }
        public void UpToSql(byte[] b,string sql)
        {
            
            SqlConnection SqlCon = new SqlConnection(connectionString); //数据库连接
            try
            {
                SqlCon.Open();
                SqlCommand com = new SqlCommand(sql, SqlCon);
                com.Parameters.Add("@file", SqlDbType.Binary, b.Length);
                com.Parameters["@file"].Value = b;
                com.ExecuteNonQuery();
                
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally
            {
                SqlCon.Close();
            }
        }
        /// <summary>
        /// byte转文件
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="filePath"></param>
        public  void byte2File(byte[] buf, String filePath)
        {

            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                fs.Write(buf, 0, buf.Length);
                fs.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("打开绘图失败，请在网页重新打开");
                Close();
            }
        }
        /// <summary>
        /// 文件转byte
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public byte[] File2byte(String filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                return buffur;
            }
            catch (Exception ex)
            {
                //MessageBoxHelper.ShowPrompt(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {

                    //关闭资源
                    fs.Close();
                }
            }
        }
        
        /// <summary>
        /// 文件打开按钮触发器,打开.cadxml文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnFileOpen(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Cad XML files (*.cadxml)|*.cadxml";
            if (dlg.ShowDialog(this) == DialogResult.OK)
                OpenDocument(dlg.FileName);
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnFileSave(object sender, EventArgs e)
        {
            DocumentForm doc = this.ActiveMdiChild as DocumentForm;
            if (doc != null)
                doc.Save();
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnFileNew(object sender, EventArgs e)
  //      {
  //          OpenDocument(string.Empty);
  //      }

        /// <summary>
        /// 根据文件名打开文件
        /// </summary>
        /// <param name="filename"></param>
		void OpenDocument(string filename)
        {
            if (this.ActiveMdiChild != null)
            {
                MessageBox.Show("请先关闭当前的文档", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                //f = new DocumentForm(filename, status, testID); 
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }
        void OpenDocument(string filename, string status)
        {
           
            if (this.ActiveMdiChild != null)
            {
                MessageBox.Show("请先关闭当前的文档", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            f = new DocumentForm(filename, status, testID);
            f.getForm(this);
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }
        /// <summary>
        /// 另存为按钮触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnFileSaveAs(object sender, EventArgs e)
        {
            DocumentForm doc = this.ActiveMdiChild as DocumentForm;
            if (doc != null)
                doc.SaveAs();
        }

        /// <summary>
        /// 退出按钮触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnFileExit(object sender, EventArgs e)
  //      {
  //          Close();
  //      }

        /// <summary>
        /// 空的？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnUpdateMenuUI(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// "关于"触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnAbout(object sender, EventArgs e)
  //      {
  //          About dlg = new About();
  //          dlg.ShowDialog(this);
  //      }

        /// <summary>
        /// "选项"按钮触发器,Options.OptionsDlg没看不懂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnOptions(object sender, EventArgs e)
        {
            ///当前没有文件编辑窗口
			DocumentForm doc = this.ActiveMdiChild as DocumentForm;
            if (doc == null)
                return;

            Options.OptionsDlg dlg = new Canvas.Options.OptionsDlg();
            dlg.Config.Grid.CopyFromLayer(doc.Model.GridLayer as GridLayer);
            dlg.Config.Background.CopyFromLayer(doc.Model.BackgroundLayer as BackgroundLayer);
            foreach (DrawingLayer layer in doc.Model.Layers)
                dlg.Config.Layers.Add(new Options.OptionsLayer(layer));

            ToolStripItem item = sender as ToolStripItem;
            dlg.SelectPage(item.Tag);

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                dlg.Config.Grid.CopyToLayer((GridLayer)doc.Model.GridLayer);
                dlg.Config.Background.CopyToLayer((BackgroundLayer)doc.Model.BackgroundLayer);
                foreach (Options.OptionsLayer optionslayer in dlg.Config.Layers)
                {
                    DrawingLayer layer = (DrawingLayer)doc.Model.GetLayer(optionslayer.Layer.Id);
                    if (layer != null)
                        optionslayer.CopyToLayer(layer);
                    else
                    {
                        // delete layer
                    }
                }

                doc.Canvas.DoInvalidate(true);
            }
        }

        private void MainWin_MdiChildActivate(object sender, EventArgs e)
        {
            if (f.Disposing)
            {
                if (!string.IsNullOrEmpty(f.m_filename))
                {
                    foreach (string pathTemp in filePathList)
                    {
                        if (pathTemp.Equals(f.m_filename))
                        {
                            return;
                        }
                    }
                    filePathList.Add(f.m_filename);
                }
            }
;
        }
        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="id"></param>
        private  void GetScreenCapture(String id)
        {
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(this.Width, this.Height); // 用于屏幕原始图片保存
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(this.Location, Point.Empty, this.Size);
            gp.DrawImage(tSrcBmp, 0, 0, new Rectangle(0,0, this.Width, this.Height), GraphicsUnit.Pixel);
            Bitmap tSrcBmp2 = new Bitmap(1600,900);
            tSrcBmp2 = tSrcBmp;
            tSrcBmp2.Save(  @"D:\" + testID + ".jpg");
        }

        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.ActiveMdiChild.Refresh();
            try
            {
                if (examName != null && flag==1)
                {
                    sendAnswer(status);
                }
                Thread.Sleep(100);
                if (examName != null && flag==1)
                {
                    GetScreenCapture(examName);
                    sendPic(status);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}