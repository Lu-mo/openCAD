using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace Canvas.DrawTools
{
	public class DrawUtils
	{
		static Pen m_selectedPen = null;
		static public Pen SelectedPen//选择画笔
		{
			get
			{
				if (m_selectedPen == null)
				{
					m_selectedPen = new Pen(Color.Magenta, 1);
					m_selectedPen.DashStyle = DashStyle.Dash;
				}
				return m_selectedPen;
			}
		}
        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="nodepoint"></param>
		static public void DrawNode(ICanvas canvas, UnitPoint nodepoint)
		{
			RectangleF r = new RectangleF(canvas.ToScreen(nodepoint), new SizeF(0, 0));
			r.Inflate(3, 3);
			if (r.Right < 0 || r.Left > canvas.ClientRectangle.Width)
				return;
			if (r.Top < 0 || r.Bottom > canvas.ClientRectangle.Height)
				return;
			canvas.Graphics.FillRectangle(Brushes.White, r);
			r.Inflate(1, 1);
			canvas.Graphics.DrawRectangle(Pens.Black, ScreenUtils.ConvertRect(r));
		}
        /// <summary>
        /// 画三角点？？（3点绘制圆弧中引用）
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="nodepoint"></param>
		static public void DrawTriangleNode(ICanvas canvas, UnitPoint nodepoint)
		{
			PointF screenpoint = canvas.ToScreen(nodepoint);
			float size = 4;
			PointF[] p = new PointF[] 
			{
				new PointF(screenpoint.X - size, screenpoint.Y),
				new PointF(screenpoint.X, screenpoint.Y + size),
				new PointF(screenpoint.X + size, screenpoint.Y),
				new PointF(screenpoint.X, screenpoint.Y - size),
			};
			canvas.Graphics.FillPolygon(Brushes.White, p);
		}
	}


	interface IObjectEditInstance
	{
		IDrawObject GetDrawObject();
	}
	abstract class DrawObjectBase
	{
		float			m_width;
		Color			m_color;
		DrawingLayer	m_layer;

		enum eFlags
		{
			selected		= 0x00000001,
			highlighted		= 0x00000002,
			useLayerWidth	= 0x00000004,
			useLayerColor	= 0x00000008,
		}
		int m_flag = (int)(eFlags.useLayerWidth | eFlags.useLayerColor);
		bool GetFlag(eFlags flag)
		{
			return ((int)m_flag & (int)flag) > 0;
		}
		void SetFlag(eFlags flag, bool enable)
		{
			if (enable)
				m_flag |= (int)flag;
			else
				m_flag &= ~(int)flag;
		}

		[XmlSerializable]
		public bool UseLayerWidth
		{
			get { return GetFlag(eFlags.useLayerWidth); }
			set { SetFlag(eFlags.useLayerWidth, value); }
		}
		[XmlSerializable]
		public bool UseLayerColor
		{
			get { return GetFlag(eFlags.useLayerColor); }
			set { SetFlag(eFlags.useLayerColor, value); }
		}
		[XmlSerializable]
		public float Width
		{
			set { m_width = value; }
			get 
			{ 
				if (Layer != null && UseLayerWidth)
					return Layer.Width;
				return m_width; 
			}
		}
		[XmlSerializable]
		public Color Color
		{
			set { m_color = value; }
			get 
			{ 
				if (Layer != null && UseLayerColor)
					return Layer.Color;
				return m_color; 
			}
		}
		public DrawingLayer Layer
		{
			get { return m_layer; }
			set { m_layer = value; }
		}

		abstract public void InitializeFromModel(UnitPoint point, DrawingLayer layer, ISnapPoint snap);//从数据集合中初始化
		public virtual bool Selected
		{
			get { return GetFlag(eFlags.selected); }
			set { SetFlag(eFlags.selected, value); }
		}
		public virtual bool Highlighted
		{
			get { return GetFlag(eFlags.highlighted); }
			set { SetFlag(eFlags.highlighted, value); }
		}
        /// <summary>
        /// 线段复制
        /// </summary>
        /// <param name="acopy"></param>
		public virtual void Copy(DrawObjectBase acopy)
		{
			UseLayerColor = acopy.UseLayerColor;
			UseLayerWidth = acopy.UseLayerWidth;
			Width = acopy.Width;
			Color = acopy.Color;
		}

	}
}
