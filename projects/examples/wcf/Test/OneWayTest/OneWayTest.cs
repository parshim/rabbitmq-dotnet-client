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


using System.ServiceModel.Description;

namespace RabbitMQ.ServiceModel.Test.OneWayTest
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using RabbitMQ.ServiceModel;
    using System.Threading;

    public class OneWayTest : IServiceTest<ILogServiceContract>
    {
        private ChannelFactory<ILogServiceContract> m_factory;
        private ServiceHost m_host;
        private ServiceHost m_alternativeHost;
        private bool m_serviceStarted;
        private ILogServiceContract m_client;
        private Uri m_baseAddresses = new Uri("soap.amqp://amq.direct:2020/");

        public void BeginRun()
        {
            StartService(Program.GetBinding());
        
            m_client = GetClient(Program.GetBinding());
            m_client.Log(new LogData(LogLevel.High, "Hello Rabbit"));
            m_client.Log(new LogData(LogLevel.Medium, "Hello Rabbit"));
            m_client.Log(new LogData(LogLevel.Low, "Hello Rabbit"));
            m_client.Log(new LogData(LogLevel.Low, "Last Message"));

        }

        public void Run()
        {
            BeginRun();
            Thread.Sleep(2500);
            EndRun();
        }

        public void EndRun()
        {
            StopClient(m_client);
            Thread.Sleep(2500);
            StopService();
        }

        public void StartService(Binding binding)
        {
            m_host = new ServiceHost(typeof (LogService), m_baseAddresses);
            m_alternativeHost = new ServiceHost(typeof(AlternativeLogService), m_baseAddresses);

            StartService((RabbitMQBinding) binding, m_host, "LogService");

            StartService(Program.GetBinding(), m_alternativeHost, "AnotherLog");

            m_serviceStarted = true;
        }

        private static void StartService(RabbitMQBinding binding, ServiceHost host, string address)
        {
            Util.Write(ConsoleColor.Yellow, "  Binding Service...");

            binding.OneWayOnly = true;
            binding.TransactionFlow = false;
            binding.ExactlyOnce = true;

            ServiceEndpoint se = host.AddServiceEndpoint(typeof (ILogServiceContract), binding, address);

            //se.Behaviors.Add(new TransactedBatchingBehavior(100));

            host.Open();
            
            Thread.Sleep(500);
            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public void StopService()
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Service...");
            if (m_serviceStarted)
            {
                m_host.Close();
                m_serviceStarted = false;
            }

            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }

        public ILogServiceContract GetClient(Binding binding)
        {
            ((RabbitMQBinding)binding).OneWayOnly = true;
            ((RabbitMQBinding)binding).TransactionFlow = false;
            m_factory = new ChannelFactory<ILogServiceContract>(binding, new EndpointAddress(m_baseAddresses));
           
            m_factory.Open();
            return m_factory.CreateChannel();
        }

        public void StopClient(ILogServiceContract client)
        {
            Util.Write(ConsoleColor.Yellow, "  Stopping Client...");

            ((IClientChannel)client).Close();
            m_factory.Close();

            Util.WriteLine(ConsoleColor.Green, "[DONE]");
        }
    }
}
