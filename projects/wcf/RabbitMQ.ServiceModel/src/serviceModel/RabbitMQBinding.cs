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
        private CompositeDuplexBindingElement m_compositeDuplex;
        private TextMessageEncodingBindingElement m_encoding;
        private bool m_isInitialized;
        private ReliableSessionBindingElement m_session;
        private TransactionFlowBindingElement m_transactionFlow;
        private bool m_transactionsEnabled;
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
        /// <param name="username">The broker username to connect with</param>
        /// <param name="password">The broker password to connect with</param>
        /// <param name="virtualhost">The broker virtual host</param>
        /// <param name="maxMessageSize">The largest allowable encoded message size</param>
        /// <param name="protocol">The protocol version to use</param>
        public RabbitMQBinding(String username, String password,  String virtualhost,
                               long maxMessageSize, IProtocol protocol)
            : this(protocol)
        {
            this.Transport.Username = username;
            this.Transport.Password = password;
            this.Transport.VirtualHost = virtualhost;
            this.Transport.TransactedReceiveEnabled = false;
            this.MaxMessageSize = maxMessageSize;
        }

        /// <summary>
        /// Uses the specified protocol. The broker must be set before use.
        /// </summary>
        /// <param name="protocol">The protocol version to use</param>
        public RabbitMQBinding(IProtocol protocol)
        {
            BrokerProtocol = protocol;
            base.Name = "RabbitMQBinding";
            base.Namespace = "http://schemas.rabbitmq.com/2007/RabbitMQ/";

            Initialize();

            this.TransactionFlow = true;
        }

        public override BindingElementCollection CreateBindingElements()
        {
            m_transport.BrokerProtocol = this.BrokerProtocol;
            m_transport.TransactedReceiveEnabled = this.ExactlyOnce;
            m_transport.TTL = this.TTL;
            m_transport.RoutingKey = this.RoutingKey;
            if (MaxMessageSize != DefaultMaxMessageSize)
            {
                m_transport.MaxReceivedMessageSize = MaxMessageSize;
            }
            BindingElementCollection elements = new BindingElementCollection();

            if (m_transactionsEnabled)
            {
                elements.Add(m_transactionFlow);
            }
            if (!OneWayOnly)
            {
                elements.Add(m_session);
                elements.Add(m_compositeDuplex);
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
                    m_encoding = new TextMessageEncodingBindingElement(); // new TextMessageEncodingBindingElement();
                    m_session = new ReliableSessionBindingElement();
                    m_compositeDuplex = new CompositeDuplexBindingElement();
                    m_transactionFlow = new TransactionFlowBindingElement();
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
        /// Gets the reliable session parameters for this binding instance
        /// </summary>
        public ReliableSession ReliableSession
        {
            get { return new ReliableSession(m_session); }
        }

        /// <summary>
        /// Determines whether or not the TransactionFlowBindingElement will
        /// be added to the channel stack
        /// </summary>
        public bool TransactionFlow
        {
            get { return m_transactionsEnabled; }
            set { m_transactionsEnabled = value; }
        }

        /// <summary>
        /// Specifies whether or not the CompositeDuplex and ReliableSession
        /// binding elements are added to the channel stack.
        /// </summary>
        public bool OneWayOnly { get; set; }

        /// <summary>
        /// Message routing key
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        public bool ExactlyOnce { get; set; }

        /// <summary>
        /// Message time to live
        /// </summary>
        public string TTL { get; set; }
    }
}
