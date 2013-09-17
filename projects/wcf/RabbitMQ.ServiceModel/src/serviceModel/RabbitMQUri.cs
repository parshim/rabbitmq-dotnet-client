using System;

namespace RabbitMQ.ServiceModel
{
    internal sealed class RabbitMQUriWellKnownQueryKeys
    {
        public const string RoutingKey = "routingKey";
    }

    internal sealed class RabbitMQUri
    {
        private readonly Uri m_uri;

        public RabbitMQUri(Uri uri)
        {
            m_uri = uri;
        }

        public string Schema
        {
            get { return m_uri.Scheme; }
        }
        
        public string Username
        {
            get
            {
                string userInfo = m_uri.UserInfo;

                if (string.IsNullOrEmpty(userInfo)) return null;

                int index = userInfo.IndexOf(':');

                if (index <= 0) return userInfo;

                return userInfo.Substring(0, index + 1);
            }
        }
        
        public string Password
        {
            get
            {
                string userInfo = m_uri.UserInfo;

                if (string.IsNullOrEmpty(userInfo)) return null;

                int index = userInfo.IndexOf(':');

                if (index <= 0) return "";

                return userInfo.Substring(index + 1);
            }
        }

        public string Host
        {
            get { return m_uri.Host; }
        }

        public int? Port
        {
            get { return m_uri.IsDefaultPort ? null : (int?)m_uri.Port; }
        }

        public string VirtualHost
        {
            get
            {
                string[] segments = m_uri.Segments;

                // First segment is always /
                // If there is only one actual segment it will represent exchange or queue name
                if (segments.Length <= 2) return null;

                // If there is more then one actual segment first will always be virtual host
                return segments[1].Trim('/');
            }
        }

        public string Endpoint
        {
            get
            {
                string[] segments = m_uri.Segments;

                // First segment is always /
                // If there is only one actual segment it will represent exchange or queue name
                if (segments.Length <= 1) return null;

                if (segments.Length == 2)
                {
                    return segments[1].Trim('/');
                }

                return segments[2].Trim('/');
            }
        }

        public string this[string queryKey]
        {
            get
            {
                string query = m_uri.Query;

                if (string.IsNullOrEmpty(query)) return "";

                foreach (string pair in query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] strings = pair.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                    if (strings[0] == queryKey) return strings[1];
                }

                return "";
            }
        }

        public string RoutingKey
        {
            get { return this[RabbitMQUriWellKnownQueryKeys.RoutingKey]; }
        }

    }
}
