using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel.Proxy
{
	public abstract class ProxyOutputChannel : IOutputChannel
	{
	    protected ProxyOutputChannel(IOutputChannel inner)
		{
			m_innerOutputChannel = inner;
		}

		private readonly IOutputChannel m_innerOutputChannel;

		public virtual System.ServiceModel.EndpointAddress RemoteAddress
		{
			get
			{
				return m_innerOutputChannel.RemoteAddress;
			}
		}
		public virtual System.Uri Via
		{
			get
			{
				return m_innerOutputChannel.Via;
			}
		}
		public virtual System.ServiceModel.CommunicationState State
		{
			get
			{
				return m_innerOutputChannel.State;
			}
		}
		public virtual event System.EventHandler Closed
		{

			add
			{
				m_innerOutputChannel.Closed += value;
			}

			remove
			{
				m_innerOutputChannel.Closed -= value;
			}
		}
		public virtual event System.EventHandler Closing
		{

			add
			{
				m_innerOutputChannel.Closing += value;
			}

			remove
			{
				m_innerOutputChannel.Closing -= value;
			}
		}
		public virtual event System.EventHandler Faulted
		{

			add
			{
				m_innerOutputChannel.Faulted += value;
			}

			remove
			{
				m_innerOutputChannel.Faulted -= value;
			}
		}
		public virtual event System.EventHandler Opened
		{

			add
			{
				m_innerOutputChannel.Opened += value;
			}

			remove
			{
				m_innerOutputChannel.Opened -= value;
			}
		}
		public virtual event System.EventHandler Opening
		{

			add
			{
				m_innerOutputChannel.Opening += value;
			}

			remove
			{
				m_innerOutputChannel.Opening -= value;
			}
		}

		public virtual void Send(Message message)
		{
			m_innerOutputChannel.Send(message);
		}
		public virtual void Send(Message message, System.TimeSpan timeout)
		{
			m_innerOutputChannel.Send(message, timeout);
		}
		public virtual System.IAsyncResult BeginSend(Message message, System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginSend(message, callback, state);
		}
		public virtual System.IAsyncResult BeginSend(Message message, System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginSend(message, timeout, callback, state);
		}
		public virtual void EndSend(System.IAsyncResult result)
		{
			m_innerOutputChannel.EndSend(result);
		}
		public virtual T GetProperty<T>() where T : class
		{
			return m_innerOutputChannel.GetProperty<T>();
		}
		public virtual void Abort()
		{
			m_innerOutputChannel.Abort();
		}
		public virtual void Close()
		{
			m_innerOutputChannel.Close();
		}
		public virtual void Close(System.TimeSpan timeout)
		{
			m_innerOutputChannel.Close(timeout);
		}
		public virtual System.IAsyncResult BeginClose(System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginClose(callback, state);
		}
		public virtual System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginClose(timeout, callback, state);
		}
		public virtual void EndClose(System.IAsyncResult result)
		{
			m_innerOutputChannel.EndClose(result);
		}
		public virtual void Open()
		{
			m_innerOutputChannel.Open();
		}
		public virtual void Open(System.TimeSpan timeout)
		{
			m_innerOutputChannel.Open(timeout);
		}
		public virtual System.IAsyncResult BeginOpen(System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginOpen(callback, state);
		}
		public virtual System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerOutputChannel.BeginOpen(timeout, callback, state);
		}
		public virtual void EndOpen(System.IAsyncResult result)
		{
			m_innerOutputChannel.EndOpen(result);
		}
	}
}
