using P2P_Netwerken.ChatModels;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace P2P_Netwerken.ChatBusiness
{
    public class ChatProxy
    {
        private ChatProxy.ShowReceivedMessage _srm;
        private ChatProxy.ShowStatus _sst;
        private HttpClient _client;
        private HttpSelfHostServer _server;

        public bool Status { get; set; }

        public ChatProxy(
          ChatProxy.ShowReceivedMessage srm,
          ChatProxy.ShowStatus sst,
          string myport,
          string partneraddress)
        {
            this.StartChatServer(myport);
            if (!this.Status)
                return;
            this._srm = srm;
            this._sst = sst;
            this._client = new HttpClient()
            {
                BaseAddress = new Uri(partneraddress)
            };
            ChatController.ThrowMessageArrivedEvent += (ChatController.EventHandler)((sender, args) => this.ShowMessage(args.Message));
        }

        private void StartChatServer(string myport)
        {
            try
            {
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration("http://localhost:" + myport + "/");
                configuration.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", (object)new
                {
                    id = System.Web.Http.RouteParameter.Optional
                });
                this._server = new HttpSelfHostServer(configuration);
                this._server.OpenAsync().Wait();
                this.Status = true;
            }
            catch (Exception ex)
            {
                this.Status = false;
                this.ShowError("Something happened!");
            }
        }

        private void stopChatServer() => this._server.CloseAsync().Wait();

        private void ShowMessage(Message m) => this._srm(m);

        private void ShowError(string txt) => this._sst(txt);

        public async void SendMessage(Message m)
        {
            try
            {
                HttpResponseMessage response = await this._client.PostAsync("api/chat", (HttpContent)m.serializedMessage);
                if (response.StatusCode != HttpStatusCode.OK)
                    this.ShowError("Partner responded, but awkwardly! Better hide!");
                this.ShowMessage(m);
                response = (HttpResponseMessage)null;
            }
            catch (Exception ex)
            {
                Exception e = ex;
                this.stopChatServer();
                this.ShowError("Partner unreachable. Closing your server!");
            }
        }

        public delegate void ShowReceivedMessage(Message m);

        public delegate void ShowStatus(string txt);
    }
}
