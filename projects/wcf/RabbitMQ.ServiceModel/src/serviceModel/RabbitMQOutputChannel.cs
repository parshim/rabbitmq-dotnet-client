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


namespace RabbitMQ.ServiceModel
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Client;

    internal sealed class RabbitMQOutputChannel : RabbitMQOutputChannelBase
    {
        private readonly RabbitMQTransportBindingElement m_bindingElement;
        private readonly MessageEncoder m_encoder;
        private IModel m_model;

        public RabbitMQOutputChannel(BindingContext context, EndpointAddress address)
            : base(context, address)
        {
            //m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            MessageEncodingBindingElement encoderElement = context.Binding.Elements.Find<MessageEncodingBindingElement>();
            if (encoderElement != null) {
                m_encoder = encoderElement.CreateMessageEncoderFactory().Encoder;
            }

            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
        }

        public override void Send(Message message, TimeSpan timeout)
        {
            if (message.State != MessageState.Closed)
            {
                
#if VERBOSE
                DebugHelper.Start();
#endif
                byte[] body;

                
                string exchange = GetExchangeName(RemoteAddress);

                string routingKey = m_bindingElement.RoutingKey;

                IBasicProperties basicProperties = m_model.CreateBasicProperties();

                basicProperties.Timestamp = new AmqpTimestamp(DateTime.Now);
                basicProperties.ContentType = "SOAP";

                // TODO: read custom headers and put it into the message properties
                //foreach (MessageHeaderInfo messageHeaderInfo in message.Headers)
                //{
                //    basicProperties.Headers.Add(messageHeaderInfo.Name, "");
                //}

                // TODO: move message persistency to transport configuration
                //basicProperties.SetPersistent(false);

                if (!string.IsNullOrEmpty(m_bindingElement.TTL))
                {
                    basicProperties.Expiration = m_bindingElement.TTL;
                }

                using (MemoryStream str = new MemoryStream())
                {
                    m_encoder.WriteMessage(message, str);
                    body = str.ToArray();
                }

                m_model.BasicPublish(exchange,
                                     routingKey,
                                     basicProperties,
                                     body);

#if VERBOSE
                DebugHelper.Stop(" #### Message.Send {{\n\tAction={2}, \n\tBytes={1}, \n\tTime={0}ms}}.",
                    body.Length,
                    message.Headers.Action.Remove(0, message.Headers.Action.LastIndexOf('/')));
#endif
            }
        }

        public override void Close(TimeSpan timeout)
        {
            if (State == CommunicationState.Closed || State == CommunicationState.Closing)
                return; // Ignore the call, we're already closing.

            OnClosing();

#if VERBOSE
            DebugHelper.Start();
#endif

            if (m_model != null)
            {
                ConnectionManager.Instance.CloseModel(m_model, timeout);
                m_model = null;
            }

#if VERBOSE
            DebugHelper.Stop(" ## Out.Close {{Time={0}ms}}.");
#endif
            OnClosed();
        }

        public override void Open(TimeSpan timeout)
        {
            if (State != CommunicationState.Created && State != CommunicationState.Closed)
                throw new InvalidOperationException(string.Format("Cannot open the channel from the {0} state.", State));

            OnOpening();
#if VERBOSE
            DebugHelper.Start();
#endif
            m_model = ConnectionManager.Instance.OpenModel(RemoteAddress, m_bindingElement, timeout);
#if VERBOSE
            DebugHelper.Stop(" ## Out.Open {{Time={0}ms}}.");
#endif
            OnOpened();
        }
    }
}
