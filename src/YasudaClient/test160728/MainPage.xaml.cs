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

        void OnCallHisotry(object sender, EventArgs e)
        {
            var screenName = phoneNumberText.Text;
            //var tweetList =  
            App.PhoneNumbers.Add(phoneNumberText.Text);
            Navigation.PushAsync(new CallHistoryPage());        
        }
    }
}
