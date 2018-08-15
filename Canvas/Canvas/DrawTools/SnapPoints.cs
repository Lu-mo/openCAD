using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Canvas
{
	class SnapPointBase : ISnapPoint
	{
		protected UnitPoint		m_snappoint;
		protected RectangleF	m_boundingRect;
		protected IDrawObject	m_owner;
		public IDrawObject Owner
		{
			get { return m_owner; }
		}
		public SnapPointBase(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)//构造函数
		{
			m_owner = owner;
			m_snappoint = snappoint;
			float size = (float)canvas.ToUnit(14);
			m_boundingRect.X = (float)(snappoint.X - size / 2);
			m_boundingRect.Y = (float)(snappoint.Y - size / 2);
			m_boundingRect.Width = size;
			m_boundingRect.Height = size;
		}
		#region ISnapPoint Members
		public virtual UnitPoint SnapPoint
		{
			get { return m_snappoint; }
		}
		public virtual RectangleF BoundingRect
		{
			get { return m_boundingRect; }
		}
		public virtual void Draw(ICanvas canvas)
		{
		}
		#endregion
        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pen"></param>
        /// <param name="fillBrush"></param>
		protected void DrawPoint(ICanvas canvas, Pen pen, Brush fillBrush)
		{
			Rectangle screenrect = ScreenUtils.ConvertRect(ScreenUtils.ToScreenNormalized(canvas, m_boundingRect));
			canvas.Graphics.DrawRectangle(pen, screenrect);
			screenrect.X++;
			screenrect.Y++;
			screenrect.Width--;
			screenrect.Height--;
			if (fillBrush != null)
				canvas.Graphics.FillRectangle(fillBrush, screenrect);
		}
	}  
    /// <summary>
    /// 网格点快照类型？
    /// </summary>
	class GridSnapPoint : SnapPointBase
	{
		public GridSnapPoint(ICanvas canvas, UnitPoint snappoint)//构造函数？
			: base(canvas, null, snappoint)
		{
		}
		#region ISnapPoint Members
        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="canvas"></param>
		public override void Draw(ICanvas canvas)//构造函数？
        {
			DrawPoint(canvas, Pens.Gray, null);
		}
		#endregion
	}
    /// <summary>
    /// 顶点快照类型？
    /// </summary>
    class VertextSnapPoint : SnapPointBase
	{
		public VertextSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)//构造函数？
            : base(canvas, owner, snappoint)
		{
		}
        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="canvas"></param>
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.Blue, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 中点快照类型？
    /// </summary>
	class MidpointSnapPoint : SnapPointBase//
	{
		public MidpointSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 交点快照类型？
    /// </summary>
    class IntersectSnapPoint : SnapPointBase
	{
		public IntersectSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 最近点快照类型？
    /// </summary>
    class NearestSnapPoint : SnapPointBase
	{
		public NearestSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		#region ISnapPoint Members
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
		#endregion
	}
    /// <summary>
    /// 四分点快照类型？
    /// </summary>
	class QuadrantSnapPoint : SnapPointBase
	{
		public QuadrantSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 分割点快照类型？
    /// </summary>
	class DivisionSnapPoint : SnapPointBase
	{
		public DivisionSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 中心点快照类型
    /// </summary>
	class CenterSnapPoint : SnapPointBase
	{
		public CenterSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 垂点快照类型？
    /// </summary>
	class PerpendicularSnapPoint : SnapPointBase
	{
		public PerpendicularSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
    /// <summary>
    /// 切点快照类型？
    /// </summary>
	class TangentSnapPoint : SnapPointBase
	{
		public TangentSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
			: base(canvas, owner, snappoint)
		{
		}
		public override void Draw(ICanvas canvas)
		{
			DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
		}
	}
}
