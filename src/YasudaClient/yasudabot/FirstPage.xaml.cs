using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace yasudabot
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            InitializeComponent();
            this.okButton.IsVisible = false;
            this.tweets.IsVisible = false;
        }

        async void OnTwitterAccount(object sender, EventArgs e)
        {
            this.tweets.IsVisible = true;
            var screenName = twitterAccountFrom.Text;
            var tweetDatas = await Core.Twitter.TweetList(screenName);
            App.ScreenName = screenName;

            App.PartnerInfo = new Core.ChatModel
            {
                Name = screenName,
                Chatter = Core.Chatter.Partner,
                UserProfileIconUrl = tweetDatas.UserProfileIconUrl,
            };

            this.tweets.ItemsSource = tweetDatas.Tweets;
            this.okButton.IsVisible = true;
        }

        async void OnOk(object sender, EventArgs e)
        { 
        
            await Navigation.PushAsync(new TodoList());
        }
    }
}