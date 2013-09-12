using System;
using System.Transactions;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;

namespace RabbitMQ.Client
{
    /// <summary>
    /// Queue consumer with transaction support
    /// </summary>
    public class TransactionalQueueConsumer : QueueingBasicConsumerBase
    {
        public TransactionalQueueConsumer()
        {
        }

        public TransactionalQueueConsumer(IModel model) : base(model)
        {
        }

        public TransactionalQueueConsumer(IModel model, SharedQueue<BasicDeliverEventArgs> queue)
            : base(model, queue)
        {
        }

        public override BasicDeliverEventArgs Dequeue()
        {
            BasicDeliverEventArgs message = base.Dequeue();

            Transaction.Current.EnlistVolatile(new TransactionalQueueConsumerEnslistment(message.DeliveryTag, Model), EnlistmentOptions.None);
            
            return message;
        }

        public override bool Dequeue(TimeSpan timeout, out BasicDeliverEventArgs message)
        {
            bool dequeue = base.Dequeue(timeout, out message);

            if (dequeue)
            {
                Transaction.Current.EnlistVolatile(new TransactionalQueueConsumerEnslistment(message.DeliveryTag, Model), EnlistmentOptions.None);
            }

            return dequeue;
        }

        public override BasicDeliverEventArgs DequeueNoWait()
        {
            BasicDeliverEventArgs message = base.DequeueNoWait();

            if (message != null)
            {
                Transaction.Current.EnlistVolatile(new TransactionalQueueConsumerEnslistment(message.DeliveryTag, Model), EnlistmentOptions.None);
            }

            return message;
        }
    }

    public class TransactionalQueueConsumerEnslistment : IEnlistmentNotification
    {
        private readonly ulong m_deliveryTag;
        private readonly IModel m_model;

        public TransactionalQueueConsumerEnslistment(ulong deliveryTag, IModel model)
        {
            m_deliveryTag = deliveryTag;
            m_model = model;
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            m_model.BasicAck(m_deliveryTag, false);

            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            m_model.BasicNack(m_deliveryTag, false, true);

            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }
}