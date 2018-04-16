using DiplomadoBot.App.Translator;
using DiplomadoBot.App.Utilities;
using Microsoft.Bot.Builder.Dialogs;

namespace DiplomadoBot.App.Extensions
{
    public static class StringExtensions
    {
        public static string ToUserLocale(this string text, IDialogContext context)
        {
            context.UserData.TryGetValue(StringConstants.UserLanguageKey, out string userLanguageCode);

            text = TranslationHandler.TranslateText(text, StringConstants.DefaultLanguage, userLanguageCode);

            return text;
        }
    }
}