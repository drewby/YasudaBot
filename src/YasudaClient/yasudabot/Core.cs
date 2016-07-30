using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using CoreTweet;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml;
using System.Runtime.Serialization.Json;

namespace Core
{

    #region Chat

    public class ChatModel
    {
        public string Name;
        public Chatter Chatter; // 自分？相手？
        public string Created;
        public string Text;
        public string UserProfileIconUrl;
        public string ImageUri;
    }

    public enum Chatter { Me, Partner }

    [DataContract]
    public class BotResponseModel
    {
        [DataMember]
        public string conversationId;// conversationId
        [DataMember]
        public string token; // token
        [DataMember]
        public string eTag; // eTag // 同時会話を制御するためのパラメータ。使わない
    }

    public static class Bot
    {
        const string webAddress = "https://yasudabot0.azurewebsites.net/api/";
        // 最初に叩くAPI
        public static async Task<BotResponseModel> FirstConnect(string text)
        { 
            using (var client = new HttpClient())
            {
                string uri = webAddress +  "messages/";
                var responseMessage = await client.PostAsync(uri, new StringContent("{\"message\":\"" + text + "\"}"));
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<BotResponseModel>(jsonResponse);
                    return _Data;
                }
            }

            return null;

            /*
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress + "messages/");
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            var stream = await httpWebRequest.GetRequestStreamAsync();

            using (var streamWriter = new StreamWriter(stream))
            {
                string json = "{\"message\":\"" + text + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = await httpWebRequest.GetResponseAsync() as HttpWebResponse;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                var serializer = new DataContractJsonSerializer(typeof(BotResponseModel));
                var res = (BotResponseModel)serializer.ReadObject(httpResponse.GetResponseStream());

                return res;
            }
            */
        }
    }


    #endregion

    #region Twitter

    /// <summary>
    /// 投稿に使う各種キーの集まり。（型定義）
    /// </summary>
    public class TwitterKeys
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
    }

    public class Twitter
    {
        public string UserProfileIconUrl;
        public List<string> Tweets;

        // ツイート一覧 (スクリーンネームを渡したら、そのアカウントのツイート一覧を返す)
        public static async Task<Twitter> TweetList(string screenName)
        {
            const int tweetCount = 10;

            var tokens = CoreTweet.Tokens.Create(MyTokens.ConsumerKey, MyTokens.ConsumerSecret, MyTokens.AccessToken, MyTokens.AccessSecret);

            var pram = new Dictionary<string, object>();
            pram["count"] = tweetCount;
            pram["screen_name"] = screenName;

            var tweets = await tokens.Statuses.UserTimelineAsync(pram);

            var tweetList = new List<string>();

            foreach (var tweet in tweets)
            {
                tweetList.Add(tweet.Text);
            }
            return new Twitter { Tweets = tweetList, UserProfileIconUrl = tweets.FirstOrDefault()?.User.ProfileImageUrl ?? "" };
        }
        #region OAuth 認証

        /// <summary>
        /// アクセストークンなど、投稿に必要なキーを取得。（キャッシュがあればキャッシュから。無かったらファイルから読み込む）
        /// </summary>
        private static TwitterKeys MyTokens => _myTokens ?? (_myTokens = new TwitterKeys { AccessSecret = yasudabot.MyTwitterApiKeys.AccessSecret, AccessToken = yasudabot.MyTwitterApiKeys.AccessToken, ConsumerKey = yasudabot.MyTwitterApiKeys.ConsumerKey, ConsumerSecret = yasudabot.MyTwitterApiKeys.ConsumerSecret });
        static TwitterKeys _myTokens = null;

        #endregion
    }
    #endregion
}