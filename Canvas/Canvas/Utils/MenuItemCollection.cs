using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Canvas
{
    /// <summary>
    /// 菜单项
    /// </summary>
	public class MenuItem
	{
		public string Text = string.Empty;  //控件名
		public string ToolTipText = string.Empty;   //在用户指向控件时显示相应的文本
        public string ShortcutKeyDisplayString = string.Empty;  //显示为快捷键的字符串
		public Shortcut ShortcutKeys = Shortcut.None;   //菜单项指定快捷键
		public Keys SingleKey = Keys.None;  //处理键盘输入的常量
        public Image Image; //菜单项图标
		public System.EventHandler Click;   //菜单项单击事件
		bool m_enabled = true;  //指示菜单项是否启用
		public bool Enabled
		{
			get { return m_enabled; }
			set
			{
				foreach (ToolStripItem item in m_items)
					item.Enabled = value;
				m_enabled = value;
			}
		}
		public object Tag;  //与菜单项关联的用户定义数据 标记控件 加以区别
        List<ToolStripItem> m_items = new List<ToolStripItem>();    //工具栏项列表
		public ToolStripItem[] Items
		{
			get { return m_items.ToArray(); }
		}

        /// <summary>
        /// 在工具栏项下创建一个工具项
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		ToolStripItem CreateItem(Type type)
		{
			ToolStripItem item = Activator.CreateInstance(type) as ToolStripItem;
			item.Text = Text;
			item.Image = Image;
			item.Click += Click;
			item.Tag = Tag;
			item.ToolTipText = ToolTipText;
			item.Enabled = Enabled;
			m_items.Add(item);

			ToolStripMenuItem menuitem = item as ToolStripMenuItem;
			if (menuitem != null)
			{
				if (ShortcutKeyDisplayString.Length > 0)
					menuitem.ShortcutKeyDisplayString = ShortcutKeyDisplayString;
				if (ShortcutKeys != Shortcut.None)
					menuitem.ShortcutKeys = (Keys)ShortcutKeys;
			}
			return item;
		}

        /// <summary>
        /// 为工具项创建按钮
        /// </summary>
        /// <returns></returns>
		public ToolStripButton CreateButton()
		{
			ToolStripButton b = CreateItem(typeof(ToolStripButton)) as ToolStripButton;
			b.DisplayStyle = ToolStripItemDisplayStyle.Image;
			return b;
		}

        /// <summary>
        /// 创建工具栏弹出式菜单项
        /// </summary>
        /// <returns></returns>
		public ToolStripMenuItem CreateMenuItem()
		{
			return CreateItem(typeof(ToolStripMenuItem)) as ToolStripMenuItem;
		}
	}

    /// <summary>
    /// 菜单项管理
    /// </summary>
	public class MenuItemManager
	{
		Control m_owner; //定义控件的基类，控件是带有可视化表示形式的组件
		Dictionary<object, MenuItem> m_items = new Dictionary<object, MenuItem>();
		Dictionary<object, ToolStrip> m_strips = new Dictionary<object,ToolStrip>();
		Dictionary<object, ToolStripItem> m_stripsItem= new Dictionary<object, ToolStripItem>();
		public MenuItemManager(Control owner)
		{
			m_owner = owner;
		}

		public MenuItemManager()
		{
		}

		public MenuItem GetItem(object key)
		{
			if (m_items.ContainsKey(key) == false)
				m_items[key] = new MenuItem();
			return m_items[key];
		}
        
		public ToolStrip GetStrip(object key)
		{
			if (m_strips.ContainsKey(key) == false)
				m_strips[key] = new ToolStrip();
			return m_strips[key];
		}

		public ToolStripMenuItem GetMenuStrip(object key)
		{
			if (m_stripsItem.ContainsKey(key) == false)
				m_stripsItem[key] = new ToolStripMenuItem();
			return m_stripsItem[key] as ToolStripMenuItem;
		}
        
		public StatusStrip GetStatusStrip(object key)
		{
			if (m_strips.ContainsKey(key) == false)
				m_strips[key] = new StatusStrip();
			return m_strips[key] as StatusStrip;
		}

        /// <summary>
        /// 禁用所有控件
        /// </summary>
		public void DisableAll()
		{
			foreach (MenuItem item in m_items.Values)
				item.Enabled = false;
		}

        /// <summary>
        /// 获取工具栏面板
        /// </summary>
        /// <param name="dockedLocation">指定控件的位置和控件停靠的方式</param>
        /// <returns></returns>
		public ToolStripPanel GetStripPanel(DockStyle dockedLocation)
		{
			if (m_owner == null)
				return null;
			foreach (Control ctrl in m_owner.Controls)
			{
				if (ctrl is ToolStripPanel && ((ToolStripPanel)ctrl).Dock == dockedLocation)
					return ctrl as ToolStripPanel;
			}
			return null;
		}

        /// <summary>
        /// 创建工具栏面板
        /// </summary>
        /// <param name="dockedLocation">指定控件的位置和控件停靠的方式。</param>
		void CreateStripPanel(DockStyle dockedLocation)
		{
			if (m_owner != null && GetStripPanel(dockedLocation) == null)
			{
				ToolStripPanel panel = new ToolStripPanel();
				panel.Dock = dockedLocation;
				m_owner.Controls.Add(panel);
			}
		}

		public void SetupStripPanels()
		{
			if (m_owner == null)
				return;
			CreateStripPanel(DockStyle.Left);
			CreateStripPanel(DockStyle.Right);
			CreateStripPanel(DockStyle.Top);
			CreateStripPanel(DockStyle.Bottom);
		}

        /// <summary>
        /// 根据key对应菜单项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public MenuItem FindFromSingleKey(Keys key)
		{
			foreach (MenuItem item in m_items.Values)
			{
				if (item.SingleKey == key)
					return item;
			}
			return null;
		}
	}
}
