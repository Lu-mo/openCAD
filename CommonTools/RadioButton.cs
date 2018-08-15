using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonTools
{
	public class MyRadioButton : RadioButton
	{
        /// <summary>
        /// 无操作
        /// </summary>
		public MyRadioButton()
		{
			
		}
		object m_checkedValue = null;
        /// <summary>
        /// 只有get
        /// </summary>
		public object CheckedValue
		{
			get { return m_checkedValue; }
			set {}
		}

        /// <summary>
        /// 添加绑定数据源,决定网格点型还是线性,m_gridBinding为存储变量名
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="datamember"></param>
        /// <param name="controlValue"></param>
		public void AddDatabinding(MyBindingSource datasource, string datamember, object controlValue)
		{
			m_checkedValue = controlValue;
			DataBindings.Add(new RadioButtonBinder("CheckedValue", datasource, datamember));
		}
		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			if (Checked && DataBindings != null && DataBindings.Count > 0)
			{
				BindingWithNotify binding = DataBindings[0] as BindingWithNotify;
				if (binding != null)
					binding.WriteNotify();
			}
		}
	}
}
