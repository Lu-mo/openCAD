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
        string id=null;
        public List<string> filePathList = new List<string>();

        MenuItemManager m_menuItems;

        public MainWin(String status)
        {
            ///pΪ�����е�
			UnitPoint p = HitUtil.CenterPointFrom3Points(new UnitPoint(0, 2), new UnitPoint(1.4142136f, 1.4142136f), new UnitPoint(2, 0));

            InitializeComponent();
            Text = Program.AppName;         //���ô�������
            string[] args = Environment.GetCommandLineArgs();

            //��ĳ���ļ���ʱ������������
            if (args.Length == 2)           // assume it points to a file
                OpenDocument(args[1], status);
            else
                OpenDocument(string.Empty, status);

            m_menuItems = new MenuItemManager(this);
            m_menuItems.SetupStripPanels(); //��ʼ�����������ĸ��������
            SetupToolbars();                //��װ����������

            Application.Idle += new EventHandler(OnIdle);//��Ӵ���,����ˢ�������ؼ�

            //OnFileOpen(id.ToString());
        }

        /// <summary>
        /// ��װ������(��/��/��)
        /// </summary>
        void SetupToolbars()
        {
            //#region "�ļ�"�����˵���ͨ��MenuItemManager��ʼ��
            //MenuItem mmitem = m_menuItems.GetItem("New");
            //mmitem.Text = "&New";
            //mmitem.Text = "&�½�";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.NewDocument);
            //mmitem.Click += new EventHandler(OnFileNew);
            //mmitem.ToolTipText = "New document";

            //mmitem = m_menuItems.GetItem("Open");
            //mmitem.Text = "&Open";
            //mmitem.Text = "&��";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.OpenDocument);
            //mmitem.Click += new EventHandler(OnFileOpen);
            //mmitem.ToolTipText = "Open document";

            //mmitem = m_menuItems.GetItem("Save");
            //mmitem.Text = "&Save";
            //mmitem.Text = "&����";
            //mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.SaveDocument);
            //mmitem.Click += new EventHandler(OnFileSave);
            //mmitem.ToolTipText = "Save document";

            //mmitem = m_menuItems.GetItem("SaveAs");
            //mmitem.Text = "Save &As";
            //mmitem.Text = "���Ϊ";
            //mmitem.Click += new EventHandler(OnFileSaveAs);

            //mmitem = m_menuItems.GetItem("Exit");
            //mmitem.Text = "E&xit";
            //mmitem.Text = "�˳�";
            //mmitem.Click += new EventHandler(OnFileExit);
            //#endregion

            //#region "�ļ�"ͼƬ�й���������
            //ToolStrip strip = m_menuItems.GetStrip("file");
            //strip.Items.Add(m_menuItems.GetItem("New").CreateButton());
            //strip.Items.Add(m_menuItems.GetItem("Open").CreateButton());
            //strip.Items.Add(m_menuItems.GetItem("Save").CreateButton());
            //#endregion

            //#region "�ļ�"�����˵�����ӿؼ�
            //ToolStripMenuItem menuitem = m_menuItems.GetMenuStrip("file");
            //menuitem.Text = "&File";
            //menuitem.Text = "�ļ�";
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("New").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Open").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Save").CreateMenuItem());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("SaveAs").CreateMenuItem());
            //menuitem.DropDownItems.Add(new ToolStripSeparator());
            //menuitem.DropDownItems.Add(m_menuItems.GetItem("Exit").CreateMenuItem());
            //m_mainMenu.Items.Insert(0, menuitem);
            //#endregion

            #region �����²˵���/״̬�����ÿؼ�
            ToolStripPanel panel = m_menuItems.GetStripPanel(DockStyle.Top);

            panel.Join(m_menuItems.GetStrip("layer"));
            panel.Join(m_menuItems.GetStrip("draw"));
            panel.Join(m_menuItems.GetStrip("edit"));
            //panel.Join(m_menuItems.GetStrip("file"));
            panel.Join(m_mainMenu); //���˵��������

            panel = m_menuItems.GetStripPanel(DockStyle.Left);
            panel.Join(m_menuItems.GetStrip("modify")); //����޸���

            panel = m_menuItems.GetStripPanel(DockStyle.Bottom);
            panel.Join(m_menuItems.GetStatusStrip("status")); //�ײ�״̬��
            #endregion
        }

        /// <summary>
        /// ����ĵ�����,ˢ�³���ǰ���ƶ��ؼ�
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
        /// ��ȡ�����ݿ���Ŀ
        /// </summary>
        /// <param name="id"></param>
        void OnFileOpen(string id)
        {
            string connectionString = "";
            SqlConnection SqlCon = new SqlConnection(connectionString); //���ݿ�����
            SqlCon.Open();
            string sql = "";
            SqlCommand com = new SqlCommand(sql, SqlCon);
            SqlDataReader dr = com.ExecuteReader();
            byte[] xmlbytes = (byte[])dr.GetValue(1);
            string filePath = "c://" + id + ".cadxml";
            byte2File(xmlbytes,filePath);
            OpenDocument(filePath);
            SqlCon.Close();
        }
        /// <summary>
        /// ���𰸴��䵽���ݿ�
        /// </summary>
        void sendAnswer(string id) {
            byte[] answer = File2byte("c://" + id + ".cadxml ");
            string sql = "insert into pro_table (pro_name,pro_file) values('�����ļ�',@file)";
            UpToSql(answer,sql);
        }
        /// <summary>
        /// �����ͼ
        /// </summary>
        /// <param name="id"></param>
        void sendPic(string id)
        {
            byte[] pic = File2byte("c://" + id + ".jpg");
            string sql = "insert into pro_table (pro_name,pro_file) values('�����ļ�',@file)";
            UpToSql(pic, sql);
        }
        public void UpToSql(byte[] b,string sql)
        {
            string connectionString = "";
            SqlConnection SqlCon = new SqlConnection(connectionString); //���ݿ�����
            SqlCon.Open();
            SqlCommand com = new SqlCommand(sql, SqlCon);
            com.Parameters.Add("@file", SqlDbType.Binary,b.Length);
            com.Parameters["@file"].Value =b;
            com.ExecuteNonQuery();
            SqlCon.Close();
        }
        /// <summary>
        /// byteת�ļ�
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

                MessageBox.Show("�򿪻�ͼʧ�ܣ�������ҳ���´�");
                Close();
            }
        }
        /// <summary>
        /// �ļ�תbyte
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

                    //�ر���Դ
                    fs.Close();
                }
            }
        }
        
        /// <summary>
        /// �ļ��򿪰�ť������,��.cadxml�ļ�
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
        /// �ļ�����
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
        /// �½��ļ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnFileNew(object sender, EventArgs e)
  //      {
  //          OpenDocument(string.Empty);
  //      }

        /// <summary>
        /// �����ļ������ļ�
        /// </summary>
        /// <param name="filename"></param>
		void OpenDocument(string filename)
        {
            if (this.ActiveMdiChild != null)
            {
                MessageBox.Show("���ȹرյ�ǰ���ĵ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            f = new DocumentForm(filename);
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }
        void OpenDocument(string filename, String status)
        {
            if (this.ActiveMdiChild != null)
            {
                MessageBox.Show("���ȹرյ�ǰ���ĵ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            f = new DocumentForm(filename, status);
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }
        /// <summary>
        /// ���Ϊ��ť������
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
        /// �˳���ť������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnFileExit(object sender, EventArgs e)
  //      {
  //          Close();
  //      }

        /// <summary>
        /// �յģ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnUpdateMenuUI(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// "����"������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		//private void OnAbout(object sender, EventArgs e)
  //      {
  //          About dlg = new About();
  //          dlg.ShowDialog(this);
  //      }

        /// <summary>
        /// "ѡ��"��ť������,Options.OptionsDlgû������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnOptions(object sender, EventArgs e)
        {
            ///��ǰû���ļ��༭����
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
        /// ����
        /// </summary>
        /// <param name="id"></param>
        private  void GetScreenCapture(String id)
        {
            Rectangle tScreenRect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Bitmap tSrcBmp = new Bitmap(this.Width, this.Height); // ������ĻԭʼͼƬ����
            Graphics gp = Graphics.FromImage(tSrcBmp);
            gp.CopyFromScreen(this.Location, Point.Empty, this.Size);
            gp.DrawImage(tSrcBmp, 0, 0, new Rectangle(0,0, this.Width, this.Height), GraphicsUnit.Pixel);
            Bitmap tSrcBmp2 = new Bitmap(1600,900);
            tSrcBmp2 = tSrcBmp;
            tSrcBmp2.Save(@"c:\" + id + ".jpg");
        }

        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.ActiveMdiChild.Refresh();
            if (id != null) {
                sendAnswer(id);
                sendPic(id);
            }
            Thread.Sleep(100);
            if (id != null)
            {
                GetScreenCapture(id);
            }
        }
    }
}