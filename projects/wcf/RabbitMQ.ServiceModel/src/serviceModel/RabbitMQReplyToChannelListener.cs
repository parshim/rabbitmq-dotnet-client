using System;
using System.ServiceModel.Channels;
using RabbitMQ.ServiceModel.Proxy;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQReplyToChannelListener<T> : ProxyChannelListener<T> where T : class, IChannel
    {
        public RabbitMQReplyToChannelListener(IChannelListener<T> channelListener)
            : base(channelListener)
        {
            
        }

        public override T AcceptChannel()
        {
            T channel = base.AcceptChannel();

            if (typeof (T) == typeof (IDuplexChannel))
            {
                
            }

            return channel;
        }

        public override T AcceptChannel(TimeSpan timeout)
        {
            T channel = base.AcceptChannel();

            if (typeof(T) == typeof(IDuplexChannel))
            {

            }

            return channel;
        }
    }
}