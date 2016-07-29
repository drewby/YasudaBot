using System;
using CoreTweet;
using System.IO;
using Newtonsoft.Json;
using Core;
using System.Collections.Generic;

namespace test160728.iOS
{
    public static class CoreIos
    {

        public class Twitter
        {
            // ツイート一覧 (スクリーンネームを渡したら、そのアカウントのツイート一覧を返す)
            public static List<string> TweetList(string screenName)
            {
                const int tweetCount = 10;

                var tokens = CoreTweet.Tokens.Create(MyTokens.ConsumerKey, MyTokens.ConsumerSecret, MyTokens.AccessToken, MyTokens.AccessSecret);

                var pram = new Dictionary<string, object>();
                pram["count"] = tweetCount;
                pram["screen_name"] = screenName;

                var tweets = tokens.Statuses.UserTimeline(pram);

                var tweetList = new List<string>();

                foreach (var tweet in tweets)
                {
                    tweetList.Add(tweet.Text);   
                }
                return tweetList;
            }

            #region OAuth 認証

            /// <summary>
            /// アクセストークンなど、投稿に必要なキーを取得。（キャッシュがあればキャッシュから。無かったらファイルから読み込む）
            /// </summary>
            private static Keys MyTokens => _myTokens ?? (_myTokens = ReadTokens());
            static Keys _myTokens = null;

            /// <summary>
            /// key.json ファイルから、アクセストークンなどを読み込む。
            /// </summary>
            public static Keys ReadTokens()
            {
                var json = File.ReadAllText("AppConfig/keys.json");
                return JsonConvert.DeserializeObject<Keys>(json);
            }
            #endregion
        }

    }
}
