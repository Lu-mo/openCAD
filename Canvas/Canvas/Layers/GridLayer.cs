using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace Canvas
{
	public class GridLayer : ICanvasLayer, ISerialize
	{
		public enum eStyle
		{
			Dots,
			Lines,
		}
		public SizeF m_spacing = new SizeF(1f, 1f); // 12"
		private bool m_enabled = true;
		private int m_minSize = 15;
		private eStyle m_gridStyle = eStyle.Lines;
		private Color m_color = Color.FromArgb(50, Color.Gray);
		[XmlSerializable]
		public SizeF Spacing//矩形(画布？)
		{
			get { return m_spacing; }
			set { m_spacing = value; }
		}
		[XmlSerializable]
		public int MinSize//最小尺寸
		{
			get { return m_minSize; }
			set { m_minSize = value; }
		}
		[XmlSerializable]
		public eStyle GridStyle//网格类型？？
		{
			get { return m_gridStyle; }
			set { m_gridStyle = value; }
		}
		[XmlSerializable]
		public Color Color//颜色
		{
			get { return m_color; }
			set { m_color = value; }
		}
        /// <summary>
        /// 网格复制？
        /// </summary>
        /// <param name="acopy"></param>
		public void Copy(GridLayer acopy)
		{
			m_enabled = acopy.m_enabled;
			m_spacing = acopy.m_spacing;
			m_minSize = acopy.m_minSize;
			m_gridStyle = acopy.m_gridStyle;
			m_color = acopy.m_color;
		}
		#region ICanvasLayer Members
        /// <summary>
        /// 绘制网格？
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		public void Draw(ICanvas canvas, RectangleF unitrect)
		{
			if (Enabled == false)
				return;
            float gridX = Spacing.Width;
            float gridY = Spacing.Height;
            float gridscreensizeX = canvas.ToScreen(gridX);
			float gridscreensizeY = canvas.ToScreen(gridY);
			if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)//超过最小尺寸则不绘制网格
				return;

			PointF leftpoint = unitrect.Location;
			PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

			float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
			float top = unitrect.Height + unitrect.Y;
			float right = rightpoint.X;
			float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;

			if (GridStyle == eStyle.Dots)//如果网格类型是点？？
			{
				GDI gdi = new GDI();
				gdi.BeginGDI(canvas.Graphics);
				for (float x = left; x <= right; x += gridX)
				{
					for (float y = bottom; y <= top; y += gridY)
					{
						PointF p1 = canvas.ToScreen(new UnitPoint(x, y));
						gdi.SetPixel((int)p1.X, (int)p1.Y, m_color.ToArgb());//设置像素
					}
				}
				gdi.EndGDI();
			}
			if (GridStyle == eStyle.Lines)//如果是线
			{
				Pen pen = new Pen(m_color);
                Pen pen1 = new Pen(Color.Blue, 2);//每10根线条第10根线条得颜色粗细
                Pen pen2 = new Pen(Color.Yellow, 2);//边界线条
				GraphicsPath path = new GraphicsPath();
                // draw vertical lines画垂线
                while (left < right)
				{
                    PointF p1 = canvas.ToScreen(new UnitPoint(left, leftpoint.Y));
					PointF p2 = canvas.ToScreen(new UnitPoint(left, rightpoint.Y));
                    if (left % 50 == 0&&left!=0) { canvas.Graphics.DrawLine(pen2, p1, p2); }
                    else if(left % 10 == 0) { canvas.Graphics.DrawLine(pen1, p1, p2); }
                    else
                    {
                        path.AddLine(p1, p2);
                        path.CloseFigure();
                    }
                    left += gridX;
                }

                // draw horizontal lines绘制水平线
                while (bottom < top)
				{
                    PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X, bottom));
					PointF p2 = canvas.ToScreen(new UnitPoint(rightpoint.X, bottom));
                    if (bottom%50==0 && bottom!=0) { canvas.Graphics.DrawLine(pen2, p1, p2); }
                    else if (bottom % 10 == 0) { canvas.Graphics.DrawLine(pen1, p1, p2); }
                    else
                    {
                        path.AddLine(p1, p2);
                        path.CloseFigure();
                    }
                    bottom += gridY;
                }
                canvas.Graphics.DrawPath(pen, path);
			}
		}
        public void Draw5x5(ICanvas canvas, RectangleF unitrect)
        {
            if (Enabled == false)
                return;
            float gridX = Spacing.Width/5;
            float gridY = Spacing.Height/5;
            float gridscreensizeX = canvas.ToScreen(gridX);
            float gridscreensizeY = canvas.ToScreen(gridY);
            if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)//超过最小尺寸则不绘制网格
                return;

            PointF leftpoint = unitrect.Location;
            PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

            float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
            float top = unitrect.Height + unitrect.Y;
            float right = rightpoint.X;
            float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;

            if (GridStyle == eStyle.Dots)//如果网格类型是点？？
            {
                GDI gdi = new GDI();
                gdi.BeginGDI(canvas.Graphics);
                for (float x = left; x <= right; x += gridX)
                {
                    for (float y = bottom; y <= top; y += gridY)
                    {
                        PointF p1 = canvas.ToScreen(new UnitPoint(x, y));
                        gdi.SetPixel((int)p1.X, (int)p1.Y, m_color.ToArgb());//设置像素
                    }
                }
                gdi.EndGDI();
            }
            if (GridStyle == eStyle.Lines)//如果是线
            {
                Pen pen = new Pen(m_color);
                GraphicsPath path = new GraphicsPath();
                // draw vertical lines画垂线
                while (left < right)
                {
                    PointF p1 = canvas.ToScreen(new UnitPoint(left, leftpoint.Y));
                    PointF p2 = canvas.ToScreen(new UnitPoint(left, rightpoint.Y));
                    path.AddLine(p1, p2);
                    path.CloseFigure();
                    left += gridX;
                }

                // draw horizontal lines绘制水平线
                while (bottom < top)
                {
                    PointF p1 = canvas.ToScreen(new UnitPoint(leftpoint.X, bottom));
                    PointF p2 = canvas.ToScreen(new UnitPoint(rightpoint.X, bottom));
                    path.AddLine(p1, p2);
                    path.CloseFigure();
                    bottom += gridY;
                }
                canvas.Graphics.DrawPath(pen, path);
            }
        }
        public string Id
		{
			get { return "grid"; }
		}
        /// <summary>
        /// 捕捉网格点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="otherobj"></param>
        /// <returns></returns>
		public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
		{
            
			if (Enabled == false)
				return null;
			UnitPoint snappoint = new UnitPoint();
			UnitPoint mousepoint = point;
			float gridX = Spacing.Width;
			float gridY = Spacing.Height;
			snappoint.X = (float)(Math.Round(mousepoint.X / gridX)) * gridX;
			snappoint.Y = (float)(Math.Round(mousepoint.Y / gridY)) * gridY;
			double threshold = canvas.ToUnit(/*ThresholdPixel*/6);
			if ((snappoint.X < point.X - threshold) || (snappoint.X > point.X + threshold))
				return null;
			if ((snappoint.Y < point.Y - threshold) || (snappoint.Y > point.Y + threshold))
				return null;
			return new GridSnapPoint(canvas, snappoint);
		}
        public ISnapPoint SnapPoint5x5(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {

            if (Enabled == false)
                return null;
            UnitPoint snappoint = new UnitPoint();
            UnitPoint mousepoint = point;
            float gridX = Spacing.Width/5;
            float gridY = Spacing.Height/5;
            snappoint.X = (float)(Math.Round(mousepoint.X / gridX)) * gridX;
            snappoint.Y = (float)(Math.Round(mousepoint.Y / gridY)) * gridY;
            double threshold = canvas.ToUnit(/*ThresholdPixel*/6);
            if ((snappoint.X < point.X - threshold) || (snappoint.X > point.X + threshold))
                return null;
            if ((snappoint.Y < point.Y - threshold) || (snappoint.Y > point.Y + threshold))
                return null;
            return new GridSnapPoint(canvas, snappoint);
        }
        //获取Objects（线条？）的信息
        public IEnumerable<IDrawObject> Objects
		{
			get { return null; }
		}
		[XmlSerializable]
		public bool Enabled//是否能编辑？
        {
			get { return m_enabled; }
			set { m_enabled = value; }
		}
		public bool Visible//是否可见
        {
			get { return true; }
		}
		#endregion
		#region ISerialize
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="wr"></param>
		public void GetObjectData(XmlWriter wr)
		{
			wr.WriteStartElement("gridlayer");
			XmlUtil.WriteProperties(this, wr);
			wr.WriteEndElement();
		}
		public void AfterSerializedIn()
		{
		}
		#endregion
	}
}
