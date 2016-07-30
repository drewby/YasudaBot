using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

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
            var tweetList = await Core.Twitter.TweetList(screenName);
            App.ScreenName = screenName;
            this.tweets.ItemsSource = tweetList;
            this.okButton.IsVisible = true;
        }

        async void OnOk(object sender, EventArgs e)
        { 
        
            await Navigation.PushAsync(new TodoList());
        }
    }
}