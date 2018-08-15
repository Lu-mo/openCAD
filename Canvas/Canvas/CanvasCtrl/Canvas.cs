using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Canvas
{
	public struct CanvasWrapper : ICanvas
	{
		CanvasCtrl m_canvas; 
		Graphics m_graphics;
		Rectangle m_rect;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="canvas"></param>
		public CanvasWrapper(CanvasCtrl canvas)
		{
			m_canvas = canvas;
			m_graphics = null;
			m_rect = new Rectangle();
		}
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="graphics"></param>
        /// <param name="clientrect"></param>
		public CanvasWrapper(CanvasCtrl canvas, Graphics graphics, Rectangle clientrect)
        {
			m_canvas = canvas;
			m_graphics = graphics;
			m_rect = clientrect;
		}
        /// <summary>
        /// 获取数据
        /// </summary>
		public IModel Model
		{
			get { return m_canvas.Model; }
		}
		public CanvasCtrl CanvasCtrl
		{
			get { return m_canvas; }
		}
        /// <summary>
        /// 释放资源
        /// </summary>
		public void Dispose()
		{
			m_graphics = null;
		}
		#region ICanvas Members
		public IModel DataModel
		{
			get { return m_canvas.Model; }
		}
		public UnitPoint ScreenTopLeftToUnitPoint()
		{
			return m_canvas.ScreenTopLeftToUnitPoint();
		}
		public UnitPoint ScreenBottomRightToUnitPoint()
		{
			return m_canvas.ScreenBottomRightToUnitPoint();
		}
		public PointF ToScreen(UnitPoint unitpoint)
		{
			return m_canvas.ToScreen(unitpoint);
		}
		public float ToScreen(double unitvalue)
		{
			return m_canvas.ToScreen(unitvalue);
		}
		public double ToUnit(float screenvalue)
		{
			return m_canvas.ToUnit(screenvalue);
		}
		public UnitPoint ToUnit(PointF screenpoint)
		{
			return m_canvas.ToUnit(screenpoint);
		}
		public Graphics Graphics
		{
			get { return m_graphics; }
		}
        /// <summary>
        /// 工作区域
        /// </summary>
		public Rectangle ClientRectangle
		{
			get { return m_rect; }
			set { m_rect = value; }
		}/// <summary>
        /// 创建画笔实例
        /// </summary>
        /// <param name="color"></param>
        /// <param name="unitWidth"></param>
        /// <returns></returns>
		public Pen CreatePen(Color color, float unitWidth)
		{
			return m_canvas.CreatePen(color, unitWidth);
		}
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pen"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
		public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
		{
			m_canvas.DrawLine(canvas, pen, p1, p2);
		}
        /// <summary>
        /// 画弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pen"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="beginangle"></param>
        /// <param name="angle"></param>
		public void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float beginangle, float angle)
		{
			m_canvas.DrawArc(canvas, pen, center, radius, beginangle, angle);
		}
		public void Invalidate()
		{
			m_canvas.DoInvalidate(false);
		}
		public IDrawObject CurrentObject
		{
			get { return m_canvas.NewObject; }
		}
		#endregion
	}
	public partial class CanvasCtrl : UserControl
	{
		enum eCommandType
		{
			select,
			pan,
			move,
			draw,
			edit,
			editNode,
		}

		ICanvasOwner		m_owner;
		CursorCollection	m_cursors = new CursorCollection();
		IModel				m_model;//数据集合
        MoveHelper			m_moveHelper = null;
		NodeMoveHelper		m_nodeMoveHelper = null;
		CanvasWrapper		m_canvaswrapper;
		eCommandType		m_commandType = eCommandType.select;
		bool				m_runningSnaps = true;//是否在绘图模式？？
		Type[]				m_runningSnapTypes = null;//绘图类型？？
        PointF				m_mousedownPoint;//鼠标按下的坐标
		IDrawObject			m_newObject = null;//绘图工具？？
		IEditTool			m_editTool = null;//编辑工具
		SelectionRectangle	m_selection = null;//被选中的区域
		string				m_drawObjectId = string.Empty;//绘图工具ID
		string				m_editToolId = string.Empty;//编辑工具ID
		Bitmap				m_staticImage = null;//静态图片
		bool				m_staticDirty = true;//标记是否为脏
		ISnapPoint			m_snappoint = null;//数据集合

        public Type[] RunningSnaps
		{
			get { return m_runningSnapTypes; }
			set { m_runningSnapTypes = value; }
		}
		public bool RunningSnapsEnabled
		{
			get { return m_runningSnaps; }
			set { m_runningSnaps = value; }
		}

		System.Drawing.Drawing2D.SmoothingMode	m_smoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;//指定未抗锯齿
		public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
		{
			get { return m_smoothingMode; }
			set { m_smoothingMode = value;}
		}

		public IModel Model
		{
			get { return m_model; }
			set { m_model = value; }
		}
		public CanvasCtrl(ICanvasOwner owner, IModel datamodel)
		{
			m_canvaswrapper = new CanvasWrapper(this);
			m_owner = owner;
			m_model = datamodel;

			InitializeComponent();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);//忽略窗口信息减少闪烁，由控件来绘制自身
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);//控件将首先绘制到缓冲区而不是绘制到屏幕。

			m_commandType = eCommandType.select;
			m_cursors.AddCursor(eCommandType.select, Cursors.Arrow);//设置选中光标类型
            m_cursors.AddCursor(eCommandType.draw, Cursors.Cross);//设置画图光标类型
            m_cursors.AddCursor(eCommandType.pan, "hmove.cur");//设置移动画面光标类型
            m_cursors.AddCursor(eCommandType.move, Cursors.SizeAll);//设置移动光标类型
            m_cursors.AddCursor(eCommandType.edit, Cursors.Cross);//设置编辑光标类型
            UpdateCursor();//更新光标

			m_moveHelper = new MoveHelper(this);
			m_nodeMoveHelper = new NodeMoveHelper(m_canvaswrapper);
		}
        /// <summary>
        /// 将光标位置换算成世界坐标并返回
        /// </summary>
        /// <returns></returns>
		public UnitPoint GetMousePoint()
		{
			Point point = this.PointToClient(Control.MousePosition);
			return ToUnit(point);
		}
        /// <summary>
        /// 图像居中
        /// </summary>
        /// <param name="unitPoint"></param>
		public void SetCenter(UnitPoint unitPoint)
		{
			PointF point = ToScreen(unitPoint);//坐标换算
			m_lastCenterPoint = unitPoint;
			SetCenterScreen(point, false);//调用居中函数
		}
        /// <summary>
        /// 图像居中
        /// </summary>
		public void SetCenter()
        {
			Point point = this.PointToClient(Control.MousePosition);//将当前光标位置换算成屏幕坐标
			SetCenterScreen(point, true);
		}/// <summary>
        /// 获取中心点
        /// </summary>
        /// <returns></returns>
		public UnitPoint GetCenter()
		{
			return ToUnit(new PointF(this.ClientRectangle.Width/2, this.ClientRectangle.Height/2));
		}
		protected  void SetCenterScreen(PointF screenPoint, bool setCursor)//设置中心点
        {
			float centerX = ClientRectangle.Width / 2;
			m_panOffset.X += centerX - screenPoint.X;
			
			float centerY = ClientRectangle.Height / 2;
			m_panOffset.Y += centerY - screenPoint.Y;

			if (setCursor)
				Cursor.Position = this.PointToScreen(new Point((int)centerX, (int)centerY));//坐标换算，设置坐标位置。
			DoInvalidate(true);
		}
        /// <summary>
        /// 窗体重画
        /// </summary>
        /// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			CommonTools.Tracing.StartTrack(Program.TracePaint);//TracePaint=1
            ClearPens();//清空画笔
			e.Graphics.SmoothingMode = m_smoothingMode;
			CanvasWrapper dc = new CanvasWrapper(this, e.Graphics, ClientRectangle);
			Rectangle cliprectangle = e.ClipRectangle;
			if (m_staticImage == null)
			{
				cliprectangle = ClientRectangle;
                m_staticImage = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                m_staticDirty = true;
			}
			RectangleF r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
			if (float.IsNaN(r.Width) || float.IsInfinity(r.Width))
			{
				r = ScreenUtils.ToUnitNormalized(dc, cliprectangle);
			}
			if (m_staticDirty)
			{ 
				m_staticDirty = false;
				CanvasWrapper dcStatic = new CanvasWrapper(this, Graphics.FromImage(m_staticImage), ClientRectangle);
				dcStatic.Graphics.SmoothingMode = m_smoothingMode;
				m_model.BackgroundLayer.Draw(dcStatic, r);
				if (m_model.GridLayer.Enabled)
					m_model.GridLayer.Draw(dcStatic, r);
                if (m_model.Zoom > 3.05)
                {
                    m_model.GridLayer.Draw5x5(dcStatic, r);
                }
                PointF nullPoint = ToScreen(new UnitPoint(0, 0));
				dcStatic.Graphics.DrawLine(Pens.Pink, nullPoint.X - 10, nullPoint.Y, nullPoint.X + 10, nullPoint.Y);//坐标系原点
				dcStatic.Graphics.DrawLine(Pens.Pink, nullPoint.X, nullPoint.Y - 10, nullPoint.X, nullPoint.Y + 10);
				ICanvasLayer[] layers = m_model.Layers;//线条粗细颜色
				for (int layerindex = layers.Length - 1; layerindex >= 0; layerindex--)
				{
                    if (layers[layerindex] != m_model.ActiveLayer && layers[layerindex].Visible)
                    {
                        layers[layerindex].Draw(dcStatic, r);
                    }
				}
                if (m_model.ActiveLayer != null)
                {
                    m_model.ActiveLayer.Draw(dcStatic, r);
                }

				dcStatic.Dispose();
			}
			e.Graphics.DrawImage(m_staticImage, cliprectangle, cliprectangle, GraphicsUnit.Pixel);
			
			foreach (IDrawObject drawobject in m_model.SelectedObjects)
				drawobject.Draw(dc, r);

			if (m_newObject != null)
				m_newObject.Draw(dc, r);
			
			if (m_snappoint != null)
				m_snappoint.Draw(dc);
			
			if (m_selection != null)
			{
				m_selection.Reset();
				m_selection.SetMousePoint(e.Graphics, this.PointToClient(Control.MousePosition));
			}
			if (m_moveHelper.IsEmpty == false)
				m_moveHelper.DrawObjects(dc, r);

			if (m_nodeMoveHelper.IsEmpty == false)
				m_nodeMoveHelper.DrawObjects(dc, r);
			dc.Dispose();
			ClearPens();
			CommonTools.Tracing.EndTrack(Program.TracePaint, "OnPaint complete");
		}
        /// <summary>
        /// 在画布按照Rectangle重画
        /// </summary>
        /// <param name="r"></param>
		void RepaintStatic(Rectangle r)
		{
			if (m_staticImage == null)
				return;
			Graphics dc = Graphics.FromHwnd(Handle);
			if (r.X < 0) r.X = 0;
			if (r.X > m_staticImage.Width) r.X = 0;
			if (r.Y < 0) r.Y = 0;
			if (r.Y > m_staticImage.Height) r.Y = 0;
			
			if (r.Width > m_staticImage.Width || r.Width < 0)
				r.Width = m_staticImage.Width;
			if (r.Height > m_staticImage.Height || r.Height < 0)
				r.Height = m_staticImage.Height;
			dc.DrawImage(m_staticImage, r, r, GraphicsUnit.Pixel);
			dc.Dispose();
		}/// <summary>
        /// 在画布按照snappoint重画
        /// </summary>
        /// <param name="snappoint"></param>
		void RepaintSnappoint(ISnapPoint snappoint)
        {
			if (snappoint == null)
				return;
			CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
			snappoint.Draw(dc);
			dc.Graphics.Dispose();
			dc.Dispose();
		}/// <summary>
         /// 在画布按照IDrawObject重画
         /// </summary>
         /// <param name="obj"></param>
        void RepaintObject(IDrawObject obj)
        {
			if (obj == null)
				return;
			CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
			RectangleF invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(dc, obj.GetBoundingRect(dc)));
			obj.Draw(dc, invalidaterect);
			dc.Graphics.Dispose();
			dc.Dispose();
		}
        /// <summary>
        /// 使选中方块无效并重绘（移动选中的方块）
        /// </summary>
        /// <param name="dostatic"></param>
        /// <param name="rect"></param>
		public void DoInvalidate(bool dostatic, RectangleF rect)
		{
			if (dostatic)
				m_staticDirty = true;
			Invalidate(ScreenUtils.ConvertRect(rect));
		}
        /// <summary>
        /// 使画面无效并重绘
        /// </summary>
        /// <param name="dostatic"></param>
		public void DoInvalidate(bool dostatic)
		{
			if (dostatic)
				m_staticDirty = true;
			Invalidate();
		}/// <summary>
         /// 获取 m_newObject
         /// </summary>
        public IDrawObject NewObject
		{
			get { return m_newObject; }
		}
        /// <summary>
        /// 区域选中（shift为全选，ctrl为正/反选）
        /// </summary>
        /// <param name="selected"></param>
		protected void HandleSelection(List<IDrawObject> selected)
		{
			bool add = Control.ModifierKeys == Keys.Shift;//设置shift为修改键
			bool toggle = Control.ModifierKeys == Keys.Control;//设置ctrl为修改键
            bool invalidate = false;
			bool anyoldsel = false;
			int selcount = 0;
			if (selected != null)
				selcount = selected.Count;
			foreach(IDrawObject obj in m_model.SelectedObjects)
			{
				anyoldsel = true;
				break;
			}
			if (toggle && selcount > 0)
			{
				invalidate = true;
				foreach (IDrawObject obj in selected)
				{
					if (m_model.IsSelected(obj))
						m_model.RemoveSelectedObject(obj);
					else
						m_model.AddSelectedObject(obj);
				}
			}
			if (add && selcount > 0)
			{
				invalidate = true;
				foreach (IDrawObject obj in selected)
					m_model.AddSelectedObject(obj);
			}
			if (add == false && toggle == false && selcount > 0)
			{
				invalidate = true;
				m_model.ClearSelectedObjects();
				foreach (IDrawObject obj in selected)
					m_model.AddSelectedObject(obj);
			}
			if (add == false && toggle == false && selcount == 0 && anyoldsel)
			{
				invalidate = true;
				m_model.ClearSelectedObjects();
			}

			if (invalidate)
				DoInvalidate(false);
		}
        /// <summary>
        /// 完成节点编辑
        /// </summary>
		void FinishNodeEdit()
		{
			m_commandType = eCommandType.select;
			m_snappoint = null;
		}
        /// <summary>
        /// 画图时，鼠标按下事件。
        /// </summary>
        /// <param name="mouseunitpoint"></param>
        /// <param name="snappoint"></param>
		protected virtual void HandleMouseDownWhenDrawing(UnitPoint mouseunitpoint, ISnapPoint snappoint)
		{
			if (m_commandType == eCommandType.draw)
			{
				if (m_newObject == null)
				{
					m_newObject = m_model.CreateObject(m_drawObjectId, mouseunitpoint, snappoint);
					DoInvalidate(false, m_newObject.GetBoundingRect(m_canvaswrapper));
				}
				else
				{
					if (m_newObject != null)
					{
						eDrawObjectMouseDown result = m_newObject.OnMouseDown(m_canvaswrapper, mouseunitpoint, snappoint);
						switch (result)
						{
							case eDrawObjectMouseDown.Done:
								m_model.AddObject(m_model.ActiveLayer, m_newObject);
								m_newObject = null;
								DoInvalidate(true);
								break;
							case eDrawObjectMouseDown.DoneRepeat:
								m_model.AddObject(m_model.ActiveLayer, m_newObject);
								m_newObject = m_model.CreateObject(m_newObject.Id, m_newObject.RepeatStartingPoint, null);
								DoInvalidate(true);
								break;
							case eDrawObjectMouseDown.Continue:
								break;
						}
					}
				}
			}
		}
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_mousedownPoint = new PointF(e.X, e.Y); // used when panning
			m_dragOffset = new PointF(0,0);

			UnitPoint mousepoint = ToUnit(m_mousedownPoint);//获取当前光标世界坐标
			if (m_snappoint != null)
				mousepoint = m_snappoint.SnapPoint;
			
			if (m_commandType == eCommandType.editNode)//如果为编辑节点
			{
				bool handled = false;
				if (m_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
				{
					FinishNodeEdit();
					base.OnMouseDown(e);
					return;
				}
			}
			if (m_commandType == eCommandType.select)//如果为选中
			{
				bool handled = false;
				if (m_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
				{
					m_commandType = eCommandType.editNode;
					m_snappoint = null;
					base.OnMouseDown(e);
					return;
				}
				m_selection = new SelectionRectangle(m_mousedownPoint);
			}
			if (m_commandType == eCommandType.move)//如果为移动
            {
				m_moveHelper.HandleMouseDownForMove(mousepoint, m_snappoint);
			}
			if (m_commandType == eCommandType.draw)//如果为绘制
            {
				HandleMouseDownWhenDrawing(mousepoint, null);
				DoInvalidate(true);
			}
			if (m_commandType == eCommandType.edit)//如果为编辑
            {
				if (m_editTool == null)//如果编辑工具为空
					m_editTool = m_model.GetEditTool(m_editToolId);
				if (m_editTool != null)//如果编辑工具不为空
                {
					if (m_editTool.SupportSelection)
						m_selection = new SelectionRectangle(m_mousedownPoint);

					eDrawObjectMouseDown mouseresult = m_editTool.OnMouseDown(m_canvaswrapper, mousepoint, m_snappoint);
					/*
					if (mouseresult == eDrawObjectMouseDown.Continue)
					{
						if (m_editTool.SupportSelection)
							m_selection = new SelectionRectangle(m_mousedownPoint);
					}
					 * */
					if (mouseresult == eDrawObjectMouseDown.Done)//如果绘制已经完成
					{
						m_editTool.Finished();
						m_editTool = m_model.GetEditTool(m_editToolId); // continue with new tool
						//m_editTool = null;
						
						if (m_editTool.SupportSelection)
							m_selection = new SelectionRectangle(m_mousedownPoint);
					}
				}
				DoInvalidate(true);
				UpdateCursor();//更新光标
			}
			base.OnMouseDown(e);
		}
        /// <summary>
        /// 鼠标抬起时
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (m_commandType == eCommandType.pan)//如果是移动画布
			{
				m_panOffset.X += m_dragOffset.X;
				m_panOffset.Y += m_dragOffset.Y;
				m_dragOffset = new PointF(0, 0);
			}

			List<IDrawObject> hitlist = null;
			Rectangle screenSelRect = Rectangle.Empty;
			if (m_selection != null)//如果被选中的区域不为空
			{
				screenSelRect = m_selection.ScreenRect();
				RectangleF selectionRect = m_selection.Selection(m_canvaswrapper);
				if (selectionRect != RectangleF.Empty)
				{
                    // is any selection rectangle. use it for selection选中是任意选择矩形。使用它进行选择
                    hitlist = m_model.GetHitObjects(m_canvaswrapper, selectionRect, m_selection.AnyPoint());
					DoInvalidate(true);
				}
				else
				{
                    // else use mouse point否则使用光标指向的点
                    UnitPoint mousepoint = ToUnit(new PointF(e.X, e.Y));
					hitlist = m_model.GetHitObjects(m_canvaswrapper, mousepoint);
				}
				m_selection = null;
			}
			if (m_commandType == eCommandType.select)//如果是选中
			{
				if (hitlist != null)
					HandleSelection(hitlist);
			}
			if (m_commandType == eCommandType.edit && m_editTool != null)//如果是编辑
			{
				UnitPoint mousepoint = ToUnit(m_mousedownPoint);
				if (m_snappoint != null)
					mousepoint = m_snappoint.SnapPoint;
				if (screenSelRect != Rectangle.Empty)
					m_editTool.SetHitObjects(mousepoint, hitlist);
				m_editTool.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
			}
			if (m_commandType == eCommandType.draw && m_newObject != null)//如果是绘制且绘图工具不为
			{
				UnitPoint mousepoint = ToUnit(m_mousedownPoint);
				if (m_snappoint != null)
					mousepoint = m_snappoint.SnapPoint;
				m_newObject.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
			}
		}
        protected void overside()//使移动不能超过100*100
        {
            if (m_canvaswrapper.ScreenTopLeftToUnitPoint().X <= (-50))
            {
                UnitPoint stl = m_canvaswrapper.ScreenTopLeftToUnitPoint();
                double dis = (stl.X + 50) / 50;
                m_lastCenterPoint.X = m_lastCenterPoint.X - dis;
                SetCenter(m_lastCenterPoint);
            }
            if (m_canvaswrapper.ScreenTopLeftToUnitPoint().Y >= 50)
            {
                UnitPoint stl = m_canvaswrapper.ScreenTopLeftToUnitPoint();
                double dis = (stl.Y - 50) / 50;
                m_lastCenterPoint.Y = m_lastCenterPoint.Y - dis;
                SetCenter(m_lastCenterPoint);
            }
            if (m_canvaswrapper.ScreenBottomRightToUnitPoint().X >= 50)
            {
                UnitPoint sbr = m_canvaswrapper.ScreenBottomRightToUnitPoint();
                double dis = (sbr.X - 50) / 50;
                m_lastCenterPoint.X = m_lastCenterPoint.X - dis;
                SetCenter(m_lastCenterPoint);
            }
            if (m_canvaswrapper.ScreenBottomRightToUnitPoint().Y <= (-50))
            {
                UnitPoint sbr = m_canvaswrapper.ScreenBottomRightToUnitPoint();
                double dis = (sbr.Y + 50) / 50;
                m_lastCenterPoint.Y = m_lastCenterPoint.Y - dis;
                SetCenter(m_lastCenterPoint);
            }
        }
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
            if (m_selection != null)
			{
				Graphics dc = Graphics.FromHwnd(Handle);
				m_selection.SetMousePoint(dc, new PointF(e.X, e.Y));
				dc.Dispose();
				return;
			}

            if (m_commandType == eCommandType.pan && e.Button == MouseButtons.Left)//如果选择pan（移动画布），切按住鼠标左键
            {
                m_dragOffset.X = -(m_mousedownPoint.X - e.X);
                m_dragOffset.Y = -(m_mousedownPoint.Y - e.Y);
                m_lastCenterPoint = CenterPointUnit();
                overside();
                m_lastCenterPoint = CenterPointUnit();
                DoInvalidate(true);
            }
			UnitPoint mousepoint;
			UnitPoint unitpoint = ToUnit(new PointF(e.X, e.Y));
			if (m_commandType == eCommandType.draw || m_commandType == eCommandType.move || m_nodeMoveHelper.IsEmpty == false)//如果使绘制或者移动或节点移动或节点移动为空
			{
                overside();
                Rectangle invalidaterect = Rectangle.Empty;
				ISnapPoint newsnap = null;
				mousepoint = GetMousePoint();
				if (RunningSnapsEnabled)
					newsnap = m_model.SnapPoint(m_canvaswrapper, mousepoint, m_runningSnapTypes, null);
				if (newsnap == null)
					newsnap = m_model.GridLayer.SnapPoint(m_canvaswrapper, mousepoint, null);
                if(newsnap == null&&m_model.Zoom >3.05)
                {
                    newsnap = m_model.GridLayer.SnapPoint5x5(m_canvaswrapper, mousepoint, null);
                }
				if ((m_snappoint != null) && ((newsnap == null) || (newsnap.SnapPoint != m_snappoint.SnapPoint) || m_snappoint.GetType() != newsnap.GetType()))//绘图时点击左键生成节点
				{
                    
					invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_snappoint.BoundingRect));
					invalidaterect.Inflate(2, 2);
					RepaintStatic(invalidaterect); // remove old snappoint
					m_snappoint = newsnap;
				}
                if (m_commandType == eCommandType.move)//使用移动工具时，重画图像
                {
                    Invalidate(invalidaterect); 
                }

				if (m_snappoint == null)
					m_snappoint = newsnap;
			}
			m_owner.SetPositionInfo(unitpoint);//给出绘制信息
			m_owner.SetSnapInfo(m_snappoint);//给出绘制信息


            //UnitPoint mousepoint;
            if (m_snappoint != null)
				mousepoint = m_snappoint.SnapPoint;
			else
				mousepoint = GetMousePoint();

			if (m_newObject != null)//如果不为空则重画
			{
				Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_newObject.GetBoundingRect(m_canvaswrapper)));
				invalidaterect.Inflate(2, 2);
				RepaintStatic(invalidaterect);
				m_newObject.OnMouseMove(m_canvaswrapper, mousepoint);
				RepaintObject(m_newObject);
			}
			if (m_snappoint != null)//如果不为空则重画
                RepaintSnappoint(m_snappoint);

			if (m_moveHelper.HandleMouseMoveForMove(mousepoint))//重绘
				Refresh(); //Invalidate();

			RectangleF rNoderect = m_nodeMoveHelper.HandleMouseMoveForNode(mousepoint);//坐标获取
			if (rNoderect != RectangleF.Empty)
			{
				Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, rNoderect));//坐标转换
				RepaintStatic(invalidaterect);//重绘
                CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
				dc.Graphics.Clip = new Region(ClientRectangle);
				//m_nodeMoveHelper.DrawOriginalObjects(dc, rNoderect);
				m_nodeMoveHelper.DrawObjects(dc, rNoderect);
				if (m_snappoint != null)
					RepaintSnappoint(m_snappoint);//重绘
                dc.Graphics.Dispose();
				dc.Dispose();
			}
		}
        /// <summary>
        /// 鼠标滚轮事件（放大缩小）
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			UnitPoint p = GetMousePoint();
			float wheeldeltatick = 120;
            int mouseDelta = Math.Abs(e.Delta);
            float zoomdelta = (1.25f * (mouseDelta / wheeldeltatick));
            if (e.Delta < 0)
            {
                if (m_model.Zoom < 0.4096) return;
                m_model.Zoom = m_model.Zoom / zoomdelta;
            }
            else
            {
                if (m_model.Zoom > 3.05) return;
                m_model.Zoom = m_model.Zoom * zoomdelta;
            }
			SetCenterScreen(ToScreen(p), true); 
            
            DoInvalidate(true);
			base.OnMouseWheel(e);
		}
        /// <summary>
        /// 调整窗口大小
        /// </summary>
        /// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);         
            if (m_lastCenterPoint != UnitPoint.Empty && Width != 0)
				SetCenterScreen(ToScreen(m_lastCenterPoint), false);
			m_lastCenterPoint = CenterPointUnit();
			m_staticImage = null;
			DoInvalidate(true);
		}

		UnitPoint m_lastCenterPoint;
		PointF m_panOffset = new PointF(25, -25);
		PointF m_dragOffset = new PointF(0, 0);
		float m_screenResolution = 96;
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
		PointF Translate(UnitPoint point)
		{
			return point.Point;
		}/// <summary>
        /// 屏幕高度（换算）
        /// </summary>
        /// <returns></returns>
		float ScreenHeight()
		{
			return (float)(ToUnit(this.ClientRectangle.Height) / m_model.Zoom);
		}

		#region ICanvas
        /// <summary>
        /// 中心坐标
        /// </summary>
        /// <returns></returns>
		public UnitPoint CenterPointUnit()
		{
			UnitPoint p1 = ScreenTopLeftToUnitPoint();
			UnitPoint p2 = ScreenBottomRightToUnitPoint();
			UnitPoint center = new UnitPoint();
			center.X = (p1.X + p2.X) / 2;
			center.Y = (p1.Y + p2.Y) / 2;
			return center;
		}
        /// <summary>
        /// 屏幕左上角坐标（屏幕坐标转换世界坐标）
        /// </summary>
        /// <returns></returns>
		public UnitPoint ScreenTopLeftToUnitPoint()
		{
			return ToUnit(new PointF(0, 0));
		}
        /// <summary>
        /// 屏幕右下角坐标（屏幕坐标转换世界坐标）
        /// </summary>
        /// <returns></returns>
		public UnitPoint ScreenBottomRightToUnitPoint()
		{
			return ToUnit(new PointF(this.ClientRectangle.Width, this.ClientRectangle.Height));
		}
        /// <summary>
        /// 世界坐标转换屏幕
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
		public PointF ToScreen(UnitPoint point)
		{
			PointF transformedPoint = Translate(point);
			transformedPoint.Y = ScreenHeight() - transformedPoint.Y;
			transformedPoint.Y *= m_screenResolution * m_model.Zoom;
			transformedPoint.X *= m_screenResolution * m_model.Zoom;

			transformedPoint.X += m_panOffset.X + m_dragOffset.X;
			transformedPoint.Y += m_panOffset.Y + m_dragOffset.Y;
			return transformedPoint;
		}
        /// <summary>
        /// 单位换算？？（世界坐标到系屏幕）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public float ToScreen(double value)
		{
			return (float)(value * m_screenResolution * m_model.Zoom);
		}
        /// <summary>
        /// 换算？？？（屏幕到世界坐标系）
        /// </summary>
        /// <param name="screenvalue"></param>
        /// <returns></returns>
		public double ToUnit(float screenvalue)
		{
			return (double)screenvalue / (double)(m_screenResolution * m_model.Zoom);
		}
        /// <summary>
        /// 坐标换算（屏幕到世界坐标）
        /// </summary>
        /// <param name="screenpoint"></param>
        /// <returns></returns>
		public UnitPoint ToUnit(PointF screenpoint)
		{
			float panoffsetX = m_panOffset.X + m_dragOffset.X;
			float panoffsetY = m_panOffset.Y + m_dragOffset.Y;
			float xpos = (screenpoint.X - panoffsetX) / (m_screenResolution * m_model.Zoom);
			float ypos = ScreenHeight() - ((screenpoint.Y - panoffsetY)) / (m_screenResolution * m_model.Zoom);
			return new UnitPoint(xpos, ypos);
		}
        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="color"></param>
        /// <param name="unitWidth"></param>
        /// <returns></returns>
		public Pen CreatePen(Color color, float unitWidth)
		{
			return GetPen(color, ToScreen(unitWidth));
		}
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pen"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
		public void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2)
		{
			PointF tmpp1 = ToScreen(p1);
			PointF tmpp2 = ToScreen(p2);
			canvas.Graphics.DrawLine(pen, tmpp1, tmpp2);
		}
        /// <summary>
        /// 画圆形，半圆，弧形
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pen"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="sweepAngle"></param>
		public void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float startAngle, float sweepAngle)
		{
            PointF p1 = ToScreen(center);
			radius = (float)Math.Round(ToScreen(radius));
			RectangleF r = new RectangleF(p1, new SizeF());
			r.Inflate(radius, radius);
			if (radius > 0 && radius < 1e8f )
				canvas.Graphics.DrawArc(pen, r, -startAngle, -sweepAngle);
		}

		#endregion

		Dictionary<float, Dictionary<Color, Pen>> m_penCache = new Dictionary<float,Dictionary<Color,Pen>>();
        /// <summary>
        /// 设置画笔粗细颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        Pen GetPen(Color color, float width)
		{
			if (m_penCache.ContainsKey(width) == false)
				m_penCache[width] = new Dictionary<Color,Pen>();
			if (m_penCache[width].ContainsKey(color) == false)
				m_penCache[width][color] = new Pen(color, width);
			return m_penCache[width][color];
		}
        /// <summary>
        /// 初始化画笔
        /// </summary>
		void ClearPens()
		{
			m_penCache.Clear();
		}
		/// <summary>
        /// 更新光标图像
        /// </summary>
		void UpdateCursor()
		{
			Cursor = m_cursors.GetCursor(m_commandType);
		}

		Dictionary<Keys, Type> m_QuickSnap = new Dictionary<Keys,Type>();//快照？？？？
        /// <summary>
        /// 添加快照类型？？？？
        /// </summary>
        /// <param name="key"></param>
        /// <param name="snaptype"></param>
		public void AddQuickSnapType(Keys key, Type snaptype)
		{
			m_QuickSnap.Add(key, snaptype);
		}
        /// <summary>
        /// 选择绘图工具
        /// </summary>
        /// <param name="drawobjectid"></param>
		public void CommandSelectDrawTool(string drawobjectid)
		{
			CommandEscape();
			m_model.ClearSelectedObjects();
			m_commandType = eCommandType.draw;
			m_drawObjectId = drawobjectid;
			UpdateCursor();
		}
        /// <summary>
        /// 取消当前工具选择？？？
        /// </summary>
		public void CommandEscape()
		{
            bool dirty = (m_newObject != null) || (m_snappoint != null);
			m_newObject = null;
			m_snappoint = null;
			if (m_editTool != null)
				m_editTool.Finished();
			m_editTool	= null;
			m_commandType = eCommandType.select;
			m_moveHelper.HandleCancelMove();
			m_nodeMoveHelper.HandleCancelMove();
			DoInvalidate(dirty);
			UpdateCursor();
		}
        /// <summary>
        /// 移动画布
        /// </summary>
		public void CommandPan()
		{
            
            if (m_commandType == eCommandType.select || m_commandType == eCommandType.move)
				m_commandType = eCommandType.pan;
			UpdateCursor();
		}
        /// <summary>
        /// 移动选中线条或图像
        /// </summary>
        /// <param name="handleImmediately"></param>
		public void CommandMove(bool handleImmediately)
		{
			if (m_model.SelectedCount > 0)
			{
				if (handleImmediately && m_commandType == eCommandType.move)
					m_moveHelper.HandleMouseDownForMove(GetMousePoint(), m_snappoint);
				m_commandType = eCommandType.move;
				UpdateCursor();
			}
		}
        /// <summary>
        /// 删除选中线条或图像
        /// </summary>
		public void CommandDeleteSelected()
		{           
            m_model.DeleteObjects(m_model.SelectedObjects);
			m_model.ClearSelectedObjects();
			DoInvalidate(true);
			UpdateCursor();
		}
        /// <summary>
        /// 选择左侧工具？？
        /// </summary>
        /// <param name="editid"></param>
		public void CommandEdit(string editid)
		{
            CommandEscape();
			m_model.ClearSelectedObjects();
			m_commandType = eCommandType.edit;
			m_editToolId = editid;
			m_editTool = m_model.GetEditTool(m_editToolId);
			UpdateCursor();
		}
        /// <summary>
        /// 响应键盘快捷键处理
        /// </summary>
        /// <param name="e"></param>
		void HandleQuickSnap(KeyEventArgs e)
		{
            
            if (m_commandType == eCommandType.select || m_commandType == eCommandType.pan)
				return;
			ISnapPoint p = null;
			UnitPoint mousepoint = GetMousePoint();
			if (m_QuickSnap.ContainsKey(e.KeyCode))
				p = m_model.SnapPoint(m_canvaswrapper, mousepoint, null, m_QuickSnap[e.KeyCode]);
			if (p != null)
			{
				if (m_commandType == eCommandType.draw)
				{
					HandleMouseDownWhenDrawing(p.SnapPoint, p);
					if (m_newObject != null)
						m_newObject.OnMouseMove(m_canvaswrapper, GetMousePoint());
					DoInvalidate(true);
					e.Handled = true;
				}
				if (m_commandType == eCommandType.move)
				{                 
                    m_moveHelper.HandleMouseDownForMove(p.SnapPoint, p);
					e.Handled = true;
				}
				if (m_nodeMoveHelper.IsEmpty == false)
				{
					bool handled = false;
					m_nodeMoveHelper.HandleMouseDown(p.SnapPoint, ref handled);
					FinishNodeEdit();
					e.Handled = true;
				}
				if (m_commandType == eCommandType.edit)
				{
				}
			}
		}
        /// <summary>
        /// 重载键盘监听
        /// </summary>
        /// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			HandleQuickSnap(e);

			if (m_nodeMoveHelper.IsEmpty == false)
			{
				m_nodeMoveHelper.OnKeyDown(m_canvaswrapper, e);
				if (e.Handled)
					return;
			}
			base.OnKeyDown(e);
			if (e.Handled)
			{
				UpdateCursor();
				return;
			}
			if (m_editTool != null)
			{
				m_editTool.OnKeyDown(m_canvaswrapper, e);
				if (e.Handled)
					return;
			}
			if (m_newObject != null)
			{
				m_newObject.OnKeyDown(m_canvaswrapper, e);
				if (e.Handled)
					return;
			}
			foreach (IDrawObject obj in m_model.SelectedObjects)
			{
				obj.OnKeyDown(m_canvaswrapper, e);
				if (e.Handled)
					return;
			}

			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (e.KeyCode == Keys.G)//ctrl+g网格是否显示
				{
                    m_model.GridLayer.Enabled = !m_model.GridLayer.Enabled;
					DoInvalidate(true);
				}
				if (e.KeyCode == Keys.S)
				{
					RunningSnapsEnabled = !RunningSnapsEnabled;
                    MessageBox.Show("1");
                    if (!RunningSnapsEnabled)
						m_snappoint = null;
					DoInvalidate(false);
				}
				return;
			}

			if (e.KeyCode == Keys.Escape)//取消选择
			{
				CommandEscape();
			}
			if (e.KeyCode == Keys.P)//移动画布工具
			{
				CommandPan();
			}
			if (e.KeyCode == Keys.S)//画单条线工具
			{
				RunningSnapsEnabled = !RunningSnapsEnabled;
				if (!RunningSnapsEnabled)
					m_snappoint = null;
				DoInvalidate(false);
			}
			if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)//数字键盘选择线条粗细颜色
			{
				int layerindex = (int)e.KeyCode - (int)Keys.D1;
				if (layerindex >=0 && layerindex < m_model.Layers.Length)
				{
					m_model.ActiveLayer = m_model.Layers[layerindex];
					DoInvalidate(true);
				}
			}
			if (e.KeyCode == Keys.Delete)//删除选中
			{
				CommandDeleteSelected();
			}
			if (e.KeyCode == Keys.O)//建立两线延长线直到相交点
			{
                CommandEdit("linesmeet");
			}
			UpdateCursor();
		}
	}
}
