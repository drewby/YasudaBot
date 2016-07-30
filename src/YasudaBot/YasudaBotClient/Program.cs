using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace YasudaBotClient
{
    class Program
    {

        private static string baseUrl = "https://directline.botframework.com/";

        static void Main(string[] args)
        {
            BotCall();
        }

        static async void BotCall() {
            var token = await GetDirectLineAuthToken();
            var response = await GetMessageCustom(token);
            //            var authTokenModel = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        }



        private async static Task<AuthTokenModel> GetDirectLineAuthToken()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "FS9SpM6uD9Y.cwA.d9A.eGWMSUrmhzQaOMIGKSxeEQnDvIcWzjcU4WNKc66uleg");
            httpClient.BaseAddress = new Uri(baseUrl);

            //var response = await httpClient.PostAsync("api/conversations",
            //    new FormUrlEncodedContent(
            //    new System.Collections.Generic.SortedDictionary(
            //    ))
            //    );
            var json = await response.Content.ReadAsStringAsync();
            var authTokenModel = JsonConvert.DeserializeObject<AuthTokenModel>(json);

            return authTokenModel;
        }

        private async static Task<HttpResponseMessage> GetMessageCustom(AuthTokenModel token, string watermark = null)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", token.Token);
            httpClient.BaseAddress = new Uri(baseUrl);
            var url = "";
            if (watermark != null)
            {
                url = "api/conversations/" + token.ConversationId + "/messages" + "?watermark=" + watermark;
            }
            else
            {
                url = "api/conversations/" + token.ConversationId + "/messages";
            }

            var response = await httpClient.GetAsync(url);

            return response;
        }

        //private static async Task Send(AuthTokenModel token, string Text)
        //{
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json; charset=utf-8"));
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", token.Token);
        //    httpClient.BaseAddress = new Uri(baseUrl);

        //    var message = new Message 
        //    {
        //        text = "hogehoge"
        //    };
        //    var json = JsonConvert.SerializeObject(message);

        //    //var response = await httpClient.PostAsJsonAsync(
        //    //    "api/conversations/" + token.ConversationId + "/messages",
        //    //    message);
        //}


    }

    class AuthTokenModel
    {
        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("eTag")]
        public string ETag { get; set; }
    }

    internal class ChannelData
    {
    }

    internal class Attachment
    {
        public string url { get; set; }
        public string contentType { get; set; }
    }

    internal class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public string created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public ChannelData channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }

}
