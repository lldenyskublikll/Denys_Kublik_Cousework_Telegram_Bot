using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Search_for_Anime_or_Manga_Telegram_Bot.Models;

namespace Search_for_Anime_or_Manga_Telegram_Bot.Clients
{
    class Search_for_Anime_or_Manga_BotClient
    {
        private readonly HttpClient _client;
        private static string _address;


        public Search_for_Anime_or_Manga_BotClient()
        {
            _address = Constants.adress;

            _client = new HttpClient();

            _client.BaseAddress = new Uri(_address);
        }


        #region Anime_Tasks
        public async Task<string> Search_Anime_by_name(string Q, int Limit) 
        {
            var response = await _client.GetAsync($"/Animanga/Search_by_anime_name?Q={Q}&Limit={Limit}");

            var content = response.Content.ReadAsStringAsync().Result;                  

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound || content.Contains("{\"data\":[]}"))
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Anime_by_name_model>(content);
                var result_list = result.Data;
                int count = 0;

                string result_message = $"Search results for ''{Q}'':\n\n";

                foreach (Data_2 data in result_list)
                {
                    count++;
                    result_message += $"{count}) ID: {data.Node.ID}, ''{data.Node.Title}''\n";
                }
                return result_message;
            }
        }

        public async Task<Anime_by_ID_Model> Get_info_by_Anime_ID(int Title_ID) 
        {
            var response = await _client.GetAsync($"/Animanga/Get_anime_by_id_info?ID={Title_ID}");

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response == null || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Anime_by_ID_Model>(content);
                return result;                          
            }
        }

        public async Task<string> Get_Anime_Ranking_by_type(string Ranking_model, int Limit, string Ranking_from_upper)
        {
            var response = await _client.GetAsync($"/Animanga/Get_anime_ranking?Ranking_model={Ranking_model}&Limit={Limit}");

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Anime_ranking_Model>(content);
                var result_list = result.Data;

                string result_message = $"Anime ranking by type ''{Ranking_from_upper}'':\n\n";

                foreach (Data_1 data in result_list)
                {
                    result_message += $"{data.Ranking.Rank}) ID: {data.Node.ID}, ''{data.Node.Title}''\n";
                }
                return result_message;
            }
        }
        #endregion


        #region Manga_Tasks
        public async Task<string> Search_Manga_by_name(string Q, int Limit)
        {
            var response = await _client.GetAsync($"/Animanga/Search_by_manga_name?Q={Q}&Limit={Limit}");

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response == null || response.StatusCode == System.Net.HttpStatusCode.NotFound || content.Contains("{\"data\":[]}"))
            {
                return null;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response == null)
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Manga_by_name_model>(content);
                var result_list = result.Data;
                int count = 0;

                string result_message = $"Search results for ''{Q}'':\n\n";

                foreach (Data_2 data in result_list)
                {
                    count++;
                    result_message += $"{count}) ID: {data.Node.ID}, ''{data.Node.Title}''\n";
                }
                return result_message;
            }
        }

        public async Task<Manga_by_ID_Model> Get_info_by_Manga_ID(int Title_ID)
        {
            var response = await _client.GetAsync($"/Animanga/Get_manga_by_id_info?ID={Title_ID}");

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response == null || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Manga_by_ID_Model>(content);
                return result;
            }
        }

        public async Task<string> Get_Manga_Ranking_by_type(string Ranking_model, int Limit, string Ranking_from_upper)
        {
            var response = await _client.GetAsync($"/Animanga/Get_manga_ranking?Ranking_model={Ranking_model}&Limit={Limit}");

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<Manga_ranking_Model>(content);
                var result_list = result.Data;

                string result_message = $"Manga ranking by type ''{Ranking_from_upper}'':\n\n";

                foreach (Data_1 data in result_list)
                {
                    result_message += $"{data.Ranking.Rank}) ID: {data.Node.ID}, ''{data.Node.Title}''\n";
                }
                return result_message;
            }
        }
        #endregion
        


        #region DB_tasks
        public async Task<string> Get_All_user_favourites(int Telegram_user_ID)
        {
            var response = await _client.GetAsync($"/Animanga/Get_ALL_user_favourites_from_DB?Telegram_user_ID={Telegram_user_ID}");
            
            var result_list = new List<DB_response>();

            var content = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                // convert from json
                var result = JsonConvert.DeserializeObject<List<DB_response>>(content);
                foreach (var item in result)
                {
                    result_list.Add(item);
                }

                int count = 0;
                string result_message = "Your ''Favorites'' list:\n\n";
                foreach (var item in result_list)
                {
                    if (count <= result_list.Count)
                    {
                        count++;
                        result_message += $"{count}) ID: {item.Title_ID}, ''{item.Title_name}'' ({item.Title_type})\n";
                    }                    
                }
                return result_message;
            }
        }


        public async Task<string> Post_data_to_Data_Base(DB_object obj) 
        {
            var response = await _client.GetAsync($"/Animanga/Get_info_about item_from_DB?Telegram_ID={obj.Telegram_ID}&Title_ID={obj.Title_ID}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return "This item is already in your ''Favourites'' list";                
            }
            else
            {
                var json_obj = JsonConvert.SerializeObject(obj);

                var data = new StringContent(json_obj, Encoding.UTF8, "application/json");

                try
                {
                    var send_item = await _client.PostAsync($"/Animanga/Add_favourite_item_to_DB", data);

                    return "The item was successfully added to your 'Favourites list'";
                }
                catch (Exception)
                {

                    return "Unablle to add this item to your ''Favourites list''";
                }
            }                        
        }


        public async Task<string> Delete_one_user_item_from_DB(int Telegram_user_ID, int Title_ID)
        {
            var response = await _client.DeleteAsync($"/Animanga/Delete_item_from_DB?Telegram_ID={Telegram_user_ID}&Title_ID={Title_ID}");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return "You don't have this item in your ''Favourites list''";
            }
            else
            { 
                return "The item was successfully deleted from your ''Favourites list''"; 
            }            
        }


        public async Task<string> Delete_All_user_items_from_DB(int Telegram_user_ID) 
        {
            var response = await _client.DeleteAsync($"/Animanga/Delete_ALL_user_data_from_DB?Telegram_user_ID={Telegram_user_ID}");
             
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return "You don't have any items in your ''Favourites list''";
            }
            else
            {
                return "All items were successfully deleted from your ''Favourites list''";
            }
        }
        #endregion
    }
}
