using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search_for_Anime_or_Manga_Telegram_Bot.Models
{
    #region Working_correctly
    public class Anime_ranking_Model // модель для отримуваних даних з публічної АПІ при пошуку схожих за введеною назвою аніме                 
    {
        public List<Data_1> Data { get; set; }
    }


    public class Anime_by_name_model // модель для отримуваних даних з публічної АПІ при з запиті ранкінгу аніме
    {
        public List<Data_2> Data { get; set; }
    }


    public class Anime_by_ID_Model // модель для отримуваних даних з публічної АПІ при пошуку повної інформації про обраний тайтл
    {
        public string Title { get; set; }
        public Main_picture Main_Picture { get; set; }
        public Alternative_titles Alternative_Titles { get; set; }
        public int ID { get; set; }
        public double Mean { get; set; }
        public int Rank { get; set; }
        public string Media_type { get; set; }
        public List<Genres> Genres { get; set; }
        public string Rating { get; set; }
        public int Num_episodes { get; set; }
        public Start_season Start_Season { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
        public List<Studios> Studios { get; set; }
        public List<Related_anime> Related_Anime { get; set; }
        public List<Recommendations> Recommendations { get; set; }
    }
    #endregion


    public class Start_season
    {
        public int Year { get; set; }
        public string Season { get; set; }
    }
    public class Studios
    {
        public string Name { get; set; }
    }
    public class Related_anime
    {
        public Node Node { get; set; }
        public string Relation_type { get; set; }
    }
}
