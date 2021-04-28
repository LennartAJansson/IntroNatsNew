using NATS.Client;

using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace NatsConsumer.Nats
{
    public class NatsConnection : IDisposable
    {
        private const string CLIENTCERT = "./tls/nats.pfx";
        private const string SERVERCERT = "./tls/servercert";
        private const string PASSWORD = "insecurePassword1";

        private readonly string clientCert;
        private readonly string serverCert;

        private readonly Options options;
        private readonly ConnectionFactory factory = new ConnectionFactory();
        private readonly IConnection connection = null;
        private bool _disposed = false;

        public NatsConnection(string connectionString)
        {
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            clientCert = Path.GetFullPath(Path.Combine(workingDirectory, CLIENTCERT));
            serverCert = Path.GetFullPath(Path.Combine(workingDirectory, SERVERCERT));

            options = ConnectionFactory.GetDefaultOptions();
            options.Url = connectionString;
            options.Timeout = 10000;
            options.Verbose = true;
            if (File.Exists(serverCert))
            {
                options.Secure = true;
                X509Certificate2 cert = new X509Certificate2(clientCert, PASSWORD);
                options.AddCertificate(cert);
                options.TLSRemoteCertificationValidationCallback = TLSRemoteCertificationValidationCallback;
            }
            options.ClosedEventHandler = ClosedEventHandler;
            options.DisconnectedEventHandler = DisconnectedEventHandler;

            connection = factory.CreateConnection(options);
            Console.WriteLine($"NATS Server got connected");
        }

        private void ClosedEventHandler(object sender, ConnEventArgs e)
        {
            Console.WriteLine("NATS Server got closed");

            //throw new NATSConnectionClosedException();
        }

        private void DisconnectedEventHandler(object sender, ConnEventArgs e)
        {
            Console.WriteLine("NATS Server got disconnected");

            //throw new NATSConnectionClosedException();
        }

        private bool TLSRemoteCertificationValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            X509Certificate cert = new X509Certificate(serverCert);

            return cert.Issuer.Equals(certificate.Issuer);
        }

        public IConnection GetConnection() => connection;

        public void CloseConnection()
        {
            connection.Opts.ClosedEventHandler -= ClosedEventHandler;
            connection.Opts.DisconnectedEventHandler -= DisconnectedEventHandler;
            connection?.Drain();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                connection?.Dispose();
            }

            _disposed = true;
        }
    }
}