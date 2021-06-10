using NATS.Client;

using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace NatsApi.Nats
{
    public class NatsConnection : IDisposable
    {
        private const string clientCertificatePath = "./tls/nats.pfx";
        private const string serverCertificatePath = "./tls/servercert";
        private const string password = "insecurePassword1";

        private readonly string clientCert;
        private readonly string serverCert;

        private readonly Options options;
        private readonly ConnectionFactory factory = new();
        private readonly IConnection connection = null;
        private bool disposed = false;

        public NatsConnection(string connectionString)
        {
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            clientCert = Path.GetFullPath(Path.Combine(workingDirectory, clientCertificatePath));
            serverCert = Path.GetFullPath(Path.Combine(workingDirectory, serverCertificatePath));

            options = ConnectionFactory.GetDefaultOptions();
            options.Url = connectionString;
            options.Timeout = 10000;
            options.Verbose = true;
            Thread.Sleep(100);
            if (File.Exists(serverCert))
            {
                options.Secure = true;
                X509Certificate2 cert = new(clientCert, password);
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

            X509Certificate cert = new(serverCert);

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
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                connection?.Dispose();
            }

            disposed = true;
        }
    }
}