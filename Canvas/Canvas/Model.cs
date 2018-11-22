using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Canvas
{
	public class DataModel : IModel
	{
		static Dictionary<string, Type> m_toolTypes = new Dictionary<string,Type>();
        /// <summary>
        /// �½���ͼͼ��
        /// </summary>
        /// <param name="objecttype"></param>
        /// <returns></returns>
		static public IDrawObject NewDrawObject(string objecttype)
		{
			if (m_toolTypes.ContainsKey(objecttype))
			{
				string type = m_toolTypes[objecttype].ToString();
				return Assembly.GetExecutingAssembly().CreateInstance(type) as IDrawObject;
			}
			return null;
		}
		Dictionary<string, IDrawObject> m_drawObjectTypes = new Dictionary<string, IDrawObject>();
        /// <summary>
        /// �����������ݣ���
        /// </summary>
        /// <param name="objecttype"></param>
        /// <returns></returns>
        DrawTools.DrawObjectBase CreateObject(string objecttype)
		{
			if (m_drawObjectTypes.ContainsKey(objecttype))
			{
				return m_drawObjectTypes[objecttype].Clone() as DrawTools.DrawObjectBase;
			}
			return null;
		}
		Dictionary<string, IEditTool> m_editTools = new Dictionary<string, IEditTool>();
        /// <summary>
        /// ��ӱ༭����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tool"></param>
        public void AddEditTool(string key, IEditTool tool)
		{
			m_editTools.Add(key, tool);
		}
		public bool IsDirty
		{
			get { return m_undoBuffer.Dirty; }
		}
		UndoRedoBuffer m_undoBuffer = new UndoRedoBuffer();		
		UnitPoint m_centerPoint = UnitPoint.Empty;
		[XmlSerializable]
		public UnitPoint CenterPoint
		{
			get { return m_centerPoint; }
			set { m_centerPoint = value; }
		}

		float m_zoom = 1;
		GridLayer m_gridLayer = new GridLayer();
		BackgroundLayer m_backgroundLayer = new BackgroundLayer();
		List<ICanvasLayer> m_layers = new List<ICanvasLayer>();//����������Ϣ������
		ICanvasLayer m_activeLayer;
		Dictionary<IDrawObject, bool> m_selection = new Dictionary<IDrawObject, bool>();
		public DataModel()//���캯������ʼ����
		{
			m_toolTypes.Clear();
			m_toolTypes[DrawTools.Line.ObjectType] = typeof(DrawTools.Line);
			m_toolTypes[DrawTools.Circle.ObjectType] = typeof(DrawTools.Circle);
			m_toolTypes[DrawTools.Arc.ObjectType] = typeof(DrawTools.Arc);
			m_toolTypes[DrawTools.Arc3Point.ObjectType] = typeof(DrawTools.Arc3Point);
            m_toolTypes[DrawTools.Rectangle.ObjectType] = typeof(DrawTools.Rectangle);
            m_toolTypes[DrawTools.BezierCurve.ObjectType] = typeof(DrawTools.BezierCurve);
            m_toolTypes[DrawTools.Circle3Point.ObjectType] = typeof(DrawTools.Circle3Point);
            DefaultLayer();
			m_centerPoint = new UnitPoint(0,0);
            Console.WriteLine("DataModel()");
		}
        Color color;
        public DataModel(Color color)//���캯������ʼ����
        {
            m_toolTypes.Clear();
            m_toolTypes[DrawTools.Line.ObjectType] = typeof(DrawTools.Line);
            m_toolTypes[DrawTools.Circle.ObjectType] = typeof(DrawTools.Circle);
            m_toolTypes[DrawTools.Arc.ObjectType] = typeof(DrawTools.Arc);
            //m_toolTypes[DrawTools.Arc3Point.ObjectType] = typeof(DrawTools.Arc3Point);
            m_toolTypes[DrawTools.Rectangle.ObjectType] = typeof(DrawTools.Rectangle);
            m_toolTypes[DrawTools.BezierCurve.ObjectType] = typeof(DrawTools.BezierCurve);
            //m_toolTypes[DrawTools.Circle3Point.ObjectType] = typeof(DrawTools.Circle3Point);
            this.color = color;
            DefaultLayer();
            m_centerPoint = new UnitPoint(0, 0);
        }
        public void AddDrawTool(string key, IDrawObject drawtool)
		{
			m_drawObjectTypes[key] = drawtool;
		}/// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filename"></param>
		public void Save(string filename)
		{
			try
			{
				XmlTextWriter wr = new XmlTextWriter(filename, null);
				wr.Formatting = Formatting.Indented;
				wr.WriteStartElement("CanvasDataModel");
				m_backgroundLayer.GetObjectData(wr);
				m_gridLayer.GetObjectData(wr);
				foreach (ICanvasLayer layer in m_layers)
				{
					if (layer is ISerialize)
						((ISerialize)layer).GetObjectData(wr);
				}
				XmlUtil.WriteProperties(this, wr);
				wr.WriteEndElement();
				wr.Close();
				m_undoBuffer.Dirty = false;
			}
			catch { }
		}
        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
		public bool Load(string filename)
		{
			try
			{
				StreamReader sr = new StreamReader(filename);
				//XmlTextReader rd = new XmlTextReader(sr);
				XmlDocument doc = new XmlDocument();
				doc.Load(sr);
				sr.Dispose();
				XmlElement root = doc.DocumentElement;
				if (root.Name != "CanvasDataModel")
					return false;

				m_layers.Clear();
				m_undoBuffer.Clear();
				m_undoBuffer.Dirty = false;
				foreach (XmlElement childnode in root.ChildNodes)
				{
					if (childnode.Name == "backgroundlayer")
					{
						XmlUtil.ParseProperties(childnode, m_backgroundLayer);
						continue;
					}
					if (childnode.Name == "gridlayer")
					{
						XmlUtil.ParseProperties(childnode, m_gridLayer);
						continue;
					}
					if (childnode.Name == "layer")
					{                      
						DrawingLayer l = DrawingLayer.NewLayer(childnode as XmlElement);
						m_layers.Add(l);
					}
					if (childnode.Name == "property")
						XmlUtil.ParseProperty(childnode, this);
				}
				return true;
			}
			catch (Exception e)
			{
				DefaultLayer();
				Console.WriteLine("Load exception - {0}", e.Message);
			}
			return false;
		}
        /// <summary>
        /// ����������������ɫ����ϸ��
        /// </summary>
		void DefaultLayer()
		{
			m_layers.Clear();
            //m_layers.Add(new DrawingLayer("layer0", "Hairline Layer", Color.White, 0.0f));
            //m_layers.Add(new DrawingLayer("layer1", "0.005 Layer", Color.Red, 0.005f));
            //m_layers.Add(new DrawingLayer("layer2", "0.025 Layer", Color.Green, 0.025f));
            m_layers.Add(new DrawingLayer("layer0", "Hairline Layer", color, 0.0f));
            m_layers.Add(new DrawingLayer("layer1", "0.025 Layer", color, 0.025f));
        }
		public IDrawObject GetFirstSelected()
		{
			if (m_selection.Count > 0)
			{
				Dictionary<IDrawObject, bool>.KeyCollection.Enumerator e = m_selection.Keys.GetEnumerator();
				e.MoveNext();
				return e.Current;
			}
			return null;
		}
		#region IModel Members
		[XmlSerializable]
		public float Zoom
		{
			get { return m_zoom; }
			set { m_zoom = value; }
		}
		public ICanvasLayer BackgroundLayer
		{
			get { return m_backgroundLayer; }
		}
		public ICanvasLayer GridLayer
		{
			get { return m_gridLayer; }
		}
		public ICanvasLayer[] Layers
		{
			get { return m_layers.ToArray(); }
		}
		public ICanvasLayer ActiveLayer
		{
			get
			{
				if (m_activeLayer == null)
					m_activeLayer = m_layers[0];
				return m_activeLayer;
			}
			set
			{
				m_activeLayer = value;
			}
		}
		public ICanvasLayer GetLayer(string id)
		{
			foreach (ICanvasLayer layer in m_layers)
			{
				if (layer.Id == id)
					return layer;
			}
			return null;
		}
        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="point"></param>
        /// <param name="snappoint"></param>
        /// <returns></returns>
		public IDrawObject CreateObject(string type, UnitPoint point, ISnapPoint snappoint)
		{
			DrawingLayer layer = ActiveLayer as DrawingLayer;
			if (layer.Enabled == false)
				return null;
			DrawTools.DrawObjectBase newobj = CreateObject(type);
			if (newobj != null)
			{
				newobj.Layer = layer;
				newobj.InitializeFromModel(point, layer, snappoint);
			}
			return newobj as IDrawObject;
		}
        /// <summary>
        /// ����ÿ��һ�ʵ����ݣ���
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="drawobject"></param>
		public void AddObject(ICanvasLayer layer, IDrawObject drawobject)
		{
			if (drawobject is DrawTools.IObjectEditInstance)
				drawobject = ((DrawTools.IObjectEditInstance)drawobject).GetDrawObject();
			if (m_undoBuffer.CanCapture)
				m_undoBuffer.AddCommand(new EditCommandAdd(layer, drawobject));
			((DrawingLayer)layer).AddObject(drawobject);
		}
        /// <summary>
        /// ɾ�����ݣ���������
        /// </summary>
        /// <param name="objects"></param>
		public void DeleteObjects(IEnumerable<IDrawObject> objects)
		{
			EditCommandRemove undocommand = null;
			if (m_undoBuffer.CanCapture)
				undocommand = new EditCommandRemove();
			foreach (ICanvasLayer layer in m_layers)
			{
				List<IDrawObject> removedobjects = ((DrawingLayer)layer).DeleteObjects(objects);
				if (removedobjects != null && undocommand != null)
					undocommand.AddLayerObjects(layer, removedobjects);
			}
			if (undocommand != null)
				m_undoBuffer.AddCommand(undocommand);
		}
        /// <summary>
        /// �ƶ�����ʱ���ݵĴ���
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="objects"></param>
		public void MoveObjects(UnitPoint offset, IEnumerable<IDrawObject> objects)
		{
			if (m_undoBuffer.CanCapture)
				m_undoBuffer.AddCommand(new EditCommandMove(offset, objects));
			foreach (IDrawObject obj in objects)
				obj.Move(offset);
		}
        /// <summary>
        /// ��������ʱ�Ĵ�����
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="objects"></param>
		public void CopyObjects(UnitPoint offset, IEnumerable<IDrawObject> objects)
		{
			ClearSelectedObjects();
			List<IDrawObject> newobjects = new List<IDrawObject>();
			foreach (IDrawObject obj in objects)
			{
				IDrawObject newobj = obj.Clone();
				newobjects.Add(newobj);
				newobj.Move(offset);
				((DrawingLayer)ActiveLayer).AddObject(newobj);
				AddSelectedObject(newobj);
			}
			if (m_undoBuffer.CanCapture)
				m_undoBuffer.AddCommand(new EditCommandAdd(ActiveLayer, newobjects));
		}
        /// <summary>
        /// ʹ�����༭���ߺ���е����ݴ�������
        /// </summary>
        /// <param name="edittool"></param>
		public void AfterEditObjects(IEditTool edittool)
		{           
			edittool.Finished();
			if (m_undoBuffer.CanCapture)
				m_undoBuffer.AddCommand(new EditCommandEditTool(edittool));
		}
        /// <summary>
        /// ��ȡ�༭����
        /// </summary>
        /// <param name="edittoolid"></param>
        /// <returns></returns>
		public IEditTool GetEditTool(string edittoolid)
		{
			if (m_editTools.ContainsKey(edittoolid))
				return m_editTools[edittoolid].Clone();
			return null;
		}/// <summary>
        /// �ƶ����
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nodes"></param>
		public void MoveNodes(UnitPoint position, IEnumerable<INodePoint> nodes)
		{
			if (m_undoBuffer.CanCapture)
				m_undoBuffer.AddCommand(new EditCommandNodeMove(nodes));
			foreach (INodePoint node in nodes)
			{
				node.SetPosition(position);
				node.Finish();
			}
		}
        /// <summary>
        /// ��ȡѡ�����������,��������Ϊlist<IDrawObject>�������Ϊ�ø���ͼ���߸�ʽ�洢�����ݣ���
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="selection"></param>
        /// <param name="anyPoint"></param>
        /// <returns></returns>
		public List<IDrawObject> GetHitObjects(ICanvas canvas, RectangleF selection, bool anyPoint)
		{
            //MessageBox.Show("1");
			List<IDrawObject> selected = new List<IDrawObject>();
			foreach (ICanvasLayer layer in m_layers)
			{
				if (layer.Visible == false)
					continue;
				foreach (IDrawObject drawobject in layer.Objects)
				{
					if (drawobject.ObjectInRectangle(canvas, selection, anyPoint))
						selected.Add(drawobject);
				}
			}
			return selected;
		}
        /// <summary>
        /// ��ȡѡ�еĵ�����ݣ���������Ϊlist<IDrawObject>,�����Ϊ�øø���ͼ���߸�ʽ�洢�����ݣ���
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <returns></returns>
		public List<IDrawObject> GetHitObjects(ICanvas canvas, UnitPoint point)
		{
			List<IDrawObject> selected = new List<IDrawObject>();
			foreach (ICanvasLayer layer in m_layers)
			{
				if (layer.Visible == false)
					continue;
				foreach (IDrawObject drawobject in layer.Objects)
				{
					if (drawobject.PointInObject(canvas, point))
						selected.Add(drawobject);
				}
			}
			return selected;
		}
        /// <summary>
        /// �����Ƿ�ѡ�У�
        /// </summary>
        /// <param name="drawobject"></param>
        /// <returns></returns>
		public bool IsSelected(IDrawObject drawobject)
		{
			return m_selection.ContainsKey(drawobject);
		}
        /// <summary>
        /// ��ӱ�ѡ�е��������ݣ�
        /// </summary>
        /// <param name="drawobject"></param>
		public void AddSelectedObject(IDrawObject drawobject)
		{
			DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
			RemoveSelectedObject(drawobject);
			m_selection[drawobject] = true;
			obj.Selected = true;
		}
        /// <summary>
        /// �Ƴ���ѡ�л�δ��ѡ�е�����������ѡ��������[����]
        /// </summary>
        /// <param name="drawobject"></param>
		public void RemoveSelectedObject(IDrawObject drawobject)
		{
			if (m_selection.ContainsKey(drawobject))
			{
				DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
				obj.Selected = false;
				m_selection.Remove(drawobject);
			}
		}
		public IEnumerable<IDrawObject> SelectedObjects
		{
			get
			{
				return m_selection.Keys;
			}
		}
		public int SelectedCount
		{
			get { return m_selection.Count; }
		}
        /// <summary>
        /// ����ȫ����ѡ�е�����[ȫ��]
        /// </summary>
		public void ClearSelectedObjects()
		{
			IEnumerable<IDrawObject> x = SelectedObjects;
			foreach (IDrawObject drawobject in x)
			{
				DrawTools.DrawObjectBase obj = drawobject as DrawTools.DrawObjectBase;
				obj.Selected = false;
			}
			m_selection.Clear();
		}
        /// <summary>
        /// ��ȡ��Ŀ�����Ϣ����
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="point"></param>
        /// <param name="runningsnaptypes"></param>
        /// <param name="usersnaptype"></param>
        /// <returns></returns>
		public ISnapPoint SnapPoint(ICanvas canvas, UnitPoint point, Type[] runningsnaptypes, Type usersnaptype)
		{
			List<IDrawObject> objects = GetHitObjects(canvas, point);
			if (objects.Count == 0)
				return null;

			foreach (IDrawObject obj in objects)
			{
				ISnapPoint snap = obj.SnapPoint(canvas, point, objects, runningsnaptypes, usersnaptype);
				if (snap != null)
					return snap;
			}
			return null;
		}
        /// <summary>
        /// �ܷ��
        /// </summary>
        /// <returns></returns>
		public bool CanUndo()
		{
			return m_undoBuffer.CanUndo;
		}
        /// <summary>
        /// ִ�к�
        /// </summary>
        /// <returns></returns>
		public bool DoUndo()
		{
			return m_undoBuffer.DoUndo(this);
		}
        /// <summary>
        /// �ܷ�������ǰ����
        /// </summary>
        /// <returns></returns>
		public bool CanRedo()
		{
			return m_undoBuffer.CanRedo;

		}
        /// <summary>
        /// ִ��������ǰ����
        /// </summary>
        /// <returns></returns>
		public bool DoRedo()
		{
			return m_undoBuffer.DoRedo(this);
		}
		#endregion
	}
}
