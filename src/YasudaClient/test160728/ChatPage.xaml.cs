using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace test160728
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
            this.Title = $"{App.ScreenName}";
        }
    }
}
