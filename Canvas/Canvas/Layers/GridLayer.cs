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
		public SizeF Spacing//����(������)
		{
			get { return m_spacing; }
			set { m_spacing = value; }
		}
		[XmlSerializable]
		public int MinSize//��С�ߴ�
		{
			get { return m_minSize; }
			set { m_minSize = value; }
		}
		[XmlSerializable]
		public eStyle GridStyle//�������ͣ���
		{
			get { return m_gridStyle; }
			set { m_gridStyle = value; }
		}
		[XmlSerializable]
		public Color Color//��ɫ
		{
			get { return m_color; }
			set { m_color = value; }
		}
        /// <summary>
        /// �����ƣ�
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
        /// ��������
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
			if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)//������С�ߴ��򲻻�������
				return;

			PointF leftpoint = unitrect.Location;
			PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

			float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
			float top = unitrect.Height + unitrect.Y;
			float right = rightpoint.X;
			float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;

			if (GridStyle == eStyle.Dots)//������������ǵ㣿��
			{
				GDI gdi = new GDI();
				gdi.BeginGDI(canvas.Graphics);
				for (float x = left; x <= right; x += gridX)
				{
					for (float y = bottom; y <= top; y += gridY)
					{
						PointF p1 = canvas.ToScreen(new UnitPoint(x, y));
						gdi.SetPixel((int)p1.X, (int)p1.Y, m_color.ToArgb());//��������
					}
				}
				gdi.EndGDI();
			}
			if (GridStyle == eStyle.Lines)//�������
			{
				Pen pen = new Pen(m_color);
                Pen pen1 = new Pen(Color.Blue, 2);//ÿ10��������10����������ɫ��ϸ
                Pen pen2 = new Pen(Color.Yellow, 2);//�߽�����
				GraphicsPath path = new GraphicsPath();
                // draw vertical lines������
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

                // draw horizontal lines����ˮƽ��
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
            if (gridscreensizeX < MinSize || gridscreensizeY < MinSize)//������С�ߴ��򲻻�������
                return;

            PointF leftpoint = unitrect.Location;
            PointF rightpoint = ScreenUtils.RightPoint(canvas, unitrect);

            float left = (float)Math.Round(leftpoint.X / gridX) * gridX;
            float top = unitrect.Height + unitrect.Y;
            float right = rightpoint.X;
            float bottom = (float)Math.Round(leftpoint.Y / gridY) * gridY;

            if (GridStyle == eStyle.Dots)//������������ǵ㣿��
            {
                GDI gdi = new GDI();
                gdi.BeginGDI(canvas.Graphics);
                for (float x = left; x <= right; x += gridX)
                {
                    for (float y = bottom; y <= top; y += gridY)
                    {
                        PointF p1 = canvas.ToScreen(new UnitPoint(x, y));
                        gdi.SetPixel((int)p1.X, (int)p1.Y, m_color.ToArgb());//��������
                    }
                }
                gdi.EndGDI();
            }
            if (GridStyle == eStyle.Lines)//�������
            {
                Pen pen = new Pen(m_color);
                GraphicsPath path = new GraphicsPath();
                // draw vertical lines������
                while (left < right)
                {
                    PointF p1 = canvas.ToScreen(new UnitPoint(left, leftpoint.Y));
                    PointF p2 = canvas.ToScreen(new UnitPoint(left, rightpoint.Y));
                    path.AddLine(p1, p2);
                    path.CloseFigure();
                    left += gridX;
                }

                // draw horizontal lines����ˮƽ��
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
        /// ��׽�����
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
        //��ȡObjects��������������Ϣ
        public IEnumerable<IDrawObject> Objects
		{
			get { return null; }
		}
		[XmlSerializable]
		public bool Enabled//�Ƿ��ܱ༭��
        {
			get { return m_enabled; }
			set { m_enabled = value; }
		}
		public bool Visible//�Ƿ�ɼ�
        {
			get { return true; }
		}
		#endregion
		#region ISerialize
        /// <summary>
        /// ��ȡ��Ϣ
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
