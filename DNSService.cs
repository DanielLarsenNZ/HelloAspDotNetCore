using DnsClient;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HelloAspDotNetCore
{
    public class TestSocketResult
    {
        public bool Connected { get; set;}
        public string RemoteEndPoint { get; set; }
        public TimeSpan Latency { get; set; }
        public string SocketExceptionMessage { get; set; }
        public string Hostname { get; set; }
    }
    public class DNSService
    {

        public static string GetNetDNSHostAddresses(string hostname)
        {
            var ips = Dns.GetHostEntry(hostname);
            return $"{hostname} IP = {string.Join(',', ips.AddressList.Select(ip => ip.ToString()).ToArray())} Hostname: {ips.HostName}";
        }

        [Obsolete]
        public static string GetHostAddresses(string hostname, string nameserver = "", ILogger logger = null)
        {
            var lookup = (!string.IsNullOrEmpty(nameserver)) ? new LookupClient(IPAddress.Parse(nameserver)) : new LookupClient();
            lookup.UseCache = false;
            var result = lookup.Query(hostname, QueryType.A);
            return $"{hostname} IP = {string.Join(',', result.Answers.ARecords().Select(record => record.Address.ToString()).ToArray())} Nameserver:{result.NameServer.Address} Answers: {string.Join("->", result.Answers.Select(record => record.ToString()).ToArray())}";

        }
        public static TestSocketResult TestConnection(string host, int port, ILogger logger = null)
        {
            TestSocketResult result = new TestSocketResult { Connected = false, Hostname = host };
            
            DnsEndPoint endPoint = new DnsEndPoint(host, port);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    socket.Connect(endPoint);
                }
                catch (SocketException SoEx)
                {
                    result.SocketExceptionMessage = SoEx.Message;
                }
                sw.Stop();
                
                result.Latency = sw.Elapsed;
                result.Connected = socket.Connected;
                result.RemoteEndPoint = socket.RemoteEndPoint?.ToString();
                socket.Close();
            }
            return result;
        }
        public static string GetDNSServerIP()
        {
            var lookup = new LookupClient();
            return string.Join(',', lookup.NameServers.Select(ns => ns.Address));
        }


    }
}
