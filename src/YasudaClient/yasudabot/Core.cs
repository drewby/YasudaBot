using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using CoreTweet;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 投稿に使う各種キーの集まり。（型定義）
    /// </summary>
    public class Keys
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
    }
    public class Twitter
    {
        // ツイート一覧 (スクリーンネームを渡したら、そのアカウントのツイート一覧を返す)
        public static async Task<List<string>> TweetList(string screenName)
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
            return tweetList;
        }
        #region OAuth 認証

        /// <summary>
        /// アクセストークンなど、投稿に必要なキーを取得。（キャッシュがあればキャッシュから。無かったらファイルから読み込む）
        /// </summary>
        private static Keys MyTokens => _myTokens ?? (_myTokens = new Keys { AccessSecret = yasudabot.MyKeys.AccessSecret, AccessToken = yasudabot.MyKeys.AccessToken, ConsumerKey = yasudabot.MyKeys.ConsumerKey, ConsumerSecret = yasudabot.MyKeys.ConsumerSecret });
        static Keys _myTokens = null;

        #endregion
    }
}