using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using DiplomadoBot.App.Dialogs;
using DiplomadoBot.App.Extensions;
using DiplomadoBot.App.Models;
using DiplomadoBot.App.Services;
using DiplomadoBot.App.Translator;
using DiplomadoBot.App.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace DiplomadoBot.App
{
    [BotAuthentication]
    [RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        private static HourQuery query;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Trace.TraceInformation($"Incoming Activity is {activity.ToJson()}");
            if (activity.Type == ActivityTypes.Message)
            {

                //detect language of input text
                var userLanguage = TranslationHandler.DetectLanguage(activity);
                if (string.IsNullOrEmpty(userLanguage))
                    userLanguage = StringConstants.DefaultLanguage;
                //save user's LanguageCode to Azure Table Storage
                var message = activity as IMessageActivity;

                try
                {
                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                    {
                        var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                        var key = new AddressKey()
                        {
                            BotId = message.Recipient.Id,
                            ChannelId = message.ChannelId,
                            UserId = message.From.Id,
                            ConversationId = message.Conversation.Id,
                            ServiceUrl = message.ServiceUrl
                        };

                        var userData = await botDataStore.LoadAsync(key, BotStoreType.BotUserData, CancellationToken.None);

                        var storedLanguageCode = userData.GetProperty<string>(StringConstants.UserLanguageKey);

                        //update user's language in Azure Table Storage
                        if (storedLanguageCode != userLanguage)
                        {
                            userData.SetProperty(StringConstants.UserLanguageKey, userLanguage);
                            await botDataStore.SaveAsync(key, BotStoreType.BotUserData, userData, CancellationToken.None);
                            await botDataStore.FlushAsync(key, CancellationToken.None);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                dynamic value = message.Value;
                string submitType = value?.Type?.ToString();
                switch (submitType)
                {
                    case "SelectHours":
                        query = HourQuery.Parse(value);
                        activity.Text = query.Hours + " confirmado";
                        activity.Text = TranslationHandler.TranslateTextToDefaultLanguage(activity, userLanguage);
                        await Conversation.SendAsync(activity, MakeRoot);
                        break;
                    case "ConfirmReservation":
                        ReservationDto reservation = ReservationDto.Parse(value);
                        reservation.Hour = query.Hours;
                        var result = await StoreServices.CreateReservationAsync(reservation);
                        if (result.IsValid)
                        {
                            activity.Text = "Gracias";
                            await Conversation.SendAsync(activity, MakeRoot);
                        }
                        break;
                    default:
                        activity.Text = TranslationHandler.TranslateTextToDefaultLanguage(activity, userLanguage);
                        await Conversation.SendAsync(activity, MakeRoot);
                        break;
                }
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        internal static IDialog<object> MakeRoot()
        {
            try
            {
                return Chain.From(() => new ChatDialog());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            try
            {
                if (message.Type == ActivityTypes.DeleteUserData)
                {
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                }
                else if (message.Type == ActivityTypes.ConversationUpdate)
                {

                    IConversationUpdateActivity conversationupdate = message;

                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                    {
                        var client = scope.Resolve<IConnectorClient>();
                        if (conversationupdate.MembersAdded.Any())
                        {
                            var reply = message.CreateReply();
                            foreach (var newMember in conversationupdate.MembersAdded)
                            {
                                if (newMember.Id == message.Recipient.Id)
                                {
                                    reply.Text = ChatResponse.Greeting;

                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                        }
                    }
                }
                else if (message.Type == ActivityTypes.ContactRelationUpdate)
                {
                }
                else if (message.Type == ActivityTypes.Typing)
                {
                    // Handle knowing tha the user is typing
                }
                else if (message.Type == ActivityTypes.Ping)
                {
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }


            return null;
        }


    }
}