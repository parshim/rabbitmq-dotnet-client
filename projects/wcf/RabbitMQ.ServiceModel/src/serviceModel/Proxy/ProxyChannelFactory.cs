using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel.Proxy
{
	public abstract class ProxyChannelFactory<TChannel> : IChannelFactory<TChannel>
	{
	    protected ProxyChannelFactory(IChannelFactory<TChannel> inner)
		{
			m_innerChannelFactory = inner;
		}

		private readonly IChannelFactory<TChannel> m_innerChannelFactory;

		public virtual System.ServiceModel.CommunicationState State
		{
			get
			{
				return m_innerChannelFactory.State;
			}
		}
		public virtual event System.EventHandler Closed
		{

			add
			{
				m_innerChannelFactory.Closed += value;
			}

			remove
			{
				m_innerChannelFactory.Closed -= value;
			}
		}
		public virtual event System.EventHandler Closing
		{

			add
			{
				m_innerChannelFactory.Closing += value;
			}

			remove
			{
				m_innerChannelFactory.Closing -= value;
			}
		}
		public virtual event System.EventHandler Faulted
		{

			add
			{
				m_innerChannelFactory.Faulted += value;
			}

			remove
			{
				m_innerChannelFactory.Faulted -= value;
			}
		}
		public virtual event System.EventHandler Opened
		{

			add
			{
				m_innerChannelFactory.Opened += value;
			}

			remove
			{
				m_innerChannelFactory.Opened -= value;
			}
		}
		public virtual event System.EventHandler Opening
		{

			add
			{
				m_innerChannelFactory.Opening += value;
			}

			remove
			{
				m_innerChannelFactory.Opening -= value;
			}
		}

		public virtual TChannel CreateChannel(System.ServiceModel.EndpointAddress to)
		{
			return m_innerChannelFactory.CreateChannel(to);
		}
		public virtual TChannel CreateChannel(System.ServiceModel.EndpointAddress to, System.Uri via)
		{
			return m_innerChannelFactory.CreateChannel(to, via);
		}
		public virtual T GetProperty<T>() where T : class
		{
			return m_innerChannelFactory.GetProperty<T>();
		}
		public virtual void Abort()
		{
			m_innerChannelFactory.Abort();
		}
		public virtual void Close()
		{
			m_innerChannelFactory.Close();
		}
		public virtual void Close(System.TimeSpan timeout)
		{
			m_innerChannelFactory.Close(timeout);
		}
		public virtual System.IAsyncResult BeginClose(System.AsyncCallback callback, System.Object state)
		{
			return m_innerChannelFactory.BeginClose(callback, state);
		}
		public virtual System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerChannelFactory.BeginClose(timeout, callback, state);
		}
		public virtual void EndClose(System.IAsyncResult result)
		{
			m_innerChannelFactory.EndClose(result);
		}
		public virtual void Open()
		{
			m_innerChannelFactory.Open();
		}
		public virtual void Open(System.TimeSpan timeout)
		{
			m_innerChannelFactory.Open(timeout);
		}
		public virtual System.IAsyncResult BeginOpen(System.AsyncCallback callback, System.Object state)
		{
			return m_innerChannelFactory.BeginOpen(callback, state);
		}
		public virtual System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, System.Object state)
		{
			return m_innerChannelFactory.BeginOpen(timeout, callback, state);
		}
		public virtual void EndOpen(System.IAsyncResult result)
		{
			m_innerChannelFactory.EndOpen(result);
		}
	}
}
