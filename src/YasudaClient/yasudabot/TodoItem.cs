﻿using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace yasudabot
{
	public class TodoItem
	{
		string id;
        string name;
		string text;
        string profileIconUri;
        string imageUri;
        DateTime created;
		bool done;
                
		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value;}
		}

        [JsonProperty(PropertyName = "text")]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        [JsonProperty(PropertyName = "profileIconUri")]
        public string ProfileIconUri
        {
            get { return profileIconUri; }
            set { profileIconUri = value; }
        }

        [JsonProperty(PropertyName = "imageUri")]
        public string ImageUri
        {
            get { return imageUri; }
            set { imageUri = value; }
        }

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get { return name; }
			set { name = value;}
		}

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        [JsonProperty(PropertyName = "complete")]
		public bool Done
		{
			get { return done; }
			set { done = value;}
		}

        [Version]
        public string Version { get; set; }
	}
}

