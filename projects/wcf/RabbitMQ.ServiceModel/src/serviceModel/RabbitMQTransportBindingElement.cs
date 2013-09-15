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
    using System.ServiceModel.Channels;

    using Client;

    /// <summary>
    /// Represents the binding element used to specify AMQP transport for transmitting messages.
    /// </summary>
    public sealed class RabbitMQTransportBindingElement : TransportBindingElement, ITransactedBindingElement
    {
        private string m_routingKey;

        /// <summary>
        /// Creates a new instance of the RabbitMQTransportBindingElement Class using the default protocol.
        /// </summary>
        public RabbitMQTransportBindingElement()
        {
            MaxReceivedMessageSize = RabbitMQBinding.DefaultMaxMessageSize;
        }

        private RabbitMQTransportBindingElement(RabbitMQTransportBindingElement other)
        {
            BrokerProtocol = other.BrokerProtocol;
            Username = other.Username;
            Password = other.Password;
            VirtualHost = other.VirtualHost;
            MaxReceivedMessageSize = other.MaxReceivedMessageSize;
            TransactedReceiveEnabled = other.TransactedReceiveEnabled;
            TTL = other.TTL;
            PersistentDelivery = other.PersistentDelivery;
            RoutingKey = other.RoutingKey;
            AutoBindExchange = other.AutoBindExchange;
        }
        
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return (IChannelFactory<TChannel>)(object)new RabbitMQChannelFactory(context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            return (IChannelListener<TChannel>)((object)new RabbitMQChannelListener(context));
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IOutputChannel);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IInputChannel);
        }

        public override BindingElement Clone()
        {
            return new RabbitMQTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return context.GetInnerProperty<T>();
        }

        /// <summary>
        /// Gets the scheme used by the binding
        /// </summary>
        public override string Scheme
        {
            get { return CurrentVersion.Scheme; }
        }
        
        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        public bool TransactedReceiveEnabled
        {
            get; set;
        }

        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        public string TTL
        {
            get; set;
        }

        /// <summary>
        /// The largest receivable encoded message
        /// </summary>
        public override long MaxReceivedMessageSize
        {
            get; set;
        }

        /// <summary>
        /// The username  to use when authenticating with the broker
        /// </summary>
        internal string Username
        {
            get; set;
        }

        /// <summary>
        /// Password to use when authenticating with the broker
        /// </summary>
        internal string Password
        {
            get; set;
        }

        /// <summary>
        /// Specifies the broker virtual host
        /// </summary>
        internal string VirtualHost
        {
            get; set;
        }

        /// <summary>
        /// Specifies the version of the AMQP protocol that should be used to 
        /// communicate with the broker
        /// </summary>
        public IProtocol BrokerProtocol
        {
            get; set;
        }

        /// <summary>
        /// Routing key for message dispatching
        /// </summary>
        public string RoutingKey
        {
            get { return m_routingKey ?? ""; }
            set { m_routingKey = value; }
        }

        /// <summary>
        /// Exchange name to bind the listening queue. Value can be null.
        /// </summary>
        /// <remarks>If null queue will not be binded automaticaly</remarks>
        public string AutoBindExchange
        {
            get; set;
        }

        /// <summary>
        /// Defines messages delivery mode
        /// </summary>
        public bool PersistentDelivery
        {
            get; set;
        }
    }
}
