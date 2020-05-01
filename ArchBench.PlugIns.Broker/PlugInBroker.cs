using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HttpServer;
using HttpServer.Sessions;

namespace ArchBench.PlugIns.Broker
{
    public class PlugInBroker : IArchBenchHttpPlugIn
    {
        public string Name => "PlugIn Broker pattern";
        public string Description => "Implementing Broker pattern";
        public string Author => "Bruno Rocha";
        public string Version => "1.0.0";


        public bool Enabled { get; set; } = false;
        public IArchBenchPlugInHost Host { get; set; }
        public IArchBenchSettings Settings { get; } = new ArchBenchSettings();


        public TcpListener Listener { get; private set; }
        public Thread Thread { get; private set; }


        public static List<ArrayList> Bservers { get; set; } = new List<ArrayList>();
        private int NextServer { get; set; }

        public void Initialize()
        {
            Listener = new TcpListener(IPAddress.Any, 9000);
            Thread = new Thread(ReceiveThreadFunction) { IsBackground = true };
            Thread.Start();
        }

        public void Dispose()
        {
        }

        public bool Process(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession)
        {
            var index = GetNextServer();
            if (index == -1) return false;
           
            var urlLast = aRequest.Uri.AbsolutePath;
            var urlExtension = urlLast.Split('.');

            var urlServer = urlExtension[0].Split('/');
            var server = Bservers[index][3].ToString().Split('/')[1];
            var url = "";
            var message = "";

            if (urlServer.Length == 3)
            {
                if (urlServer[1] != server)
                {
                    message = "<b>SERVER NOT FOUND</b>";
                }
                else
                {
                    url = string.Format("http://{0}:{1}{2}", Bservers[index][0], Bservers[index][1], urlLast);
                }
            }
            try
            {
                ProcessContinue(aRequest, aResponse, aSession, url, urlExtension, message);
            }
            catch (WebException e)
            {
                    HttpWebResponse response = (HttpWebResponse) e.Response;
                    aResponse.Status = response.StatusCode;

                    StandardWriter(e.Message, aResponse.Body);
            }
            return true;
        }

        private void ProcessContinue(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession, string url, string[] aExtension, string aMessage)
        {
            if (url != "")
            {
                if (aExtension.Length != 1)
                {
                    ProcessMedia(aRequest, aResponse, aSession, url);
                }
                else
                {
                    ProcessHtml(aRequest, aResponse, aSession, url);
                }
            }
            else
            {
                StandardWriter(aMessage, aResponse.Body);
            }
        }

        private void ProcessMedia(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.CookieContainer = new CookieContainer();
            request.Method = aRequest.Method;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            aResponse.Status = response.StatusCode;
            aResponse.ContentType = response.ContentType;

            Uri uri = new Uri(url);
            CookieCollection cookies = request.CookieContainer.GetCookies(uri);

            GetFormValues(aRequest, aResponse);

            Stream stream = response.GetResponseStream();
            MemoryStream ms = new MemoryStream();

            stream.CopyTo(ms);

            TryAddCookies(cookies, aResponse);

            byte[] decrypedBytes = ms.ToArray();

            var writer = new BinaryWriter(aResponse.Body);
            writer.Write(decrypedBytes);
            writer.Flush();

            response.Close();
        }
        
        private void ProcessHtml(IHttpRequest aRequest, IHttpResponse aResponse, IHttpSession aSession, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = aRequest.Method;
            request.CookieContainer = new CookieContainer();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            aResponse.Status = response.StatusCode;
            aResponse.ContentType = response.ContentType;

            Uri uri = new Uri(url);
            CookieCollection cookies = request.CookieContainer.GetCookies(uri);

            GetFormValues(aRequest, aResponse);

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string responseFromServer = reader.ReadToEnd();
            
            TryAddCookies(cookies, aResponse);
            StandardWriter(responseFromServer, aResponse.Body);

            response.Close();
        }

        private void GetFormValues(IHttpRequest aRequest, IHttpResponse aResponse)
        {
            if (aRequest.Form.Count() != 0)
            {
                for (var i = 0; i < aRequest.Form.Count(); i++)
                {
                    HttpInputItem var;
                    var = aRequest.Form.ElementAt(i);
                    string var2 = var.ToString().Trim();
                    string[] items = var2.Split('=');
                    string name = items.First();
                    string value = items.Last();

                    aResponse.AddHeader(name, value);
                }
            }
        }

        private void TryAddCookies(CookieCollection cookies, IHttpResponse aResponse)
        {
            for (int i = 0; i < cookies.Count; i++)
            {
                string cookieValue = cookies[i].Value;
                string cookieKey = cookies[i].Name;
                DateTime cookieExpire = cookies[i].Expires;

                ResponseCookie newCookie = new ResponseCookie(cookieKey, cookieValue, cookieExpire);
                aResponse.Cookies.Add(newCookie);
            }
        }

        private void ReceiveThreadFunction()
        {
            try
            {
                Listener.Start();
                byte[] bytes = new byte[256];

                while (true)
                {
                    var client = Listener.AcceptTcpClient();
                    var stream = client.GetStream();

                    int count = stream.Read(bytes, 0, bytes.Length);
                    if (count != 0)
                    {
                        string data = Encoding.ASCII.GetString(bytes, 0, count);
                        var parts = data.Split(':');
                        switch (parts[0])
                        {
                            case "+":
                                Regist(parts[1], int.Parse(parts[2]), int.Parse(parts[3]), parts[4]);
                                break;
                            case "-":
                                Unregist(parts[1], int.Parse(parts[2]), int.Parse(parts[3]), parts[4]);
                                break;
                        }
                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Host.Logger.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                Listener.Stop();
            }
        }

        private int GetNextServer()
        {
            if (Bservers.Count == 0) return -1;
            NextServer = (NextServer + 1) % Bservers.Count;
            return NextServer;
        }

        private void StandardWriter(string response, Stream stream)
        {
            var writer = new StreamWriter(stream);
            writer.WriteLine(response);
            writer.Flush();
        }

        private void Regist(string aAddress, int aPort, int aId, string aPath)
        {
            if (Bservers.Count != 0)
            {
                foreach (ArrayList list in Bservers)
                {
                    if (list.Contains(aAddress) && list.Contains(aPort) && list.Contains(aPath) && list.Contains(aId)) 
                        return;
                    else
                    {
                        ArrayList newList = Server(aAddress, aPort, aId, aPath);

                        Bservers.Add(newList);
                        Host.Logger.WriteLine("Added server with Id {0} on {1}:{2}.", aId, aAddress, aPort);
                        break;
                    }
                    
                }
            }
            else {
                ArrayList newList = Server(aAddress, aPort, aId, aPath);

                Bservers.Add(newList);
                Host.Logger.WriteLine("Added server with Id {0} on {1}:{2}.", aId, aAddress, aPort);
            }
        }

        private void Unregist(string aAddress, int aPort, int aId, string aPath)
        {
            List<ArrayList> temp = Bservers;

            foreach(ArrayList testList in temp)
            {
                ArrayList list = Server(aAddress, aPort, aId, aPath);

                if (testList.Count != list.Count) return;

                if ((list.ToArray() as IStructuralEquatable).Equals(testList.ToArray(), EqualityComparer<object>.Default))
                {
                    Host.Logger.WriteLine(
                        Bservers.Remove(testList)
                            ? "Removed server {0}:{1}."
                            : "The server {0}:{1} is not registered.", aAddress, aPort);
                    break;
                }
            }
        }

        private ArrayList Server(string address, int port, int id, string path)
        {
            ArrayList newComponent = new ArrayList();

            newComponent.Add(address);
            newComponent.Add(port);
            newComponent.Add(id);
            newComponent.Add(path);

            return newComponent;
        }
    }
}
