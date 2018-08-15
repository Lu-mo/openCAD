using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonTools
{
	public class Tracing//���
    {
		static Tracing m_instance = new Tracing();
        /// <summary>
        /// ���濪ʼ����ʱ��
        /// </summary>
        /// <param name="id"></param>
		static public void StartTrack(int id)
		{
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PushTick();//���濪ʼ����ʱ��
		}
        /// <summary>
        /// //������
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
		static public void EndTrack(int id, string text)
		{
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PopTick(text, null);
        }
		static public void EndTrack(int id, string text, params object[] args)//������
        {
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PopTick(text, args);
		}
		static public void AddId(int id)//���id
		{
			m_instance.m_ids[id] = true;
		}
		static public void WriteLine(int id, string text, params object[] args)
		{
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PopTick(text, args);
		}
		static public void EnableTrace()
		{
			m_instance.m_thread = new Thread(new ThreadStart(m_instance.DoTrace));//����DoTrace�������̸߳��� m_thread
            m_instance.m_thread.Name = "Tracing";//�����߳�����
			m_instance.m_thread.Priority = ThreadPriority.Normal;//�����߳����ȼ�
			m_instance.m_thread.Start();//�����߳�״̬Ϊrunning
		}
		static public void Terminate()//����
		{
			if (m_instance.m_thread != null)
				m_instance.m_thread.Abort();//��ֹ�߳�
			m_instance.m_thread = null;//��մ��߳�
		}
		Stack<int> m_ticks = new Stack<int>();
		Queue<StringBuilder> m_strings = new Queue<StringBuilder>();//�Ƚ��ȳ�����
		Dictionary<int, bool> m_ids = new Dictionary<int,bool>();
		Thread m_thread;
		ManualResetEvent m_wait = new ManualResetEvent(false);//��ʼ������ֹ״̬
        Tracing()
		{
		}
		bool CanTrace(int id)//�ܷ�
		{
			return m_thread != null && m_ids.ContainsKey(id);
		}
		void PushTick()//��ϵͳ�������ʱ�����ջ��
		{
			m_ticks.Push(Environment.TickCount);
		}
        /// <summary>
        /// �Ƴ��Ƴ�m_ticksջ�еĵ�ջ���������Ϣ���ַ���sb�У�����ӵ��Ƚ��ȳ����Ͻ�β��
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
		void PopTick(string text, params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			int elapsedtime = Environment.TickCount - m_ticks.Pop();//�����ϴλ�ȡ��ʱ���ȥ�˶�ã����Ƴ�m_ticksջ�еĵ�ջ��
            if (args == null)//���argsΪ��
                sb.AppendFormat("{0}: {1}, Ticks({2})", DateTime.Now.ToLongTimeString(), text, elapsedtime.ToString());//�����ַ���Ϊ��ǰʱ�䣬text�����뿪ʼ����ʱ���ȥ�˶��
            else//����
			{
				sb.AppendFormat("{0}: ", DateTime.Now.ToLongTimeString());//���ַ����м��뵱ǰʱ��
				sb.AppendFormat(text, args);//����text�ĸ�ʽ��args�����ַ�����
				sb.AppendFormat(", Ticks({0})", elapsedtime.ToString());//���ַ����м���࿪ʼ���ʱ���ȥ�˶��
            }
			sb.AppendLine();//�����ֹ��
			lock (m_strings)
			{
				m_strings.Enqueue(sb);//��ӵ��Ƚ��ȳ����Ͻ�β��
				m_wait.Set();//�ָ����б������߳�
            }
		}
        /// <summary>
        /// �����Ϣ��sb�У�����ӵ��Ƚ��ȳ����Ͻ�β��
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
		void WriteLine(string text, params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			if (args == null)//���argsΪ��
                sb.AppendFormat("{0}: {1}", DateTime.Now.ToLongTimeString(), text);//�����ַ���Ϊ��ǰʱ�䣬text
            else//����
            {
				sb.AppendFormat("{0}: ", DateTime.Now.ToLongTimeString());//���ַ����м��뵱ǰʱ��
                sb.AppendFormat(text, args);//����text�ĸ�ʽ��args�����ַ�����
            }
			sb.AppendLine();//�����ֹ��
            lock (m_strings)
			{
				m_strings.Enqueue(sb);//��ӵ��Ƚ��ȳ����Ͻ�β��
                m_wait.Set(); //�ָ����б������߳�

            }
		}
        /// <summary>
        /// ��������е���Ϣ
        /// </summary>
		void DoTrace()
		{
			while (true)
			{
				m_wait.WaitOne();//�����߳�
				while (m_strings.Count > 0)//�Ƚ��ȳ����ϴ���Ԫ��
				{
					StringBuilder sb = null;
					lock (m_strings)
					{
						sb = m_strings.Dequeue();//�Ƴ���ʼλ�õ�ֵ������
					}
					Console.Write(sb.ToString());//����ַ���
				}
				m_wait.Reset();//�����ź���
			}
		}
	}
}
