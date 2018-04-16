using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveCards;
using DiplomadoBot.App.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace DiplomadoBot.App.Dialogs
{
    [Serializable]
    [LuisModel("8d1d602b-a53b-418d-8220-e4f7d6e2f0fd", "9cedea9db46449a386b56cc58dd3dd5c")]
    public class ChatDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Default;

            await context.PostAsync(response.ToUserLocale(context));

            context.Wait(MessageReceived);
        }

        [LuisIntent("Saludo")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Saludo;
            var reply = context.MakeMessage();
            reply.Text = response.ToUserLocale(context);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }


        [LuisIntent("Ubicacion")]
        public async Task Location(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Location;

            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            var imageAttachment = new Attachment()
            {
                ContentType = "image/png",
                ContentUrl = "https://i.pinimg.com/originals/c0/5e/a8/c05ea82025e9dccc365a560822aee7a2.jpg",
            };
            reply.Text = response.ToUserLocale(context);
            reply.Attachments.Add(imageAttachment);
            await context.PostAsync(reply);

            context.Wait(MessageReceived);
        }

        [LuisIntent("Horarios")]
        public async Task Hours(IDialogContext context, LuisResult result)
        {
            var response = ChatResponse.Hours;
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();
            card.Body.Add(new TextBlock()
            {
                Text = response.ToUserLocale(context),
                Size = TextSize.Large,
                Weight = TextWeight.Bolder
            });

            card.Body.Add(new TextBlock()
            {
                Text = "8:00 am a 5:00 pm:".ToUserLocale(context)
            });
            
            card.Body.Add(new ChoiceSet()
            {
                Id = "Hours",
                Style = ChoiceInputStyle.Compact,
                Choices = new List<Choice>()
                        {
                            new Choice() { Title = "8 a 9 am", Value = "8 a 9 am", IsSelected = true },
                            new Choice() { Title = "9 a 10 am", Value = "9 a 10 am" },
                            new Choice() { Title = "10 a 11 am", Value = "10 a 11 am" },
                            new Choice() { Title = "11 a 12 pm", Value = "11 a 12 pm" },
                            new Choice() { Title = "2 a 3 pm", Value = "2 a 3 pm" },
                            new Choice() { Title = "3 a 4 pm", Value = "3 a 4 pm" },
                            new Choice() { Title = "4 a 5 pm", Value = "4 a 5 pm" }
                        }
            });

            // Add buttons to the card.
            card.Actions.Add(new SubmitAction()
            {
                Type = "Action.Submit",
                Title = "Reservar".ToUserLocale(context),
                DataJson = "{ \"Type\": \"SelectHours\" }"
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,

                Content = card
            };

            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        
        [LuisIntent("Confirmar")]
        public async Task Confirmar(IDialogContext context, LuisResult result)
        {
            try
            {
                var response = ChatResponse.Confirmar;
                var reply = context.MakeMessage();
                
                reply.Attachments = new List<Attachment>();
                AdaptiveCard card = new AdaptiveCard();
                card.Body.Add(new TextBlock()
                {
                    Text = response.ToUserLocale(context),
                    Size = TextSize.Large,
                    Weight = TextWeight.Bolder
                });
                card.Body.Add(new ColumnSet()
                {
                    Columns = new List<Column>()
                        {
                            new Column(){
                                Items = new List<CardElement>
                                {
                                    
                                    new TextInput
                                    {
                                        Id="name",
                                        Placeholder= "Ingrese su nombre".ToUserLocale(context)
                                    },
                                    new TextInput
                                    {
                                        Id="telephone",
                                        Placeholder = "Numero de telefono".ToUserLocale(context),
                                        Style = TextInputStyle.Tel
                                    }
                                }
                            }
                        }
                });

                // Add buttons to the card.
                card.Actions.Add(new SubmitAction()
                {
                    Type = "Action.Submit",
                    Title = "Confirmar".ToUserLocale(context),
                    DataJson = "{ \"Type\": \"ConfirmReservation\" }"
                });

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);

                context.Wait(MessageReceived);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [LuisIntent("adios")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            try
            {
                var response = ChatResponse.Thanks;
                var reply = context.MakeMessage();
                reply.Text = response.ToUserLocale(context);
                await context.PostAsync(reply);

                context.Wait(MessageReceived);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


}