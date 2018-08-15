using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Canvas.DrawTools
{
    /// <summary>
    /// 意义暂时不明。。。。
    /// </summary>
	class NodePointArc3PointPoint : INodePoint
	{
		protected Arc3Point m_owner;
		protected Arc3Point m_clone;
		protected UnitPoint[] m_originalPoints = new UnitPoint[3];
		protected UnitPoint[] m_endPoints = new UnitPoint[3];
		protected Arc3Point.eCurrentPoint m_curPoint;
		public NodePointArc3PointPoint(Arc3Point owner, Arc3Point.eCurrentPoint curpoint)
		{
			m_owner = owner;
			m_clone = m_owner.Clone() as Arc3Point;
			m_owner.Selected = false;
			m_clone.Selected = true;
			m_originalPoints[0] = m_owner.P1;
			m_originalPoints[1] = m_owner.P2;
			m_originalPoints[2] = m_owner.P3;
			m_curPoint = curpoint;
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
			SetPoint(m_clone, pos);
		}
		/*
		UnitPoint GetPoint()
		{
			if (m_curPoint == Arc3Point.eCurrentPoint.p1)
				return m_clone.P1;
			if (m_curPoint == Arc3Point.eCurrentPoint.p2)
				return m_clone.P2;
			if (m_curPoint == Arc3Point.eCurrentPoint.p3)
				return m_clone.P3;
			if (m_curPoint == Arc3Point.eCurrentPoint.startangle)
				return m_clone.P1;
			if (m_curPoint == Arc3Point.eCurrentPoint.endangle)
				return m_clone.P3;
			if (m_curPoint == Arc3Point.eCurrentPoint.radius)
				return m_clone.P2;
			return UnitPoint.Empty;
		}
		 * */
		void SetPoint(Arc3Point arc, UnitPoint pos)
		{
			if (m_curPoint == Arc3Point.eCurrentPoint.p1)
				arc.P1 = pos;
			if (m_curPoint == Arc3Point.eCurrentPoint.p2)
				arc.P2 = pos;
			if (m_curPoint == Arc3Point.eCurrentPoint.p3)
				arc.P3 = pos;

			double angleToRound = 0;
			if (Control.ModifierKeys == Keys.Control)
				angleToRound = HitUtil.DegressToRadians(45);
			double angleR = HitUtil.LineAngleR(arc.Center, pos, angleToRound);

			if (m_curPoint == Arc3Point.eCurrentPoint.startangle)
				arc.P1 = HitUtil.PointOncircle(arc.Center, arc.Radius, angleR);
			if (m_curPoint == Arc3Point.eCurrentPoint.endangle)
				arc.P3 = HitUtil.PointOncircle(arc.Center, arc.Radius, angleR);
			if (m_curPoint == Arc3Point.eCurrentPoint.radius)
			{
				double radius = HitUtil.Distance(arc.Center, pos);
				arc.P1 = HitUtil.PointOncircle(arc.Center, radius, HitUtil.DegressToRadians(arc.StartAngle));
				arc.P2 = pos;
				arc.P3 = HitUtil.PointOncircle(arc.Center, radius, HitUtil.DegressToRadians(arc.EndAngle));
			}
			
			arc.UpdateArcFrom3Points();

			if ((m_curPoint == Arc3Point.eCurrentPoint.startangle) || (m_curPoint == Arc3Point.eCurrentPoint.endangle))
				arc.UpdateCenterNodeFromAngles();
		}
		public virtual void Finish()
		{
			m_endPoints[0] = m_clone.P1;
			m_endPoints[1] = m_clone.P2;
			m_endPoints[2] = m_clone.P3;
			m_owner.Copy(m_clone);
			m_owner.Selected = true;
			m_clone = null;
		}
		public void Cancel()
		{
			m_owner.Selected = true;
		}
		public virtual void Undo()
		{
			m_owner.P1 = m_originalPoints[0];
			m_owner.P2 = m_originalPoints[1];
			m_owner.P3 = m_originalPoints[2];
			m_owner.UpdateArcFrom3Points();
		}
		public virtual void Redo()
		{
			m_owner.P1 = m_endPoints[0];
			m_owner.P2 = m_endPoints[1];
			m_owner.P3 = m_endPoints[2];
			m_owner.UpdateArcFrom3Points();
		}
		public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
		{
		}
		#endregion
	}

    /// <summary>
    /// 三点成弧？？
    /// </summary>
	class Arc3Point : DrawObjectBase, IArc, IDrawObject, ISerialize
	{
        /// <summary>
        /// 圆弧类型
        /// </summary>
		public enum eArcType
		{
			kArc3P132,  //三个节点根据132的顺序绘制圆弧
			kArc3P123,  //三个节点根据123的顺序绘制圆弧
        }	
		eArcType m_type = eArcType.kArc3P132;   //默认类型为132

        /// <summary>
        /// 圆弧方向
        /// </summary>
		public enum eDirection
		{
            /// <summary>
            /// 顺时针
            /// </summary>
			kCW,     
            /// <summary>
            /// 逆时针
            /// </summary>
			kCCW,
		}
        
        //三点成弧中三个点的属性设置
		UnitPoint m_p1 = UnitPoint.Empty;
		UnitPoint m_p2 = UnitPoint.Empty;
		UnitPoint m_p3 = UnitPoint.Empty;
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
		
        //圆弧各个属性设置
		UnitPoint m_center;
		float m_radius;
		float m_startAngle = 0;
		float m_endAngle = 0;
		//[XmlSerializable]
		public UnitPoint Center
		{
			get { return m_center; }
			set { m_center = value; }
		}
		//[XmlSerializable]
		public float Radius
		{
			get { return m_radius; }
			set { m_radius = value; }
		}
		//[XmlSerializable]
		public float StartAngle
		{
			get { return m_startAngle; }
			set { m_startAngle = value; }
		}
		//[XmlSerializable]
		public float EndAngle
		{
			get { return m_endAngle; }
			set { m_endAngle = value; }
		}
		eDirection m_direction = eDirection.kCCW;
		[XmlSerializable]
		public eDirection Direction
		{
			get { return m_direction; }
			set { m_direction = value; }
		}

		public static string ObjectType
		{
			get { return "arc3p"; }
		}

		public Arc3Point()
		{
		}

		public Arc3Point(eArcType type)
		{
			m_type = type;
			m_curPoint = eCurrentPoint.p1;
		}


        /// <summary>
        /// 根据Model进行初始化
        /// </summary>
        /// <param name="point"></param>
        /// <param name="layer"></param>
        /// <param name="snap"></param>
        public override void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap)
		{
			Width = layer.Width;
			Color = layer.Color;
			Selected = true;
			OnMouseDown(null, point, snap);
		}

        /// <summary>
        /// 拷贝or复制
        /// </summary>
        /// <param name="acopy"></param>
        public void Copy(Arc3Point acopy)
		{
			base.Copy(acopy);
			m_p1 = acopy.m_p1;
			m_p2 = acopy.m_p2;
			m_p3 = acopy.m_p3;
			m_center = acopy.m_center;
			m_radius = acopy.m_radius;
			m_startAngle = acopy.m_startAngle;
			m_endAngle = acopy.m_endAngle;
			m_direction = acopy.m_direction;
			m_curPoint = acopy.m_curPoint;
		}

        /// <summary>
        /// 扫描圆弧获取圆弧的角度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
		double GetSweep(double start, double end, eDirection direction)
		{
				double sweep = 360;
				if (start == end)
					return sweep;
				if (direction == eDirection.kCCW)
				{
					if (end >= start)
						sweep = end - start;
					else
						sweep = 360 - (start - end);
				}
				if (direction == eDirection.kCW)
				{
					if (end >= start)
						sweep = -(360 - (end - start));
					else
						sweep = -(start - end);
				}
				return sweep;
		}
		public float SweepAngle
		{
			get { return (float)GetSweep(StartAngle, EndAngle, Direction); }
		}

		#region IDrawObject Members
		public virtual string Id
		{
			get { return ObjectType; }
		}
		public virtual IDrawObject Clone()
		{
			Arc3Point a = new Arc3Point(m_type);
			a.Copy(this);
			return a;
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
				rect = RectangleF.Union(rect, new RectangleF(m_lastPoint.Point, new SizeF(0,0)));
			return rect;
		}

        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		void DrawArc(ICanvas canvas, RectangleF unitrect)
		{
			Pen pen = canvas.CreatePen(Color, Width);
			bool inline = PointsInLine();
			double sweep = GetSweep(StartAngle, EndAngle, Direction);

			if (inline == false)
				canvas.DrawArc(canvas, pen, m_center, m_radius, StartAngle, (float)sweep);
			else
			{
				canvas.DrawLine(canvas, pen, P1, P2);
				canvas.DrawLine(canvas, pen, P1, P3);
			}

			if (Selected)
			{
				if (inline == false)
					canvas.DrawArc(canvas, DrawUtils.SelectedPen, m_center, m_radius, StartAngle, (float)sweep);
				else
				{
					canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P2);
					canvas.DrawLine(canvas, DrawUtils.SelectedPen, P1, P3);
				}
				if (m_p1.IsEmpty == false)
				{
					DrawUtils.DrawNode(canvas, P1);
					UnitPoint anglepoint = StartAngleNodePoint(canvas);
					if (!anglepoint.IsEmpty)
						DrawUtils.DrawTriangleNode(canvas, anglepoint);
					anglepoint = EndAngleNodePoint(canvas);
					if (!anglepoint.IsEmpty)
						DrawUtils.DrawTriangleNode(canvas, anglepoint);
					anglepoint = RadiusNodePoint(canvas);
					if (!anglepoint.IsEmpty)
						DrawUtils.DrawTriangleNode(canvas, anglepoint);
				}
				if (m_p2.IsEmpty == false)
					DrawUtils.DrawNode(canvas, P2);
				if (m_p3.IsEmpty == false)
					DrawUtils.DrawNode(canvas, P3);
			}
		}

        /// <summary>
        /// 返回一个bool值指示点是否在线条中
        /// </summary>
        /// <returns></returns>
		bool PointsInLine()
		{
			double slope1 = HitUtil.LineSlope(P1, P2);
			double slope2 = HitUtil.LineSlope(P1, P3);
			return slope1 == slope2;
		}
        /// <summary>
        /// 根据132的顺序绘制圆弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		void Draw3P132(ICanvas canvas, RectangleF unitrect)
		{
			if (m_curPoint == eCurrentPoint.p3)
			{
				Pen pen = canvas.CreatePen(Color, Width);
				canvas.DrawLine(canvas, pen, m_p1, m_p3);
			}
			if (m_curPoint == eCurrentPoint.p2 || m_curPoint == eCurrentPoint.done)
			{
				DrawArc(canvas, unitrect);
			}
        }
        /// <summary>
        /// 根据123的顺序绘制圆弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		void Draw3P123(ICanvas canvas, RectangleF unitrect)
		{
			if (m_curPoint == eCurrentPoint.p2)
			{
				Pen pen = canvas.CreatePen(Color, Width);
				canvas.DrawLine(canvas, pen, m_p1, m_p2);
			}
			if (m_curPoint == eCurrentPoint.p3 || m_curPoint == eCurrentPoint.done)
			{
				DrawArc(canvas, unitrect);
			}
		}
        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		public virtual void Draw(ICanvas canvas, RectangleF unitrect)
		{
			if (m_type == eArcType.kArc3P132)
				Draw3P132(canvas, unitrect);
			if (m_type == eArcType.kArc3P123)
				Draw3P123(canvas, unitrect);
		}

        /// <summary>
        /// 根据角度来更新圆弧的中点
        /// </summary>
		public void UpdateCenterNodeFromAngles()
		{
			float angle = StartAngle + SweepAngle / 2;
			P2 = HitUtil.PointOncircle(m_center, m_radius, HitUtil.DegressToRadians(angle));
		}
        /// <summary>
        /// 根据三个点来更新圆弧
        /// </summary>
		public void UpdateArcFrom3Points()
		{
			m_center = HitUtil.CenterPointFrom3Points(m_p1, m_p2, m_p3);
			m_radius = (float)HitUtil.Distance(m_center, m_p1);
			StartAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_center, m_p1, 0));
			EndAngle = (float)HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_center, m_p3, 0));
			// find angle from P1 on line P1|P3 to P2. If this angle is 0-180 the direction is CCW else it is CW

			double p1p3angle = HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_p1, m_p3, 0));
			double p1p2angle = HitUtil.RadiansToDegrees(HitUtil.LineAngleR(m_p1, m_p2, 0));
			double diff = p1p3angle - p1p2angle;
            //我知道这个逻辑存在问题，在某些情况下，圆弧的绘制不跟随鼠标，
            //但是现在也会奏效。
			Direction = eDirection.kCCW;
			if (diff < 0 || diff > 180)
				Direction = eDirection.kCW;

			if (p1p3angle == 0)
			{
				if (diff < -180)
					Direction = eDirection.kCCW;
				else
					Direction = eDirection.kCW;
			}
			if (p1p3angle == 90)
			{
				if (diff < -180)
					Direction = eDirection.kCCW;
			}
		}
        /// <summary>
        /// 鼠标移动时根据132的顺序绘制圆弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		void MoveMouse3P132(ICanvas canvas, UnitPoint point)
		{
			if (m_curPoint == eCurrentPoint.p1)
			{
				m_p1 = point;
				return;
			}
			if (m_curPoint == eCurrentPoint.p3)
			{
				m_p3 = point;
				return;
			}
			if (m_curPoint == eCurrentPoint.p2)
			{
				m_p2 = point;
				UpdateArcFrom3Points();
				return;
			}
        }
        /// <summary>
        /// 鼠标移动时根据123的顺序绘制圆弧
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		void MoveMouse3P123(ICanvas canvas, UnitPoint point)
		{
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
				UpdateArcFrom3Points();
				return;
			}
		}
        /// <summary>
        /// 鼠标按下时指定为132的方式
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
		eDrawObjectMouseDown MouseDown3P132(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
		{
			OnMouseMove(canvas, point);
			if (m_curPoint == eCurrentPoint.p1)
			{
				m_curPoint = eCurrentPoint.p3;
				return eDrawObjectMouseDown.Continue;
			}
			if (m_curPoint == eCurrentPoint.p3)
			{
				m_curPoint = eCurrentPoint.p2;
				return eDrawObjectMouseDown.Continue;
			}
			if (m_curPoint == eCurrentPoint.p2)
			{
				m_curPoint = eCurrentPoint.done;
				Selected = false;
				return eDrawObjectMouseDown.Done;
			}
			return eDrawObjectMouseDown.Done;
        }
        /// <summary>
        /// 鼠标按下时指定为123的方式
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
		eDrawObjectMouseDown MouseDown3P123(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
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
				return eDrawObjectMouseDown.Done;
			}
			return eDrawObjectMouseDown.Done;
		}

		UnitPoint StartAngleNodePoint(ICanvas canvas)
		{
			double r = Radius + canvas.ToUnit(8);
			return HitUtil.PointOncircle(m_center, r, HitUtil.DegressToRadians(StartAngle));
		}
		UnitPoint EndAngleNodePoint(ICanvas canvas)
		{
			double r = Radius + canvas.ToUnit(8);
			return HitUtil.PointOncircle(m_center, r, HitUtil.DegressToRadians(EndAngle));
		}
		UnitPoint RadiusNodePoint(ICanvas canvas)
		{
			double r = Radius + canvas.ToUnit(8);
			float angle = StartAngle + SweepAngle / 2;
			return HitUtil.PointOncircle(m_center, r, HitUtil.DegressToRadians(angle));
		}
        /// <summary>
        /// 监听鼠标移动事件
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
		public virtual void OnMouseMove(ICanvas canvas, UnitPoint point)
		{
			m_lastPoint = point;
			if (m_type == eArcType.kArc3P132)
				MoveMouse3P132(canvas, point);
			if (m_type == eArcType.kArc3P123)
				MoveMouse3P123(canvas, point);
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
			if (m_type == eArcType.kArc3P132)
				return MouseDown3P132(canvas, point, snappoint);
			if (m_type == eArcType.kArc3P123)
				return MouseDown3P123(canvas, point, snappoint);
			return eDrawObjectMouseDown.Done;
		}
		public virtual void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
		{
		}
        /// <summary>
        /// 监听D键按下时的事件
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="e"></param>
		public virtual void OnKeyDown(ICanvas canvas, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.D)
			{
				if (Direction == eDirection.kCW)
					Direction = eDirection.kCCW;
				else
					Direction = eDirection.kCW;
				e.Handled = true;
			}
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
		public virtual UnitPoint RepeatStartingPoint
		{
			get { return UnitPoint.Empty; }
		}
		public virtual INodePoint NodePoint(ICanvas canvas, UnitPoint point)
		{
			float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
			if (HitUtil.PointInPoint(P1, point, thWidth))
			{
				m_lastPoint = P1;
				return new NodePointArc3PointPoint(this, eCurrentPoint.p1);
			}
			if (HitUtil.PointInPoint(P2, point, thWidth))
			{
				m_lastPoint = P2;
				return new NodePointArc3PointPoint(this, eCurrentPoint.p2);
			}
			if (HitUtil.PointInPoint(P3, point, thWidth))
			{
				m_lastPoint = P3;
				return new NodePointArc3PointPoint(this, eCurrentPoint.p3);
			}
			UnitPoint p = StartAngleNodePoint(canvas);
			if (HitUtil.PointInPoint(p, point, thWidth))
			{
				m_lastPoint = p;
				return new NodePointArc3PointPoint(this, eCurrentPoint.startangle);
			}
			p = EndAngleNodePoint(canvas);
			if (HitUtil.PointInPoint(p, point, thWidth))
			{
				m_lastPoint = p;
				return new NodePointArc3PointPoint(this, eCurrentPoint.endangle);
			}
			p = RadiusNodePoint(canvas);
			if (HitUtil.PointInPoint(p, point, thWidth))
			{
				m_lastPoint = p;
				return new NodePointArc3PointPoint(this, eCurrentPoint.radius);
			}
			return null;
		}
		public virtual ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype)
		{
			float thWidth = Line.ThresholdWidth(canvas, Width, ThresholdPixel);
			if (runningsnaptypes != null)
			{
				foreach (Type snaptype in runningsnaptypes)
				{
					/*
					if (snaptype == typeof(QuadrantSnapPoint))
					{
						UnitPoint p = HitUtil.NearestPointOnCircle(m_center, m_radius, point, 90);
						if (p != UnitPoint.Empty && HitUtil.PointInPoint(p, point, thWidth))
							return new QuadrantSnapPoint(canvas, this, p);
					}
					if (snaptype == typeof(CenterSnapPoint))
					{
						if (HitUtil.PointInPoint(m_center, point, thWidth))
							return new CenterSnapPoint(canvas, this, m_center);
					}
					 * */
					if (snaptype == typeof(VertextSnapPoint))
					{
						if (HitUtil.PointInPoint(P1, point, thWidth))
							return new VertextSnapPoint(canvas, this, P1);
						if (HitUtil.PointInPoint(P3, point, thWidth))
							return new VertextSnapPoint(canvas, this, P3);
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
		}
		public virtual void Move(UnitPoint offset)
		{
			P1 += offset;
			P2 += offset;
			P3 += offset;
			UpdateArcFrom3Points();
			m_lastPoint = m_center;
		}

        /// <summary>
        /// 获取圆弧的信息并转化成字符串类型
        /// </summary>
        /// <returns></returns>
        public virtual string GetInfoAsString()
		{
			return string.Format("Arc@{0}, r={1:f4}, A1={2:f4}, A2={3:f4}", Center.PosAsString(), Radius, StartAngle, EndAngle);
		}
		#endregion
		#region ISerialize
		public virtual void GetObjectData(XmlWriter wr)
		{
			wr.WriteStartElement(Id);
			XmlUtil.WriteProperties(this, wr);
			wr.WriteEndElement();
		}
        /// <summary>
        /// 序列化之后
        /// </summary>
		public void AfterSerializedIn()
		{
			UpdateArcFrom3Points();
		}
		#endregion

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
		}
		public eCurrentPoint CurrentPoint
		{
			get { return m_curPoint; }
			set { m_curPoint = value; }
		}
		protected eCurrentPoint m_curPoint = eCurrentPoint.done;
		protected UnitPoint m_lastPoint = UnitPoint.Empty;
	}
}
