using System;
using System.ServiceModel.Channels;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQReplyToBindingElement : BindingElement
    {
        public override BindingElement Clone()
        {
            return new RabbitMQReplyToBindingElement
                {
                    ReplyToExchange = ReplyToExchange
                };
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return new RabbitMQReplyToChannelFactory<TChannel>(ReplyToExchange, context.BuildInnerChannelFactory<TChannel>());
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            return new RabbitMQReplyToChannelListener<TChannel>(context.BuildInnerChannelListener<TChannel>());
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return typeof(TChannel) == typeof(IOutputChannel) && context.CanBuildInnerChannelFactory<IOutputChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return typeof(TChannel) == typeof(IInputChannel) && context.CanBuildInnerChannelListener<IInputChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return context.GetInnerProperty<T>();
        }

        /// <summary>
        /// ReplyTo exchange URI for duplex communication callbacks
        /// </summary>
        public Uri ReplyToExchange { get; set; }
    }
}