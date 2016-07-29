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
            var tweetList = await Core.Twitter.TweetList(phoneNumberText.Text);
            App.PhoneNumbers.AddRange(tweetList);
            await Navigation.PushAsync(new CallHistoryPage());        
        }

    }
}
