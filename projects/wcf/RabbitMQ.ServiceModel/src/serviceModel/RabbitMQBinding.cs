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


//TODO: Rename to RabbitMQBinding
namespace RabbitMQ.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using Client;

    /// <summary>
    /// A windows communication foundation binding over AMQP
    /// </summary>
    public sealed class RabbitMQBinding : Binding
    {
        private long m_maxMessageSize;
        private bool m_isInitialized;

        private CompositeDuplexBindingElement m_duplex;
        private TextMessageEncodingBindingElement m_encoding;
        private RabbitMQTransportBindingElement m_transport;
        
        public static readonly long DefaultMaxMessageSize = 8192L;

        /// <summary>
        /// Creates a new instance of the RabbitMQBinding class initialized
        /// to use the Protocols.DefaultProtocol. The broker must be set
        /// before use.
        /// </summary>
        public RabbitMQBinding()
            : this(Protocols.DefaultProtocol)
        { }
        
        /// <summary>
        /// Uses the broker, login and protocol specified
        /// </summary>
       /// <param name="maxMessageSize">The largest allowable encoded message size</param>
        /// <param name="protocol">The protocol version to use</param>
        public RabbitMQBinding(long maxMessageSize, IProtocol protocol)
            : this(protocol)
        {
            this.MaxMessageSize = maxMessageSize;
        }

        /// <summary>
        /// Uses the specified protocol. The broker must be set before use.
        /// </summary>
        /// <param name="protocol">The protocol version to use</param>
        public RabbitMQBinding(IProtocol protocol)
        {
            BrokerProtocol = protocol;

            // Set defaults
            this.OneWayOnly = true;
            this.ExactlyOnce = false;

            base.Name = "RabbitMQBinding";
            base.Namespace = "http://schemas.rabbitmq.com/2007/RabbitMQ/";

            Initialize();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            m_transport.BrokerProtocol = this.BrokerProtocol;
            m_transport.TransactedReceiveEnabled = this.ExactlyOnce;
            m_transport.TTL = this.TTL;
            m_transport.PersistentDelivery = this.PersistentDelivery;
            m_transport.AutoBindExchange = this.AutoBindExchange;
            m_transport.ReplyToQueue = this.ReplyToQueue;
            m_transport.ReplyToExchange = this.ReplyToExchange;
            m_transport.OneWayOnly = this.OneWayOnly;
            
            if (MaxMessageSize != DefaultMaxMessageSize)
            {
                m_transport.MaxReceivedMessageSize = MaxMessageSize;
            }

            BindingElementCollection elements = new BindingElementCollection();

            if (!OneWayOnly)
            {
                elements.Add(m_duplex);
            }
            elements.Add(m_encoding);
            elements.Add(m_transport);

            return elements;
        }
        
        private void Initialize()
        {
            lock (this)
            {
                if (!m_isInitialized)
                {
                    m_transport = new RabbitMQTransportBindingElement();
                    m_encoding = new TextMessageEncodingBindingElement();
                    m_duplex = new CompositeDuplexBindingElement();

                    m_maxMessageSize = DefaultMaxMessageSize;
                    m_isInitialized = true;
                }
            }
        }

        /// <summary>
        /// Gets the scheme used by the binding
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        public long MaxMessageSize
        {
            get { return m_maxMessageSize; }
            set { m_maxMessageSize = value; }
        }

        /// <summary>
        /// Specifies the version of the AMQP protocol that should be used to communicate with the broker
        /// </summary>
        public IProtocol BrokerProtocol { get; set; }

        /// <summary>
        /// Gets the AMQP transport binding element
        /// </summary>
        public RabbitMQTransportBindingElement Transport
        {
            get { return m_transport; }
        }
        
        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        public bool ExactlyOnce { get; set; }

        /// <summary>
        /// Message time to live
        /// </summary>
        public string TTL { get; set; }

        /// <summary>
        /// ReplyTo queue name for duplex communication
        /// </summary>
        /// <remarks>If null will auto delete queue will be generated</remarks>
        public string ReplyToQueue { get; set; }

        /// <summary>
        /// ReplyTo exchange URI for duplex communication callbacks
        /// </summary>
        public Uri ReplyToExchange { get; set; }

        /// <summary>
        /// Exchange name to bind the listening queue. Value can be null.
        /// </summary>
        /// <remarks>If null queue will not be binded automaticaly</remarks>
        public string AutoBindExchange { get; set; }

        /// <summary>
        /// Defines messages delivery mode
        /// </summary>
        public bool PersistentDelivery { get; set; }

        /// <summary>
        /// Defines if one way or duplex comunication is required over this binding
        /// </summary>
        public bool OneWayOnly { get; set; }
    }
}
