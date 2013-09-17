using System;
using System.ServiceModel.Channels;
using RabbitMQ.ServiceModel.Proxy;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQReplyToOutputChannel : ProxyOutputChannel
    {
        private readonly Uri m_replyToExchange;

        public RabbitMQReplyToOutputChannel(Uri replyToExchange, IOutputChannel channel) : base(channel)
        {
            m_replyToExchange = replyToExchange;
        }

        public override void Send(Message message)
        {
            ApplyReplyTo(message);

            base.Send(message);
        }
        public override void Send(Message message, TimeSpan timeout)
        {
            ApplyReplyTo(message);

            base.Send(message, timeout);
        }

        void ApplyReplyTo(Message message)
        {
            if (m_replyToExchange == null)
            {
                return;
            }

            if (message.Headers.MessageId == null)
            {
                message.Headers.MessageId = new System.Xml.UniqueId();
            }
            if (message.Headers.From == null)
            {
                message.Headers.From = new System.ServiceModel.EndpointAddress(m_replyToExchange);
            }
            if (message.Headers.ReplyTo == null)
            {
                message.Headers.ReplyTo = new System.ServiceModel.EndpointAddress(m_replyToExchange);
            }
        }

    }
}