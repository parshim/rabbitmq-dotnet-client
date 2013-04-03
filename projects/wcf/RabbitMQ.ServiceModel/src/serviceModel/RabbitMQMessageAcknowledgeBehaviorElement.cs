using System;
using System.ServiceModel.Configuration;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQMessageAcknowledgeBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new RabbitMQMessageAcknowledgeBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(RabbitMQMessageAcknowledgeBehavior); }
        }
    }
}