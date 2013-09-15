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

using System;
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using RabbitMQ.Client;

namespace RabbitMQ.ServiceModel
{
    public sealed class RabbitMQTransportElement : TransportElement
    {

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            if (bindingElement == null)
                throw new ArgumentNullException("binding");

            RabbitMQTransportBindingElement rabbind = bindingElement as RabbitMQTransportBindingElement;
            if (rabbind == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid type for binding. Expected {0}, Passed: {1}",
                        typeof(RabbitMQBinding).AssemblyQualifiedName,
                        bindingElement.GetType().AssemblyQualifiedName));
            }

            rabbind.PersistentDelivery = this.PersistentDelivery;
            rabbind.RoutingKey = this.RoutingKey;
            rabbind.AutoBindExchange = this.AutoBindExchange;
            rabbind.TTL = this.TTL;
            rabbind.BrokerProtocol = this.Protocol;
            rabbind.Password = this.Password;
            rabbind.Username = this.Username;
            rabbind.VirtualHost = this.VirtualHost;
            rabbind.TransactedReceiveEnabled = this.ExactlyOnce;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            RabbitMQTransportElement element = from as RabbitMQTransportElement;
            if (element != null)
            {
                this.PersistentDelivery = this.PersistentDelivery;
                this.RoutingKey = this.RoutingKey;
                this.AutoBindExchange = this.AutoBindExchange;
                this.TTL = element.TTL;
                this.ProtocolVersion = element.ProtocolVersion;
                this.Password = element.Password;
                this.Username = element.Username;
                this.VirtualHost = element.VirtualHost;
                this.ExactlyOnce = element.ExactlyOnce;
            }
        }

        protected override BindingElement CreateBindingElement()
        {
            TransportBindingElement element = CreateDefaultBindingElement();
            this.ApplyConfiguration(element);
            return element;
        }

        protected override System.ServiceModel.Channels.TransportBindingElement CreateDefaultBindingElement()
        {
            return new RabbitMQTransportBindingElement();
        }

        protected override void InitializeFrom(System.ServiceModel.Channels.BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);

            if (bindingElement == null)
                throw new ArgumentNullException("binding");

            RabbitMQTransportBindingElement rabbind = bindingElement as RabbitMQTransportBindingElement;
            if (rabbind == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid type for binding. Expected {0}, Passed: {1}",
                        typeof(RabbitMQBinding).AssemblyQualifiedName,
                        bindingElement.GetType().AssemblyQualifiedName));
            }

            this.PersistentDelivery = rabbind.PersistentDelivery;
            this.RoutingKey = rabbind.RoutingKey;
            this.AutoBindExchange = rabbind.AutoBindExchange;
            this.TTL = rabbind.TTL;
            this.ProtocolVersion = rabbind.BrokerProtocol.ApiName;
            this.Password = rabbind.Password;
            this.Username = rabbind.Username;
            this.VirtualHost = rabbind.VirtualHost;
            
        }

        public override System.Type BindingElementType
        {
            get { return typeof(RabbitMQTransportElement); }
        }

        /// <summary>
        /// Specifies the hostname of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("routingKey", IsRequired = true, DefaultValue = "")]
        public string RoutingKey
        {
            get { return ((string)base["routingKey"]); }
            set { base["routingKey"] = value; }
        }

        [ConfigurationProperty("autoBindExchange", IsRequired = true, DefaultValue = "")]
        public string AutoBindExchange
        {
            get { return ((string)base["autoBindExchange"]); }
            set { base["autoBindExchange"] = value; }
        }

        [ConfigurationProperty("persistentDelivery", IsRequired = false, DefaultValue = false)]
        public bool PersistentDelivery
        {
            get { return ((bool)base["persistentDelivery"]); }
            set { base["persistentDelivery"] = value; }
        }

        /// <summary>
        /// Specifies the port of the broker that the binding should connect to.
        /// </summary>
        [ConfigurationProperty("TTL", IsRequired = false, DefaultValue = "")]
        public string TTL
        {
            get { return ((string)base["TTL"]); }
            set { base["TTL"] = value; }
        }

        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        [ConfigurationProperty("exactlyOnce", IsRequired = false, DefaultValue = false)]
        public bool ExactlyOnce
        {
            get { return ((bool)base["exactlyOnce"]); }
            set { base["exactlyOnce"] = value; }
        }

        /// <summary>
        /// Password to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = ConnectionFactory.DefaultPass)]
        public string Password
        {
            get { return ((string)base["password"]); }
            set { base["password"] = value; }
        }

        /// <summary>
        /// The username  to use when authenticating with the broker
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = ConnectionFactory.DefaultUser)]
        public string Username
        {
            get { return ((string)base["username"]); }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Specifies the protocol version to use when communicating with the broker
        /// </summary>
        [ConfigurationProperty("protocolversion", DefaultValue = "DefaultProtocol")]
        public string ProtocolVersion
        {
            get
            {
                return ((string)base["protocolversion"]);
            }
            set
            {
                base["protocolversion"] = value;
                GetProtocol();
            }
        }

        private IProtocol GetProtocol()
        {
            IProtocol result = Protocols.Lookup(this.ProtocolVersion);
            if (result == null)
            {
                throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid AMQP protocol name",
                                                                     this.ProtocolVersion));
            }
            return result;
        }

        /// <summary>
        /// Gets the protocol version specified by the current configuration
        /// </summary>
        public IProtocol Protocol { get { return GetProtocol(); } }

        /// <summary>
        /// The virtual host to access.
        /// </summary>
        [ConfigurationProperty("virtualHost", DefaultValue = ConnectionFactory.DefaultVHost)]
        public string VirtualHost
        {
            get { return ((string)base["virtualHost"]); }
            set { base["virtualHost"] = value; }
        }

        /// <summary>
        /// The largest receivable encoded message
        /// </summary>
        public new long MaxReceivedMessageSize
        {
            get { return MaxReceivedMessageSize; }
            set { MaxReceivedMessageSize = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection configProperties = base.Properties;
                foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.DeclaredOnly
                                                                           | BindingFlags.Public
                                                                           | BindingFlags.Instance))
                {
                    foreach (ConfigurationPropertyAttribute attr in prop.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false))
                    {
                        configProperties.Add(
                            new ConfigurationProperty(attr.Name, prop.PropertyType, attr.DefaultValue));
                    }
                }

                return configProperties;
            }
        }

       
    }
}
