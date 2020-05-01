using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HttpServer;
using HttpServer.Sessions;

namespace ArchBench.PlugIns.BServers
{
    public class PlugInBServers : IArchBenchHttpPlugIn
    {
        public string Name => "PlugIn Broker Servers";
        public string Description => "Servers being used in the broker pattern";
        public string Author => "Bruno Rocha";
        public string Version => "1.0.0";

        public bool Enabled {
            get => ServerOnService; 
            set => ServerRegistration(value);
        }

        public bool ServerOnService { get; set; }

        public IArchBenchPlugInHost Host { get; set; }
        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();

        public void Initialize()
        {
            Settings["BrokerAddress"] = "127.0.0.1:9000";
            Settings["BrokerServerPort"] = "8081";
            Settings["Path"] = "/default";
            Settings["Id"] = "0";
            Settings["FilePath"] = "C:\\Users\\Bruno Rocha\\Documents\\BrokerFiles";
        }

        public void Dispose()
        {
        }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            string[] splitUri = aRequest.Uri.AbsolutePath.Split('/');
            string lastOfUri = splitUri.Last();
            string[] extension = lastOfUri.Split('.');

            AddCookie(aResponse, "Id", aSession.Id.ToString());
            AddCookie(aResponse, "NumAccess", aSession.Count.ToString());

            if (extension.Length == 1)
            {
                return false;
            }
            else if(splitUri.Length == 2)
            {
                ProcessResponse(extension[1], lastOfUri, aResponse, splitUri[0]);
            }
            else if (splitUri.Length == 3)
            {
                var middleOfUri = "\\" + splitUri[1];
                ProcessResponse(extension[1],lastOfUri, aResponse, middleOfUri);
            }
            return true;
        }

        private void ProcessResponse(string aExtension, string aUriLast, IHttpResponse aResponse, string aServer)
        {
            if (aExtension == "html")
            {
                string response = ReadFile(Settings["FilePath"] + aServer + "\\" + aUriLast);
                SendResponseStream(response, aResponse.Body);
            }
            else if (aExtension == "png" || aExtension == "jpg")
            {
                byte[] response = ReadMedia(Settings["FilePath"] + aServer + "\\" + aUriLast);
                SendResponseBinary(response, aResponse, "image/" + aExtension);
            }
            else if (aExtension == "mp4")
            {
                byte[] response = ReadMedia(Settings["FilePath"] + aServer + "\\" + aUriLast);
                SendResponseBinary(response, aResponse, "video/" + aExtension);
            }
        }

        private void SendResponseStream(string response, Stream aStream)
        {
            if (!string.IsNullOrEmpty(response))
            {
                var aWriter = new StreamWriter(aStream);
                aWriter.Write(response);
                aWriter.Flush();
            }
            else
            {
                var error = "<b>Error: File not Found</b>";
                var aWriter = new StreamWriter(aStream);
                aWriter.Write(error);
                aWriter.Flush();
            }
        }

        private void SendResponseBinary(byte[] response, IHttpResponse aHttpResponse, string aExtension)
        {
            if (response != null)
            {
                aHttpResponse.ContentType = aExtension;
                var aWriter = new BinaryWriter(aHttpResponse.Body);
                aWriter.Write(response);
                aWriter.Flush();
            }
            else
            {
                var error = "<b>Error: File not Found</b>";
                var aWriter = new BinaryWriter(aHttpResponse.Body);
                aWriter.Write(error);
                aWriter.Flush();
            }
        }

        private void ServerRegistration(bool aServerOnService)
        {
            try
            {
                if (aServerOnService == ServerOnService) return;
                ServerOnService = aServerOnService;

                var parts = Settings["BrokerAddress"].Split(':');

                if(!checkErrorBroker(parts, out int brokerPort))
                    return;

                var client = new TcpClient(parts[0], brokerPort);
                var operation = ServerOnService ? '+' : '-';

                var data = Encoding.ASCII.GetBytes(

                    $"{operation}:{GetAddress()}:{Settings["BrokerServerPort"]}:{Settings["Id"]}:{Settings["Path"]}");

                var stream = client.GetStream();

                stream.Write(data, 0, data.Length);
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Host.Logger.WriteLine("SocketException: {0}", e);
            }
        }

        private bool checkErrorBroker(string[] aSetting, out int brokerPort)
        {
            brokerPort = 0;
            if (string.IsNullOrEmpty(Settings["BrokerAddress"]))
            {
                Host.Logger.WriteLine("The Broker's Address is not defined.");
                return false;
            }

            if (aSetting.Length != 2)
            {
                Host.Logger.Write(
                    "The Broker Address format is not well defined' ); ' + (must be <ip>:<port>): ");
                Host.Logger.WriteLine($"{Settings["BrokerAddress"]}");
                return false;
            }

            if (!int.TryParse(aSetting[1], out int port))
            {
                Host.Logger.Write(
                    "The Broker Address format is not well defined' ); ' + (must be <ip>:<port>): ");
                Host.Logger.WriteLine($"A number is expected on <port> : {aSetting[1]}");
                return false;
            }

            brokerPort = port;
            return true;
        }

        private void AddCookie(IHttpResponse response, string name, string value)
        {
            var date = DateTime.Now.AddDays(1d).ToString();
            var expireTime = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            ResponseCookie cookie = new ResponseCookie(name, value, expireTime);
            response.Cookies.Add(cookie);
        }

        private static string GetAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
            }
            return "0.0.0.0";
        }

        private string ReadFile(string path)
        {
            try
            {
                StreamReader file = new StreamReader(path);
                var fileText = "";
                string fileLine;

                while ((fileLine = file.ReadLine()) != null)
                {
                    fileText += fileLine + "\n";
                }

                file.Close();
                return fileText;
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }

        private byte[] ReadMedia(string path)
        {
            try
            {
                FileStream stream = File.OpenRead(path);
                byte[] media = new byte[stream.Length];
                stream.Read(media, 0, media.Length);
                return media;

            }
            catch (FileNotFoundException e)
            {
                return null;
            }

        }
    }
}
