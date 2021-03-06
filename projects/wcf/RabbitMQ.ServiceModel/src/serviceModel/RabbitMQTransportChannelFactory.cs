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
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    
    internal sealed class RabbitMQTransportChannelFactory<T> : ChannelFactoryBase<T>
    {
        private readonly BindingContext m_context;
        private readonly CommunicationOperation m_openMethod;
        
        public RabbitMQTransportChannelFactory(BindingContext context)
        {
            m_context = context;
            m_openMethod = Open;
        }

        protected override T OnCreateChannel(EndpointAddress address, Uri via)
        {
            IChannel channel;

            if (typeof (T) == typeof (IOutputChannel))
            {    
                channel = new RabbitMQTransportOutputChannel(m_context, address);
            }
            else
            {
                return default(T);
            }

            return (T) channel;
        }
        
        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return m_openMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            m_openMethod.EndInvoke(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {

        }

        protected override void OnClose(TimeSpan timeout)
        {

        }

        protected override void OnAbort()
        {
            base.OnAbort();
            OnClose(m_context.Binding.CloseTimeout);
        }
    }
}
