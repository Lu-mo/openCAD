using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canvas.EditTools
{
	class LinePoints
	{
		DrawTools.Line	m_line;
		UnitPoint		m_p1;
		UnitPoint		m_p2;
		public UnitPoint MousePoint;
		public DrawTools.Line Line
		{
			get { return m_line; }
		}
        /// <summary>
        /// 设置线条
        /// </summary>
        /// <param name="l"></param>
		public void SetLine(DrawTools.Line l)
		{
			m_line = l;
			m_p1 = l.P1;
			m_p2 = l.P2;
		}
        /// <summary>
        /// 重置线条
        /// </summary>
		public void ResetLine()
		{
			m_line.P1 = m_p1;
			m_line.P2 = m_p2;
		}
        /// <summary>
        /// 设置新的点
        /// </summary>
        /// <param name="l"></param>
        /// <param name="hitpoint"></param>
        /// <param name="intersectpoint"></param>
		public void SetNewPoints(DrawTools.Line l, UnitPoint hitpoint, UnitPoint intersectpoint)
		{
			SetLine(l);
			double hitToVp1 = HitUtil.Distance(hitpoint, l.P1); //点击点到顶点的距离
			double ispToVp1 = HitUtil.Distance(intersectpoint, l.P1); //交点到顶点的距离
			// if hit is closer than intersect point, then keep this point and adjust the other
            //如果点击点很靠近交点，就保持这个点然后调整其他的点
			if (hitToVp1 <= ispToVp1)
				m_p2 = intersectpoint;
			else
				m_p1 = intersectpoint;
			ResetLine();
		}
	}
	class LinesMeetEditTool : IEditTool
	{
		IEditToolOwner m_owner;
        /// <summary>
        /// 线条相交编辑工具
        /// </summary>
        /// <param name="owner"></param>
		public LinesMeetEditTool(IEditToolOwner owner)
		{
			m_owner = owner;
			SetHint("Select first line");
		}
        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="text"></param>
		void SetHint(string text)
		{
			if (m_owner != null)
			{
				if (text.Length > 0)
					m_owner.SetHint("Lines Meet: " + text);
				else
					m_owner.SetHint("");
			}
		}
        /// <summary>
        /// 复制工具
        /// </summary>
        /// <returns></returns>
		public IEditTool Clone()
		{
			LinesMeetEditTool t = new LinesMeetEditTool(m_owner);
			// nothing that needs to be cloned
			return t;
		}
		LinePoints m_l1Original = new LinePoints();
		LinePoints m_l2Original = new LinePoints();
		LinePoints m_l1NewPoint = new LinePoints();
		LinePoints m_l2NewPoint = new LinePoints();

        /// <summary>
        /// 指示是否可以选中
        /// </summary>
        public bool SupportSelection 
		{ 
			get { return false; } 
		}
		public void SetHitObjects(UnitPoint point, List<IDrawObject> list)
		{
		}
		public void OnMouseMove(ICanvas canvas, UnitPoint point)
		{
		}
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
		public eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
		{
			List<IDrawObject> items = canvas.DataModel.GetHitObjects(canvas, point);
			DrawTools.Line line = null;
			//找到第一个线条
			foreach (IDrawObject item in items)
			{
				if (item is DrawTools.Line)
				{
					line = item as DrawTools.Line;
					if (line != m_l1Original.Line)
						break;
				}
			}
			if (line == null)
			{
				if (m_l1Original.Line == null)
					SetHint("No line selected. Select first line");
				else
					SetHint("No line selected. Select second line");
				return eDrawObjectMouseDown.Continue;
			}
			if (m_l1Original.Line == null)
			{
				line.Highlighted = true;
				m_l1Original.SetLine(line);
				m_l1Original.MousePoint = point;
				SetHint("Select second line");
				return eDrawObjectMouseDown.Continue;
			}
			if (m_l2Original.Line == null)
			{
				line.Highlighted = true;
				m_l2Original.SetLine(line);
				m_l2Original.MousePoint = point;

				UnitPoint intersectpoint = HitUtil.LinesIntersectPoint(
					m_l1Original.Line.P1, 
					m_l1Original.Line.P2, 
					m_l2Original.Line.P1, 
					m_l2Original.Line.P2);

                // 如果线条不相交，则将线条延伸至相交点。
                if (intersectpoint == UnitPoint.Empty)
				{
					UnitPoint apprarentISPoint = HitUtil.FindApparentIntersectPoint(m_l1Original.Line.P1, m_l1Original.Line.P2, m_l2Original.Line.P1, m_l2Original.Line.P2);
					if (apprarentISPoint == UnitPoint.Empty)
						return eDrawObjectMouseDown.Done;
					m_l1Original.Line.ExtendLineToPoint(apprarentISPoint);
					m_l2Original.Line.ExtendLineToPoint(apprarentISPoint);
					m_l1NewPoint.SetLine(m_l1Original.Line);
					m_l2NewPoint.SetLine(m_l2Original.Line);
					canvas.DataModel.AfterEditObjects(this);
					return eDrawObjectMouseDown.Done;
				}

				m_l1NewPoint.SetNewPoints(m_l1Original.Line, m_l1Original.MousePoint, intersectpoint);
				m_l2NewPoint.SetNewPoints(m_l2Original.Line, m_l2Original.MousePoint, intersectpoint);
				canvas.DataModel.AfterEditObjects(this);
				return eDrawObjectMouseDown.Done;
			}
			return eDrawObjectMouseDown.Done;
		}
		public void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint)
		{
		}
		public void OnKeyDown(ICanvas canvas, KeyEventArgs e)
		{
		}
		public void Finished()
		{
			SetHint("");
			if (m_l1Original.Line != null)
				m_l1Original.Line.Highlighted = false;
			if (m_l2Original.Line != null)
				m_l2Original.Line.Highlighted = false;
		}
        /// <summary>
        /// 撤销
        /// </summary>
		public void Undo()
		{
			m_l1Original.ResetLine();
			m_l2Original.ResetLine();
		}
        /// <summary>
        /// 重做
        /// </summary>
		public void Redo()
		{
			m_l1NewPoint.ResetLine();
			m_l2NewPoint.ResetLine();
		}
	}
}
