﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Canvas.DrawTools
{

    class NodePointCircleCenter : INodePoint
    {
        protected Circle3Point m_owner;
        protected Circle3Point m_clone;
        protected UnitPoint m_originalPoint;
        protected UnitPoint m_endPoint;
        public NodePointCircleCenter(Circle3Point owner)
        {
            m_owner = owner;
            m_clone = m_owner.Clone() as Circle3Point;
            //Console.WriteLine("!!" + m_clone.P1.X + " " + m_clone.P1.Y);

            m_originalPoint = m_owner.Center;
            //Console.WriteLine("!!" + m_clone.P1.X + " " + m_clone.P1.Y);
        }
        #region INodePoint Members
        public IDrawObject GetClone()
        {
            return m_clone;
        }
        public IDrawObject GetOriginal()
        {
            return m_owner;
        }
        public virtual void SetPosition(UnitPoint pos)
        {
            m_clone.Center = pos;
        }
        public virtual void Finish()
        {
            UnitPoint offset =new UnitPoint( m_clone.Center.X - m_owner.Center.X, m_clone.Center.Y - m_owner.Center.Y);
            m_endPoint = m_clone.Center;
            m_owner.Center = m_clone.Center;
            m_owner.Radius = m_clone.Radius;
            
            m_owner.P1 += offset;
            m_owner.P2 += offset;
            m_owner.P3 += offset;
            
            
            m_owner.Selected = true;
            m_clone = null;
        }
        public void Cancel()
        {
            m_owner.Selected = true;
        }
        public virtual void Undo()
        {
            UnitPoint offset = new UnitPoint(m_originalPoint.X - m_owner.Center.X, m_originalPoint.Y - m_owner.Center.Y);

            m_owner.P1 += offset;
            m_owner.P2 += offset;
            m_owner.P3 += offset;
            m_owner.Center = m_originalPoint;

            
        }
        public virtual void Redo()
        {
            UnitPoint offset = new UnitPoint(m_endPoint.X - m_owner.Center.X, m_endPoint.Y - m_owner.Center.Y);

            m_owner.P1 += offset;
            m_owner.P2 += offset;
            m_owner.P3 += offset;
            m_owner.Center = m_endPoint;
            
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
        #endregion
    }

    class NodePointCircleRadius : INodePoint
    {
        protected Circle3Point m_owner;
        protected Circle3Point m_clone;
        protected float m_originalValue;
        protected float m_endValue;
        float Angle1;
        float Angle2;
        float Angle3;

        public NodePointCircleRadius(Circle3Point owner)
        {
            m_owner = owner;
            m_clone = m_owner.Clone() as Circle3Point;
            m_clone.CurrentPoint = m_owner.CurrentPoint;
            m_originalValue = m_owner.Radius;
            Angle1 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center, m_owner.P1, 0));
            Angle2 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center, m_owner.P2, 0));
            Angle3 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center, m_owner.P3, 0));
        }
        #region INodePoint Members
        public IDrawObject GetClone()
        {
            return m_clone;
        }
        public IDrawObject GetOriginal()
        {
            return m_owner;
        }
        public virtual void SetPosition(UnitPoint pos)
        {
            m_clone.OnMouseMove(null, pos);
        }
        public virtual void Finish()
        {
            //float Angle1 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center,m_owner.P1, 0));
            //float Angle2 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center, m_owner.P2, 0));
            //float Angle3 = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_owner.Center, m_owner.P3, 0));
            //Console.WriteLine(Angle1);
            m_owner.P1 = HitUtil.PointOncircle(m_clone.Center, m_clone.Radius, HitUtil.DegressToRadians(Angle1));
            m_owner.P2 = HitUtil.PointOncircle(m_clone.Center, m_clone.Radius, HitUtil.DegressToRadians(Angle2));
            m_owner.P3 = HitUtil.PointOncircle(m_clone.Center, m_clone.Radius, HitUtil.DegressToRadians(Angle3));

            m_endValue = m_clone.Radius;
            m_owner.Radius = m_clone.Radius;
            m_owner.Selected = true;
            m_clone = null;
        }
        public UnitPoint AngleToPoint(float angle,UnitPoint point,float radius)
        {
            if(0<angle && angle<90)
            {
                point.X=Math.Cos(angle) * radius;
                point.Y = Math.Sin(angle) * radius;
                return point;
            }
            return UnitPoint.Empty;
        }
        public void Cancel()
        {
            m_owner.Selected = true;
        }
        public virtual void Undo()
        {
            m_owner.P1 = HitUtil.PointOncircle(m_owner.Center, m_originalValue, HitUtil.DegressToRadians(Angle1));
            m_owner.P2 = HitUtil.PointOncircle(m_owner.Center, m_originalValue, HitUtil.DegressToRadians(Angle2));
            m_owner.P3 = HitUtil.PointOncircle(m_owner.Center, m_originalValue, HitUtil.DegressToRadians(Angle3));
            m_owner.Radius = m_originalValue;
        }
        public virtual void Redo()
        {
            m_owner.P1 = HitUtil.PointOncircle(m_owner.Center, m_endValue, HitUtil.DegressToRadians(Angle1));
            m_owner.P2 = HitUtil.PointOncircle(m_owner.Center, m_endValue, HitUtil.DegressToRadians(Angle2));
            m_owner.P3 = HitUtil.PointOncircle(m_owner.Center, m_endValue, HitUtil.DegressToRadians(Angle3));
            m_owner.Radius = m_endValue;
        }
        public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }
        #endregion
    }

    class Circle3Point : DrawObjectBase, IArc, IDrawObject, ISerialize
    {

        //三点成圓中三个点的属性设置
        UnitPoint m_p1 = UnitPoint.Empty;
        UnitPoint m_p2 = UnitPoint.Empty;
        UnitPoint m_p3 = UnitPoint.Empty;
        //[XmlSerializable]
        public UnitPoint P1
        {
            get { return m_p1; }
            set { m_p1 = value; }
        }
        //[XmlSerializable]
        public UnitPoint P2
        {
            get { return m_p2; }
            set { m_p2 = value; }
        }
        //[XmlSerializable]
        public UnitPoint P3
        {
            get { return m_p3; }
            set { m_p3 = value; }
        }

        //圆弧各个属性设置
        UnitPoint m_center;
        float m_radius;
        float m_startAngle = 0;
        float m_endAngle = 0;
        Arc.eDirection m_direction = Arc.eDirection.kCCW;

        protected eCurrentPoint m_curPoint = eCurrentPoint.done;
        protected UnitPoint m_lastPoint = UnitPoint.Empty;

        protected static int ThresholdPixel = 6;
        public enum eCurrentPoint
        {
            p1,
            p2,
            p3,
            startangle,
            endangle,
            radius,
            done,
            center,
        }
        public eCurrentPoint CurrentPoint
        {
            get { return m_curPoint; }
            set { m_curPoint = value; }
        }

        [XmlSerializable]
        public UnitPoint Center
        {
            get { return m_center; }
            set { m_center = value; }
        }
        [XmlSerializable]
        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        [XmlSerializable]
        public float StartAngle
        {
            get { return m_startAngle; }
            set { m_startAngle = value; }
        }
        [XmlSerializable]
        public float EndAngle
        {
            get { return m_endAngle; }
            set { m_endAngle = value; }
        }
        [XmlSerializable]
        public Arc.eDirection Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        public static string ObjectType
        {
            get { return "circle"; }
        }
        public Circle3Point()
        {
            m_curPoint = eCurrentPoint.p1;
        }

        public virtual string Id
        {
            get { return ObjectType; }
        }

        public virtual UnitPoint RepeatStartingPoint
        {
            get { return UnitPoint.Empty; }
        }

        public override void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap)
        {
            Width = layer.Width;
            Color = layer.Color;
            Selected = true;
            OnMouseDown(null, point, snap);
        }

        public void Copy(Circle3Point acopy)
        {
            base.Copy(acopy);
            m_p1 = acopy.m_p1;
            m_p2 = acopy.m_p2;
            m_p3 = acopy.m_p3;
            m_center = acopy.m_center;
            m_radius = acopy.m_radius;
            m_curPoint = acopy.m_curPoint;
        }

        public void AfterSerializedIn()
        {
            UpdateCircleFrom3Points();
        }

        public virtual IDrawObject Clone()
        {
            Circle3Point a = new Circle3Point();
            a.Copy(this);
            return a;
        }

        /// <summary>
        /// 判斷是否三點一線
        /// </summary>
        /// <returns></returns>
		bool PointsInLine()
        {
            double slope1 = HitUtil.LineSlope(P1, P2);
            double slope2 = HitUtil.LineSlope(P1, P3);
            return slope1 == slope2;
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		public virtual void Draw(ICanvas canvas, RectangleF unitrect)
        {
              DrawCircle(canvas, unitrect);
        }

        void DrawCircle(ICanvas canvas, RectangleF unitrect)
        {
            Pen pen = canvas.CreatePen(Color, Width);
            bool inline = PointsInLine();
            //canvas.DrawArc(canvas, pen,new UnitPoint(0,0), 30, StartAngle, 180);
            if (inline == false)
                canvas.DrawArc(canvas, pen, m_center, m_radius, StartAngle, 360);
            else
            {
                canvas.DrawLine(canvas, pen, P1, P2);
                canvas.DrawLine(canvas, pen, P1, P3);
            }

            if (Selected)
            {
                if (inline == false)
                    canvas.DrawArc(canvas, DrawUtils.SelectedPen, m_center, m_radius, StartAngle, 360);
                else
                {
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P2);
                    canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P3);
                }
                if (m_p1.IsEmpty == false)
                {
                    DrawUtils.DrawNode(canvas, P1);
                }
                if (m_p2.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P2);
                if (m_p3.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, P3);
                if (m_center.IsEmpty == false)
                    DrawUtils.DrawNode(canvas, Center);
               // DrawUtils.DrawNode(canvas, AnglePoint(0));
                //DrawUtils.DrawNode(canvas, AnglePoint(90));
                //DrawUtils.DrawNode(canvas, AnglePoint(180));
                //DrawUtils.DrawNode(canvas, AnglePoint(270));
            }
        }

        /// <summary>
        /// 获取用户图像选择周围的边界矩形的坐标
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public RectangleF GetBoundingRect(ICanvas canvas)
        {
            float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (thWidth < Width)
                thWidth = Width;

            RectangleF rect = RectangleF.Empty;

            //如果其中一个点是空的，那么绘制一条直线，因此返回一条直线的边界。
            if (m_p2.IsEmpty || m_p3.IsEmpty)
                rect = ScreenUtils.GetRect(m_p1, m_lastPoint, thWidth);

            if (rect.IsEmpty)
            {
                float r = m_radius + thWidth / 2;
                rect = HitUtil.CircleBoundingRect(m_center, r);
                if (Selected)
                {
                    float w = (float)canvas.ToUnit(20); // include space for the 'extern' nodes
                    rect.Inflate(w, w);
                }
            }
            //如果绘制任一角度，则在矩形中包含鼠标点-这是重新绘制（擦除）绘制的线。
            //从中心点到鼠标点
            //if (m_curPoint == eCurrentPoint.startAngle || m_curPoint == eCurrentPoint.endAngle)
            if (m_lastPoint.IsEmpty == false)
                rect = RectangleF.Union(rect, new RectangleF(m_lastPoint.Point, new SizeF(0, 0)));
            return rect;
        }

        public virtual string GetInfoAsString()
        {
            return string.Format("Circle@{0}, r={1:f4}", Center.PosAsString(), Radius);
        }

        public virtual void GetObjectData(XmlWriter wr)
        {
            wr.WriteStartElement(Id);
            XmlUtil.WriteProperties(this, wr);
            wr.WriteEndElement();
        }

        public virtual void Move(UnitPoint offset)
        {
            P1 += offset;
            P2 += offset;
            P3 += offset;
            UpdateCircleFrom3Points();
            m_lastPoint = m_center;
        }

        public virtual INodePoint NodePoint(ICanvas canvas, UnitPoint point)
        {
            //Console.WriteLine(m_p1.X+" "+m_p1.Y);
            float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (HitUtil.PointInPoint(Center, point, thWidth))//圓心位移
                return new NodePointCircleCenter(this);
            bool radiushit = HitUtil.PointInPoint(m_p1, point, thWidth);
            if (radiushit == false)
                radiushit = HitUtil.PointInPoint(m_p2, point, thWidth);
            if (radiushit == false)
                radiushit = HitUtil.PointInPoint(m_p3, point, thWidth);
            if (radiushit)
            {
                m_curPoint = eCurrentPoint.radius;
                m_lastPoint = Center;
                return new NodePointCircleRadius(this);
            }
            return null;

            //float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            //if (HitUtil.PointInPoint(P1, point, thWidth))
            //{
            //    m_lastPoint = P1;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.p1);
            //}
            //if (HitUtil.PointInPoint(P2, point, thWidth))
            //{
            //    m_lastPoint = P2;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.p2);
            //}
            //if (HitUtil.PointInPoint(P3, point, thWidth))
            //{
            //    m_lastPoint = P3;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.p3);
            //}
            //UnitPoint p = StartAngleNodePoint(canvas);
            //if (HitUtil.PointInPoint(p, point, thWidth))
            //{
            //    m_lastPoint = p;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.startangle);
            //}
            //p = EndAngleNodePoint(canvas);
            //if (HitUtil.PointInPoint(p, point, thWidth))
            //{
            //    m_lastPoint = p;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.endangle);
            //}
            //p = RadiusNodePoint(canvas);
            //if (HitUtil.PointInPoint(p, point, thWidth))
            //{
            //    m_lastPoint = p;
            //    return new NodePointArc3PointPoint(this, eCurrentPoint.radius);
            //}
            return null;
        }

        public virtual bool ObjectInRectangle(ICanvas canvas, RectangleF rect, bool anyPoint)
        {
            float r = m_radius + Width / 2;
            RectangleF boundingrect = HitUtil.CircleBoundingRect(m_center, r);
            if (anyPoint)
            {
                UnitPoint lp1 = new UnitPoint(rect.Left, rect.Top);
                UnitPoint lp2 = new UnitPoint(rect.Left, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(m_center, m_radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Bottom);
                lp2 = new UnitPoint(rect.Right, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(m_center, m_radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Top);
                if (HitUtil.CircleIntersectWithLine(m_center, m_radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Left, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Top);
                if (HitUtil.CircleIntersectWithLine(m_center, m_radius, lp1, lp2))
                    return true;
                lp1 = new UnitPoint(rect.Right, rect.Top);
                lp2 = new UnitPoint(rect.Right, rect.Bottom);
                if (HitUtil.CircleIntersectWithLine(m_center, m_radius, lp1, lp2))
                    return true;
            }
            return rect.Contains(boundingrect);
        }

        void IDrawObject.OnKeyDown(ICanvas canvas, KeyEventArgs e)
        {
        }

        /// <summary>
        /// 监听鼠标按下事件
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
		public virtual eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
            OnMouseMove(canvas, point);
            if (m_curPoint == eCurrentPoint.p1)
            {
                m_curPoint = eCurrentPoint.p2;
                return eDrawObjectMouseDown.Continue;
            }
            if (m_curPoint == eCurrentPoint.p2)
            {
                m_curPoint = eCurrentPoint.p3;
                return eDrawObjectMouseDown.Continue;
            }
            if (m_curPoint == eCurrentPoint.p3)
            {
                m_curPoint = eCurrentPoint.done;
                Selected = false;
                //return eDrawObjectMouseDown.Done;
            }
            return eDrawObjectMouseDown.Done;
        }
        public void UpdateCircleFrom3Points()
        {
            m_center = HitUtil.CenterPointFrom3Points(m_p1, m_p2, m_p3);
            m_radius = (float)HitUtil.Distance(m_center, m_p1);
        }

        public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
        {
            m_lastPoint = point;
            if (m_curPoint == eCurrentPoint.p1)
            {
                m_p1 = point;
                return;
            }
            if (m_curPoint == eCurrentPoint.p2)
            {
                m_p2 = point;
                return;
            }
            if (m_curPoint == eCurrentPoint.p3)
            {
                m_p3 = point;
                UpdateCircleFrom3Points();
                return;
            }
            if (m_curPoint == eCurrentPoint.center)
            {
                m_center = point;
            }
            if (m_curPoint == eCurrentPoint.radius)
            {
                //StartAngle = 0;
                //EndAngle = 360;
                m_radius = (float)HitUtil.Distance(m_center, point);
            }
        }

        public virtual void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
        {
        }

        /// <summary>
        /// 判断指定点是否在绘图对象中
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool PointInObject(ICanvas canvas, UnitPoint point)
        {
            RectangleF boundingrect = GetBoundingRect(canvas);
            if (boundingrect.Contains(point.Point) == false)
                return false;
            float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (HitUtil.PointInPoint(m_center, point, thWidth))
                return true;
            return HitUtil.IsPointInCircle(m_center, m_radius, point, thWidth / 2);
        }

        protected UnitPoint AnglePoint(float angle)
        {
            return HitUtil.PointOncircle(m_center, m_radius, HitUtil.DegressToRadians(angle));
        }

        public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
        {

            float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            if (runningsnaptypes != null)
            {
                foreach (Type snaptype in runningsnaptypes)
                {
                    if (snaptype == typeof(QuadrantSnapPoint))
                    {
                        UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
                        if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
                            return new QuadrantSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(DivisionSnapPoint))
                    {
                        double angle = 360 / 8;
                        UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, angle);
                        if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
                            return new DivisionSnapPoint(canvas, this, p);
                    }
                    if (snaptype == typeof(CenterSnapPoint))
                    {
                        if (HitUtil.PointInPoint(m_center, point, thWidth))
                            return new CenterSnapPoint(canvas, this, m_center);
                    }
                    if (snaptype == typeof(VertextSnapPoint))
                    {
                        if (HitUtil.PointInPoint(HitUtil.PointOncircle(m_center,m_radius,0), point, thWidth))
                            return new VertextSnapPoint(canvas, this, HitUtil.PointOncircle(m_center, m_radius, 0));
                        if (HitUtil.PointInPoint(HitUtil.PointOncircle(m_center, m_radius, 360), point, thWidth))
                            return new VertextSnapPoint(canvas, this, HitUtil.PointOncircle(m_center, m_radius, 360));
                    }
                }
                return null;
            }
            if (usersnaptype == typeof(NearestSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 0);
                if (p != UnitPoint.Empty)
                    return new NearestSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(PerpendicularSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 0);
                if (p != UnitPoint.Empty)
                    return new PerpendicularSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(QuadrantSnapPoint))
            {
                UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
                if (p != UnitPoint.Empty)
                    return new QuadrantSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(DivisionSnapPoint))
            {
                double angle = 360 / 8;
                UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, angle);
                if (p != UnitPoint.Empty)
                    return new DivisionSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(TangentSnapPoint))
            {
                IDrawObject drawingObject = canvas.CurrentObject;
                UnitPoint p = UnitPoint.Empty;
                if (drawingObject is LineEdit)
                {
                    UnitPoint mousepoint = point;
                    point = ((LineEdit)drawingObject).P1;
                    UnitPoint p1 = HitUtil.TangentPointOnCircle(m_center, m_radius, point, false);
                    UnitPoint p2 = HitUtil.TangentPointOnCircle(m_center, m_radius, point, true);
                    double d1 = HitUtil.Distance(mousepoint, p1);
                    double d2 = HitUtil.Distance(mousepoint, p2);
                    if (d1 <= d2)
                        return new TangentSnapPoint(canvas, this, p1);
                    else
                        return new TangentSnapPoint(canvas, this, p2);
                }
                //if (p != PointF.Empty)
                return new TangentSnapPoint(canvas, this, p);
            }
            if (usersnaptype == typeof(CenterSnapPoint))
            {
                return new CenterSnapPoint(canvas, this, m_center);
            }
            return null;


            //       float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
            //       if (runningsnaptypes != null)
            //       {
            //           foreach (Type snaptype in runningsnaptypes)
            //           {
            //               /*
            //if (snaptype == typeof(QuadrantSnapPoint))
            //{
            //	UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
            //	if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
            //		return new QuadrantSnapPoint(canvas, this, p);
            //}
            //if (snaptype == typeof(CenterSnapPoint))
            //{
            //	if (HitUtil.PointInPoint(m_center, point, thWidth))
            //		return new CenterSnapPoint(canvas, this, m_center);
            //}
            // * */
            //               if (snaptype == typeof(VertextSnapPoint))
            //               {
            //                   if (HitUtil.PointInPoint(P1, point, thWidth))
            //                       return new VertextSnapPoint(canvas, this, P1);
            //                   if (HitUtil.PointInPoint(P3, point, thWidth))
            //                       return new VertextSnapPoint(canvas, this, P3);
            //               }
            //           }
            //           return null;
            //       }
            //       if (usersnaptype == typeof(NearestSnapPoint))
            //       {
            //           UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 0);
            //           if (p != UnitPoint.Empty)
            //               return new NearestSnapPoint(canvas, this, p);
            //       }
            //       if (usersnaptype == typeof(PerpendicularSnapPoint))
            //       {
            //           UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 0);
            //           if (p != UnitPoint.Empty)
            //               return new PerpendicularSnapPoint(canvas, this, p);
            //       }
            //       if (usersnaptype == typeof(QuadrantSnapPoint))
            //       {
            //           UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
            //           if (p != UnitPoint.Empty)
            //               return new QuadrantSnapPoint(canvas, this, p);
            //       }
            //       if (usersnaptype == typeof(TangentSnapPoint))
            //       {
            //           IDrawObject drawingObject = canvas.CurrentObject;
            //           UnitPoint p = UnitPoint.Empty;
            //           if (drawingObject is LineEdit)
            //           {
            //               UnitPoint mousepoint = point;
            //               point = ((LineEdit)drawingObject).P1;
            //               UnitPoint p1 = HitUtil.TangentPointOnCircle(m_center, m_radius, point, false);
            //               UnitPoint p2 = HitUtil.TangentPointOnCircle(m_center, m_radius, point, true);
            //               double d1 = HitUtil.Distance(mousepoint, p1);
            //               double d2 = HitUtil.Distance(mousepoint, p2);
            //               if (d1 <= d2)
            //                   return new TangentSnapPoint(canvas, this, p1);
            //               else
            //                   return new TangentSnapPoint(canvas, this, p2);
            //           }
            //           //if (p != PointF.Empty)
            //           return new TangentSnapPoint(canvas, this, p);
            //       }
            //       if (usersnaptype == typeof(CenterSnapPoint))
            //       {
            //           return new CenterSnapPoint(canvas, this, m_center);
            //       }
            //       return null;
        }
    }
}