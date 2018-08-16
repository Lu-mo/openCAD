using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Canvas
{
	public interface ICanvasOwner
	{
		void SetPositionInfo(UnitPoint unitpos);
		void SetSnapInfo(ISnapPoint snap);
	}
	public interface ICanvas
	{
		IModel DataModel { get; }
		UnitPoint ScreenTopLeftToUnitPoint();
		UnitPoint ScreenBottomRightToUnitPoint();
		PointF ToScreen(UnitPoint unitpoint);
		float ToScreen(double unitvalue);
		double ToUnit(float screenvalue);
		UnitPoint ToUnit(PointF screenpoint);

		void Invalidate();
		IDrawObject CurrentObject { get; }

		Rectangle ClientRectangle { get; }
		Graphics Graphics { get; }
		Pen CreatePen(Color color, float unitWidth);
		void DrawLine(ICanvas canvas, Pen pen, UnitPoint p1, UnitPoint p2);
		void DrawArc(ICanvas canvas, Pen pen, UnitPoint center, float radius, float beginangle, float angle);
        void DrawRectangle(ICanvas canvas, Pen pen, UnitPoint m_p1, UnitPoint m_p2);
    }
	public interface IModel
	{
		float Zoom { get; set; }
		ICanvasLayer BackgroundLayer { get; }
		ICanvasLayer GridLayer { get; }
		ICanvasLayer[] Layers { get; }
		ICanvasLayer ActiveLayer { get; set; }
		ICanvasLayer GetLayer(string id);
		IDrawObject CreateObject(string type, UnitPoint point, ISnapPoint snappoint);
		void AddObject(ICanvasLayer layer, IDrawObject drawobject);
		void DeleteObjects(IEnumerable<IDrawObject> objects);
		void MoveObjects(UnitPoint offset, IEnumerable<IDrawObject> objects);
		void CopyObjects(UnitPoint offset, IEnumerable<IDrawObject> objects);
		void MoveNodes(UnitPoint position, IEnumerable<INodePoint> nodes);

		IEditTool GetEditTool(string id);
		void AfterEditObjects(IEditTool edittool);

		List<IDrawObject> GetHitObjects(ICanvas canvas, RectangleF selection, bool anyPoint);
		List<IDrawObject> GetHitObjects(ICanvas canvas, UnitPoint point);
		bool IsSelected(IDrawObject drawobject);
		void AddSelectedObject(IDrawObject drawobject);
		void RemoveSelectedObject(IDrawObject drawobject);
		IEnumerable<IDrawObject> SelectedObjects { get; }
		int SelectedCount { get; }
		void ClearSelectedObjects();

		ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, Type[] runningsnaptypes, Type usersnaptype);

		bool CanUndo();
		bool DoUndo();
		bool CanRedo();
		bool DoRedo();
	}
	public interface ICanvasLayer
	{
		string Id { get; }
		void Draw(ICanvas canvas, RectangleF unitrect);
        void Draw5x5(ICanvas canvas, RectangleF unitrect);
        ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj);
        ISnapPoint SnapPoint5x5(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj);
        IEnumerable<IDrawObject> Objects { get; }
		bool Enabled { get; set; }
		bool Visible { get; }
	}
	public interface ISnapPoint
	{
		IDrawObject	Owner			{ get; }
		UnitPoint	SnapPoint		{ get; }
		RectangleF	BoundingRect	{ get; }
		void Draw(ICanvas canvas);
	}
	public enum eDrawObjectMouseDown
	{
		Done,       // this draw object is complete这个绘图对象是完整的
        DoneRepeat, // this object is complete, but create new object of same type此对象已完成，但创建相同类型的新对象
        Continue,   // this object requires additional mouse inputs此对象需要额外的鼠标输入。
    }
	public interface INodePoint
	{
		IDrawObject GetClone();
		IDrawObject GetOriginal();
		void Cancel();
		void Finish();
		void SetPosition(UnitPoint pos);
		void Undo();
		void Redo();
		void OnKeyDown(ICanvas canvas, KeyEventArgs e);
	}
	public interface IDrawObject
	{
		string Id { get; }
		IDrawObject Clone();
		bool PointInObject(ICanvas canvas, UnitPoint point);
		bool ObjectInRectangle(ICanvas canvas, RectangleF rect, bool anyPoint);
		void Draw(ICanvas canvas, RectangleF unitrect);
		RectangleF GetBoundingRect(ICanvas canvas);
		void OnMouseMove(ICanvas canvas, UnitPoint point);
		eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
		void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
		void OnKeyDown(ICanvas canvas, KeyEventArgs e);
		UnitPoint RepeatStartingPoint { get; }
		INodePoint NodePoint(ICanvas canvas, UnitPoint point);
		ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, List<IDrawObject> otherobj, Type[] runningsnaptypes, Type usersnaptype);
		void Move(UnitPoint offset);

		string GetInfoAsString();
	}
	public interface IEditTool
	{
		IEditTool Clone();

		bool SupportSelection { get; }
		void SetHitObjects(UnitPoint mousepoint, List<IDrawObject> list);

		void OnMouseMove(ICanvas canvas, UnitPoint point);
		eDrawObjectMouseDown OnMouseDown(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
		void OnMouseUp(ICanvas canvas, UnitPoint point, ISnapPoint snappoint);
		void OnKeyDown(ICanvas canvas, KeyEventArgs e);
		void Finished();
		void Undo();
		void Redo();
	}
	public interface IEditToolOwner
	{
		void SetHint(string text);
	}
}
