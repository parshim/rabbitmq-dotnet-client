using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace RabbitMQ.ServiceModel
{
    public class RabbitMQMessageAcknowledgeBehavior : IEndpointBehavior, IOperationBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {
            if (!(endpoint.Binding is RabbitMQBinding))
            {
                throw new ArgumentException(
                    string.Format("Invalid type for binding. Expected {0}, Passed: {1}",
                                  typeof(RabbitMQBinding).AssemblyQualifiedName,
                                  endpoint.Binding.GetType().AssemblyQualifiedName));
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                if (operation.Behaviors.Contains(typeof(RabbitMQMessageAcknowledgeBehavior)))
                    continue;

                operation.Behaviors.Add(new RabbitMQMessageAcknowledgeBehavior());
            }
            
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            
        }

        public void Validate(OperationDescription operationDescription)
        {
            
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new MessageAcknowledgeHandler(dispatchOperation.Invoker);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }
    }
}