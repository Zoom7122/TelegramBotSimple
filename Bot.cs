using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotSimple;
using TelegramBotSimple.Controllers;
using VoiceTexterBot.Controllers;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleBot
{

    class Bot : BackgroundService
    {
        /// <summary>
        /// объект, отвеающий за отправку сообщений клиенту
        /// </summary>
        private ITelegramBotClient _telegramClient;
        private TextMessageController _textMessageController;
        private InlineKeyboardController _inlineKeyboardController;

        public Bot(ITelegramBotClient telegramClient , TextMessageController textMessageController,
            InlineKeyboardController inlineKeyboardController)
        {
            _telegramClient = telegramClient;
            _textMessageController = textMessageController;
            _inlineKeyboardController = inlineKeyboardController;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, // receive all update types
                cancellationToken: stoppingToken);

            Console.WriteLine("Bot started");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                return;
            }

            // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                try
                {
                    var messege = update.Message;
                    bool result = !string.IsNullOrEmpty(messege.Text) && messege.Text.All(c => (c >= '0' && c <= '9') || c == ' ');
                    if (result)
                    {
                        int _toClientNums = ForCountNums.CountNums(messege.Text);
                        await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Сумма чисел: {_toClientNums}", cancellationToken: cancellationToken);
                    }
                    else
                    {

                        switch (update.Message!.Type)
                        {
                            case MessageType.Text:
                                if (messege.Text == "/start")
                                {
                                    await _textMessageController.Handle(update.Message, cancellationToken);
                                    break;
                                }
                                else
                                {
                                    await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Длина сообщения: " +
                                        $"{update.Message.Text.Length} знаков", cancellationToken: cancellationToken);
                                }
                                return;
                            default: // unsupported message
                                await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Данный тип сообщений не поддерживается. Пожалуйста отправьте текст.", cancellationToken: cancellationToken);
                                return;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.GetType().ToString());
                }
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            Console.WriteLine("Waiting 10 seconds before retry");
            Thread.Sleep(10000);
            return Task.CompletedTask;
        }
    }
}