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
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using RabbitMQ.Client;
    using System.Reflection;

    /// <summary>
    /// Represents the configuration for a RabbitMQBinding.
    /// </summary>
    /// <remarks>
    /// This configuration element should be imported into the client
    /// and server configuration files to provide declarative configuration 
    /// of a AMQP bound service.
    /// </remarks>
    public sealed class RabbitMQBindingConfigurationElement : StandardBindingElement
    {
        /// <summary>
        /// Creates a new instance of the RabbitMQBindingConfigurationElement
        /// Class initialized with values from the specified configuration.
        /// </summary>
        /// <param name="configurationName"></param>
        public RabbitMQBindingConfigurationElement(string configurationName)
            : base(configurationName) {
        }
     
        /// <summary>
        /// Creates a new instance of the RabbitMQBindingConfigurationElement Class.
        /// </summary>
        public RabbitMQBindingConfigurationElement()
            : this(null) {
        }


        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            RabbitMQBinding rabbind = binding as RabbitMQBinding;
            if (rabbind != null)
            {
                this.MaxMessageSize = rabbind.MaxMessageSize;
                this.ExactlyOnce = rabbind.ExactlyOnce;
                this.TTL = rabbind.TTL;
            }
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            RabbitMQBinding rabbind = binding as RabbitMQBinding;
            if (rabbind == null)
            {
                throw new ArgumentException(
                    string.Format("Invalid type for binding. Expected {0}, Passed: {1}", 
                        typeof(RabbitMQBinding).AssemblyQualifiedName, 
                        binding.GetType().AssemblyQualifiedName));
            }

            rabbind.AutoBindExchange = this.AutoBindExchange;
            rabbind.BrokerProtocol = this.Protocol;
            rabbind.ExactlyOnce = this.ExactlyOnce;
            rabbind.Transport.MaxReceivedMessageSize = this.MaxMessageSize;
            rabbind.OneWayOnly = this.OneWayOnly;
            rabbind.PersistentDelivery = this.PersistentDelivery;
            rabbind.ReplyToExchange = this.ReplyToExchange == null ? null : new Uri(this.ReplyToExchange);
            rabbind.ReplyToQueue = this.ReplyToQueue;
            rabbind.TTL = this.TTL;
        }

        /// <summary>
        /// Enables transactional message delivery
        /// </summary>
        [ConfigurationProperty("exactlyOnce")]
        public bool ExactlyOnce
        {
            get { return ((bool)base["exactlyOnce"]); }
            set { base["exactlyOnce"] = value; }
        }
        
        /// <summary>
        /// Defines messages delivery mode
        /// </summary>
        [ConfigurationProperty("persistentDelivery")]
        public bool PersistentDelivery
        {
            get { return ((bool)base["persistentDelivery"]); }
            set { base["persistentDelivery"] = value; }
        }

        /// <summary>
        /// Defines if one way or duplex comunication is required over this binding
        /// </summary>
        [ConfigurationProperty("oneWayOnly", DefaultValue = true)]
        public bool OneWayOnly
        {
            get { return ((bool)base["oneWayOnly"]); }
            set { base["oneWayOnly"] = value; }
        }

        /// <summary>
        /// ReplyTo exchange URI for duplex communication callbacks
        /// </summary>
        [ConfigurationProperty("replyToExchange", DefaultValue = null)]
        public string ReplyToExchange
        {
            get { return ((string)base["replyToExchange"]); }
            set { base["replyToExchange"] = value; }
        }

        /// <summary>
        /// ReplyTo queue name for duplex communication
        /// </summary>
        /// <remarks>If null will auto delete queue will be generated</remarks>
        [ConfigurationProperty("replyToQueue", DefaultValue = null)]
        public string ReplyToQueue
        {
            get { return ((string)base["replyToQueue"]); }
            set { base["replyToQueue"] = value; }
        }

        /// <summary>
        /// Exchange name to bind the listening queue. Value can be null.
        /// </summary>
        /// <remarks>If null queue will not be binded automaticaly</remarks>
        [ConfigurationProperty("autoBindExchange", DefaultValue = null)]
        public string AutoBindExchange
        {
            get { return ((string)base["autoBindExchange"]); }
            set { base["autoBindExchange"] = value; }
        }
        
        /// <summary>
        /// Specifies message TTL. For client side binding it will be per message TTL, for service side binding it will be per-queue message TTL. Use null or discard to diable message TTL.
        /// </summary>
        [ConfigurationProperty("TTL", DefaultValue = null)]
        public string TTL
        {
            get { return ((string)base["TTL"]); }
            set { base["TTL"] = value; }
        }
        
        /// <summary>
        /// Specifies the protocol version to use when communicating with the broker
        /// </summary>
        [ConfigurationProperty("protocolversion", DefaultValue = "DefaultProtocol")]
        public string ProtocolVersion
        {
            get {
                return ((string)base["protocolversion"]);
            }
            set {
                base["protocolversion"] = value;
                GetProtocol();
            }
        }

        /// <summary>
        /// Specifies the maximum encoded message size
        /// </summary>
        [ConfigurationProperty("maxmessagesize", DefaultValue = 8192L)]
        public long MaxMessageSize
        {
            get { return (long)base["maxmessagesize"]; }
            set { base["maxmessagesize"] = value; }
        }

        private IProtocol GetProtocol() {
            IProtocol result = Protocols.Lookup(this.ProtocolVersion);
            if (result == null) {
                throw new ConfigurationErrorsException(string.Format("'{0}' is not a valid AMQP protocol name",
                                                                     this.ProtocolVersion));
            }
            return result;
        }

        /// <summary>
        /// Gets the protocol version specified by the current configuration
        /// </summary>
        public IProtocol Protocol
        {
            get {
                return GetProtocol();
            }
        }

        protected override Type BindingElementType
        {
            get { return typeof(RabbitMQBinding); }
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
