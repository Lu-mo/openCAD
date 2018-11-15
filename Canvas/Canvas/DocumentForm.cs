using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Canvas
{
	public partial class DocumentForm : Form, ICanvasOwner, IEditToolOwner
	{
        CanvasCtrl m_canvas;
		DataModel m_data;

		MenuItemManager m_menuItems = new MenuItemManager();
		
		public string m_filename = string.Empty;   //文件名

        /// <summary>
        /// 文件初始化
        /// </summary>
        /// <param name="filename"></param>
        /// 
		public DocumentForm(string filename)
		{
			InitializeComponent();

			Text = "<New Document>";
			m_data = new DataModel();
			if (filename.Length > 0 && File.Exists(filename) && m_data.Load(filename))
			{
				Text = filename;
				m_filename = filename;
			}

			m_canvas = new CanvasCtrl(this, m_data);
			m_canvas.Dock = DockStyle.Fill;
			Controls.Add(m_canvas);
			m_canvas.SetCenter(new UnitPoint(0, 0));
			m_canvas.RunningSnaps = new Type[] 
				{
				typeof(VertextSnapPoint),
				typeof(MidpointSnapPoint),
				typeof(IntersectSnapPoint),
				typeof(QuadrantSnapPoint),
				typeof(CenterSnapPoint),
				typeof(DivisionSnapPoint),
				};

			m_canvas.AddQuickSnapType(Keys.N, typeof(NearestSnapPoint));
			m_canvas.AddQuickSnapType(Keys.M, typeof(MidpointSnapPoint));
			m_canvas.AddQuickSnapType(Keys.I, typeof(IntersectSnapPoint));
			m_canvas.AddQuickSnapType(Keys.V, typeof(VertextSnapPoint));
			m_canvas.AddQuickSnapType(Keys.P, typeof(PerpendicularSnapPoint));
			m_canvas.AddQuickSnapType(Keys.Q, typeof(QuadrantSnapPoint));
			m_canvas.AddQuickSnapType(Keys.C, typeof(CenterSnapPoint));
			m_canvas.AddQuickSnapType(Keys.T, typeof(TangentSnapPoint));
			m_canvas.AddQuickSnapType(Keys.D, typeof(DivisionSnapPoint));

			m_canvas.KeyDown += new KeyEventHandler(OnCanvasKeyDown);
			SetupMenuItems();
			SetupDrawTools();
			SetupLayerToolstrip();
			SetupEditTools();
			UpdateLayerUI();

			MenuStrip menuitem = new MenuStrip();
            menuitem.Items.Add(m_menuItems.GetMenuStrip("edit"));
            menuitem.Items.Add(m_menuItems.GetMenuStrip("draw"));
			menuitem.Visible = false;
			Controls.Add(menuitem);
			this.MainMenuStrip = menuitem;
		}
        public DocumentForm(string filename,String status)
        {
            InitializeComponent();
            Color color = Color.White;
            if (status.Equals("出题"))
            {
                color = Color.White;
            }
            if (status.Equals("标准答案"))
            {
                color = Color.Green;
            }
            if (status.Equals("答题"))
            {
                color = Color.Blue;
            }
            
            Text = "<New Document>";
            m_data = new DataModel(color);
            if (filename.Length > 0 && File.Exists(filename) && m_data.Load(filename))
            {
                Text = filename;
                m_filename = filename;
            }

            m_canvas = new CanvasCtrl(this, m_data);
            m_canvas.Dock = DockStyle.Fill;
            Controls.Add(m_canvas);
            m_canvas.SetCenter(new UnitPoint(0, 0));
            m_canvas.RunningSnaps = new Type[]
                {
                typeof(VertextSnapPoint),
                typeof(MidpointSnapPoint),
                typeof(IntersectSnapPoint),
                typeof(QuadrantSnapPoint),
                typeof(CenterSnapPoint),
                typeof(DivisionSnapPoint),
                };

            m_canvas.AddQuickSnapType(Keys.N, typeof(NearestSnapPoint));
            m_canvas.AddQuickSnapType(Keys.M, typeof(MidpointSnapPoint));
            m_canvas.AddQuickSnapType(Keys.I, typeof(IntersectSnapPoint));
            m_canvas.AddQuickSnapType(Keys.V, typeof(VertextSnapPoint));
            m_canvas.AddQuickSnapType(Keys.P, typeof(PerpendicularSnapPoint));
            m_canvas.AddQuickSnapType(Keys.Q, typeof(QuadrantSnapPoint));
            m_canvas.AddQuickSnapType(Keys.C, typeof(CenterSnapPoint));
            m_canvas.AddQuickSnapType(Keys.T, typeof(TangentSnapPoint));
            m_canvas.AddQuickSnapType(Keys.D, typeof(DivisionSnapPoint));

            m_canvas.KeyDown += new KeyEventHandler(OnCanvasKeyDown);
            SetupMenuItems();
            SetupDrawTools();
            SetupLayerToolstrip();
            SetupEditTools();
            UpdateLayerUI();

            MenuStrip menuitem = new MenuStrip();
            //menuitem.Items.Add(m_menuItems.GetMenuStrip("edit"));
            menuitem.Items.Add(m_menuItems.GetMenuStrip("draw"));
            menuitem.Visible = false;
            Controls.Add(menuitem);
            this.MainMenuStrip = menuitem;
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			m_canvas.SetCenter(m_data.CenterPoint);
		}

        /// <summary>
        /// 初始化菜单项
        /// </summary>
		void SetupMenuItems()
		{
			MenuItem mmitem = m_menuItems.GetItem("Undo");
			mmitem.Text = "Undo";
			mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.Undo);
			mmitem.ToolTipText = "Undo (Ctrl-Z)";
			mmitem.Click += new EventHandler(OnUndo);
			mmitem.ShortcutKeys = Shortcut.CtrlZ;

			mmitem = m_menuItems.GetItem("Redo");
			mmitem.Text = "Redo";
			mmitem.ToolTipText = "Undo (Ctrl-Y)";
			mmitem.Image = MenuImages16x16.Image(MenuImages16x16.eIndexes.Redo);
			mmitem.Click += new EventHandler(OnRedo);
			mmitem.ShortcutKeys = Shortcut.CtrlY;

			mmitem = m_menuItems.GetItem("Select");
			mmitem.Text = "Select";
			mmitem.ToolTipText = "Select (Esc)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Select);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.ShortcutKeyDisplayString = "Esc";
			mmitem.SingleKey = Keys.Escape;
			mmitem.Tag = "select";

			mmitem = m_menuItems.GetItem("Pan");
			mmitem.Text = "Pan";
			mmitem.ToolTipText = "Pan (P)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Pan);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.ShortcutKeyDisplayString = "P";
			mmitem.SingleKey = Keys.P;
			mmitem.Tag = "pan";

			mmitem = m_menuItems.GetItem("Move");
			mmitem.Text = "Move";
			mmitem.ToolTipText = "Move (M)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Move);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.ShortcutKeyDisplayString = "M";
			mmitem.SingleKey = Keys.M;
			mmitem.Tag = "move";

			ToolStrip strip = m_menuItems.GetStrip("edit");
			strip.Items.Add(m_menuItems.GetItem("Select").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Pan").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Move").CreateButton());
			strip.Items.Add(new ToolStripSeparator());
			strip.Items.Add(m_menuItems.GetItem("Undo").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Redo").CreateButton());

			ToolStripMenuItem menu = m_menuItems.GetMenuStrip("edit");
			menu.MergeAction = System.Windows.Forms.MergeAction.Insert;
			menu.MergeIndex = 1;
			menu.Text = "&Edit";
			menu.DropDownItems.Add(m_menuItems.GetItem("Undo").CreateMenuItem());
			menu.DropDownItems.Add(m_menuItems.GetItem("Redo").CreateMenuItem());
			menu.DropDownItems.Add(new ToolStripSeparator());
			menu.DropDownItems.Add(m_menuItems.GetItem("Select").CreateMenuItem());
			menu.DropDownItems.Add(m_menuItems.GetItem("Pan").CreateMenuItem());
			menu.DropDownItems.Add(m_menuItems.GetItem("Move").CreateMenuItem());
		}

        /// <summary>
        /// 初始化绘图工具
        /// </summary>
		void SetupDrawTools()
		{
			MenuItem mmitem = m_menuItems.GetItem("Lines");
			mmitem.Text = "Lines";
			mmitem.ToolTipText = "Lines (L)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Line);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.SingleKey = Keys.L;
			mmitem.ShortcutKeyDisplayString = "L";
			mmitem.Tag = "lines";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.LineEdit(false));

			mmitem = m_menuItems.GetItem("Line");
			mmitem.Text = "Line";
			mmitem.ToolTipText = "Single line (S)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Line);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.SingleKey = Keys.S;
			mmitem.ShortcutKeyDisplayString = "S";
			mmitem.Tag = "singleline";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.LineEdit(true));

            #region 添加的矩形工具

            mmitem = m_menuItems.GetItem("Rectangle");
            mmitem.Text = "Rectangle";
            mmitem.ToolTipText = "Rectangle (R)";
            mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Rectangle);
            mmitem.Click += new EventHandler(OnToolSelect);
            mmitem.SingleKey = Keys.R;
            mmitem.ShortcutKeyDisplayString = "R";
            mmitem.Tag = "Rectangle";
            m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.RectangleEdit());

            #endregion

            mmitem = m_menuItems.GetItem("Circle2P");
			mmitem.Text = "Circle 2P";
			mmitem.ToolTipText = "Circle 2 point";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Circle2P);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.Tag = "circle2P";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Circle(DrawTools.Arc.eArcType.type2point));

			mmitem = m_menuItems.GetItem("CircleCR");
			mmitem.Text = "Circle CR";
			mmitem.ToolTipText = "Circle Center-Radius";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.CircleCR);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.SingleKey = Keys.C;
			mmitem.ShortcutKeyDisplayString = "C";
			mmitem.Tag = "circleCR";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Circle(DrawTools.Arc.eArcType.typeCenterRadius));

			mmitem = m_menuItems.GetItem("Arc2P");
			mmitem.Text = "Arc 2P";
			mmitem.ToolTipText = "Arc 2 point";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Arc2P);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.Tag = "arc2P";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Arc(DrawTools.Arc.eArcType.type2point));

			mmitem = m_menuItems.GetItem("Arc3P132");
			mmitem.Text = "Arc 3P";
			mmitem.ToolTipText = "Arc 3 point (Start / End / Include)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Arc3P132);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.Tag = "arc3P132";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Arc3Point(DrawTools.Arc3Point.eArcType.kArc3P132));

			mmitem = m_menuItems.GetItem("Arc3P123");
			mmitem.Text = "Arc 3P";
			mmitem.ToolTipText = "Arc 3 point (Start / Include / End)";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Arc3P123);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.Tag = "arc3P123";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Arc3Point(DrawTools.Arc3Point.eArcType.kArc3P123));

            #region 添加的三點畫圓工具

            mmitem = m_menuItems.GetItem("Circle3Point");
            mmitem.Text = "Circle3Point";
            mmitem.ToolTipText = "Circle3Point";
            mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.Circle3Point);
            //mmitem.Image = ;
            mmitem.Click += new EventHandler(OnToolSelect);
            //mmitem.SingleKey = Keys.B;
            //mmitem.ShortcutKeyDisplayString = "B";
            mmitem.Tag = "Circle3Point";
            m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Circle3Point());

            #endregion

            mmitem = m_menuItems.GetItem("ArcCR");
			mmitem.Text = "ArcCR";
			mmitem.ToolTipText = "ArcCR";
			mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.ArcCR);
			mmitem.Click += new EventHandler(OnToolSelect);
			mmitem.SingleKey = Keys.A;
			mmitem.ShortcutKeyDisplayString = "A";
			mmitem.Tag = "ArcCR";
			m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.Arc(DrawTools.Arc.eArcType.typeCenterRadius));

            #region 添加的贝塞尔曲线工具

            mmitem = m_menuItems.GetItem("BezierCurve");
            mmitem.Text = "BezierCurve";
            mmitem.ToolTipText = "BezierCurve (B)";
            mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.BezierCurve);
            mmitem.Click += new EventHandler(OnToolSelect);
            mmitem.SingleKey = Keys.B;
            mmitem.ShortcutKeyDisplayString = "B";
            mmitem.Tag = "BezierCurve";
            m_data.AddDrawTool(mmitem.Tag.ToString(), new DrawTools.BezierCurveEdit());

            #endregion

            ToolStrip strip = m_menuItems.GetStrip("draw");
			strip.Items.Add(m_menuItems.GetItem("Lines").CreateButton());
            strip.Items.Add(m_menuItems.GetItem("Rectangle").CreateButton());//矩形工具
            strip.Items.Add(m_menuItems.GetItem("Circle2P").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("CircleCR").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Arc2P").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("ArcCR").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Arc3P132").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("Arc3P123").CreateButton());
            strip.Items.Add(m_menuItems.GetItem("Circle3Point").CreateButton());//Circle3Point工具
            strip.Items.Add(m_menuItems.GetItem("BezierCurve").CreateButton());//Bezier curve工具

            //ToolStripMenuItem menu = m_menuItems.GetMenuStrip("draw");
			//menu.MergeAction = System.Windows.Forms.MergeAction.Insert;
			//menu.MergeIndex = 2;
			//menu.Text = "Draw &Tools";
			//menu.DropDownItems.Add(m_menuItems.GetItem("Lines").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("Line").CreateMenuItem());
   //         menu.DropDownItems.Add(m_menuItems.GetItem("Rectangle").CreateMenuItem());//矩形工具
   //         menu.DropDownItems.Add(m_menuItems.GetItem("Circle2P").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("CircleCR").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("Arc2P").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("ArcCR").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("Arc3P132").CreateMenuItem());
			//menu.DropDownItems.Add(m_menuItems.GetItem("Arc3P123").CreateMenuItem());
   //         menu.DropDownItems.Add(m_menuItems.GetItem("Circle3Point").CreateMenuItem());//Circle3Point工具
   //         menu.DropDownItems.Add(m_menuItems.GetItem("BezierCurve").CreateMenuItem());//Bezier curve工具
        }

        /// <summary>
        /// 初始化编辑工具
        /// </summary>
		void SetupEditTools()
		{
			MenuItem item = m_menuItems.GetItem("Meet2Lines");
			item.Text = "Meet 2 Lines";
			item.ToolTipText = "Meet 2 Lines";
			item.Image = EditToolsImages16x16.Image(EditToolsImages16x16.eIndexes.Meet2Lines);
			item.Click += new EventHandler(OnEditToolSelect);
			item.Tag = "meet2lines";
			m_data.AddEditTool(item.Tag.ToString(), new EditTools.LinesMeetEditTool(this));

			item = m_menuItems.GetItem("ShrinkExtend");
			item.Text = "Shrink or Extend";
			item.ToolTipText = "Shrink or Extend";
			item.Image = EditToolsImages16x16.Image(EditToolsImages16x16.eIndexes.LineSrhinkExtend);
			item.Click += new EventHandler(OnEditToolSelect);
			item.Tag = "shrinkextend";
			m_data.AddEditTool(item.Tag.ToString(), new EditTools.LineShrinkExtendEditTool(this));

			ToolStrip strip = m_menuItems.GetStrip("modify");
			strip.Items.Add(m_menuItems.GetItem("Meet2Lines").CreateButton());
			strip.Items.Add(m_menuItems.GetItem("ShrinkExtend").CreateButton());
			m_toolHint = string.Empty;
		}

		ToolStripStatusLabel m_mousePosLabel = new ToolStripStatusLabel();
		ToolStripStatusLabel m_snapInfoLabel = new ToolStripStatusLabel();
		ToolStripStatusLabel m_drawInfoLabel = new ToolStripStatusLabel();
		ToolStripComboBox m_layerCombo = new ToolStripComboBox();
        /// <summary>
        /// 初始化工具栏布局
        /// </summary>
		void SetupLayerToolstrip()
		{
			StatusStrip status = m_menuItems.GetStatusStrip("status");
			m_mousePosLabel.AutoSize = true;
			m_mousePosLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
			m_mousePosLabel.Size = new System.Drawing.Size(110, 17);
			status.Items.Add(m_mousePosLabel);

			m_snapInfoLabel.AutoSize = true;
			m_snapInfoLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
			m_snapInfoLabel.Size = new System.Drawing.Size(200, 17);
			status.Items.Add(m_snapInfoLabel);

			//m_drawInfoLabel.AutoSize = true;
			m_drawInfoLabel.Spring = true;
			m_drawInfoLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
			m_drawInfoLabel.TextAlign = ContentAlignment.MiddleLeft;
			m_drawInfoLabel.Size = new System.Drawing.Size(200, 17);
			status.Items.Add(m_drawInfoLabel);

			ToolStrip strip = m_menuItems.GetStrip("layer");
			strip.Items.Add(new ToolStripLabel("Active Layer"));

			m_layerCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			int index = 1;
			foreach (DrawingLayer layer in m_data.Layers)
			{
				string name = string.Format("({0}) - {1}", index, layer.Name);

				MenuItem mmitem = m_menuItems.GetItem(name);
				mmitem.Text = name;
				mmitem.Image = DrawToolsImages16x16.Image(DrawToolsImages16x16.eIndexes.ArcCR);
				mmitem.Click += new EventHandler(OnLayerSelect);
				mmitem.SingleKey = Keys.D0 + index;
				mmitem.Tag = new CommonTools.NameObject<DrawingLayer>(mmitem.Text, layer);

				m_layerCombo.Items.Add(new CommonTools.NameObject<DrawingLayer>(mmitem.Text, layer));
				m_layerCombo.SelectedIndexChanged += mmitem.Click;
				
				index++;
			}
			strip.Items.Add(m_layerCombo);
		}

        /// <summary>
        /// 获取工具栏
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public ToolStrip GetToolStrip(string id)
		{
			return m_menuItems.GetStrip(id);
		}



		public void Save()
		{
			UpdateData();
			if (m_filename.Length == 0)
				SaveAs();
			else
				m_data.Save(m_filename);
		}

		public void SaveAs()
		{
			UpdateData();
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Cad XML files (*.cadxml)|*.cadxml";
			dlg.OverwritePrompt = true;
			if (m_filename.Length > 0)
				dlg.FileName = m_filename;
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				m_filename = dlg.FileName;
				m_data.Save(m_filename);
				Text = m_filename;
			}
		}

		public CanvasCtrl Canvas
		{
			get { return m_canvas ;}
		}
		public DataModel Model
		{
			get { return m_data; }
		}

		void UpdateData()
		{
            //更新不属于接口的数据的任何附加属性
            m_data.CenterPoint = m_canvas.GetCenter();
		}

        /// <summary>
        /// 工具选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnToolSelect(object sender, System.EventArgs e)
		{
			string toolid = string.Empty;
			bool fromKeyboard = false;
			if (sender is MenuItem) // from keyboard
			{
				toolid = ((MenuItem)sender).Tag.ToString();
				fromKeyboard = true;
			}
			if (sender is ToolStripItem) // from menu or toolbar
			{
				toolid = ((ToolStripItem)sender).Tag.ToString();
			}
			if (toolid == "select")
			{
				m_canvas.CommandEscape();
				return;
			}
			if (toolid == "pan")
			{
				m_canvas.CommandPan();
				return;
			}
			if (toolid == "move")
			{
				// if from keyboard then handle immediately, if from mouse click then only switch mode
				m_canvas.CommandMove(fromKeyboard);
				return;
			}
			m_canvas.CommandSelectDrawTool(toolid);
		}

        /// <summary>
        /// 编辑工具选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnEditToolSelect(object sender, System.EventArgs e)
		{
			string toolid = string.Empty;
			//bool fromKeyboard = false;
			if (sender is MenuItem) // from keyboard
			{
				toolid = ((MenuItem)sender).Tag.ToString();
				//fromKeyboard = true;
			}
			if (sender is ToolStripItem) // from menu or toolbar
			{
				toolid = ((ToolStripItem)sender).Tag.ToString();
			}
			m_canvas.CommandEdit(toolid);
		}

        /// <summary>
        /// 更新UI层
        /// </summary>
		void UpdateLayerUI()
		{
			CommonTools.NameObject<DrawingLayer> selitem = m_layerCombo.SelectedItem as CommonTools.NameObject<DrawingLayer>;
			if (selitem == null || selitem.Object != m_data.ActiveLayer)
			{ 
				foreach (CommonTools.NameObject<DrawingLayer> obj in m_layerCombo.Items)
				{
					if (obj.Object == m_data.ActiveLayer)
						m_layerCombo.SelectedItem = obj;
				}
			}
		}

        /// <summary>
        /// 层选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnLayerSelect(object sender, System.EventArgs e)
		{
			CommonTools.NameObject<DrawingLayer> obj = null;
			if (sender is ToolStripComboBox)
				obj = ((ToolStripComboBox)sender).SelectedItem as CommonTools.NameObject<DrawingLayer>;
			if (sender is MenuItem)
				obj = ((MenuItem)sender).Tag as CommonTools.NameObject<DrawingLayer>;
			if (obj == null)
				return;
			m_data.ActiveLayer = obj.Object as DrawingLayer;
			m_canvas.DoInvalidate(true);
			UpdateLayerUI();
		}

        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnUndo(object sender, System.EventArgs e)
		{
			if (m_data.DoUndo())
				m_canvas.DoInvalidate(true);
		}

        /// <summary>
        /// 重做
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnRedo(object sender, System.EventArgs e)
		{
			if (m_data.DoRedo())
				m_canvas.DoInvalidate(true);
		}

        /// <summary>
        /// 更新UI
        /// </summary>
		public void UpdateUI()
		{
			m_menuItems.GetItem("Undo").Enabled = m_data.CanUndo();
			m_menuItems.GetItem("Redo").Enabled = m_data.CanRedo();
			m_menuItems.GetItem("Move").Enabled = m_data.SelectedCount > 0;
		}

        /// <summary>
        /// 画板单机事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void OnCanvasKeyDown(object sender, KeyEventArgs e)
		{
			if (Control.ModifierKeys != Keys.None)
				return;

			MenuItem item = m_menuItems.FindFromSingleKey(e.KeyCode);
			if (item != null && item.Click != null)
			{
				item.Click(item, null);
				e.Handled = true;
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (m_data.IsDirty)
			{
				string s = "Save Changes to " + Path.GetFileName(m_filename) + "?";
				DialogResult result = MessageBox.Show(this, s, Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
				if (result == DialogResult.Yes)
					Save();
			}
			m_menuItems.DisableAll();
			base.OnFormClosing(e);
		}
		#region ICanvasOwner Members
		public void SetPositionInfo(UnitPoint unitpos)
		{
			m_mousePosLabel.Text = unitpos.PosAsString();
			string s = string.Empty;
			if (m_data.SelectedCount == 1 || m_canvas.NewObject != null)
			{
				IDrawObject obj = m_data.GetFirstSelected();
				if (obj == null)
					obj = m_canvas.NewObject;
				if (obj != null)
					s = obj.GetInfoAsString();
			}
			if (m_toolHint.Length > 0)
				s = m_toolHint;
			if (s != m_drawInfoLabel.Text)
				m_drawInfoLabel.Text = s;
		}
		public void SetSnapInfo(ISnapPoint snap)
		{
			m_snapHint = string.Empty;
			if (snap != null)
				m_snapHint = string.Format("Snap@{0}, {1}", snap.SnapPoint.PosAsString(), snap.GetType());
			m_snapInfoLabel.Text = m_snapHint;
		}
		#endregion
		#region IEditToolOwner
		public void SetHint(string text)
		{
			m_toolHint = text;
			m_drawInfoLabel.Text = m_toolHint;
			//SetHint();
		}
		#endregion
		string m_toolHint = string.Empty;
		string m_snapHint = string.Empty;
		/*
		void SetHint()
		{
			if (m_toolHint.Length > 0)
				m_snapInfoLabel.Text = m_toolHint;
			else
				m_snapInfoLabel.Text = m_snapHint;
		}
		*/
	}
}