using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace test160728
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnCallHisotry(object sender, EventArgs e)
        {
            var screenName = phoneNumberText.Text;
            var tweetList = await Core.Twitter.TweetList(screenName);
            App.ScreenName = screenName;
            App.PhoneNumbers.AddRange(tweetList);
            await Navigation.PushAsync(new CallHistoryPage());        
        }

    }
}
