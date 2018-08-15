using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace Canvas
{
	public class BackgroundLayer : ICanvasLayer, ISerialize
	{
		Font m_font = new System.Drawing.Font("Arial Black", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		SolidBrush m_brush = new SolidBrush(Color.FromArgb(50, 200, 200, 200));
		SolidBrush m_backgroundBrush;

		Color m_color = Color.Black;
		[XmlSerializable]
        //设置或者获取颜色
		public Color Color
		{
			get { return m_color; }
			set
			{
				m_color = value;
				m_backgroundBrush = new SolidBrush(m_color);
			}
		}
        //构造函数
		public BackgroundLayer()
		{
			m_backgroundBrush = new SolidBrush(m_color);
		}

		#region ICanvasLayer Members
        /// <summary>
        /// 画布背景设置
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="unitrect"></param>
		public void Draw(ICanvas canvas, RectangleF unitrect)
		{
			RectangleF r = ScreenUtils.ToScreenNormalized(canvas, unitrect);
			canvas.Graphics.FillRectangle(m_backgroundBrush, r);
			StringFormat f = new StringFormat();
			f.Alignment = StringAlignment.Center;
			PointF centerpoint = new PointF(r.Width / 2, r.Height / 2);
			canvas.Graphics.TranslateTransform(centerpoint.X, centerpoint.Y);//中心点标记
			canvas.Graphics.RotateTransform(-15);//设置网格线角度？？
			canvas.Graphics.DrawString("Jesper Kristiansen (2007)", m_font, m_brush, 0, 0, f);
			canvas.Graphics.ResetTransform();//设置网格线恢复变化？？
        }
        public void Draw5x5(ICanvas canvas, RectangleF unitrect)
        {          
        }

        public PointF SnapPoint(PointF unitmousepoint)
		{
			return PointF.Empty;
		}
		public string Id
		{
			get { return "background"; }
		}
        /// <summary>
        /// 点的快照信息？（抛出异常）
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="otherobj"></param>
        /// <returns></returns>
		ISnapPoint ICanvasLayer.SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
		{
			throw new Exception("The method or operation is not implemented.");
		}
        ISnapPoint ICanvasLayer.SnapPoint5x5(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// 获取数据？？
        /// </summary>
		public IEnumerable<IDrawObject> Objects
		{
			get { return null; }
		}
        /// <summary>
        /// 能否做什么？？？
        /// </summary>
		public bool Enabled
		{
			get { return false; }
			set {;}
		}
        /// <summary>
        /// 是否可见
        /// </summary>
		public bool Visible
		{
			get { return true; }
			set { ;}
		}
		#endregion
		#region ISerialize
		public void GetObjectData(XmlWriter wr)
		{
			wr.WriteStartElement("backgroundlayer");
			XmlUtil.WriteProperties(this, wr);
			wr.WriteEndElement();
		}
		public void AfterSerializedIn()
		{
		}
		#endregion
	}
}
