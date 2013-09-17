using System;
using System.ServiceModel.Channels;
using RabbitMQ.ServiceModel.Proxy;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQReplyToChannelFactory<T> : ProxyChannelFactory<T>
    {
        private readonly Uri m_replyToExchange;

        public RabbitMQReplyToChannelFactory(Uri replyToExchange, IChannelFactory<T> channelFactory)
            : base(channelFactory)
        {
            m_replyToExchange = replyToExchange;
        }

        public override T CreateChannel(System.ServiceModel.EndpointAddress to)
        {
            IChannel channel = (IChannel) base.CreateChannel(to);

            if (typeof (T) == typeof (IOutputChannel))
            {
                channel = new RabbitMQReplyToOutputChannel(m_replyToExchange, (IOutputChannel) channel);
            }

            return (T) channel;
        }

        public override T CreateChannel(System.ServiceModel.EndpointAddress to, Uri via)
        {
            IChannel channel = (IChannel)base.CreateChannel(to, via);

            if (typeof(T) == typeof(IOutputChannel))
            {
                channel = new RabbitMQReplyToOutputChannel(m_replyToExchange, (IOutputChannel)channel);
            }

            return (T)channel;
        }
    }
}