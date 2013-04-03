using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace RabbitMQ.ServiceModel
{
    public class MessageAcknowledgeHandler : IOperationInvoker
    {
        private readonly IOperationInvoker _invoker;

        public MessageAcknowledgeHandler(IOperationInvoker invoker)
        {
            _invoker = invoker;
        }
        
        public object[] AllocateInputs()
        {
            return _invoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            ulong deliveryTag = OperationContext.Current.IncomingMessageHeaders.GetHeader<ulong>("DeliveryTag", @"http://schemas.rabbitmq.com/2007/RabbitMQ/");

            //IChannel innerChannel = ((System.ServiceModel.Channels.ServiceChannel) (OperationContext.Current.Channel)).InnerChannel;

            object channel = OperationContext.Current.Channel;

            PropertyInfo property = channel.GetType().GetProperty("InnerChannel", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

            RabbitMQInputChannel rabbitMQInputChannel = property.GetValue(channel, null) as RabbitMQInputChannel;

            object invoke;

            try
            {
                invoke = _invoker.Invoke(instance, inputs, out outputs);
            }
            catch
            {
                if (rabbitMQInputChannel != null)
                {
                    rabbitMQInputChannel.Reject(deliveryTag, true);
                }
                
                throw;
            }

            if (rabbitMQInputChannel != null)
            {
                rabbitMQInputChannel.Ack(deliveryTag);
            }

            return invoke;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            var action = new Func<object>(() => Invoke(instance, inputs, out inputs));

            return action.BeginInvoke(callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronous
        {
            get { return true; }
        }
    }

}