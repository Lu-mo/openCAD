using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Canvas.DrawTools
{
    class BezierCurveTool : INodePoint
    {
        public enum ePoint
        {
            P1,
            P2,
            P3,
            P4,
        }
        static bool m_angleLocked = false;
        BezierCurve m_owner;
        BezierCurve m_clone;
        protected UnitPoint[] m_originalPoints = new UnitPoint[4];
        protected UnitPoint[] m_endPoints = new UnitPoint[4];
        ePoint m_pointId;
        protected Arc3Point.eCurrentPoint m_curPoint;
        //private BezierCurve bezierCurve;
        //private ePoint p1;

        public BezierCurveTool(BezierCurve owner, ePoint id)
        {
            m_owner = owner;
            m_clone = m_owner.Clone() as BezierCurve;
            m_pointId = id;
            m_originalPoints[0] = m_owner.P1;
            m_originalPoints[1] = m_owner.P2;
            m_originalPoints[2] = m_owner.P3;
            m_originalPoints[3] = m_owner.P4;
        }

        ///// <summary>
        ///// 获取端点
        ///// </summary>
        ///// <param name="pointid"></param>
        ///// <returns></returns>
        //protected UnitPoint GetPoint(ePoint pointid)
        //{
        //    if (pointid == ePoint.P1)
        //        return m_clone.P1;
        //    if (pointid == ePoint.P2)
        //        return m_clone.P2;
        //    if (pointid == ePoint.P3)
        //        return m_clone.P3;
        //    if (pointid == ePoint.P4)
        //        return m_clone.P4;
        //    return m_owner.P1;
        //}


        /// <summary>
        /// 取消
        /// </summary>
		public void Cancel()
        {
            m_owner.Selected = true;
        }

        /// <summary>
        /// 完成？
        /// </summary>
        public void Finish()
        {
            m_endPoints[0] = m_clone.P1;
            m_endPoints[1] = m_clone.P2;
            m_endPoints[2] = m_clone.P3;
            m_endPoints[3] = m_clone.P4;
            //m_endPoint = GetPoint(m_pointId);
            //m_owner.P1 = m_clone.P1;
            //m_owner.P2 = m_clone.P2;
            //m_owner.P3 = m_clone.P3;
            //m_owner.P4 = m_clone.P4;
            m_clone = null;
        }

        public IDrawObject GetClone()
        {
            return m_clone;
        }

        public IDrawObject GetOriginal()
        {
            return m_owner;
        }

        /// <summary>
        /// 按下B键，开始画线
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="e"></param>
		public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B)
            {
                m_angleLocked = !m_angleLocked;
                e.Handled = true;
            }
        }
        /// <summary>
        /// 前进
        /// </summary>
        public void Redo()
        {
            //SetPoint(m_pointId, m_endPoint, m_owner);
            m_owner.P1 = m_endPoints[0];
            m_owner.P2 = m_endPoints[1];
            m_owner.P3 = m_endPoints[2];
            m_owner.P4 = m_endPoints[3];
            //m_owner.UpdateBezierCurve();
        }
        /// <summary>
        /// 后撤
        /// </summary>
		public void Undo()
        {
            m_owner.P1 = m_originalPoints[0];
            m_owner.P2 = m_originalPoints[1];
            m_owner.P3 = m_originalPoints[2];
            m_owner.P4 = m_originalPoints[3];
            //m_owner.UpdateBezierCurve();
            //SetPoint(m_pointId, m_originalPoint, m_owner);
        }
        /// <summary>
        /// 设置端点
        /// </summary>
        /// <param name="pointid"></param>
        /// <param name="point"></param>
        /// <param name="line"></param>
		protected void SetPoint(ePoint pointid, UnitPoint point, BezierCurve bezierCurve)
        {
            if (pointid == ePoint.P1)
            {
                bezierCurve.P1 = point;
                return;
            }
            if (pointid == ePoint.P2)
            {
                bezierCurve.P2 = point;
                return;
            }
            if (pointid == ePoint.P3)
            {
                bezierCurve.P3 = point;
                return;
            }
            if (pointid == ePoint.P4)
            {
                bezierCurve.P4 = point;
                return;
            }
        }

        /// <summary>
        /// 设置点的位置
        /// </summary>
        /// <param name="pos"></param>
		public void SetPosition(UnitPoint pos)
        {
            //if (Control.ModifierKeys == Keys.Control)//如果按下ctrl，则以45度角找邻点
            //    pos = HitUtil.OrthoPointD(OtherPoint(m_pointId), pos, 45);
            //if (m_angleLocked || Control.ModifierKeys == (Keys)(Keys.Control | Keys.Shift))//如果角度被锁定且按下crtl或shifr则设定为点到直线最近距离直线上的点？？
            //    pos = HitUtil.NearestPointOnLine(m_owner.P1, m_owner.P2, pos, true);
            //SetPoint(m_pointId, pos, m_clone);//设置线上的点（p1,p2）的信息。
        }
    }

    class BezierCurve : DrawObjectBase, IDrawObject, ISerialize
    {
        protected UnitPoint m_p1=UnitPoint.Empty, m_p2 = UnitPoint.Empty, m_p3 = UnitPoint.Empty, m_p4 = UnitPoint.Empty, m_curPoint = UnitPoint.Empty,t = UnitPoint.Empty,m_point=UnitPoint.Empty;
        public BezierCurve(UnitPoint p1, UnitPoint p2, UnitPoint p3, UnitPoint p4, float width, Color color)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            Width = width;
            Color = color;
            Selected = true;
        }

        public BezierCurve()
        {
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
        [XmlSerializable]
        public UnitPoint P3
        {
            get { return m_p3; }
            set { m_p3 = value; }
        }
        [XmlSerializable]
        public UnitPoint P4
        {
            get { return m_p4; }
            set { m_p4 = value; }
        }
        /// <summary>
        /// 用途未知*
        /// </summary>
        public UnitPoint RepeatStartingPoint
        {
            get { return m_p1; }
        }


        public virtual string Id
        {
            get { return ObjectType; }
        }

        public static string ObjectType//获取类型
        {
            get { return "bezierCurve"; }
        }

        /// <summary>
        /// 从模型中初始化(点状)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="layer"></param>
        /// <param name="snap"></param>
        public override void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap)
        {
            P1 = P2 = P3 = P4 =m_curPoint= point;
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
            //Console.WriteLine(""+point.X+" "+point.Y);
        }

        void ISerialize.AfterSerializedIn()
        {
        }

        public virtual IDrawObject Clone()
        {
            BezierCurve b = new BezierCurve();
            b.Copy(this);
            return b;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateBezierCurve()
        {
            Console.WriteLine("UpdateBezierCurve()");
        }

        /// <summary>
        /// 绘制图形
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
        public virtual void Draw(ICanvas canvas, RectangleF unitrect)
        {
            if (m_curPoint.Equals(m_p1)) { canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); return; }
            if (m_curPoint.Equals(m_p2)) { canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p2, m_p3); return; }
            if (m_curPoint.Equals(m_p3)) { canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p2, m_p3); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p3, m_p4); return; }
            //if (m_curPoint.Equals(m_p4)) { canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p2, m_p3); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p3, m_p4); }
            Color color = Color;
            Pen pen = canvas.CreatePen(color, Width);
            canvas.DrawBezier(canvas, pen, m_p1, m_p2, m_p3, m_p4);
            if (Highlighted)//如果高亮
            {
                canvas.DrawBezier(canvas, DrawUtils.SelectedPen, m_p1, m_p2, m_p3, m_p4);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p2, m_p3); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p3, m_p4);
            }

            if (Selected)   //如果被选中，包括画图期间
            {
                canvas.DrawBezier(canvas, DrawUtils.SelectedPen, m_p1, m_p2, m_p3, m_p4);//画线
                //canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p1, m_p2); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p2, m_p3); canvas.DrawLine(canvas, DrawUtils.SelectedPen, m_p3, m_p4);
                if (m_p1.IsEmpty == false)//如果端点p1不存在
                    DrawUtils.DrawNode(canvas, m_p1);//画点
                if (m_p2.IsEmpty == false)//如果端点p2不存在
                    DrawUtils.DrawNode(canvas, m_p2);//画点
                if (m_p3.IsEmpty == false)//如果端点p3不存在
                    DrawUtils.DrawNode(canvas, m_p3);//画点
                if (m_p4.IsEmpty == false)//如果端点p4不存在
                    DrawUtils.DrawNode(canvas, m_p4);//画点
                //MessageBox.Show("");
            }

        }

        /// <summary>
        /// 获取包裹图形的矩形区域，用于清除画图轨迹
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public RectangleF GetBoundingRect(ICanvas canvas)
        {
            t.X = m_point.X>t.X?m_point.X:t.X;
            t.Y = m_point.Y > t.Y ? m_point.Y : t.Y;
            float r = Convert.ToSingle(Math.Sqrt((Math.Pow((m_curPoint.X - t.X), 2.0) + Math.Pow((m_curPoint.Y - t.Y), 2.0)) / 2.0));
            //Console.WriteLine(r+" ");
            RectangleF rect = HitUtil.CircleBoundingRect(new UnitPoint((m_curPoint.X + Math.Abs(t.X)) / 2.0, (m_curPoint.Y + Math.Abs(t.Y)) / 2.0), 2*r);
            // if drawing either angle then include the mouse point in the ractangle - this is to redraw (erase) the line drawn
            // from center point to mouse point
            //Console.WriteLine(r+" "+t.X+" "+t.Y);
            return rect;

            //float thWidth = ThresholdWidth(canvas, Width);
            //return ScreenUtils.GetRect(m_p1, m_p2, thWidth);
        }

        /// <summary>
        /// 获取线段端点坐标，长度和与x轴的角度
        /// </summary>
        /// <returns></returns>
        string IDrawObject.GetInfoAsString()
        {
            return string.Format("bezierCurve@{0},{1} - L={2:f4}<{3:f4}",
                P1.PosAsString(),
                P4.PosAsString(),
                HitUtil.Distance(P1, P4),
                HitUtil.RadiansToDegrees(HitUtil.LineAngleR(P1, P4, 0)));
        }

        /// <summary>
        /// 写入xml元素
        /// </summary>
        /// <param name="wr"></param>
        public void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement("bezierCurve");///元素名
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }

        /// <summary>
        /// 移动曲线
        /// </summary>
        /// <param name="offset"></param>
        void IDrawObject.Move(UnitPoint offset)
        {
            m_p1.X += offset.X;
            m_p1.Y += offset.Y;
            m_p2.X += offset.X;
            m_p2.Y += offset.Y;
            m_p3.X += offset.X;
            m_p3.Y += offset.Y;
            m_p4.X += offset.X;
            m_p4.Y += offset.Y;
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
        /// 作用不清晰*
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
		public INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            float thWidth = ThresholdWidth(canvas, Width);
            if (HitUtil.CircleHitPoint(m_p1, thWidth, point))
            {
                return new BezierCurveTool(this, BezierCurveTool.ePoint.P1);
            }       
            if (HitUtil.CircleHitPoint(m_p2, thWidth, point))
                return new BezierCurveTool(this, BezierCurveTool.ePoint.P2);
            if (HitUtil.CircleHitPoint(m_p3, thWidth, point))
                return new BezierCurveTool(this, BezierCurveTool.ePoint.P3);
            if (HitUtil.CircleHitPoint(m_p4, thWidth, point))
                return new BezierCurveTool(this, BezierCurveTool.ePoint.P4);
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
            double x1 = getMaxOrMin("max", m_p1.X, m_p2.X, m_p3.X, m_p4.X);
            double x2 = getMaxOrMin("min", m_p1.X, m_p2.X, m_p3.X, m_p4.X);
            double y1 = getMaxOrMin("max", m_p1.Y, m_p2.Y, m_p3.Y, m_p4.Y);
            double y2 = getMaxOrMin("min", m_p1.Y, m_p2.Y, m_p3.Y, m_p4.Y);
            RectangleF rectangleF = new RectangleF(Convert.ToSingle(x2), Convert.ToSingle(y1), Convert.ToSingle(x1 - x2), Convert.ToSingle(y1 - y2));

            if (rect.Contains(new PointF(Convert.ToSingle(x2), Convert.ToSingle(y1))) && rect.Contains(new PointF(Convert.ToSingle(x1), Convert.ToSingle(y2))))
                return true;
            else
                return false;
        }
        double getMaxOrMin(string flag, double t1, double t2, double t3, double t4)
        {
            double max = -1000;
            double min = 1000;
            if (t1 > t2)
            {
                max = t1;
                min = t2;
            }
            else
            {
                max = t2;
                min = t1;
            }
            if (max < t3) max = t3;
            if (max < t4) max = t4;
            if (min > t3) min = t3;
            if (min > t4) min = t4;
            if (flag.Equals("max")) return max;
            else return min;
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
            //OnMouseMove(canvas, point);
            //Console.WriteLine("OnMouseDown");
            //Console.WriteLine();
            if (m_curPoint.Equals(m_p1))
            {
                m_p2 = point;
                m_curPoint = m_p2;
                return eDrawObjectMouseDown.Continue;
            }
            if (m_curPoint.Equals(m_p2))
            {
                m_p3 = point;
                m_curPoint = m_p3;
                return eDrawObjectMouseDown.Continue;
            }
            if (m_curPoint.Equals(m_p3))
            {
                m_p4 = point;
                m_curPoint = m_p4;
                //return eDrawObjectMouseDown.Continue;
            }            
            
            Selected = false;
            //if (snappoint is PerpendicularSnapPoint && snappoint.Owner is BezierCurve)
            //{
            //    BezierCurve src = snappoint.Owner as BezierCurve;
            //    m_p4 = HitUtil.NearestPointOnLine(src.P1, src.P4, m_p1, true);
            //    return eDrawObjectMouseDown.Done;
            //}
            ////if (snappoint is PerpendicularSnapPoint && snappoint.Owner is Arc)
            ////{
            ////    Arc src = snappoint.Owner as Arc;
            ////    m_p2 = HitUtil.NearestPointOnCircle(src.Center, src.Radius, m_p1, 0);
            ////    return eDrawObjectMouseDown.DoneRepeat;
            ////}
            //if (Control.ModifierKeys == Keys.Control)
            //    point = HitUtil.OrthoPointD(m_p1, point, 45);
            //m_p4 = point;
            return eDrawObjectMouseDown.Done;
        }

        /// <summary>
        /// 鼠标移动，如果按下crtl则以45度角找邻点，设置p2为邻点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            m_point = point;
            if (m_curPoint.Equals(m_p1)) { m_p2 = point; }
            if (m_curPoint.Equals(m_p2)) { m_p3 = point; }
            if (m_curPoint.Equals(m_p3)) { m_p4 = point; }

        }

        void IDrawObject.OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        /// <summary>
        /// 判断鼠标焦点是否在图形内
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        bool IDrawObject.PointInObject(ICanvas canvas, UnitPoint point)
        {
            //MessageBox.Show("");
            double x1 = getMaxOrMin("max", m_p1.X, m_p2.X, m_p3.X, m_p4.X);
            double x2 = getMaxOrMin("min", m_p1.X, m_p2.X, m_p3.X, m_p4.X);
            double y1 = getMaxOrMin("max", m_p1.Y, m_p2.Y, m_p3.Y, m_p4.Y);
            double y2 = getMaxOrMin("min", m_p1.Y, m_p2.Y, m_p3.Y, m_p4.Y);
            if (point.X <= x1 && point.X >= x2)
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
                        if (HitUtil.CircleHitPoint(m_p3, thWidth, point))
                            return new VertextSnapPoint(canvas, this, m_p3);
                        if (HitUtil.CircleHitPoint(m_p4, thWidth, point))
                            return new VertextSnapPoint(canvas, this, m_p4);
                        return null;
                    }
                    if (snaptype == typeof(MidpointSnapPoint))
                    {
                        //UnitPoint p = MidPoint(canvas, m_p1, m_p2, point);
                        //if (p != UnitPoint.Empty)
                        //    return new MidpointSnapPoint(canvas, this, p);
                        return null;
                    }
                    if (snaptype == typeof(IntersectSnapPoint))
                    {
                        return null;

                    }
                }
            }
                return null;
        }
 
    }

    class BezierCurveEdit : BezierCurve, IObjectEditInstance
    {
        protected PerpendicularSnapPoint m_perSnap;
        protected TangentSnapPoint m_tanSnap;
        protected bool m_tanReverse = false;

        public BezierCurveEdit() { }

        public override string Id
        {
            get
            { return "bezierCurve"; }
        }

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
            //Console.WriteLine("public override eDrawObjectMouseDown OnMouseDown");
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
        /// 复制编辑
        /// </summary>
        /// <param name="acopy"></param>
		public new void Copy(BezierCurveEdit acopy)
        {
            base.Copy(acopy);
            m_perSnap = acopy.m_perSnap;
            m_tanSnap = acopy.m_tanSnap;
            m_tanReverse = acopy.m_tanReverse;
        }
        public override IDrawObject Clone()
        {
            BezierCurve b = new BezierCurve();
            b.Copy(this);
            return b;
        }
        public IDrawObject GetDrawObject()
        {
            return new BezierCurve(P1,P2,P3,P4,Width,Color);
        }
    }

}
