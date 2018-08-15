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
        /// ���췽��
        /// </summary>
        /// <param name="canvas"></param>
		public CanvasWrapper(CanvasCtrl canvas)
		{
			m_canvas = canvas;
			m_graphics = null;
			m_rect = new Rectangle();
		}
        /// <summary>
        /// ���췽��
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
        /// ��ȡ����
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
        /// �ͷ���Դ
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
        /// ��������
        /// </summary>
		public Rectangle ClientRectangle
		{
			get { return m_rect; }
			set { m_rect = value; }
		}/// <summary>
        /// ��������ʵ��
        /// </summary>
        /// <param name="color"></param>
        /// <param name="unitWidth"></param>
        /// <returns></returns>
		public Pen CreatePen(Color color, float unitWidth)
		{
			return m_canvas.CreatePen(color, unitWidth);
		}
        /// <summary>
        /// ����
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
        /// ����
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
		IModel				m_model;//���ݼ���
        MoveHelper			m_moveHelper = null;
		NodeMoveHelper		m_nodeMoveHelper = null;
		CanvasWrapper		m_canvaswrapper;
		eCommandType		m_commandType = eCommandType.select;
		bool				m_runningSnaps = true;//�Ƿ��ڻ�ͼģʽ����
		Type[]				m_runningSnapTypes = null;//��ͼ���ͣ���
        PointF				m_mousedownPoint;//��갴�µ�����
		IDrawObject			m_newObject = null;//��ͼ���ߣ���
		IEditTool			m_editTool = null;//�༭����
		SelectionRectangle	m_selection = null;//��ѡ�е�����
		string				m_drawObjectId = string.Empty;//��ͼ����ID
		string				m_editToolId = string.Empty;//�༭����ID
		Bitmap				m_staticImage = null;//��̬ͼƬ
		bool				m_staticDirty = true;//����Ƿ�Ϊ��
		ISnapPoint			m_snappoint = null;//���ݼ���

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

		System.Drawing.Drawing2D.SmoothingMode	m_smoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;//ָ��δ�����
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
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);//���Դ�����Ϣ������˸���ɿؼ�����������
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);//�ؼ������Ȼ��Ƶ������������ǻ��Ƶ���Ļ��

			m_commandType = eCommandType.select;
			m_cursors.AddCursor(eCommandType.select, Cursors.Arrow);//����ѡ�й������
            m_cursors.AddCursor(eCommandType.draw, Cursors.Cross);//���û�ͼ�������
            m_cursors.AddCursor(eCommandType.pan, "hmove.cur");//�����ƶ�����������
            m_cursors.AddCursor(eCommandType.move, Cursors.SizeAll);//�����ƶ��������
            m_cursors.AddCursor(eCommandType.edit, Cursors.Cross);//���ñ༭�������
            UpdateCursor();//���¹��

			m_moveHelper = new MoveHelper(this);
			m_nodeMoveHelper = new NodeMoveHelper(m_canvaswrapper);
		}
        /// <summary>
        /// �����λ�û�����������겢����
        /// </summary>
        /// <returns></returns>
		public UnitPoint GetMousePoint()
		{
			Point point = this.PointToClient(Control.MousePosition);
			return ToUnit(point);
		}
        /// <summary>
        /// ͼ�����
        /// </summary>
        /// <param name="unitPoint"></param>
		public void SetCenter(UnitPoint unitPoint)
		{
			PointF point = ToScreen(unitPoint);//���껻��
			m_lastCenterPoint = unitPoint;
			SetCenterScreen(point, false);//���þ��к���
		}
        /// <summary>
        /// ͼ�����
        /// </summary>
		public void SetCenter()
        {
			Point point = this.PointToClient(Control.MousePosition);//����ǰ���λ�û������Ļ����
			SetCenterScreen(point, true);
		}/// <summary>
        /// ��ȡ���ĵ�
        /// </summary>
        /// <returns></returns>
		public UnitPoint GetCenter()
		{
			return ToUnit(new PointF(this.ClientRectangle.Width/2, this.ClientRectangle.Height/2));
		}
		protected  void SetCenterScreen(PointF screenPoint, bool setCursor)//�������ĵ�
        {
			float centerX = ClientRectangle.Width / 2;
			m_panOffset.X += centerX - screenPoint.X;
			
			float centerY = ClientRectangle.Height / 2;
			m_panOffset.Y += centerY - screenPoint.Y;

			if (setCursor)
				Cursor.Position = this.PointToScreen(new Point((int)centerX, (int)centerY));//���껻�㣬��������λ�á�
			DoInvalidate(true);
		}
        /// <summary>
        /// �����ػ�
        /// </summary>
        /// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			CommonTools.Tracing.StartTrack(Program.TracePaint);//TracePaint=1
            ClearPens();//��ջ���
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
				dcStatic.Graphics.DrawLine(Pens.Pink, nullPoint.X - 10, nullPoint.Y, nullPoint.X + 10, nullPoint.Y);//����ϵԭ��
				dcStatic.Graphics.DrawLine(Pens.Pink, nullPoint.X, nullPoint.Y - 10, nullPoint.X, nullPoint.Y + 10);
				ICanvasLayer[] layers = m_model.Layers;//������ϸ��ɫ
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
        /// �ڻ�������Rectangle�ػ�
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
        /// �ڻ�������snappoint�ػ�
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
         /// �ڻ�������IDrawObject�ػ�
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
        /// ʹѡ�з�����Ч���ػ棨�ƶ�ѡ�еķ��飩
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
        /// ʹ������Ч���ػ�
        /// </summary>
        /// <param name="dostatic"></param>
		public void DoInvalidate(bool dostatic)
		{
			if (dostatic)
				m_staticDirty = true;
			Invalidate();
		}/// <summary>
         /// ��ȡ m_newObject
         /// </summary>
        public IDrawObject NewObject
		{
			get { return m_newObject; }
		}
        /// <summary>
        /// ����ѡ�У�shiftΪȫѡ��ctrlΪ��/��ѡ��
        /// </summary>
        /// <param name="selected"></param>
		protected void HandleSelection(List<IDrawObject> selected)
		{
			bool add = Control.ModifierKeys == Keys.Shift;//����shiftΪ�޸ļ�
			bool toggle = Control.ModifierKeys == Keys.Control;//����ctrlΪ�޸ļ�
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
        /// ��ɽڵ�༭
        /// </summary>
		void FinishNodeEdit()
		{
			m_commandType = eCommandType.select;
			m_snappoint = null;
		}
        /// <summary>
        /// ��ͼʱ����갴���¼���
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
        /// ��갴���¼�
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_mousedownPoint = new PointF(e.X, e.Y); // used when panning
			m_dragOffset = new PointF(0,0);

			UnitPoint mousepoint = ToUnit(m_mousedownPoint);//��ȡ��ǰ�����������
			if (m_snappoint != null)
				mousepoint = m_snappoint.SnapPoint;
			
			if (m_commandType == eCommandType.editNode)//���Ϊ�༭�ڵ�
			{
				bool handled = false;
				if (m_nodeMoveHelper.HandleMouseDown(mousepoint, ref handled))
				{
					FinishNodeEdit();
					base.OnMouseDown(e);
					return;
				}
			}
			if (m_commandType == eCommandType.select)//���Ϊѡ��
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
			if (m_commandType == eCommandType.move)//���Ϊ�ƶ�
            {
				m_moveHelper.HandleMouseDownForMove(mousepoint, m_snappoint);
			}
			if (m_commandType == eCommandType.draw)//���Ϊ����
            {
				HandleMouseDownWhenDrawing(mousepoint, null);
				DoInvalidate(true);
			}
			if (m_commandType == eCommandType.edit)//���Ϊ�༭
            {
				if (m_editTool == null)//����༭����Ϊ��
					m_editTool = m_model.GetEditTool(m_editToolId);
				if (m_editTool != null)//����༭���߲�Ϊ��
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
					if (mouseresult == eDrawObjectMouseDown.Done)//��������Ѿ����
					{
						m_editTool.Finished();
						m_editTool = m_model.GetEditTool(m_editToolId); // continue with new tool
						//m_editTool = null;
						
						if (m_editTool.SupportSelection)
							m_selection = new SelectionRectangle(m_mousedownPoint);
					}
				}
				DoInvalidate(true);
				UpdateCursor();//���¹��
			}
			base.OnMouseDown(e);
		}
        /// <summary>
        /// ���̧��ʱ
        /// </summary>
        /// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (m_commandType == eCommandType.pan)//������ƶ�����
			{
				m_panOffset.X += m_dragOffset.X;
				m_panOffset.Y += m_dragOffset.Y;
				m_dragOffset = new PointF(0, 0);
			}

			List<IDrawObject> hitlist = null;
			Rectangle screenSelRect = Rectangle.Empty;
			if (m_selection != null)//�����ѡ�е�����Ϊ��
			{
				screenSelRect = m_selection.ScreenRect();
				RectangleF selectionRect = m_selection.Selection(m_canvaswrapper);
				if (selectionRect != RectangleF.Empty)
				{
                    // is any selection rectangle. use it for selectionѡ��������ѡ����Ρ�ʹ��������ѡ��
                    hitlist = m_model.GetHitObjects(m_canvaswrapper, selectionRect, m_selection.AnyPoint());
					DoInvalidate(true);
				}
				else
				{
                    // else use mouse point����ʹ�ù��ָ��ĵ�
                    UnitPoint mousepoint = ToUnit(new PointF(e.X, e.Y));
					hitlist = m_model.GetHitObjects(m_canvaswrapper, mousepoint);
				}
				m_selection = null;
			}
			if (m_commandType == eCommandType.select)//�����ѡ��
			{
				if (hitlist != null)
					HandleSelection(hitlist);
			}
			if (m_commandType == eCommandType.edit && m_editTool != null)//����Ǳ༭
			{
				UnitPoint mousepoint = ToUnit(m_mousedownPoint);
				if (m_snappoint != null)
					mousepoint = m_snappoint.SnapPoint;
				if (screenSelRect != Rectangle.Empty)
					m_editTool.SetHitObjects(mousepoint, hitlist);
				m_editTool.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
			}
			if (m_commandType == eCommandType.draw && m_newObject != null)//����ǻ����һ�ͼ���߲�Ϊ
			{
				UnitPoint mousepoint = ToUnit(m_mousedownPoint);
				if (m_snappoint != null)
					mousepoint = m_snappoint.SnapPoint;
				m_newObject.OnMouseUp(m_canvaswrapper, mousepoint, m_snappoint);
			}
		}
        protected void overside()//ʹ�ƶ����ܳ���100*100
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
        /// ����ƶ��¼�
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

            if (m_commandType == eCommandType.pan && e.Button == MouseButtons.Left)//���ѡ��pan���ƶ����������а�ס������
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
			if (m_commandType == eCommandType.draw || m_commandType == eCommandType.move || m_nodeMoveHelper.IsEmpty == false)//���ʹ���ƻ����ƶ���ڵ��ƶ���ڵ��ƶ�Ϊ��
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
				if ((m_snappoint != null) && ((newsnap == null) || (newsnap.SnapPoint != m_snappoint.SnapPoint) || m_snappoint.GetType() != newsnap.GetType()))//��ͼʱ���������ɽڵ�
				{
                    
					invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_snappoint.BoundingRect));
					invalidaterect.Inflate(2, 2);
					RepaintStatic(invalidaterect); // remove old snappoint
					m_snappoint = newsnap;
				}
                if (m_commandType == eCommandType.move)//ʹ���ƶ�����ʱ���ػ�ͼ��
                {
                    Invalidate(invalidaterect); 
                }

				if (m_snappoint == null)
					m_snappoint = newsnap;
			}
			m_owner.SetPositionInfo(unitpoint);//����������Ϣ
			m_owner.SetSnapInfo(m_snappoint);//����������Ϣ


            //UnitPoint mousepoint;
            if (m_snappoint != null)
				mousepoint = m_snappoint.SnapPoint;
			else
				mousepoint = GetMousePoint();

			if (m_newObject != null)//�����Ϊ�����ػ�
			{
				Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, m_newObject.GetBoundingRect(m_canvaswrapper)));
				invalidaterect.Inflate(2, 2);
				RepaintStatic(invalidaterect);
				m_newObject.OnMouseMove(m_canvaswrapper, mousepoint);
				RepaintObject(m_newObject);
			}
			if (m_snappoint != null)//�����Ϊ�����ػ�
                RepaintSnappoint(m_snappoint);

			if (m_moveHelper.HandleMouseMoveForMove(mousepoint))//�ػ�
				Refresh(); //Invalidate();

			RectangleF rNoderect = m_nodeMoveHelper.HandleMouseMoveForNode(mousepoint);//�����ȡ
			if (rNoderect != RectangleF.Empty)
			{
				Rectangle invalidaterect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(m_canvaswrapper, rNoderect));//����ת��
				RepaintStatic(invalidaterect);//�ػ�
                CanvasWrapper dc = new CanvasWrapper(this, Graphics.FromHwnd(Handle), ClientRectangle);
				dc.Graphics.Clip = new Region(ClientRectangle);
				//m_nodeMoveHelper.DrawOriginalObjects(dc, rNoderect);
				m_nodeMoveHelper.DrawObjects(dc, rNoderect);
				if (m_snappoint != null)
					RepaintSnappoint(m_snappoint);//�ػ�
                dc.Graphics.Dispose();
				dc.Dispose();
			}
		}
        /// <summary>
        /// �������¼����Ŵ���С��
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
        /// �������ڴ�С
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
        /// ����ת��
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
		PointF Translate(UnitPoint point)
		{
			return point.Point;
		}/// <summary>
        /// ��Ļ�߶ȣ����㣩
        /// </summary>
        /// <returns></returns>
		float ScreenHeight()
		{
			return (float)(ToUnit(this.ClientRectangle.Height) / m_model.Zoom);
		}

		#region ICanvas
        /// <summary>
        /// ��������
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
        /// ��Ļ���Ͻ����꣨��Ļ����ת���������꣩
        /// </summary>
        /// <returns></returns>
		public UnitPoint ScreenTopLeftToUnitPoint()
		{
			return ToUnit(new PointF(0, 0));
		}
        /// <summary>
        /// ��Ļ���½����꣨��Ļ����ת���������꣩
        /// </summary>
        /// <returns></returns>
		public UnitPoint ScreenBottomRightToUnitPoint()
		{
			return ToUnit(new PointF(this.ClientRectangle.Width, this.ClientRectangle.Height));
		}
        /// <summary>
        /// ��������ת����Ļ
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
        /// ��λ���㣿�����������굽ϵ��Ļ��
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public float ToScreen(double value)
		{
			return (float)(value * m_screenResolution * m_model.Zoom);
		}
        /// <summary>
        /// ���㣿��������Ļ����������ϵ��
        /// </summary>
        /// <param name="screenvalue"></param>
        /// <returns></returns>
		public double ToUnit(float screenvalue)
		{
			return (double)screenvalue / (double)(m_screenResolution * m_model.Zoom);
		}
        /// <summary>
        /// ���껻�㣨��Ļ���������꣩
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
        /// ��������
        /// </summary>
        /// <param name="color"></param>
        /// <param name="unitWidth"></param>
        /// <returns></returns>
		public Pen CreatePen(Color color, float unitWidth)
		{
			return GetPen(color, ToScreen(unitWidth));
		}
        /// <summary>
        /// ����
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
        /// ��Բ�Σ���Բ������
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
        /// ���û��ʴ�ϸ��ɫ
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
        /// ��ʼ������
        /// </summary>
		void ClearPens()
		{
			m_penCache.Clear();
		}
		/// <summary>
        /// ���¹��ͼ��
        /// </summary>
		void UpdateCursor()
		{
			Cursor = m_cursors.GetCursor(m_commandType);
		}

		Dictionary<Keys, Type> m_QuickSnap = new Dictionary<Keys,Type>();//���գ�������
        /// <summary>
        /// ��ӿ������ͣ�������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="snaptype"></param>
		public void AddQuickSnapType(Keys key, Type snaptype)
		{
			m_QuickSnap.Add(key, snaptype);
		}
        /// <summary>
        /// ѡ���ͼ����
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
        /// ȡ����ǰ����ѡ�񣿣���
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
        /// �ƶ�����
        /// </summary>
		public void CommandPan()
		{
            
            if (m_commandType == eCommandType.select || m_commandType == eCommandType.move)
				m_commandType = eCommandType.pan;
			UpdateCursor();
		}
        /// <summary>
        /// �ƶ�ѡ��������ͼ��
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
        /// ɾ��ѡ��������ͼ��
        /// </summary>
		public void CommandDeleteSelected()
		{           
            m_model.DeleteObjects(m_model.SelectedObjects);
			m_model.ClearSelectedObjects();
			DoInvalidate(true);
			UpdateCursor();
		}
        /// <summary>
        /// ѡ����๤�ߣ���
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
        /// ��Ӧ���̿�ݼ�����
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
        /// ���ؼ��̼���
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
				if (e.KeyCode == Keys.G)//ctrl+g�����Ƿ���ʾ
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

			if (e.KeyCode == Keys.Escape)//ȡ��ѡ��
			{
				CommandEscape();
			}
			if (e.KeyCode == Keys.P)//�ƶ���������
			{
				CommandPan();
			}
			if (e.KeyCode == Keys.S)//�������߹���
			{
				RunningSnapsEnabled = !RunningSnapsEnabled;
				if (!RunningSnapsEnabled)
					m_snappoint = null;
				DoInvalidate(false);
			}
			if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)//���ּ���ѡ��������ϸ��ɫ
			{
				int layerindex = (int)e.KeyCode - (int)Keys.D1;
				if (layerindex >=0 && layerindex < m_model.Layers.Length)
				{
					m_model.ActiveLayer = m_model.Layers[layerindex];
					DoInvalidate(true);
				}
			}
			if (e.KeyCode == Keys.Delete)//ɾ��ѡ��
			{
				CommandDeleteSelected();
			}
			if (e.KeyCode == Keys.O)//���������ӳ���ֱ���ཻ��
			{
                CommandEdit("linesmeet");
			}
			UpdateCursor();
		}
	}
}
