// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2013 VMware, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is VMware, Inc.
//  Copyright (c) 2007-2013 VMware, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using CommonFraming = RabbitMQ.Client.Framing.v0_9;

namespace RabbitMQ.ServiceModel
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Client;
    using Client.Events;

    // We use spec version 0-9 for common constants such as frame types,
    // error codes, and the frame end byte, since they don't vary *within
    // the versions we support*. Obviously we may need to revisit this if
    // that ever changes.

    internal sealed class RabbitMQInputChannel : RabbitMQInputChannelBase
    {
        private readonly RabbitMQTransportBindingElement m_bindingElement;
        private readonly MessageEncoder m_encoder;
        private readonly IModel m_model;
        private IMessageQueue m_messageQueue;
        private readonly bool m_isTemporary;
        
        public RabbitMQInputChannel(BindingContext context, IModel model, EndpointAddress address, bool isTemporary)
            : base(context, address)
        {
            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            TextMessageEncodingBindingElement encoderElem = context.BindingParameters.Find<TextMessageEncodingBindingElement>();
            encoderElem.ReaderQuotas.MaxStringContentLength = (int)m_bindingElement.MaxReceivedMessageSize;
            m_encoder = encoderElem.CreateMessageEncoderFactory().Encoder;
            m_model = model;
            m_isTemporary = isTemporary;
            m_messageQueue = null;
        }


        public override Message Receive(TimeSpan timeout)
        {
            try
            {
                BasicDeliverEventArgs result;

                if (!m_messageQueue.Dequeue(timeout, out result))
                {
                    return null;
                }
#if VERBOSE
                DebugHelper.Start();
#endif
                Message message = m_encoder.ReadMessage(new MemoryStream(result.Body), (int)m_bindingElement.MaxReceivedMessageSize);
                message.Headers.To = LocalAddress.Uri;
#if VERBOSE
                DebugHelper.Stop(" #### Message.Receive {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                        msg.Body.Length,
                        result.Headers.Action.Remove(0, result.Headers.Action.LastIndexOf('/')));
#endif
                return message;
            }
            catch (EndOfStreamException)
            {
                if (m_messageQueue== null || m_messageQueue.ShutdownReason != null && m_messageQueue.ShutdownReason.ReplyCode != CommonFraming.Constants.ReplySuccess)
                {
                    OnFaulted();
                }
                Close();
                return null;
            }
        }

        public override bool TryReceive(TimeSpan timeout, out Message message)
        {
            message = Receive(timeout);
            return message != null;
        }

        public override bool WaitForMessage(TimeSpan timeout)
        {
            return m_messageQueue.WaitForMessage(timeout);
        }

        public override void Close(TimeSpan timeout)
        {
            if (State == CommunicationState.Closed || State == CommunicationState.Closing)
            {
                return; // Ignore the call, we're already closing.
            }

            OnClosing();
#if VERBOSE
            DebugHelper.Start();
#endif
            m_model.BasicCancel(m_messageQueue.ConsumerTag);

#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Close {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnClosed();
        }

        public override void Open(TimeSpan timeout)
        {
            if (State != CommunicationState.Created && State != CommunicationState.Closed)
                throw new InvalidOperationException(string.Format("Cannot open the channel from the {0} state.", base.State));

            OnOpening();
#if VERBOSE
            DebugHelper.Start();
#endif
            string exchange = GetExchangeName(LocalAddress);
            string queue = GetQueueName(LocalAddress);
            string routingKey = GetRoutingKey(LocalAddress);

            if (m_isTemporary)
            {
                queue = m_model.QueueDeclare(queue, false, false, true, null);
            }
            else
            {
                IDictionary args = new Dictionary<String, Object>();

                int ttl;
                if (!string.IsNullOrEmpty(m_bindingElement.TTL) && int.TryParse(m_bindingElement.TTL, out ttl))
                {
                    args.Add("x-message-ttl", ttl);
                }

                //Create a queue for messages destined to this service, bind it to the service URI routing key
                queue = m_model.QueueDeclare(queue, true, false, false, args); 
            }
            
            m_model.QueueBind(queue, exchange, routingKey, null);

            QueueingBasicConsumerBase queueingBasicConsumer;

            // Create queue
            if (m_bindingElement.TransactedReceiveEnabled)
            {
                queueingBasicConsumer = new TransactionalQueueConsumer(m_model);
            }
            else
            {
                queueingBasicConsumer = new QueueingBasicConsumer(m_model);
            }

            m_messageQueue = queueingBasicConsumer;

            //Listen to the queue
            bool noAck = !m_bindingElement.TransactedReceiveEnabled;

            m_model.BasicConsume(queue, noAck, queueingBasicConsumer);

#if VERBOSE
            DebugHelper.Stop(" ## In.Channel.Open {{\n\tAddress={1}, \n\tTime={0}ms}}.", LocalAddress.Uri.PathAndQuery);
#endif
            OnOpened();
        }

    }
}
