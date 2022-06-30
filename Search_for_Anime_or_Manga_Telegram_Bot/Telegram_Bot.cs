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
using System.IO;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Search_for_Anime_or_Manga_Telegram_Bot.Clients;
using Search_for_Anime_or_Manga_Telegram_Bot.Models;
using System.Text.RegularExpressions;

namespace Search_for_Anime_or_Manga_Telegram_Bot
{
    public class Telegram_Bot
    {
        TelegramBotClient botClient = new TelegramBotClient("5438022687:AAFzum5wFv8G3ucTPZGPiGZ4l4XVFbu9u5o");
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { }, ThrowPendingUpdates = true };

        public async Task Start()
        {
            botClient.StartReceiving(HandleUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();

            Console.WriteLine($"Бот ''{botMe.Username}'' почав працювати\n");
            //Console.ReadKey();
            Thread.Sleep(int.MaxValue);
        }


        public Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Виникла помилка у телеграм бот API:\n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}", _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            return Task.CompletedTask;
        }


        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => HandlerMessageAsync(botClient, update.Message!),
                UpdateType.EditedMessage => HandlerMessageAsync(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!)
            };
            await handler;

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            #region draft
            //if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            //{
            //    await HandlerMessageAsync(botClient, update.Message);
            //}

            //if (update?.Type == UpdateType.CallbackQuery)
            //{
            //    await HandlerCallbackQuery(botClient, update.CallbackQuery);
            //}
            #endregion
        }


        string LastText { get; set; }
        int Title_ID { get; set; }
        
        
        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Hi, {message.Chat.FirstName}!!\n" +
                                                                      "Nice to meet you!!\n" +                                                                      
                                                                      "My name is Search_for_Anime/Manga_Bot.\n" +
                                                                      "To see information about me, enter /info\n" +
                                                                      "To see the full guide, enter /guide\n" +
                                                                      "To start working with bot, enter any command\n",
                                                                      replyMarkup: Close());
                return;
            }  
            else
            if (message.Text == "/info")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                await botClient.SendTextMessageAsync(message.Chat.Id, "This bot is created to do the following tasks:\n" +
                                                                      "- search anime or manga by their name\n" +
                                                                      "- get information about the anime or manga you are interested in by its ID\n" +
                                                                      "- give you list of popular anime or manga by their types\n" +
                                                                      "- save (or delete) titles to (from) your ''Favorites'' list\n\n", 
                                                                      replyMarkup: Close());
                return;
            }
            else
            if (message.Text == "/guide")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Guide:\n\n" +

                                                                      "/info\n" +
                                                                      "This command allows you to see information about this bot.\n\n" +

                                                                      "/guide\n" +
                                                                      "This command allows you to see information about all commands.\n\n" +

                                                                      "/search_by_name\n" +
                                                                      "This command allows you to search for anime or manga by its name. " +
                                                                      "As a result, you'll receive a list of titles whoose names are similar to the word or set of words you entered. " +
                                                                      "Firstly, you need to choose the type of object that you are looking for. " +
                                                                      "Then you need to enter the word or set of words for the title you are searching for (only on english!!!).\n\n" +
                                                                      
                                                                      "/get_info_by_id\n" +
                                                                      "This  command allows you to get information about the title by its ID." +
                                                                      "As a result, you'll receive full information about this title with some recommendations. " +
                                                                      "You just need to enter ID (only numbers)\n\n" +

                                                                      "/get_ranking\n" +
                                                                      "This command allows you to view anime or manga ranking (top 10 titles) for different types of anime or manga. " +
                                                                      "Firstly, you need to choose the type of ranking you want to see. " +
                                                                      "Then you need to choose the type of anime or manga for ranking.\n\n" +
                                                                      
                                                                      "Variants for anime ranking:\n" +
                                                                      " - All\n" +
                                                                      " - Airing\n" +
                                                                      " - Upcoming\n" +
                                                                      " - TV\n" +
                                                                      " - OVA\n" +
                                                                      " - Movie\n" +
                                                                      " - Special\n" +
                                                                      " - By popularity on MAL\n\n" +

                                                                      "Variants for manga ranking:\n" +
                                                                      " - All\n" +
                                                                      " - Manga\n" +
                                                                      " - Novels\n" +
                                                                      " - Oneshots\n" +
                                                                      " - Doujin\n" +
                                                                      " - Manhwa\n" +
                                                                      " - Manhua\n" +
                                                                      " - By popularity on MAL\n\n" +                                                               
                                                                     
                                                                      "/check_list\n" +
                                                                      "This command allows you to view all titles that you added to your ''Favorites'' list.\n" +
                                                                      "Information about title is displayed im the following order:\n" +
                                                                      "Title ID => Title name => Title type (anime or manga)\n\n" +
                                                                      
                                                                      "/delete_title\n" +
                                                                      "This command allows you to delete certain title by its ID from your ''Favorites'' list. " +
                                                                      "As a result you'll receive message about succesfull or unsuccessful delete operation.\n\n" +                                                                     

                                                                      "/delete_all\n" +
                                                                      "This command allows you to delete all titles from your ''Favorites'' list. " +
                                                                      "As a result you'll receive message about succesfull or unsuccessful delete operation.\n\n" +

                                                                      "/add_item_to_list\n" +
                                                                      "This command allows you to add certain title to your ''Favourites'' list by its ID. " +
                                                                      "As a result you'll receive message about succesfull or unsuccessful add operation.\n\n", 
                                                                      replyMarkup: Close());
                return;
            }
            else
            if (message.Text == "/search_by_name")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Send_Choose_Search_by_name_Object(botClient, message);

                return;
            }
            else
            if (message.Text == "/get_info_by_id")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Send_Choose_Search_by_ID_Object(botClient, message);

                return;
            }
            else
            if (message.Text == "/get_ranking")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Send_Choose_Ranking(botClient, message);

                return;
            }
            else
            if (message.Text == "/check_list")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Check_user_favourites_in_Data_Base(botClient, message);

                return;
            }                    
            else
            if (message.Text == "/delete_title")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Send_to_get_title_ID_for_deletion(botClient, message);

                return;
            }
            else
            if (message.Text == "/delete_all")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                await Delete_All_user_favorites_from_Data_Base(botClient, message);

                return;
            }            
            else
            if (LastText == "Enter anime ID:" || LastText == "Invalid input for anime ID, please try again:")
            {                
                string ID = message.Text;

                Regex pattern = new("^[0-9]+$");

                Match match = pattern.Match(ID);
                if (match.Success)
                {
                    if (ID.Length > 9)
                    {                        
                        LastText = "Invalid input for anime ID, please try again:";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for anime ID, please try again:");
                    }
                    else
                    {
                        LastText = "";
                        await Get_Anime_info_by_anime_ID(botClient, message, Convert.ToInt32(ID));
                    }                    
                }
                else
                {
                    LastText = "Invalid input for anime ID, please try again:";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for anime ID, please try again:");
                }
                
                return;
            }
            else
            if (LastText == "Enter manga ID:" || LastText == "Invalid input for manga ID, please try again:")
            {
                string ID = message.Text;
                
                Regex pattern = new("^[0-9]+$");

                Match match = pattern.Match(ID);
                if (match.Success)
                {
                    if (ID.Length > 9)
                    {
                        LastText = "Invalid input for manga ID, please try again:";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for manga ID, please try again:");
                    }
                    else
                    {
                        LastText = "";
                        await Get_Manga_info_by_anime_ID(botClient, message, Convert.ToInt32(ID));
                    }                    
                }
                else
                {
                    LastText = "Invalid input for manga ID, please try again:";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for manga ID, please try again:");
                }
                
                return;
            }
            else
            if (LastText == "Enter ID of the title you want to delete:" || LastText == "Invalid input for title ID, please try again:")
            {
                string ID = message.Text;

                Regex pattern = new("^[0-9]+$");

                Match match = pattern.Match(ID);
                if (match.Success)
                {
                    if (ID.Length > 9)
                    {
                        LastText = "Invalid input for title ID, please try again:";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for title ID, please try again:");
                    }
                    else
                    {
                        LastText = "";
                        await Delete_one_user_favorites_from_Data_Base(botClient, message, Convert.ToInt32(ID));
                    }
                }
                else
                {
                    LastText = "Invalid input for title ID, please try again:";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for title ID, please try again:");
                }

                return;
            }
            else            
            if (LastText == "Enter anime name:")
            {
                string Q = message.Text;

                await Search_for_Anime_by_name(botClient, message, Q);

                LastText = "";
                return;
            }
            else
            if (LastText == "Enter manga name:")
            {
                string Q = message.Text;

                await Search_for_Manga_by_name(botClient, message, Q);

                LastText = "";

                return;
            }
            else
            if (message.Text == "/add_item_to_list")
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                
                await Send_choose_title_type_for_add(botClient, message);

                return;
            }
            else
            if (LastText == "Enter ID of the anime you want to add:" || LastText == "Invalid input for ID of anime you want to add, please try again:")
            {
                string ID = message.Text;

                Regex pattern = new("^[0-9]+$");

                Match match = pattern.Match(ID);
                if (match.Success)
                {
                    if (ID.Length > 9)
                    {
                        LastText = "Invalid input for ID of anime you want to add, please try again:";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for ID of anime you want to add, please try again:");
                    }
                    else
                    {
                        LastText = "";
                        Title_ID = Convert.ToInt32(ID);
                        await Post_chosen_title_to_DB(botClient, message, Title_ID, "anime");
                    }
                }
                else
                {
                    LastText = "Invalid input for ID of anime you want to add, please try again:";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for ID of anime you want to add, please try again:");
                }

                return;
            }
            else
            if (LastText == "Enter ID of the manga you want to add:" || LastText == "Invalid input for ID of manga you want to add, please try again:")
            {
                string ID = message.Text;

                Regex pattern = new("^[0-9]+$");

                Match match = pattern.Match(ID);
                if (match.Success)
                {
                    if (ID.Length > 9)
                    {
                        LastText = "Invalid input for ID of manga you want to add, please try again:";
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for ID of manga you want to add, please try again:");
                    }
                    else
                    {
                        LastText = "";
                        Title_ID = Convert.ToInt32(ID);
                        await Post_chosen_title_to_DB(botClient, message, Title_ID, "manga");
                    }
                }
                else
                {
                    LastText = "Invalid input for ID of manga you want to add, please try again:";
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Invalid input for ID of manga you want to add, please try again:");
                }

                return;
            }       
        }   


        #region Anime_by_name_search
        // Пошук аніме, назва яких схожа на введене користувачем слово або набір слів
        private async Task Search_for_Anime_by_name(ITelegramBotClient botClient, Message message, string Q)
        {
            int Limit = 10;
            
            var search_by_name_result = new Search_for_Anime_or_Manga_BotClient().Search_Anime_by_name(Q, Limit).Result;

            if (search_by_name_result == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"No results for ''{Q}''.", replyMarkup: Close());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, search_by_name_result, replyMarkup: Close());
            }
        }
        #endregion


        #region Manga_by_name_search
        // Пошук манги, назва якої схожа на введене користувачем слово або набір слів
        private async Task Search_for_Manga_by_name(ITelegramBotClient botClient, Message message, string Q)
        {
            int Limit = 10;            

            var search_by_name_result = new Search_for_Anime_or_Manga_BotClient().Search_Manga_by_name(Q, Limit).Result;

            if (search_by_name_result == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"No results for ''{Q}''.", replyMarkup: Close());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, search_by_name_result, replyMarkup: Close());
            }
        }
        #endregion


        #region Anime_by_Id_info
        // пошук інфомації про тайтл (аніме), за введеним айді
        private async Task Get_Anime_info_by_anime_ID(ITelegramBotClient botClient, Message message, int Title_ID) 
        {
            var result = new Search_for_Anime_or_Manga_BotClient().Get_info_by_Anime_ID(Title_ID).Result;

            if (result == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Anime with this ID ({Title_ID}) doesn't exist", replyMarkup: Close());
                return;
            }
            else
            {
                #region Genres
                var genres_list = result.Genres;
                string genres = "Genres and themes:  ";
                int count_genres = 0;
                foreach (Genres genre in genres_list)
                {
                    count_genres++;
                    if (count_genres < genres_list.Count)
                    {
                        genres += $"{genre.Name}, ";
                    }
                    if (count_genres == genres_list.Count)
                    {
                        genres += $"{genre.Name}";
                    }
                }
                #endregion

                #region Studios
                var studios_list = result.Studios;
                string studios = "Studios:  ";
                int count_studios = 0;
                foreach (Studios studio in studios_list)
                {
                    count_studios++;
                    if (count_studios < studios_list.Count)
                    {
                        studios += $"{studio.Name}, ";
                    }
                    if (count_studios == studios_list.Count)
                    {
                        studios += $"{studio.Name}";
                    }
                }
                #endregion

                #region Related
                var related = result.Related_Anime;
                string related_anime = $"Related titles:\n";
                if (related.Count == 0)
                {
                    related_anime = "Related titles:  none\n";
                }
                else
                {
                    foreach (Related_anime rel in related)
                    {
                        related_anime += $"- ID: {rel.Node.ID}, ''{rel.Node.Title}'' ({rel.Relation_type})\n";
                    }
                }
                #endregion

                #region Recommendations
                var recommendation_list = result.Recommendations;
                string recommendations = $"Recommendations:\n";
                if (recommendation_list.Count == 0)
                {
                    recommendations = "Recommendations:  none\n";
                }
                else
                {
                    foreach (Recommendations recom in recommendation_list)
                    {
                        recommendations += $"- ID: {recom.Node.ID}, ''{recom.Node.Title}''\n";
                    }
                }
                #endregion

                #region Rating
                string rating = "";
                if (result.Rating == "g")
                {
                    rating = "G";
                }
                if (result.Rating == "pg")
                {
                    rating = "PG";
                }
                if (result.Rating == "pg_13")
                {
                    rating = "PG-13";
                }
                if (result.Rating == "r")
                {
                    rating = "R - 17";
                }
                if (result.Rating == "r+")
                {
                    rating = "R+";
                }
                if (result.Rating == "rx")
                {
                    rating = "Rx";
                }
                #endregion

                #region Type
                string type = "";
                if (result.Media_type == "tv")
                {
                    type = "TV";
                }
                if (result.Media_type == "ova")
                {
                    type = "OVA";
                }
                if (result.Media_type == "movie")
                {
                    type = "Movie";
                }
                if (result.Media_type == "special")
                {
                    type = "Special";
                }
                if (result.Media_type == "ona")
                {
                    type = "ONA";
                }
                if (result.Media_type == "music")
                {
                    type = "Music";
                }
                #endregion

                #region Status
                string status = "";
                if (result.Status == "finished_airing")
                {
                    status = "Finished airing";
                }
                if (result.Status == "not_yet_aired")
                {
                    status = "Not yet aired";
                }
                if (result.Status == "currently_airing")
                {
                    status = "Currently airing";
                }
                #endregion

                #region Source
                string source = "";
                if (result.Source == "4_koma_manga")
                {
                    source = "4 - koma manga";
                }
                if (result.Source == "book")
                {
                    source = "Book";
                }
                if (result.Source == "card_game")
                {
                    source = "Card game";
                }
                if (result.Source == "digital_manga")
                {
                    source = "Digital manga";
                }
                if (result.Source == "game")
                {
                    source = "Game";
                }
                if (result.Source == "light_novel")
                {
                    source = "Light novel";
                }
                if (result.Source == "manga")
                {
                    source = "Manga";
                }
                if (result.Source == "mixed_media")
                {
                    source = "Mixed media";
                }
                if (result.Source == "music")
                {
                    source = "Music";
                }
                if (result.Source == "novel")
                {
                    source = "Novel";
                }
                if (result.Source == "original")
                {
                    source = "Original";
                }
                if (result.Source == "other")
                {
                    source = "Other";
                }
                if (result.Source == "picture_book")
                {
                    source = "Picture book";
                }
                if (result.Source == "radio")
                {
                    source = "Radio";
                }
                if (result.Source == "unknown")
                {
                    source = "Unknown";
                }
                if (result.Source == "visual_novel")
                {
                    source = "Visual novel";
                }
                if (result.Source == "web_manga")
                {
                    source = "Web manga";
                }
                #endregion

                string image_caption = $"Title:  ''{result.Title}''\n" +
                                       $"ID: {result.ID}";

                string result_message = $"Title:  ''{result.Title}''\n\n" +
                                        $"Alternative titles:\n" +
                                        $"- En:  {result.Alternative_Titles.En}\n" +
                                        $"- Ja/Kr:  {result.Alternative_Titles.Ja}\n" +
                                        $"ID:  {result.ID}\n" +
                                        $"Mean score:  {result.Mean}\n" +
                                        $"Rank:  {result.Rank}\n" +
                                        $"Type:  {type}\n" +
                                        $"{genres}\n" +
                                        $"Rating:  {rating}\n" +
                                        $"Episodes:  {result.Num_episodes}\n" +
                                        $"Season:  {result.Start_Season.Season}, {result.Start_Season.Year}\n" +
                                        $"Status:  {status}\n" +
                                        $"Source:  {source}\n" +
                                        $"{studios}\n\n" +
                                        $"{related_anime}\n" +
                                        $"{recommendations}\n";

                await botClient.SendPhotoAsync(
                    message.Chat.Id,
                    result.Main_Picture.Large,
                    caption: image_caption,
                    //parseMode: ParseMode.Markdown,
                    replyMarkup: Close(),
                    cancellationToken: cancellationToken);
               
                await botClient.SendTextMessageAsync(message.Chat.Id, result_message, replyMarkup: Close());

                return;
            }
        }
        #endregion


        #region Manga_by_ID_info
        // пошук інфомації про тайтл (манга), за введеним айді
        private async Task Get_Manga_info_by_anime_ID(ITelegramBotClient botClient, Message message, int Title_ID)
        {            
            var result = new Search_for_Anime_or_Manga_BotClient().Get_info_by_Manga_ID(Title_ID).Result;

            if (result == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Manga with this ID ({Title_ID}) doesn't exist", replyMarkup: Close());
                return;
            }
            else
            {
                #region Genres
                var genres_list = result.Genres;
                string genres = "Genres and themes:  ";
                int count_genres = 0;
                foreach (Genres genre in genres_list)
                {
                    count_genres++;
                    if (count_genres < genres_list.Count)
                    {
                        genres += $"{genre.Name}, ";
                    }
                    if (count_genres == genres_list.Count)
                    {
                        genres += $"{genre.Name}";
                    }
                }
                #endregion

                #region Authors
                var authors_list = result.Authors;
                string authors = "Authors:  ";
                int count_authors = 0;
                foreach (Authors author in authors_list)
                {
                    count_authors++;
                    if (count_authors < authors_list.Count)
                    {
                        authors += $"{author.Node.First_name} {author.Node.Last_name}, ";
                    }
                    if (count_authors == authors_list.Count)
                    {
                        authors += $"{author.Node.First_name} {author.Node.Last_name}";
                    }
                }
                #endregion

                #region Serialization
                var serialization_list = result.Serialization;
                string serializations = "Serialization:  ";
                int serialization_count = 0;
                foreach (Serialization ser in serialization_list)
                {
                    serialization_count++;
                    if (serialization_count < serialization_list.Count)
                    {
                        serializations += $"{ser.Node.Name}, ";
                    }
                    if (serialization_count == serialization_list.Count)
                    {
                        serializations += $"{ser.Node.Name}";
                    }
                }
                #endregion

                #region Related
                var related = result.Related_Manga;
                string related_manga = "Related titles:\n";
                if (related.Count == 0)
                {
                    related_manga = "Related titles:  none\n";
                }
                else
                {
                    foreach (Related_manga rel in related)
                    {
                        related_manga += $"- ID: {rel.Node.ID}, ''{rel.Node.Title}'' ({rel.Relation_type})\n";
                    }
                }
                #endregion

                #region Recommendations
                var recommendation_list = result.Recommendations;
                string recommendations = "Recommendations:\n";
                if (recommendation_list.Count == 0)
                {
                    recommendations = "Recommendations:  none\n";
                }
                else
                {
                    foreach (Recommendations recom in recommendation_list)
                    {
                        recommendations += $"- ID: {recom.Node.ID}, ''{recom.Node.Title}''\n";
                    }
                }
                #endregion

                #region Type
                string type = "";
                if (result.Media_type == "manga")
                {
                    type = "Manga";
                }
                if (result.Media_type == "one_shot")
                {
                    type = "One - shot";
                }
                if (result.Media_type == "doujinshi")
                {
                    type = "Doujinshi";
                }
                if (result.Media_type == "light_novel")
                {
                    type = "Light novel";
                }
                if (result.Media_type == "novel")
                {
                    type = "Novel";
                }
                if (result.Media_type == "manhwa")
                {
                    type = "Manhwa";
                }
                if (result.Media_type == "manhua")
                {
                    type = "Manhua";
                }
                #endregion

                #region Status  
                string status = "";
                if (result.Status == "finished")
                {
                    status = "Finished";
                }
                if (result.Status == "currently_publishing")
                {
                    status = "Currently publishing";
                }
                if (result.Status == "on_hiatus")
                {
                    status = "On Hiatus";
                }
                if (result.Status == "discontinued")
                {
                    status = "Discontinued";
                }
                if (result.Status == "not_yet_published")
                {
                    status = "Not yet published";
                }
                #endregion

                string image_caption = $"Title:  ''{result.Title}''\n" +
                                       $"ID: {result.ID}";

                string result_message = $"Title:  ''{result.Title}''\n\n" +
                                        $"Alternative titles:\n" +
                                        $"- En:  {result.Alternative_Titles.En}\n" +
                                        $"- Ja/Kr:  {result.Alternative_Titles.Ja}\n" +
                                        $"ID:  {result.ID}\n" +
                                        $"Mean score:  {result.Mean}\n" +
                                        $"Rank:  {result.Rank}\n" +
                                        $"Type:  {type}\n" +
                                        $"{genres}\n" +
                                        $"Status:  {status}\n" +
                                        $"Volumes:  {result.Num_Volumes}\n" +
                                        $"Chapters:  {result.Num_chapters}\n" +
                                        $"{authors}\n" +
                                        $"{serializations}\n\n" +
                                        $"{related_manga}\n" +
                                        $"{recommendations}\n";
                
                await botClient.SendPhotoAsync(
                    message.Chat.Id,
                    result.Main_Picture.Large,
                    caption: image_caption,
                    //parseMode: ParseMode.Markdown,
                    replyMarkup: Close(),
                    cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(message.Chat.Id, result_message, replyMarkup: Close());

                return;
            }
        }
        #endregion


        #region Anime_ranking
        // повертає ранкінг аніме за популярністю
        private async Task Get_Anime_Ranking(ITelegramBotClient botClient, Message message, string Ranking_Type, string Ranking_from_upper)
        {
            int Limit = 10;            

            var get_ranking = new Search_for_Anime_or_Manga_BotClient().Get_Anime_Ranking_by_type(Ranking_Type, Limit, Ranking_from_upper).Result;

            if (get_ranking == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Something went wrong, try again", replyMarkup: Close());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, get_ranking, replyMarkup: Close());
            }
            return;
        }
        #endregion


        #region Manga_ranking
        // повертає ранкінг манги за популярністю
        private async Task Get_Manga_Ranking(ITelegramBotClient botClient, Message message, string Ranking_Type, string Ranking_from_upper)
        {
            int Limit = 10;           

            var get_ranking = new Search_for_Anime_or_Manga_BotClient().Get_Manga_Ranking_by_type(Ranking_Type, Limit, Ranking_from_upper).Result;

            if (get_ranking == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Something went wrong, try again", replyMarkup: Close());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, get_ranking, replyMarkup: Close());
            }
            return;
        }
        #endregion


        #region Check_all_user_items
        // перревіряє наявність у юзера збережених до БД тайтлів
        private async Task Check_user_favourites_in_Data_Base(ITelegramBotClient botClient, Message message)
        {
            int Telegram_user_ID = Convert.ToInt32(message.Chat.Id);

            var check_all_items_result = new Search_for_Anime_or_Manga_BotClient().Get_All_user_favourites(Telegram_user_ID).Result;
                        
            if (check_all_items_result == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Unfortunately, you don't have any items in your 'Favorites' list 😢", replyMarkup: Close());
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, check_all_items_result, replyMarkup: Close());
            }           
            return;
        }
        #endregion


        #region Post_item_to_DB
        // додає обраний юзером об'єкт до БД
        private async Task Post_chosen_title_to_DB(ITelegramBotClient botClient, Message message, int title_ID, string title_type) 
        {
            if (title_type == "anime")
            {
                var check_item = new Search_for_Anime_or_Manga_BotClient().Get_info_by_Anime_ID(Title_ID).Result;

                if (check_item == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Anime with this ID ({Title_ID}) doesn't exist", replyMarkup: Close());
                    return;
                }
                else
                {
                    var obj = new DB_object()
                    {
                        Telegram_ID = Convert.ToInt32(message.Chat.Id),
                        Title_ID = title_ID,
                        Title_name = check_item.Title,
                        Title_type = title_type
                    };

                    var post_result = new Search_for_Anime_or_Manga_BotClient().Post_data_to_Data_Base(obj).Result;

                    await botClient.SendTextMessageAsync(message.Chat.Id, post_result, replyMarkup: Close());
                    return;
                }
            }
            if (title_type == "manga")
            {
                var check_item = new Search_for_Anime_or_Manga_BotClient().Get_info_by_Manga_ID(Title_ID).Result;

                if (check_item == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Manga with this ID ({Title_ID}) doesn't exist", replyMarkup: Close());
                    return;
                }
                else
                {
                    var obj = new DB_object()
                    {
                        Telegram_ID = Convert.ToInt32(message.Chat.Id),
                        Title_ID = title_ID,
                        Title_name = check_item.Title,
                        Title_type = title_type
                    };

                    var post_result = new Search_for_Anime_or_Manga_BotClient().Post_data_to_Data_Base(obj).Result;

                    await botClient.SendTextMessageAsync(message.Chat.Id, post_result, replyMarkup: Close());
                    return;
                }
            }          
        }
        #endregion


        #region Delete_item
        // видаляє з БД певний елемент юзера за айді елемента (айді обраного тайтла)
        private async Task Delete_one_user_favorites_from_Data_Base(ITelegramBotClient botClient, Message message, int ID)
        {
            int Telegram_user_ID = Convert.ToInt32(message.Chat.Id);           

            var delete_item_result = new Search_for_Anime_or_Manga_BotClient().Delete_one_user_item_from_DB(Telegram_user_ID, ID).Result;

            await botClient.SendTextMessageAsync(message.Chat.Id, delete_item_result, replyMarkup: Close());
            return;
        }
        #endregion


        #region Delete_all_items   
        // видаляє з БД усі елементи юзера
        private async Task Delete_All_user_favorites_from_Data_Base(ITelegramBotClient botClient, Message message) 
        {
            int Telegram_user_ID = Convert.ToInt32(message.Chat.Id);

            var delete_item_result = new Search_for_Anime_or_Manga_BotClient().Delete_All_user_items_from_DB(Telegram_user_ID).Result;

            await botClient.SendTextMessageAsync(message.Chat.Id, delete_item_result, replyMarkup: Close());
            return;
        }
        #endregion




        #region Search_title_by_name
        private async Task Send_Choose_Search_by_name_Object(ITelegramBotClient botClient, Message message) 
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the object that you want to search by name:",
                replyMarkup: Choose_Search_by_name_object_keyboard()
                );
        }
        private InlineKeyboardMarkup Choose_Search_by_name_object_keyboard() 
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                   new[]
                   {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Anime", $"Anime_by_name"),
                            InlineKeyboardButton.WithCallbackData("Manga", $"Manga_by_name")
                        }
                   });

            return inlineKeyboard;
        }
        private async Task Send_to_get_anime_by_name(ITelegramBotClient botClient, Message message)
        {
            LastText = "Enter anime name:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter anime name:");
            return;
        }
        private async Task Send_to_get_manga_by_name(ITelegramBotClient botClient, Message message)
        {
            LastText = "Enter manga name:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter manga name:");
            return;
        }
        #endregion

        #region Get_info_by_title_ID
        private async Task Send_Choose_Search_by_ID_Object(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the object that you want to search by ID:",
                replyMarkup: Choose_ID_search_object_keyboard()
                );
        }
        private InlineKeyboardMarkup Choose_ID_search_object_keyboard()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Anime", $"Anime_by_ID"),
                            InlineKeyboardButton.WithCallbackData("Manga", $"Manga_by_ID")
                        }
                    });

            return inlineKeyboard;
        }
        //private InlineKeyboardMarkup After_ID_search()
        //{
        //    InlineKeyboardMarkup inlineKeyboard = new(
        //            new[]
        //            {
        //                // first row
        //                new[]
        //                {
        //                    InlineKeyboardButton.WithCallbackData("Close", $"Close"),
        //                    InlineKeyboardButton.WithCallbackData("Add to favorites", $"Add_to_favorites")
        //                }
        //            });

        //    return inlineKeyboard;
        //}
        private async Task Send_to_get_anime_ID(ITelegramBotClient botClient, Message message) 
        {
            LastText = "Enter anime ID:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter anime ID:");
            return;
        }
        private async Task Send_to_get_manga_ID(ITelegramBotClient botClient, Message message)
        {
            LastText = "Enter manga ID:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter manga ID:");
            return;
        }
        #endregion

        #region Get_Ranking
        private async Task Send_Choose_Ranking(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the ranking object:",
                replyMarkup: Choose_ranking_keyboard()
                );
        }
        private async Task Send_Choose_Anime_Ranking_Type(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the type of anime for ranking :",
                replyMarkup: Choose_anime_ranking_type_keyboard()
                );
        }
        private async Task Send_Choose_Manga_Ranking_Type(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the type of manga for ranking :",
                replyMarkup: Choose_manga_ranking_type_keyboard()
                );
        }
        private InlineKeyboardMarkup Choose_ranking_keyboard()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Anime", $"Anime_ranking"),
                            InlineKeyboardButton.WithCallbackData("Manga", $"Manga_ranking")
                        }
                    });

            return inlineKeyboard;
        }
        private InlineKeyboardMarkup Choose_anime_ranking_type_keyboard()
        {
            // -- типи аніме, за яким буде проводитись ранкінг:
            //                                                 * all (усі)
            //                                                 * airing (ті, що зараз виходять)
            //                                                 * upcoming (анонси)
            //                                                 * tv (ТВ)
            //                                                 * ova (ОВА)
            //                                                 * movie (фільм)
            //                                                 * special (спешл)
            //                                                 * bypopularity (за популярністю на MAL)
            //                                                 * favorite (за додаванням до улюблених на МАL)
            InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("All", $"All_anime"),
                            InlineKeyboardButton.WithCallbackData("Airing", $"Airing")                  
                        },
                        // second row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Upcoming", $"Upcoming"),
                            InlineKeyboardButton.WithCallbackData("TV", $"TV")
                        },
                        // third row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("OVA", $"OVA"),
                            InlineKeyboardButton.WithCallbackData("Movie", $"Movie")
                        },
                        // fourth row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Special", $"Special"),
                            InlineKeyboardButton.WithCallbackData("By popularity on MAL", $"By_anime_popularity")
                        }
                    });

            return inlineKeyboard;
        }
        private InlineKeyboardMarkup Choose_manga_ranking_type_keyboard()
        {
            // -- типи манги, за яким буде проводитись ранкінг:
            //                                                 * all (усі)
            //                                                 * manga (манга)
            //                                                 * novels (новели)
            //                                                 * oneshots (ваншоти)
            //                                                 * doujin (додзін)
            //                                                 * manhwa (манхва)
            //                                                 * manhua (маньхуа)
            //                                                 * bypopularity (за популярністю на MAL)
            //                                                 * favorite (за додаванням до улюблених на МАL)
            InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("All", $"All_manga"),
                            InlineKeyboardButton.WithCallbackData("Manga", $"Manga")                    
                        },
                        // second row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Novels", $"Novels"),
                            InlineKeyboardButton.WithCallbackData("Oneshots", $"Oneshots")
                        },
                        // third row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Doujin", $"Doujin"),
                            InlineKeyboardButton.WithCallbackData("Manhwa", $"Manhwa")
                        },
                        // fourth row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Manhua", $"Manhua"),
                            InlineKeyboardButton.WithCallbackData("By popularity on MAL", $"By_manga_popularity")
                        }
                    });

            return inlineKeyboard;
        }
        #endregion

        #region Delete_item
        private async Task Send_to_get_title_ID_for_deletion(ITelegramBotClient botClient, Message message)
        {
            LastText = "Enter ID of the title you want to delete:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter ID of the title you want to delete:");
            return;
        }
        #endregion

        #region Close
        private InlineKeyboardMarkup Close()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Close", $"Close")
                        }
                    });

            return inlineKeyboard;
        }
        #endregion

        #region Add_title_to_DB
        private async Task Send_choose_title_type_for_add(ITelegramBotClient botClient, Message message) 
        {
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Choose the object that you want to add to the favourites list:",
                replyMarkup: Choose_title_type_for_add_keyboard()
                );
        }
        private InlineKeyboardMarkup Choose_title_type_for_add_keyboard()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                   new[]
                   {
                        // first row
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Anime", $"Anime_add"),
                            InlineKeyboardButton.WithCallbackData("Manga", $"Manga_add")
                        }
                   });

            return inlineKeyboard;
        }
        private async Task Enter_anime_id_for_add(ITelegramBotClient botClient, Message message) 
        {
            LastText = "Enter ID of the anime you want to add:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter ID of the anime you want to add:");
            return;
        }
        private async Task Enter_manga_id_for_add(ITelegramBotClient botClient, Message message)
        {
            LastText = "Enter ID of the manga you want to add:";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Enter ID of the manga you want to add:");
            return;
        }
        #endregion




        #region CallbackQuery
        private async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                #region Close_case
                // close_case
                case "Close":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    break;
                #endregion

                #region Ranking_cases
                // ranking cases              
                case "Anime_ranking":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_Choose_Anime_Ranking_Type(botClient, callbackQuery.Message!);
                    break;
                
                case "Manga_ranking":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_Choose_Manga_Ranking_Type(botClient, callbackQuery.Message!);
                    break;
                #endregion

                #region Anime_ranking_cases
                // anime_ranking_cases
                case "All_anime":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "all", "All");
                    break;

                case "Airing":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "airing", "Airing");
                    break;

                case "Upcoming":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "upcoming", "Upcoming");
                    break;

                case "TV":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "tv", "TV");
                    break;

                case "OVA":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "ova", "OVA");
                    break;

                case "Movie":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "movie", "Movie");
                    break;

                case "Special":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "special", "Special");
                    break;

                case "By_anime_popularity":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Anime_Ranking(botClient, callbackQuery.Message!, "bypopularity", "By anime popularity");
                    break;
                #endregion

                #region Manga_ranking_cases
                // manga_ranking_cases
                case "All_manga":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "all", "All");
                    break;

                case "Manga":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "manga", "Manga");
                    break;

                case "Novels":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "novels", "Novels");
                    break;

                case "Oneshots":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "oneshots", "Oneshots");
                    break;

                case "Doujin":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "doujin", "Doujin");
                    break;

                case "Manhwa":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "manhwa", "Manhwa");
                    break;

                case "Manhua":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "manhua", "Manhua");
                    break;

                case "By_manga_popularity":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Get_Manga_Ranking(botClient, callbackQuery.Message!, "bypopularity", "By manga popularity");
                    break;
                #endregion

                #region Anime_by_ID_case
                // anime_by_ID_case
                case "Anime_by_ID":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_to_get_anime_ID(botClient, callbackQuery.Message!);
                    break;
                #endregion

                #region Manga_by_ID_case
                // manga_by_ID_case
                case "Manga_by_ID":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_to_get_manga_ID(botClient, callbackQuery.Message!);
                    break;
                #endregion

                #region Anime_by_name_case
                // anime_by_name_case
                case "Anime_by_name":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_to_get_anime_by_name(botClient, callbackQuery.Message!);
                    break;
                #endregion

                #region Manga_by_name_case
                // manga_by_name_case
                case "Manga_by_name":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Send_to_get_manga_by_name(botClient, callbackQuery.Message!);
                    break;
                #endregion

                #region Add_to_favourites_case
                //// Add_to_favourites_case
                //case "Add_to_favorites":

                //    break;
                #endregion

                #region Add_to_favourites_choose_case
                // Add_to_favourites_case
                case "Anime_add":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Enter_anime_id_for_add(botClient, callbackQuery.Message!);
                    break;

                case "Manga_add":
                    await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId);
                    await Enter_manga_id_for_add(botClient, callbackQuery.Message!);
                    break;
                    #endregion
            }

            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);
            return;
        }
        #endregion
    }
}
