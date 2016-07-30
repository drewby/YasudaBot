using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using CoreTweet;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{

    #region Chat

    public class ChatModel
    {
        public string Name;
        public Chatter Chatter; // 自分？相手？
        //public DateTime Created;
        public string Text;
        public string UserProfileIconUrl;
    }

    public enum Chatter { Me, Partner }
    #endregion

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
        private static Keys MyTokens => _myTokens ?? (_myTokens = new Keys { AccessSecret = yasudabot.MyTwitterApiKeys.AccessSecret, AccessToken = yasudabot.MyTwitterApiKeys.AccessToken, ConsumerKey = yasudabot.MyTwitterApiKeys.ConsumerKey, ConsumerSecret = yasudabot.MyTwitterApiKeys.ConsumerSecret });
        static Keys _myTokens = null;

        #endregion
    }
}