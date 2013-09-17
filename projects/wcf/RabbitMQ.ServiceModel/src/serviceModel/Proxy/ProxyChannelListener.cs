using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel.Proxy
{
	public abstract class ProxyChannelListener<TChannel> : IChannelListener<TChannel> where TChannel : class, IChannel
	{
	    protected ProxyChannelListener(IChannelListener<TChannel> inner)
		{
			m_innerChannelListener = inner;
		}
		private readonly IChannelListener<TChannel> m_innerChannelListener;

		public virtual Uri Uri
		{
			get
			{
				return m_innerChannelListener.Uri;
			}
		}
		public virtual CommunicationState State
		{
			get
			{
				return m_innerChannelListener.State;
			}
		}
		public virtual event EventHandler Closed
		{

			add
			{
				m_innerChannelListener.Closed += value;
			}

			remove
			{
				m_innerChannelListener.Closed -= value;
			}
		}
		public virtual event EventHandler Closing
		{

			add
			{
				m_innerChannelListener.Closing += value;
			}

			remove
			{
				m_innerChannelListener.Closing -= value;
			}
		}
		public virtual event EventHandler Faulted
		{

			add
			{
				m_innerChannelListener.Faulted += value;
			}

			remove
			{
				m_innerChannelListener.Faulted -= value;
			}
		}
		public virtual event EventHandler Opened
		{

			add
			{
				m_innerChannelListener.Opened += value;
			}

			remove
			{
				m_innerChannelListener.Opened -= value;
			}
		}
		public virtual event EventHandler Opening
		{

			add
			{
				m_innerChannelListener.Opening += value;
			}

			remove
			{
				m_innerChannelListener.Opening -= value;
			}
		}

		public virtual TChannel AcceptChannel()
		{
			return m_innerChannelListener.AcceptChannel();
		}
		public virtual TChannel AcceptChannel(TimeSpan timeout)
		{
			return m_innerChannelListener.AcceptChannel(timeout);
		}
		public virtual IAsyncResult BeginAcceptChannel(AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginAcceptChannel(callback, state);
		}
		public virtual IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginAcceptChannel(timeout, callback, state);
		}
		public virtual TChannel EndAcceptChannel(IAsyncResult result)
		{
			return m_innerChannelListener.EndAcceptChannel(result);
		}
		public virtual T GetProperty<T>() where T : class
		{
			return m_innerChannelListener.GetProperty<T>();
		}
		public virtual Boolean WaitForChannel(TimeSpan timeout)
		{
			return m_innerChannelListener.WaitForChannel(timeout);
		}
		public virtual IAsyncResult BeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginWaitForChannel(timeout, callback, state);
		}
		public virtual Boolean EndWaitForChannel(IAsyncResult result)
		{
			return m_innerChannelListener.EndWaitForChannel(result);
		}
		public virtual void Abort()
		{
			m_innerChannelListener.Abort();
		}
		public virtual void Close()
		{
			m_innerChannelListener.Close();
		}
		public virtual void Close(TimeSpan timeout)
		{
			m_innerChannelListener.Close(timeout);
		}
		public virtual IAsyncResult BeginClose(AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginClose(callback, state);
		}
		public virtual IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginClose(timeout, callback, state);
		}
		public virtual void EndClose(IAsyncResult result)
		{
			m_innerChannelListener.EndClose(result);
		}
		public virtual void Open()
		{
			m_innerChannelListener.Open();
		}
		public virtual void Open(TimeSpan timeout)
		{
			m_innerChannelListener.Open(timeout);
		}
		public virtual IAsyncResult BeginOpen(AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginOpen(callback, state);
		}
		public virtual IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, Object state)
		{
			return m_innerChannelListener.BeginOpen(timeout, callback, state);
		}
		public virtual void EndOpen(IAsyncResult result)
		{
			m_innerChannelListener.EndOpen(result);
		}
	}
}
