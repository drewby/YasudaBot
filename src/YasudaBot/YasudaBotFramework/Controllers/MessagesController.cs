
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace YasudaBotFramework
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// 

        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// 

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                // Parse the user's meaning via Language Understanding (LUIS) in Cognitive Services
                YasudaLUIS Luis = await LUIS.ParseUserInput(activity.Text);
                string strRet = string.Empty;

                if (Luis.intents.Count() > 0)
                {
                    var talent = string.Empty;
                    var MediaType = string.Empty;

                    switch (Luis.intents[0].intent)
                    {
                        case "ShowMeMedia":
                            // call 
                            talent = Luis.entities.Count() > 0 ? Luis.entities[0].entity : "";
                            MediaType = Luis.entities.Count() > 1 ? Luis.entities[1].entity : "";
                            strRet = $"Talent: {talent} / Media {MediaType} ";
                            break;
                        case "RecommandMedia":
                            // call 
                            talent = "Akarui Yasumura";
                            MediaType = Luis.entities.Count() > 1 ? Luis.entities[1].entity : "";
                            strRet = $"Talent: {talent} / Media {MediaType} ";
                            break;
                        default:
                            strRet = "Hi! I'm the Yasuda Bot. I help your fun time :)." + "\n" +
                                @"Simple Ask me like ""show me {talent Name} Video""." + "\n" +
                                "Enjoy!"
                                ;
                            break;
                    }

                    if (talent != string.Empty)
                    {
                        
                        BingVideoSearchData Videos = await BingVideoSearch.VideoSearch(talent, "ja-jp");
                        if (Videos.value.Count() > 0)
                        {
                            strRet = $"Title:{Videos.value[0].name}. URL:{Videos.value[0].contentUrl}";
                        }
                    }

                }
                else
                {
                    strRet = "I'm sorry but I don't understand what you're asking";
                }

                /// return our reply to the user
                Activity reply = activity.CreateReply(strRet);
                await connector.Conversations.ReplyToActivityAsync(reply);

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
      
    }
}
