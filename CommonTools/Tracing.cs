using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommonTools
{
	public class Tracing//描绘
    {
		static Tracing m_instance = new Tracing();
        /// <summary>
        /// 储存开始描绘的时间
        /// </summary>
        /// <param name="id"></param>
		static public void StartTrack(int id)
		{
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PushTick();//储存开始描绘的时间
		}
        /// <summary>
        /// //描绘结束
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
		static public void EndTrack(int id, string text)
		{
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PopTick(text, null);
        }
		static public void EndTrack(int id, string text, params object[] args)//描绘结束
        {
			if (m_instance.CanTrace(id) == false)
				return;
			m_instance.PopTick(text, args);
		}
		static public void AddId(int id)//添加id
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
			m_instance.m_thread = new Thread(new ThreadStart(m_instance.DoTrace));//调用DoTrace创建新线程赋予 m_thread
            m_instance.m_thread.Name = "Tracing";//设置线程名称
			m_instance.m_thread.Priority = ThreadPriority.Normal;//设置线程优先级
			m_instance.m_thread.Start();//更改线程状态为running
		}
		static public void Terminate()//结束
		{
			if (m_instance.m_thread != null)
				m_instance.m_thread.Abort();//终止线程
			m_instance.m_thread = null;//清空此线程
		}
		Stack<int> m_ticks = new Stack<int>();
		Queue<StringBuilder> m_strings = new Queue<StringBuilder>();//先进先出集合
		Dictionary<int, bool> m_ids = new Dictionary<int,bool>();
		Thread m_thread;
		ManualResetEvent m_wait = new ManualResetEvent(false);//初始化非终止状态
        Tracing()
		{
		}
		bool CanTrace(int id)//能否画
		{
			return m_thread != null && m_ids.ContainsKey(id);
		}
		void PushTick()//将系统启动后的时间加入栈中
		{
			m_ticks.Push(Environment.TickCount);
		}
        /// <summary>
        /// 移除移除m_ticks栈中的的栈顶，添加信息到字符串sb中，并添加到先进先出集合结尾处
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
		void PopTick(string text, params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			int elapsedtime = Environment.TickCount - m_ticks.Pop();//距离上次获取的时间过去了多久，并移除m_ticks栈中的的栈顶
            if (args == null)//如果args为空
                sb.AppendFormat("{0}: {1}, Ticks({2})", DateTime.Now.ToLongTimeString(), text, elapsedtime.ToString());//设置字符串为当前时间，text，距离开始描绘的时间过去了多久
            else//否则
			{
				sb.AppendFormat("{0}: ", DateTime.Now.ToLongTimeString());//在字符串中加入当前时间
				sb.AppendFormat(text, args);//按照text的格式将args加入字符串。
				sb.AppendFormat(", Ticks({0})", elapsedtime.ToString());//在字符串中加入距开始描绘时间过去了多久
            }
			sb.AppendLine();//添加终止符
			lock (m_strings)
			{
				m_strings.Enqueue(sb);//添加到先进先出集合结尾处
				m_wait.Set();//恢复所有被阻塞线程
            }
		}
        /// <summary>
        /// 添加信息到sb中，并添加到先进先出集合结尾处
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
		void WriteLine(string text, params object[] args)
		{
			StringBuilder sb = new StringBuilder();
			if (args == null)//如果args为空
                sb.AppendFormat("{0}: {1}", DateTime.Now.ToLongTimeString(), text);//设置字符串为当前时间，text
            else//否则
            {
				sb.AppendFormat("{0}: ", DateTime.Now.ToLongTimeString());//在字符串中加入当前时间
                sb.AppendFormat(text, args);//按照text的格式将args加入字符串。
            }
			sb.AppendLine();//添加终止符
            lock (m_strings)
			{
				m_strings.Enqueue(sb);//添加到先进先出集合结尾处
                m_wait.Set(); //恢复所有被阻塞线程

            }
		}
        /// <summary>
        /// 输出集合中的信息
        /// </summary>
		void DoTrace()
		{
			while (true)
			{
				m_wait.WaitOne();//阻塞线程
				while (m_strings.Count > 0)//先进先出集合存在元素
				{
					StringBuilder sb = null;
					lock (m_strings)
					{
						sb = m_strings.Dequeue();//移除开始位置的值并返回
					}
					Console.Write(sb.ToString());//输出字符串
				}
				m_wait.Reset();//重置信号量
			}
		}
	}
}
