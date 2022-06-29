using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Search_for_Anime_or_Manga_Telegram_Bot;

namespace Search_for_Anime_or_Manga_Telegram_Bot
{
    class Program
    {
        public static async Task Main() 
        {
            Telegram_Bot telegram__Bot = new Telegram_Bot();

            await telegram__Bot.Start();

            //Console.ReadKey();
        }
    }
}
