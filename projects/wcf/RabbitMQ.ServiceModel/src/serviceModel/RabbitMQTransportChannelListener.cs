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
    
    internal sealed class RabbitMQTransportChannelListener<T> : RabbitMQChannelListenerBase<T> where T : class, IChannel
    {
        private readonly bool m_autoDelete;
        private readonly string m_bindToExchange;
        private IChannel m_channel;
        
        internal RabbitMQTransportChannelListener(BindingContext context, Uri listenUri, bool autoDelete, string bindToExchange)
            : base(context, listenUri)
        {
            m_autoDelete = autoDelete;
            m_bindToExchange = bindToExchange;
            m_channel = null;
        }

        protected override T OnAcceptChannel(TimeSpan timeout)
        {
            // Since only one connection to a broker is required (even for communication
            // with multiple exchanges 
            if (m_channel != null)
                return null;

            if (typeof (T) == typeof (IInputChannel))
            {
                m_channel = new RabbitMQTransportInputChannel(Context, new EndpointAddress(Uri.ToString()), m_autoDelete, m_bindToExchange);
            }
            else
            {
                return null;
            }

            m_channel.Closed += ListenChannelClosed;

            return (T) m_channel;
        }
        
        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return false;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
#if VERBOSE
            DebugHelper.Start();
#endif  
            if (m_channel != null)
            {
                m_channel.Close();
                m_channel = null;
            }
#if VERBOSE
            DebugHelper.Stop(" ## In.Close {{Time={0}ms}}.");
#endif
        }

        private void ListenChannelClosed(object sender, EventArgs args)
        {
            ((IInputChannel)sender).Closed -= ListenChannelClosed;

            Close();
        }
}
}
