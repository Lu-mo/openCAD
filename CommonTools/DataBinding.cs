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
    /// ���ݰ�
    /// </summary>
	public class MyBindingSource : System.Windows.Forms.BindingSource
	{
		public event EventHandler ValueChanged;
        /// <summary>
        /// ���������ִ���κ����������Ϊ
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
        /// ��ʼ��һ��binding��ʵ��,�������������Դ
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
        /// �ڿؼ���֤ʱ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void Control_Validating(object sender, CancelEventArgs e)
		{
			WriteNotifyIfChanged();
		}
        /// <summary>
        /// �������Դд��ֵ
        /// </summary>
		public void WriteNotify()
		{
			WriteValue();
			NotifyChanged();
		}

        /// <summary>
        /// ������Դ��д�벢֪ͨд���Ƿ�ɹ�,Ȼ����û��֪ͨ
        /// </summary>
		public void WriteNotifyIfChanged()
		{
			object dataobject = DataSource;
            //��������ӿ�ICurrencyManagerProvider,��ȡ�б�
            if (dataobject is ICurrencyManagerProvider)
				dataobject = ((ICurrencyManagerProvider)dataobject).CurrencyManager.Current;

            //������Դ��д�벢��֤д��ɹ�
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
        /// ��Ϊ�������ֻ����һ���޲�������,����Ҳû������
        /// </summary>
		protected virtual void NotifyChanged()
		{
			MyBindingSource ds = DataSource as MyBindingSource;
			if (ds != null)
				ds.RaiseValueChanged(this);
		}

        /// <summary>
        /// �޲���
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
        /// ��ʼ��һ��BindingWithNotify(binding)��ʵ��,�������������Դ
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataMember"></param>
		public RadioButtonBinder(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
		{
		}

        /// <summary>
        /// ������Դ�ж�ȡ��ѡֵ,Ȼ���ڴ򿪵�GridPage�Ϲ�ѡ��
        /// </summary>
        /// <param name="cevent"></param>
        protected override void OnFormat(ConvertEventArgs cevent)
        {
            MyRadioButton b = Control as MyRadioButton;
            b.Checked = b.CheckedValue.Equals(cevent.Value);
        }

        /// <summary>
        /// ���湴ѡ���
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
    /// ������û���õ�
    /// </summary>
	public class NameObjectBinder : BindingWithNotify
	{
		public NameObjectBinder(string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
		{
            MessageBox.Show("NameObjectBinder");
		}
		int cnt = 0;

        /// <summary>
        /// ������Դ�ж�ȡ��ѡֵ,Ȼ���ڴ򿪵�GridPage�Ϲ�ѡ��
        /// </summary>
        /// <param name="cevent"></param>
		protected override void OnFormat(ConvertEventArgs cevent)
		{
			cnt = 0;
			base.OnFormat(cevent);
		}

        /// <summary>
        /// ���湴ѡ���
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
