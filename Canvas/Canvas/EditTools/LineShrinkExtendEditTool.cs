using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canvas.EditTools
{
	class LineShrinkExtendEditTool : IEditTool
	{
		IEditToolOwner m_owner;
        /// <summary>
        /// 线条收缩或延长工具
        /// </summary>
        /// <param name="owner"></param>
		public LineShrinkExtendEditTool(IEditToolOwner owner)
		{
			m_owner = owner;
			SetHint("Select line to extend");//选择要延伸的线条
		}
        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="text">提示信息</param>
		void SetHint(string text)
		{
			if (m_owner != null)
				if (text.Length > 0)
					m_owner.SetHint("Extend Lines: " + text);
				else
					m_owner.SetHint("");
		}
        /// <summary>
        /// 复制工具
        /// </summary>
        /// <returns></returns>
		public IEditTool Clone()
		{
			LineShrinkExtendEditTool t = new LineShrinkExtendEditTool(m_owner);
			// nothing that needs to be cloned
			return t;
		}
		Dictionary<DrawTools.Line, LinePoints> m_originalLines = new Dictionary<DrawTools.Line, LinePoints>();  //储存最开始的线条
		Dictionary<DrawTools.Line, LinePoints> m_modifiedLines = new Dictionary<DrawTools.Line, LinePoints>();  //储存修改后的线条

		public void OnMouseMove(ICanvas canvas, UnitPoint point)
		{
		}

        /// <summary>
        /// 指示是否可以选中
        /// </summary>
		public bool SupportSelection
		{
			get { return true; }
		}

        /// <summary>
        /// 清空所有线条
        /// </summary>
		void ClearAll()
		{
			foreach (LinePoints p in m_originalLines.Values)
				p.Line.Highlighted = false;
			m_originalLines.Clear();
		}

        /// <summary>
        /// 添加线条
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
		void AddLine(UnitPoint point, DrawTools.Line line)
		{
			if (m_originalLines.ContainsKey(line) == false)
			{
				line.Highlighted = true;
				LinePoints lp = new LinePoints();
				lp.SetLine(line);
				lp.MousePoint = point;
				m_originalLines.Add(line, lp);
			}
		}
        
        /// <summary>
        /// 移除线条
        /// </summary>
        /// <param name="line"></param>
		void RemoveLine(DrawTools.Line line)
		{
			if (m_originalLines.ContainsKey(line))
			{
				m_originalLines[line].Line.Highlighted = false;
				m_originalLines.Remove(line);
			}
		}

        /// <summary>
        /// 设置命中对象？？
        /// </summary>
        /// <param name="point"></param>
        /// <param name="list"></param>
		public void SetHitObjects(UnitPoint point, List<IDrawObject> list)
		{
            //当从选择矩形中选择Obj/Objt时调用
            //如果列表是空的，什么也不做
            //如果没有shift或ctrl，则用列表中的项替换选择
            //如果shift，则追加
            //如果ctrl然后切换
            if (list == null)
				return;
			List<DrawTools.Line> lines = GetLines(list);
			if (lines.Count == 0)
				return;

			bool shift = Control.ModifierKeys == Keys.Shift;
			bool ctrl = Control.ModifierKeys == Keys.Control;

			if (shift == false && ctrl == false)
				ClearAll();

			if (ctrl == false) // append all lines, either no-key or shift
			{
				foreach (DrawTools.Line line in lines)
					AddLine(point, line);
			}
			if (ctrl)
			{
				foreach (DrawTools.Line line in lines)
				{
					if (m_originalLines.ContainsKey(line))
						RemoveLine(line);
					else
						AddLine(point, line);
				}
			}
			SetSelectHint();
		}

        /// <summary>
        /// 设置选中时的提示
        /// </summary>
		void SetSelectHint()
		{
			if (m_originalLines.Count == 0)
				SetHint("Select line to extend");
			else
				SetHint("Select Line to extend line(s) to, or [Ctrl+click] to extend more lines");
		}

        /// <summary>
        /// 获取线条
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
		List<DrawTools.Line> GetLines(List<IDrawObject> objs)
		{
			List<DrawTools.Line> lines = new List<Canvas.DrawTools.Line>();
			foreach (IDrawObject obj in objs)
			{
				if (obj is DrawTools.Line)
					lines.Add((DrawTools.Line)obj);
			}
			return lines;
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
			List<IDrawObject> drawitems = canvas.DataModel.GetHitObjects(canvas, point);
			List<DrawTools.Line> lines = GetLines(drawitems);
			
			// add to source lines
			if (m_originalLines.Count == 0 || Control.ModifierKeys == Keys.Shift)
			{
				foreach (DrawTools.Line line in lines)
					AddLine(point, line);
				SetSelectHint();
				return eDrawObjectMouseDown.Continue;
			}
			if (m_originalLines.Count == 0 || Control.ModifierKeys == Keys.Control)
			{
				foreach (DrawTools.Line line in lines)
				{
					if (m_originalLines.ContainsKey(line))
						RemoveLine(line);
					else
						AddLine(point, line);
				}
				SetSelectHint();
				return eDrawObjectMouseDown.Continue;
			}

			if (drawitems.Count == 0)
				return eDrawObjectMouseDown.Continue;

            //所有的线条已经被添加，现在找到边延伸到哪里

            if (drawitems[0] is DrawTools.Line)
			{
				DrawTools.Line edge = (DrawTools.Line)drawitems[0];
				bool modified = false;
				foreach (LinePoints originalLp in m_originalLines.Values)
				{
					UnitPoint intersectpoint = HitUtil.LinesIntersectPoint(edge.P1, edge.P2, originalLp.Line.P1, originalLp.Line.P2);
					// 线条相交就收缩线
					if (intersectpoint != UnitPoint.Empty)
					{
						LinePoints lp = new LinePoints();
						lp.SetLine(originalLp.Line);
						lp.MousePoint = originalLp.MousePoint;
						m_modifiedLines.Add(lp.Line, lp);
						lp.SetNewPoints(lp.Line, lp.MousePoint, intersectpoint);
						modified = true;
						continue;
					}
                    //线条不相交就在已有边线上寻找明显的交点
                    if (intersectpoint == UnitPoint.Empty)
					{
						UnitPoint apprarentISPoint = HitUtil.FindApparentIntersectPoint(
							edge.P1,
							edge.P2,
							originalLp.Line.P1,
							originalLp.Line.P2,
							false,
							true);
						if (apprarentISPoint == UnitPoint.Empty)
							continue;

						modified = true;
						originalLp.Line.ExtendLineToPoint(apprarentISPoint);

						LinePoints lp = new LinePoints();
						lp.SetLine(originalLp.Line);
						lp.MousePoint = point;
						m_modifiedLines.Add(lp.Line, lp);
					}
				}
				if (modified)
					canvas.DataModel.AfterEditObjects(this);
				return eDrawObjectMouseDown.Done;
			}
			if (drawitems[0] is DrawTools.Arc)
			{
				DrawTools.Arc edge = (DrawTools.Arc)drawitems[0];
				foreach (LinePoints originalLp in m_originalLines.Values)
				{
				}
				bool modified = false;
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
            foreach (LinePoints originalLp in m_originalLines.Values)
                originalLp.Line.Highlighted = false;
        }

        /// <summary>
        /// 撤销
        /// </summary>
		public void Undo()
		{
			foreach (LinePoints lp in m_originalLines.Values)
				lp.ResetLine();
		}

        /// <summary>
        /// 重做
        /// </summary>
		public void Redo()
		{
			foreach (LinePoints lp in m_modifiedLines.Values)
				lp.ResetLine();
		}
	}
}