﻿using System;

using Xamarin.Forms;

namespace yasudabot
{
	public class App : Application
	{
        public static string ScreenName = "";
        public static Core.ChatModel PartnerInfo = new Core.ChatModel();

		public App ()
		{
			// The root page of your application
            MainPage = new NavigationPage(new FirstPage());
		}

		protected override void OnStart ()
		{
            
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

