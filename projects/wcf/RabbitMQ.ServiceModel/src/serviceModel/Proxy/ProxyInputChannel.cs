using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel.Proxy
{
	public abstract class ProxyInputChannel : IInputChannel
	{
	    protected ProxyInputChannel(IInputChannel inner)
		{
			m_innerInputChannel = inner;
		}
		private readonly IInputChannel m_innerInputChannel;

		public virtual System.ServiceModel.EndpointAddress LocalAddress
		{
			get
			{
				return m_innerInputChannel.LocalAddress;
			}
		}
		public virtual System.ServiceModel.CommunicationState State
		{
			get
			{
				return m_innerInputChannel.State;
			}
		}
		public virtual event System.EventHandler Closed
		{

			add
			{
				m_innerInputChannel.Closed += value;
			}

			remove
			{
				m_innerInputChannel.Closed -= value;
			}
		}
		public virtual event System.EventHandler Closing
		{

			add
			{
				m_innerInputChannel.Closing += value;
			}

			remove
			{
				m_innerInputChannel.Closing -= value;
			}
		}
		public virtual event System.EventHandler Faulted
		{

			add
			{
				m_innerInputChannel.Faulted += value;
			}

			remove
			{
				m_innerInputChannel.Faulted -= value;
			}
		}
		public virtual event System.EventHandler Opened
		{

			add
			{
				m_innerInputChannel.Opened += value;
			}

			remove
			{
				m_innerInputChannel.Opened -= value;
			}
		}
		public virtual event System.EventHandler Opening
		{

			add
			{
				m_innerInputChannel.Opening += value;
			}

			remove
			{
				m_innerInputChannel.Opening -= value;
			}
		}

		public virtual Message Receive()
		{
			return m_innerInputChannel.Receive();
		}
		public virtual Message Receive(System.TimeSpan timeout)
		{
			return m_innerInputChannel.Receive(timeout);
		}
		public virtual System.IAsyncResult BeginReceive(System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginReceive(callback, state);
		}
		public virtual System.IAsyncResult BeginReceive(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginReceive(timeout, callback, state);
		}
		public virtual Message EndReceive(System.IAsyncResult result)
		{
			return m_innerInputChannel.EndReceive(result);
		}
		public virtual System.Boolean TryReceive(System.TimeSpan timeout, out Message message)
		{
			return m_innerInputChannel.TryReceive(timeout, out message);
		}
		public virtual System.IAsyncResult BeginTryReceive(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginTryReceive(timeout, callback, state);
		}
		public virtual System.Boolean EndTryReceive(System.IAsyncResult result, out Message message)
		{
			return m_innerInputChannel.EndTryReceive(result, out message);
		}
		public virtual System.Boolean WaitForMessage(System.TimeSpan timeout)
		{
			return m_innerInputChannel.WaitForMessage(timeout);
		}
		public virtual System.IAsyncResult BeginWaitForMessage(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginWaitForMessage(timeout, callback, state);
		}
		public virtual System.Boolean EndWaitForMessage(System.IAsyncResult result)
		{
			return m_innerInputChannel.EndWaitForMessage(result);
		}
		public virtual T GetProperty<T>() where T : class
		{
			return m_innerInputChannel.GetProperty<T>();
		}
		public virtual void Abort()
		{
			m_innerInputChannel.Abort();
		}
		public virtual void Close()
		{
			m_innerInputChannel.Close();
		}
		public virtual void Close(System.TimeSpan timeout)
		{
			m_innerInputChannel.Close(timeout);
		}
		public virtual System.IAsyncResult BeginClose(System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginClose(callback, state);
		}
		public virtual System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginClose(timeout, callback, state);
		}
		public virtual void EndClose(System.IAsyncResult result)
		{
			m_innerInputChannel.EndClose(result);
		}
		public virtual void Open()
		{
			m_innerInputChannel.Open();
		}
		public virtual void Open(System.TimeSpan timeout)
		{
			m_innerInputChannel.Open(timeout);
		}
		public virtual System.IAsyncResult BeginOpen(System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginOpen(callback, state);
		}
		public virtual System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerInputChannel.BeginOpen(timeout, callback, state);
		}
		public virtual void EndOpen(System.IAsyncResult result)
		{
			m_innerInputChannel.EndOpen(result);
		}
	}
}
