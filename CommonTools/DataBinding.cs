using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace CommonTools
{
    /// <summary>
    /// 数据绑定
    /// </summary>
	public class MyBindingSource : System.Windows.Forms.BindingSource
	{
		public event EventHandler ValueChanged;
        /// <summary>
        /// 这个方法不执行任何有意义的行为
        /// </summary>
        /// <param name="sender"></param>
		public void RaiseValueChanged(object sender)
		{
            //MessageBox.Show((ValueChanged==null).ToString());
			if (ValueChanged != null)
				ValueChanged(sender, null);
		}
	}
	public class BindingWithNotify : System.Windows.Forms.Binding
	{
        /// <summary>
        /// 初始化一个binding新实例,将输入绑定至数据源
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataMember"></param>
		public BindingWithNotify(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember, true)
		{
		}
		protected override void OnBindingComplete(BindingCompleteEventArgs e)
		{
			base.OnBindingComplete(e);
			this.Control.Validating += new CancelEventHandler(Control_Validating);
		}

        /// <summary>
        /// 在控件验证时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void Control_Validating(object sender, CancelEventArgs e)
		{
			WriteNotifyIfChanged();
		}
        /// <summary>
        /// 向绑定数据源写入值
        /// </summary>
		public void WriteNotify()
		{
			WriteValue();
			NotifyChanged();
		}

        /// <summary>
        /// 向数据源中写入并通知写入是否成功,然而并没有通知
        /// </summary>
		public void WriteNotifyIfChanged()
		{
			object dataobject = DataSource;
            //如果包含接口ICurrencyManagerProvider,读取列表
            if (dataobject is ICurrencyManagerProvider)
				dataobject = ((ICurrencyManagerProvider)dataobject).CurrencyManager.Current;

            //向数据源中写入并验证写入成功
			PropertyInfo info = PropertyUtil.GetNestedProperty(ref dataobject, BindingMemberInfo.BindingMember);
			if (info != null)
			{
                //MessageBox.Show("info != null");
				object objBefore = info.GetValue(dataobject, null);
				WriteValue();
				object objAfter = info.GetValue(dataobject, null);
				if (objBefore != null && objAfter != null && objBefore.Equals(objAfter) == false)
					NotifyChanged();
				ReadValue();
			}
		}
        /// <summary>
        /// 因为这个方法只引用一个无操作方法,所以也没有意义
        /// </summary>
		protected virtual void NotifyChanged()
		{
			MyBindingSource ds = DataSource as MyBindingSource;
			if (ds != null)
				ds.RaiseValueChanged(this);
		}

        /// <summary>
        /// 无操作
        /// </summary>
        /// <param name="cevent"></param>
		protected override void OnParse(ConvertEventArgs cevent)
		{
			//base.OnParse(cevent);
			//WriteNotifyIfChanged();
		}
	}
	public class RadioButtonBinder : BindingWithNotify
	{
        /// <summary>
        /// 初始化一个BindingWithNotify(binding)新实例,将输入绑定至数据源
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataMember"></param>
		public RadioButtonBinder(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
		{
		}

        /// <summary>
        /// 从数据源中读取勾选值,然后在打开的GridPage上勾选上
        /// </summary>
        /// <param name="cevent"></param>
        protected override void OnFormat(ConvertEventArgs cevent)
        {
            MyRadioButton b = Control as MyRadioButton;
            b.Checked = b.CheckedValue.Equals(cevent.Value);
        }

        /// <summary>
        /// 保存勾选情况
        /// </summary>
        /// <param name="cevent"></param>
        protected override void OnParse(ConvertEventArgs cevent)
		{
			MyRadioButton b = Control as MyRadioButton;
			if (b.Checked)
				cevent.Value = b.CheckedValue;
		}
	}

    /// <summary>
    /// 估计是没有用到
    /// </summary>
	public class NameObjectBinder : BindingWithNotify
	{
		public NameObjectBinder(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
		{
            MessageBox.Show("NameObjectBinder");
		}
		int cnt = 0;

        /// <summary>
        /// 从数据源中读取勾选值,然后在打开的GridPage上勾选上
        /// </summary>
        /// <param name="cevent"></param>
		protected override void OnFormat(ConvertEventArgs cevent)
		{
			cnt = 0;
			base.OnFormat(cevent);
		}

        /// <summary>
        /// 保存勾选情况
        /// </summary>
        /// <param name="cevent"></param>
		protected override void OnParse(ConvertEventArgs cevent)
		{
			Console.WriteLine("OnParse ({0},{1})", cevent.DesiredType, cevent.Value);
			if (cevent.Value == DBNull.Value)
			{
			}
			//base.OnParse(cevent);
			cevent.Value = 1;
		}
	}

}
