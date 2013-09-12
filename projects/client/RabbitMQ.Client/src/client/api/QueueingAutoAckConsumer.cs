using System;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;

namespace RabbitMQ.Client
{
    /// <summary>
    /// Queue consumer with auto acknowledge on dequeue
    /// </summary>
    public class QueueingAutoAckConsumer : QueueingBasicConsumerBase
    {
        public QueueingAutoAckConsumer()
        {
        }

        public QueueingAutoAckConsumer(IModel model) : base(model)
        {
        }

        public QueueingAutoAckConsumer(IModel model, SharedQueue<BasicDeliverEventArgs> queue) : base(model, queue)
        {
        }

        public override BasicDeliverEventArgs Dequeue()
        {
            BasicDeliverEventArgs message = base.Dequeue();

            Model.BasicAck(message.DeliveryTag, false);

            return message;
        }

        public override bool Dequeue(TimeSpan timeout, out BasicDeliverEventArgs message)
        {
            bool dequeue = base.Dequeue(timeout, out message);

            if (dequeue)
            {
                Model.BasicAck(message.DeliveryTag, false);
            }

            return dequeue;
        }

        public override BasicDeliverEventArgs DequeueNoWait()
        {
            BasicDeliverEventArgs message = base.DequeueNoWait();

            if (message != null)
            {
                Model.BasicAck(message.DeliveryTag, false);
            }

            return message;
        }
    }
}