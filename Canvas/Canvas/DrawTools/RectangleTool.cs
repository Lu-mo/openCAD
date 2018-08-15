using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace Canvas.DrawTools
{
    class RectangleTool : INodePoint
    {
        public enum ePoint
        {
            P1,
            P2,
        }
        static bool m_angleLocked = false;
        Rectangle m_owner;
        Rectangle m_clone;
        UnitPoint m_originalPoint;
        UnitPoint m_endPoint;
        ePoint m_pointId;
        static double xLeft;
        static double yTop;
        static double widthMax;
        double HeightMax;
        
        /// <summary>
        /// 赋值,初始化图形开始点
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        public RectangleTool(Rectangle owner, ePoint id)
        {
            m_owner = owner;
            m_clone = m_owner.Clone() as Rectangle;
            m_pointId = id;
            m_originalPoint = GetPoint(m_pointId);
        }

        #region INodePoint Members
        void INodePoint.Cancel()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 完成
        /// </summary>
		public void Finish()
        {
            m_endPoint = GetPoint(m_pointId);
            m_owner.P1 = m_clone.P1;
            m_owner.P2 = m_clone.P2;
            m_clone = null;
        }

        /// <summary>
        /// 返回克隆的Rectangle对象
        /// </summary>
        /// <returns></returns>
        public IDrawObject GetClone()
        {
            return m_clone;
        }

        /// <summary>
        /// 返回原始赋值的Rectangle对象
        /// </summary>
        /// <returns></returns>
        public IDrawObject GetOriginal()
        {
            return m_owner;
        }

        /// <summary>
        /// 设置点的位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(UnitPoint pos)
        {
            if (Control.ModifierKeys == Keys.Control)//如果按下ctrl，则以45度角找邻点
                pos = HitUtil.OrthoPointD(OtherPoint(m_pointId), pos, 45);
            if (m_angleLocked || Control.ModifierKeys == (Keys)(Keys.Control | Keys.Shift))//如果角度被锁定且按下crtl或shifr则设定为点到直线最近距离直线上的点？？
                pos = HitUtil.NearestPointOnLine(m_owner.P1, m_owner.P2, pos, true);
            SetPoint(m_pointId, pos, m_clone);//设置线上的点（p1,p2）的信息。
        }

        /// <summary>
        /// 后撤
        /// </summary>
		public void Undo()
        {
            SetPoint(m_pointId, m_originalPoint, m_owner);
        }

        /// <summary>
        /// 前进
        /// </summary>
		public void Redo()
        {
            SetPoint(m_pointId, m_endPoint, m_owner);
        }

        /// <summary>
        /// 按下R键，开始画线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="e"></param>
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                m_angleLocked = !m_angleLocked;
                e.Handled = true;
            }
        }
        #endregion

        /// <summary>
        /// 获取线上的另一个端点
        /// </summary>
        /// <param name="currentpointid"></param>
        /// <returns></returns>
		protected UnitPoint OtherPoint(ePoint currentpointid)
        {
            if (currentpointid == ePoint.P1)
                return GetPoint(ePoint.P2);
            return GetPoint(ePoint.P1);
        }
        /// <summary>
        /// 获取线上的端点
        /// </summary>
        /// <param name="pointid"></param>
        /// <returns></returns>
        protected UnitPoint GetPoint(ePoint pointid)
        {
            if (pointid == ePoint.P1)
                return m_clone.P1;
            if (pointid == ePoint.P2)
                return m_clone.P2;
            return m_owner.P1;
        }
        /// <summary>
        /// 设置线段端点
        /// </summary>
        /// <param name="pointid"></param>
        /// <param name="point"></param>
        /// <param name="rectangle"></param>
		protected void SetPoint(ePoint pointid, UnitPoint point, Rectangle rectangle)
        {
            if (pointid == ePoint.P1)
                rectangle.P1 = point;
            if (pointid == ePoint.P2)
                rectangle.P2 = point;
        }
    }

    class Rectangle : DrawObjectBase, IDrawObject, ISerialize
    {
        protected UnitPoint m_p1, m_p2;

        public Rectangle()
        {
        }
        public Rectangle(UnitPoint p1, UnitPoint p2, float width, Color color)
        {
            P1 = p1;
            P2 = p2;
            Width = width;
            Color = color;
            Selected = true;
        }

        [XmlSerializable]
        public UnitPoint P1
        {
            get { return m_p1; }
            set { m_p1 = value; }
        }
        [XmlSerializable]
        public UnitPoint P2
        {
            get { return m_p2; }
            set { m_p2 = value; }
        }
        public static string ObjectType//获取类型
        {
            get { return "Rectangle"; }
        }

        static int ThresholdPixel = 6;//像素值
        /// <summary>
        /// 获取换算之后的宽度
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="objectwidth"></param>
        /// <returns></returns>
		static float ThresholdWidth(ICanvas canvas, float objectwidth)
        {
            return ThresholdWidth(canvas, objectwidth, ThresholdPixel);
        }
        /// <summary>
        /// 宽度换算
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="objectwidth"></param>
        /// <param name="pixelwidth"></param>
        /// <returns></returns>
		public static float ThresholdWidth(ICanvas canvas, float objectwidth, float pixelwidth)//pixelwidth为像素值
        {
            double minWidth = canvas.ToUnit(pixelwidth);
            double width = Math.Max(objectwidth / 2, minWidth);
            return (float)width;
        }
        /// <summary>
        /// 复制矩形
        /// </summary>
        /// <param name="acopy"></param>
		public virtual void Copy(Rectangle acopy)
        {
            base.Copy(acopy);
            m_p1 = acopy.m_p1;
            m_p2 = acopy.m_p2;
            Selected = acopy.Selected;
        }


        #region IDrawObject
        public virtual string Id
        {
            get { return ObjectType; }
        }

        public UnitPoint RepeatStartingPoint
        {
            get { return m_p2; }
        }

        /// <summary>
        /// 从模型中初始化矩形(点状)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="layer"></param>
        /// <param name="snap"></param>
        public override void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap)
        {
            P1 = P2 = point;
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
        }

        /// <summary>
        /// 获取中点，返回四边中点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="hitpoint"></param>
        /// <returns></returns>
        UnitPoint MidPoint(ICanvas canvas, UnitPoint p1, UnitPoint p2, UnitPoint hitpoint)
        {
            //MessageBox.Show("Mid");
            UnitPoint p3 = new UnitPoint(p1.X, p2.Y);
            UnitPoint p4 = new UnitPoint(p2.X, p1.Y);
            //UnitPoint mid1 = HitUtil.LineMidpoint(p1, p3);
            //UnitPoint mid2 = HitUtil.LineMidpoint(p1, p4);
            //UnitPoint mid3 = HitUtil.LineMidpoint(p3, p2);
            //UnitPoint mid4 = HitUtil.LineMidpoint(p4, p2);
            //UnitPoint mid5 = HitUtil.LineMidpoint(p1, p2);
            UnitPoint mid6 = HitUtil.LineMidpoint(p3, p3);
            UnitPoint mid7 = HitUtil.LineMidpoint(p4, p4);
            float thWidth = ThresholdWidth(canvas, Width);
            Console.WriteLine("thWidth"+thWidth+"");
            //if (HitUtil.CircleHitPoint(mid1, thWidth, hitpoint))
            //{
            //    return mid1;
            //}
            //if (HitUtil.CircleHitPoint(mid2, thWidth, hitpoint))
            //{
            //    return mid2;
            //}
            //if (HitUtil.CircleHitPoint(mid3, thWidth, hitpoint))
            //{
            //    return mid3;
            //}
            //if (HitUtil.CircleHitPoint(mid4, thWidth, hitpoint))
            //{
            //    return mid4;
            //}
            //if (HitUtil.CircleHitPoint(mid5, thWidth, hitpoint))
            //{
            //    return mid5;
            //}
            if (HitUtil.CircleHitPoint(mid6, thWidth, hitpoint))
            {
                return mid6;
            }
            if (HitUtil.CircleHitPoint(mid7, thWidth, hitpoint))
            {
                return mid7;
            }

            return UnitPoint.Empty;
        }

        public virtual IDrawObject Clone()
        {
            Rectangle r = new Rectangle();
            r.Copy(this);
            return r;
        }

        /// <summary>
        /// 绘制矩形*
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
        public virtual void Draw(ICanvas canvas, RectangleF unitrect)
        {

            //canvas.Invalidate();            
            Color color = Color;
            Pen pen = canvas.CreatePen(color, Width);
            //pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            //pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            //canvas.DrawLine(canvas, pen, m_p1, m_p2);
            //UnitPoint p3 = new UnitPoint(m_p1.X, m_p2.Y);
            //UnitPoint p4 = new UnitPoint(m_p2.X, m_p1.Y);
            canvas.DrawRectangle(canvas, pen, m_p1, m_p2);
            if (Highlighted)//如果高亮
            {
                canvas.DrawRectangle(canvas, DrawUtils.SelectedPen, m_p1, m_p2);//画线
                //UnitPoint p3 = new UnitPoint(m_p1.X,m_p2.Y);
                //UnitPoint p4 = new UnitPoint(m_p2.X, m_p1.Y);
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, p3);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, p4);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, p3, m_p2);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, p4, m_p2);//画线

            }
    
            if (Selected)   //如果被选中
            {
                canvas.DrawRectangle(canvas, DrawUtils.SelectedPen, m_p1, m_p2);//画线
                //UnitPoint p3 = new UnitPoint(m_p1.X, m_p2.Y);
                //UnitPoint p4 = new UnitPoint(m_p2.X, m_p1.Y);
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, p3);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, p4);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, p3, m_p2);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, p4, m_p2);//画线
                if (m_p1.IsEmpty == false)//如果端点p1不存在
                    DrawUtils.DrawNode(canvas, m_p1);//画点
                if (m_p2.IsEmpty == false)//如果端点p2不存在
                    DrawUtils.DrawNode(canvas, m_p2);//画点
                //MessageBox.Show("");
            }
            //if(Selected==false)
            //{
            //    canvas.DrawLine(canvas, pen, m_p1, p3);//画线
            //    canvas.DrawLine(canvas, pen, m_p1, p4);//画线
            //    canvas.DrawLine(canvas, pen, p3, m_p2);//画线
            //    canvas.DrawLine(canvas, pen, p4, m_p2);//画线
            //}
            
        }

        public RectangleF GetBoundingRect(ICanvas canvas)
        {
            float r =Convert.ToSingle(Math.Sqrt((Math.Pow((m_p1.X - m_p2.X),2.0)+ Math.Pow((m_p1.Y - m_p2.Y), 2.0))/2));
            Console.WriteLine(r+" ");
            RectangleF rect = HitUtil.CircleBoundingRect(new UnitPoint((m_p1.X+m_p2.X)/2.0, (m_p1.Y + m_p2.Y) / 2.0), r);
            // if drawing either angle then include the mouse point in the ractangle - this is to redraw (erase) the line drawn
            // from center point to mouse point
            
            return rect;
            //double x1 = m_p1.X > m_p2.X ? m_p1.X : m_p2.X;
            //double x2 = m_p1.X > m_p2.X ? m_p2.X : m_p1.X;
            //double y1 = m_p1.Y > m_p2.Y ? m_p1.Y : m_p2.Y;
            //double y2 = m_p1.Y > m_p2.Y ? m_p2.Y : m_p1.Y;
            ////throw new NotImplementedException();
            //Console.WriteLine("GetBoundingRect "+x2 +" "+y1);
            //return new RectangleF(Convert.ToSingle(x2), Convert.ToSingle(y1), Convert.ToSingle(x1 - x2), Convert.ToSingle(y1 - y2));
            //Console.WriteLine();
        }

        /// <summary>
        /// 获取线段端点坐标，长度和与x轴的角度
        /// </summary>
        /// <returns></returns>
        string IDrawObject.GetInfoAsString()
        {
            return string.Format("Rectangle@{0},{1} - L={2:f4}<{3:f4}",
                P1.PosAsString(),
                P2.PosAsString(),
                HitUtil.Distance(P1, P2),
                HitUtil.RadiansToDegrees(HitUtil.LineAngleR(P1, P2, 0)));
        }

        /// <summary>
        /// 移动矩形
        /// </summary>
        /// <param name="offset"></param>
        void IDrawObject.Move(UnitPoint offset)
        {
            m_p1.X += offset.X;
            m_p1.Y += offset.Y;
            m_p2.X += offset.X;
            m_p2.Y += offset.Y;
        }

        /// <summary>
        /// 判断线的端点是否在圆内，是则返回新构造的NodePointLine
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
		public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            float thWidth = ThresholdWidth(canvas, Width);
            if (HitUtil.CircleHitPoint(m_p1, thWidth, point))
                return new RectangleTool(this, RectangleTool.ePoint.P1);
            if (HitUtil.CircleHitPoint(m_p2, thWidth, point))
                return new RectangleTool(this, RectangleTool.ePoint.P2);
            return null;
        }
        /// <summary>
        /// 图形是否包含于选择矩形
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="rect"></param>
        /// <param name="anyPoint"></param>
        /// <returns></returns>
        bool IDrawObject.ObjectInRectangle(ICanvas canvas, RectangleF rect, bool anyPoint)
        {
            //MessageBox.Show("");
            double x1 = m_p1.X > m_p2.X ? m_p1.X : m_p2.X;
            double x2 = m_p1.X > m_p2.X ? m_p2.X : m_p1.X;
            double y1 = m_p1.Y > m_p2.Y ? m_p1.Y : m_p2.Y;
            double y2 = m_p1.Y > m_p2.Y ? m_p2.Y : m_p1.Y;
            RectangleF rectangleF= new RectangleF(Convert.ToSingle(x2), Convert.ToSingle(y1), Convert.ToSingle(x1 - x2), Convert.ToSingle(y1 - y2));

            if (rect.Contains(new PointF(Convert.ToSingle(x2), Convert.ToSingle(y1))) && rect.Contains(new PointF(Convert.ToSingle(x1), Convert.ToSingle(y2))))
                return true;
            else
                return false;
            //RectangleF boundingrect = GetBoundingRect(canvas);
            //if (anyPoint)
            //    return HitUtil.LineIntersectWithRect(m_p1, m_p2, rect);
            //return rect.Contains(boundingrect);
        }

        void IDrawObject.OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        /// <summary>
        /// 按下鼠标时寻找点到直线最近距离直线上的点或距离园最近的点以角度找邻点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
        public virtual eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            OnMouseMove(canvas, point);
            Selected = false;
            if (snappoint is PerpendicularSnapPoint && snappoint.Owner is Rectangle)
            {
                Rectangle src = snappoint.Owner as Rectangle;
                m_p2 = HitUtil.NearestPointOnLine(src.P1, src.P2, m_p1, true);
                return eDrawObjectMouseDown.Done;
            }
            //if (snappoint is PerpendicularSnapPoint && snappoint.Owner is Arc)
            //{
            //    Arc src = snappoint.Owner as Arc;
            //    m_p2 = HitUtil.NearestPointOnCircle(src.Center, src.Radius, m_p1, 0);
            //    return eDrawObjectMouseDown.DoneRepeat;
            //}
            if (Control.ModifierKeys == Keys.Control)
                point = HitUtil.OrthoPointD(m_p1, point, 45);
            m_p2 = point;
            return eDrawObjectMouseDown.Done;
        }

        /// <summary>
        /// 鼠标移动，如果按下crtl则以45度角找邻点，设置p2为邻点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            //canvas.Invalidate();
            //canvas.
            //if (Control.ModifierKeys == Keys.Control)
            //    point = HitUtil.OrthoPointD(m_p1, point, 45);
            m_p2 = point;
            
            //MessageBox.Show("Move");
        }

        void IDrawObject.OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        /// <summary>
        /// 判断鼠标焦点是否在矩形内
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        bool IDrawObject.PointInObject(ICanvas canvas, UnitPoint point)
        {
            //MessageBox.Show("");
            double x1 = m_p1.X > m_p2.X ? m_p1.X : m_p2.X;
            double x2 = m_p1.X > m_p2.X ? m_p2.X : m_p1.X;
            double y1 = m_p1.Y > m_p2.Y ? m_p1.Y : m_p2.Y;
            double y2 = m_p1.Y > m_p2.Y ? m_p2.Y : m_p1.Y;
            if (point.X<=x1 && point.X>=x2)
            {
                if (point.Y <= y1 && point.Y >= y2)
                {
                    //MessageBox.Show("");
                    return true;
                    
                }
                    
            }
            return false;
        }

        /// <summary>
        /// 根据给定类型返回点的快照信息
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="otherobj"></param>
        /// <param name="runningsnaptypes"></param>
        /// <param name="usersnaptype"></param>
        /// <returns></returns>
        ISnapPoint IDrawObject.SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {
            float thWidth = ThresholdWidth(canvas, Width);
            if (runningsnaptypes != null)
            {
                foreach (Type snaptype in runningsnaptypes)
                {
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.CircleHitPoint(m_p1, thWidth, point))
                            return new VertextSnapPoint(canvas, this, m_p1);
                        if (HitUtil.CircleHitPoint(m_p2, thWidth, point))
                            return new VertextSnapPoint(canvas, this, m_p2);
                    }
                    if (snaptype == typeof(MidpointSnapPoint))
                    {
                        UnitPoint p = MidPoint(canvas, m_p1, m_p2, point);
                        if (p != UnitPoint.Empty)
                            return new MidpointSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(IntersectSnapPoint))
                    {
                        Rectangle rectangle = Utils.FindObjectTypeInList(this, otherobj, typeof(Rectangle)) as Rectangle;
                        if (rectangle == null)
                            continue;
                        UnitPoint p = HitUtil.LinesIntersectPoint(m_p1, m_p2, rectangle.m_p1, rectangle.m_p2);
                        if (p != UnitPoint.Empty)
                            return new IntersectSnapPoint(canvas, this, p);
                    }
                }
                return null;
            }

            if (usersnaptype == typeof(MidpointSnapPoint))
                return new MidpointSnapPoint(canvas, this, HitUtil.LineMidpoint(m_p1, m_p2));
            if (usersnaptype == typeof(IntersectSnapPoint))
            {
                Rectangle rectangle = Utils.FindObjectTypeInList(this, otherobj, typeof(Rectangle)) as Rectangle;
                if (rectangle == null)
                    return null;
                UnitPoint p = HitUtil.LinesIntersectPoint(m_p1, m_p2, rectangle.m_p1, rectangle.m_p2);
                if (p != UnitPoint.Empty)
                    return new IntersectSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(VertextSnapPoint))
            {
                double d1 = HitUtil.Distance(point, m_p1);
                double d2 = HitUtil.Distance(point, m_p2);
                if (d1 <= d2)
                    return new VertextSnapPoint(canvas, this, m_p1);
                return new VertextSnapPoint(canvas, this, m_p2);
            }
            if (usersnaptype == typeof(NearestSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnLine(m_p1, m_p2, point);
                if (p != UnitPoint.Empty)
                    return new NearestSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(PerpendicularSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnLine(m_p1, m_p2, point);
                if (p != UnitPoint.Empty)
                    return new PerpendicularSnapPoint(canvas, this, p);
            }
            return null;
        }
        #endregion


        #region ISerialize
        /// <summary>
        /// 写入xml元素
        /// </summary>
        /// <param name="wr"></param>
        public void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement("Rectangle");///元素名
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }
        void ISerialize.AfterSerializedIn()
        {
        }
        #endregion

    }

    class RectangleEdit : Rectangle, IObjectEditInstance
    {
        protected PerpendicularSnapPoint m_perSnap;
        protected TangentSnapPoint m_tanSnap;
        protected bool m_tanReverse = false;

        public RectangleEdit()
        {
        }

        public override string Id
        {
            get
            { return "Rectangle";}
        }
        //public RectangleEdit(bool rectangle): base()
        //{
        //    m_singleRectangleSegment = rectangle;
        //}

        /// <summary>
        /// 从数据集合中初始化线条并设置垂线快照，正切快照
        /// </summary>
        /// <param name="point"></param>
        /// <param name="layer"></param>
        /// <param name="snap"></param>
		public override void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap)
        {
            base.InitializeFromModel(point, layer, snap);
            m_perSnap = snap as PerpendicularSnapPoint;
            m_tanSnap = snap as TangentSnapPoint;
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		public override void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            if (m_perSnap != null)
            {
                MouseMovePerpendicular(canvas, point);
                return;
            }
            if (m_tanSnap != null)
            {
                MouseMoveTangent(canvas, point);
                return;
            }
            base.OnMouseMove(canvas, point);
        }

        public override eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            if (m_tanSnap != null && Control.MouseButtons == MouseButtons.Right)
            {
                ReverseTangent(canvas);
                return eDrawObjectMouseDown.Continue;
            }

            if (m_perSnap != null || m_tanSnap != null)
            {
                if (snappoint != null)
                    point = snappoint.SnapPoint;
                OnMouseMove(canvas, point);
                return eDrawObjectMouseDown.Done;
                ///return eDrawObjectMouseDown.DoneRepeat;
            }
            eDrawObjectMouseDown result = base.OnMouseDown(canvas, point, snappoint);
            return eDrawObjectMouseDown.Done;
        }

        /// <summary>
        /// 反转是否正切
        /// </summary>
        /// <param name="canvas"></param>
		protected virtual void ReverseTangent(ICanvas canvas)
        {
            m_tanReverse = !m_tanReverse;
            MouseMoveTangent(canvas, m_p2);
            canvas.Invalidate();
        }

        /// <summary>
        /// 鼠标移动正切？如果m_tanSnap.Owner is IArc，则设置端点P1为圆上的切点，p2为传入的值，如果p1为空，则设置p1p2均为圆弧的中点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		protected virtual void MouseMoveTangent(ICanvas canvas, UnitPoint point)
        {
            if (m_tanSnap.Owner is IArc)
            {
                IArc src = m_tanSnap.Owner as IArc;
                m_p1 = HitUtil.TangentPointOnCircle(src.Center, src.Radius, point, m_tanReverse);
                m_p2 = point;
                if (m_p1 == UnitPoint.Empty)
                    m_p2 = m_p1 = src.Center;
            }
        }

        /// <summary>
        /// 鼠标垂直移动？如果是perSnap.Owner线则设置p1为点到直线最近距离直线上的点，p2为传入的点，如果是IArc则设置p1为到圆最近距离的点，p2为传入的点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		protected virtual void MouseMovePerpendicular(ICanvas canvas, UnitPoint point)
        {
            if (m_perSnap.Owner is Line)
            {
                Line src = m_perSnap.Owner as Line;
                m_p1 = HitUtil.NearestPointOnLine(src.P1, src.P2, point, true);
                m_p2 = point;
            }
            if (m_perSnap.Owner is IArc)
            {
                IArc src = m_perSnap.Owner as IArc;
                m_p1 = HitUtil.NearestPointOnCircle(src.Center, src.Radius, point, 0);
                m_p2 = point;
            }
        }

        /// <summary>
        /// 复制编辑矩形
        /// </summary>
        /// <param name="acopy"></param>
		public new void Copy(RectangleEdit acopy)
        {
            base.Copy(acopy);
            m_perSnap = acopy.m_perSnap;
            m_tanSnap = acopy.m_tanSnap;
            m_tanReverse = acopy.m_tanReverse;
        }

        /// <summary>
        /// 克隆编辑矩形
        /// </summary>
        /// <returns></returns>
		public override IDrawObject Clone()
        {
            RectangleEdit r = new RectangleEdit();
            r.Copy(this);
            return r;
        }

        #region IObjectEditInstance
        /// <summary>
        /// 获取新的矩形new Line(P1, P2, Width, Color);
        /// </summary>
        /// <returns></returns>
        public IDrawObject GetDrawObject()
        {
            return new Rectangle(P1, P2, Width, Color);
        }
        #endregion
    }
}