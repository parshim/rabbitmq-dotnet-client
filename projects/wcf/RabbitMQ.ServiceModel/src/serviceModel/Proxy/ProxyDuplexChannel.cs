using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel.Proxy
{
	public abstract class ProxyDuplexChannel : IDuplexChannel
	{
	    protected ProxyDuplexChannel(IDuplexChannel inner)
		{
			m_innerDuplexChannel = inner;
		}
		private readonly IDuplexChannel m_innerDuplexChannel;

		public virtual System.ServiceModel.EndpointAddress LocalAddress
		{
			get
			{
				return m_innerDuplexChannel.LocalAddress;
			}
		}
		public virtual System.ServiceModel.CommunicationState State
		{
			get
			{
				return m_innerDuplexChannel.State;
			}
		}
		public virtual System.ServiceModel.EndpointAddress RemoteAddress
		{
			get
			{
				return m_innerDuplexChannel.RemoteAddress;
			}
		}
		public virtual System.Uri Via
		{
			get
			{
				return m_innerDuplexChannel.Via;
			}
		}
		public virtual event System.EventHandler Closed
		{

			add
			{
				m_innerDuplexChannel.Closed += value;
			}

			remove
			{
				m_innerDuplexChannel.Closed -= value;
			}
		}
		public virtual event System.EventHandler Closing
		{

			add
			{
				m_innerDuplexChannel.Closing += value;
			}

			remove
			{
				m_innerDuplexChannel.Closing -= value;
			}
		}
		public virtual event System.EventHandler Faulted
		{

			add
			{
				m_innerDuplexChannel.Faulted += value;
			}

			remove
			{
				m_innerDuplexChannel.Faulted -= value;
			}
		}
		public virtual event System.EventHandler Opened
		{

			add
			{
				m_innerDuplexChannel.Opened += value;
			}

			remove
			{
				m_innerDuplexChannel.Opened -= value;
			}
		}
		public virtual event System.EventHandler Opening
		{

			add
			{
				m_innerDuplexChannel.Opening += value;
			}

			remove
			{
				m_innerDuplexChannel.Opening -= value;
			}
		}

		public virtual Message Receive()
		{
			return m_innerDuplexChannel.Receive();
		}
		public virtual Message Receive(System.TimeSpan timeout)
		{
			return m_innerDuplexChannel.Receive(timeout);
		}
		public virtual System.IAsyncResult BeginReceive(System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginReceive(callback, state);
		}
		public virtual System.IAsyncResult BeginReceive(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginReceive(timeout, callback, state);
		}
		public virtual Message EndReceive(System.IAsyncResult result)
		{
			return m_innerDuplexChannel.EndReceive(result);
		}
		public virtual System.Boolean TryReceive(System.TimeSpan timeout, out Message message)
		{
			return m_innerDuplexChannel.TryReceive(timeout, out message);
		}
		public virtual System.IAsyncResult BeginTryReceive(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginTryReceive(timeout, callback, state);
		}
		public virtual System.Boolean EndTryReceive(System.IAsyncResult result, out Message message)
		{
			return m_innerDuplexChannel.EndTryReceive(result, out message);
		}
		public virtual System.Boolean WaitForMessage(System.TimeSpan timeout)
		{
			return m_innerDuplexChannel.WaitForMessage(timeout);
		}
		public virtual System.IAsyncResult BeginWaitForMessage(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginWaitForMessage(timeout, callback, state);
		}
		public virtual System.Boolean EndWaitForMessage(System.IAsyncResult result)
		{
			return m_innerDuplexChannel.EndWaitForMessage(result);
		}
		public virtual T GetProperty<T>() where T : class
		{
			return m_innerDuplexChannel.GetProperty<T>();
		}
		public virtual void Abort()
		{
			m_innerDuplexChannel.Abort();
		}
		public virtual void Close()
		{
			m_innerDuplexChannel.Close();
		}
		public virtual void Close(System.TimeSpan timeout)
		{
			m_innerDuplexChannel.Close(timeout);
		}
		public virtual System.IAsyncResult BeginClose(System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginClose(callback, state);
		}
		public virtual System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginClose(timeout, callback, state);
		}
		public virtual void EndClose(System.IAsyncResult result)
		{
			m_innerDuplexChannel.EndClose(result);
		}
		public virtual void Open()
		{
			m_innerDuplexChannel.Open();
		}
		public virtual void Open(System.TimeSpan timeout)
		{
			m_innerDuplexChannel.Open(timeout);
		}
		public virtual System.IAsyncResult BeginOpen(System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginOpen(callback, state);
		}
		public virtual System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginOpen(timeout, callback, state);
		}
		public virtual void EndOpen(System.IAsyncResult result)
		{
			m_innerDuplexChannel.EndOpen(result);
		}
		public virtual void Send(Message message)
		{
			m_innerDuplexChannel.Send(message);
		}
		public virtual void Send(Message message, System.TimeSpan timeout)
		{
			m_innerDuplexChannel.Send(message, timeout);
		}
		public virtual System.IAsyncResult BeginSend(Message message, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginSend(message, callback, state);
		}
		public virtual System.IAsyncResult BeginSend(Message message, System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerDuplexChannel.BeginSend(message, timeout, callback, state);
		}
		public virtual void EndSend(System.IAsyncResult result)
		{
			m_innerDuplexChannel.EndSend(result);
		}
	}
}
