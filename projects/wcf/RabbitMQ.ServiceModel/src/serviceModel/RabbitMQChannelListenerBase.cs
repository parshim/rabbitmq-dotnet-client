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
    using System.ServiceModel.Channels;

    public abstract class RabbitMQChannelListenerBase<TChannel> : ChannelListenerBase<TChannel> where TChannel: class, IChannel
    {
        private readonly Uri m_listenUri;
        private readonly BindingContext m_context;
        protected RabbitMQTransportBindingElement m_bindingElement;
        private readonly CommunicationOperation m_closeMethod;
        private readonly CommunicationOperation m_openMethod;
        private readonly CommunicationOperation<TChannel> m_acceptChannelMethod;
        private readonly CommunicationOperation<bool> m_waitForChannelMethod;
        
        protected RabbitMQChannelListenerBase(BindingContext context, Uri listenUri)
        {
            m_context = context;
            m_listenUri = listenUri;
            m_bindingElement = context.Binding.Elements.Find<RabbitMQTransportBindingElement>();
            m_closeMethod = OnClose;
            m_openMethod = OnOpen;
            m_waitForChannelMethod = OnWaitForChannel;
            m_acceptChannelMethod = OnAcceptChannel;
        }
        
        protected override void OnAbort()
        {
            OnClose(m_context.Binding.CloseTimeout);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return m_acceptChannelMethod.BeginInvoke(timeout, callback, state);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return m_acceptChannelMethod.EndInvoke(result);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return m_waitForChannelMethod.BeginInvoke(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return m_waitForChannelMethod.EndInvoke(result);
        }
        
        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return m_closeMethod.BeginInvoke(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return m_openMethod.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            m_closeMethod.EndInvoke(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            m_openMethod.EndInvoke(result);
        }
            
        
        public override Uri Uri
        {
            get { return m_listenUri; }
        }

        protected BindingContext Context
        {
            get { return m_context; }
        }
    }
}
