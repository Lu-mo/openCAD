using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canvas
{
	class CursorCollection
	{
		Dictionary<object, Cursor> m_map = new Dictionary<object, Cursor>();//��������Ϣ���ֵ�
        /// <summary>
        /// ���ӹ�꣬�������Ϣ�����ֵ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor"></param>
        public void AddCursor(object key, Cursor cursor)
		{
			m_map[key] = cursor;
		}
        /// <summary>
        /// //���ӹ�꣬�������Ϣ�����ֵ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resourcename"></param>
		public void AddCursor(object key, string resourcename)
        {
			string name = "Resources." + resourcename;//��Դ��
			Type type = GetType();
			Cursor cursor = new Cursor(GetType(), name);//������Դ����ȡ��ǰʵ���Ĺ����Դ
			m_map[key] = cursor;
		}
        /// <summary>
        /// ��ȡ�����Ϣ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public Cursor GetCursor(object key)
		{
			if (m_map.ContainsKey(key))//�����򷵻��ֵ���Ĺ��
				return m_map[key];
			return System.Windows.Forms.Cursors.Arrow;//�����ھͷ��ش��ڵĹ��
		}
	}
}
