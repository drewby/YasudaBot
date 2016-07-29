using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace test160728
{
    public partial class CallHistoryPage : ContentPage
    {
        public CallHistoryPage()
        {
            InitializeComponent();
            this.Title = $"@{App.PhoneNumbers.FirstOrDefault()}'s tweets";
        }

        public void OnGoToChatScreen(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChatPage());
        }

    }
}
