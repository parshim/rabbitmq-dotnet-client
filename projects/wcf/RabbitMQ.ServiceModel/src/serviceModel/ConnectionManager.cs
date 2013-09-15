using System;
using System.Collections.Generic;
using System.ServiceModel;
using RabbitMQ.Client;

namespace RabbitMQ.ServiceModel
{
    public class ConnectionManager
    {
        private static readonly ConnectionManager m_instance = new ConnectionManager();

        private readonly Dictionary<ConnectionKey, IConnection> m_connections = new Dictionary<ConnectionKey, IConnection>();

        private ConnectionManager()
        {
            
        }

        public static ConnectionManager Instance
        {
            get { return m_instance; }
        }
        
        public void CloseModel(IModel model, TimeSpan timeout)
        {
            model.Close((ushort) CurrentVersion.StatusCodes.Ok, "Goodbye");
        }

        public IModel OpenModel(EndpointAddress address, RabbitMQTransportBindingElement bindingElement, TimeSpan timeout)
        {
            string host = address.Uri.Host;
            int port = address.Uri.IsDefaultPort ? bindingElement.BrokerProtocol.DefaultPort : address.Uri.Port;

            ConnectionKey key = new ConnectionKey(host, port);

            lock (m_connections)
            {
                IConnection connection;
              
                if (m_connections.ContainsKey(key))
                {
                    connection = m_connections[key];
                }
                else
                {
                    connection = OpenConnection(key, bindingElement);

                    m_connections.Add(key, connection);
                }

                IModel model = connection.CreateModel();

                connection.AutoClose = true;

                return model;
            }
        }

        private IConnection OpenConnection(ConnectionKey key, RabbitMQTransportBindingElement bindingElement)
        {
            ConnectionFactory connFactory = new ConnectionFactory
                {
                    HostName = key.Host,
                    Port = key.Port
                };

            if (bindingElement.BrokerProtocol != null)
            {
                connFactory.Protocol = bindingElement.BrokerProtocol;
            }
            if (bindingElement.Username != null)
            {
                connFactory.UserName = bindingElement.Username;
            }
            if (bindingElement.Password != null)
            {
                connFactory.Password = bindingElement.Password;
            }
            if (bindingElement.VirtualHost != null)
            {
                connFactory.VirtualHost = bindingElement.VirtualHost;
            }

            IConnection connection = connFactory.CreateConnection();

            connection.ConnectionShutdown += OnConnectionShutdown;

            return connection;
        }

        private void OnConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
        {
            ConnectionKey key = new ConnectionKey(connection.Endpoint.HostName, connection.Endpoint.Port);

            lock (m_connections)
            {
                if (m_connections.ContainsKey(key))
                {
                    m_connections.Remove(key);
                }

                connection.ConnectionShutdown -= OnConnectionShutdown;
            }
        }
    }

    public class ConnectionKey
    {
        private readonly string m_host;
        private readonly int m_port;

        public ConnectionKey(string host, int port)
        {
            m_host = host;
            m_port = port;
        }

        public string Host
        {
            get { return m_host; }
        }

        public int Port
        {
            get { return m_port; }
        }

        protected bool Equals(ConnectionKey other)
        {
            return string.Equals(m_host, other.m_host) && string.Equals(m_port, other.m_port);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConnectionKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_host.GetHashCode()*397) ^ m_port.GetHashCode();
            }
        }
    }
}