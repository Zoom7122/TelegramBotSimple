using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotSimple.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;

        public TextMessageController(ITelegramBotClient telegramBotClient)
        {
            _telegramClient = telegramBotClient;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
           
            var buttons = new List<InlineKeyboardButton[]>();
            buttons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($" Сумма" , $"sum"),
                InlineKeyboardButton.WithCallbackData($" Кол-во букв" , $"count")
            });

            // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Доступные кнопки", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

        }
    }
}
