using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canvas
{
	class CursorCollection
	{
		Dictionary<object, Cursor> m_map = new Dictionary<object, Cursor>();//储存光标信息的字典
        /// <summary>
        /// 增加光标，将光标信息加入字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor"></param>
        public void AddCursor(object key, Cursor cursor)
		{
			m_map[key] = cursor;
		}
        /// <summary>
        /// //增加光标，将光标信息加入字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resourcename"></param>
		public void AddCursor(object key, string resourcename)
        {
			string name = "Resources." + resourcename;//资源名
			Type type = GetType();
			Cursor cursor = new Cursor(GetType(), name);//按照资源名获取当前实例的光标资源
			m_map[key] = cursor;
		}
        /// <summary>
        /// 获取光标信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public Cursor GetCursor(object key)
		{
			if (m_map.ContainsKey(key))//存在则返回字典里的光标
				return m_map[key];
			return System.Windows.Forms.Cursors.Arrow;//不存在就返回窗口的光标
		}
	}
}
