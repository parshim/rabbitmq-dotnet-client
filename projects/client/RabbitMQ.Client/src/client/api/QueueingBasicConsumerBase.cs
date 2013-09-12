using System;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;

namespace RabbitMQ.Client
{
    ///<summary>Simple IBasicConsumer implementation that uses a
    ///SharedQueue to buffer incoming deliveries.</summary>
    ///<remarks>
    ///<para>
    /// Received messages are placed in the SharedQueue as instances
    /// of BasicDeliverEventArgs.
    ///</para>
    ///<para>
    /// Note that messages taken from the SharedQueue may need
    /// acknowledging with IModel.BasicAck.
    ///</para>
    ///<para>
    /// When the consumer is closed, through BasicCancel or through
    /// the shutdown of the underlying IModel or IConnection, the
    /// SharedQueue's Close() method is called, which causes any
    /// Enqueue() operations, and Dequeue() operations when the queue
    /// is empty, to throw EndOfStreamException (see the comment for
    /// SharedQueue.Close()).
    ///</para>
    ///<para>
    /// The following is a simple example of the usage of this class:
    ///</para>
    ///<example><code>
    ///	IModel channel = ...;
    ///	QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
    ///	channel.BasicConsume(queueName, null, consumer);
    ///	
    ///	// At this point, messages will be being asynchronously delivered,
    ///	// and will be queuing up in consumer.Queue.
    ///	
    ///	while (true) {
    ///	    try {
    ///	        BasicDeliverEventArgs e = (BasicDeliverEventArgs) consumer.Queue.Dequeue();
    ///	        // ... handle the delivery ...
    ///	        channel.BasicAck(e.DeliveryTag, false);
    ///	    } catch (EndOfStreamException ex) {
    ///	        // The consumer was cancelled, the model closed, or the
    ///	        // connection went away.
    ///	        break;
    ///	    }
    ///	}
    ///</code></example>
    ///</remarks>
    /// 
    /// 
    public abstract class QueueingBasicConsumerBase : DefaultBasicConsumer, IMessageQueue
    {
        protected SharedQueue<BasicDeliverEventArgs> m_queue;

        ///<summary>Creates a fresh QueueingBasicConsumer,
        ///initializing the Model property to null and the Queue
        ///property to a fresh SharedQueue.</summary>
        protected QueueingBasicConsumerBase() : this(null) { }

        ///<summary>Creates a fresh QueueingBasicConsumer, with Model
        ///set to the argument, and Queue set to a fresh
        ///SharedQueue.</summary>
        protected QueueingBasicConsumerBase(IModel model) : this(model, new SharedQueue<BasicDeliverEventArgs>()) { }

        ///<summary>Creates a fresh QueueingBasicConsumer,
        ///initializing the Model and Queue properties to the given
        ///values.</summary>
        protected QueueingBasicConsumerBase(IModel model, SharedQueue<BasicDeliverEventArgs> queue)
            : base(model)
        {
            m_queue = queue;
        }

        ///<summary>Retrieves the SharedQueue that messages arrive on.</summary>
        public SharedQueue<BasicDeliverEventArgs> Queue
        {
            get { return m_queue; }
        }

        ///<summary>Overrides DefaultBasicConsumer's OnCancel
        ///implementation, extending it to call the Close() method of
        ///the SharedQueue.</summary>
        public override void OnCancel()
        {
            m_queue.Close();
            base.OnCancel();
        }

        ///<summary>Overrides DefaultBasicConsumer's
        ///HandleBasicDeliver implementation, building a
        ///BasicDeliverEventArgs instance and placing it in the
        ///Queue.</summary>
        public override void HandleBasicDeliver(string consumerTag,
                                                ulong deliveryTag,
                                                bool redelivered,
                                                string exchange,
                                                string routingKey,
                                                IBasicProperties properties,
                                                byte[] body)
        {
            BasicDeliverEventArgs e = new BasicDeliverEventArgs
                {
                    ConsumerTag = consumerTag,
                    DeliveryTag = deliveryTag,
                    Redelivered = redelivered,
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    BasicProperties = properties,
                    Body = body
                };

            m_queue.Enqueue(e);
        }

        public virtual bool WaitForMessage(TimeSpan timeout)
        {
            return m_queue.WaitForMessage(timeout);
        }

        public virtual bool Dequeue(TimeSpan timeout, out BasicDeliverEventArgs message)
        {
            return m_queue.Dequeue(timeout, out message);
        }

        public virtual BasicDeliverEventArgs DequeueNoWait()
        {
            return m_queue.DequeueNoWait(null);
        }

        public virtual BasicDeliverEventArgs Dequeue()
        {
            return m_queue.Dequeue();
        }
    }
}