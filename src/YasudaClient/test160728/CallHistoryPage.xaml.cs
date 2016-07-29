using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace test160728
{
    public partial class CallHistoryPage : ContentPage
    {
        public CallHistoryPage()
        {
            InitializeComponent();
        }

        public void OnGoToChatScreen(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChatPage());
        }

    }
}
