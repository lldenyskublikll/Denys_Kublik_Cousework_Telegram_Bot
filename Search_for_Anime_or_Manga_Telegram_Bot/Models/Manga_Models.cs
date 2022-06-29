using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search_for_Anime_or_Manga_Telegram_Bot.Models
{
    #region Working_correctly
    public class Manga_ranking_Model // модель для отримуваних даних з публічної АПІ при пошуку схожої за введеною назвою манги
    {
        public List<Data_1> Data { get; set; }
    }


    public class Manga_by_name_model // модель для отримуваних даних з публічної АПІ при з запиті ранкінгу манги
    {
        public List<Data_2> Data { get; set; }
    }


    public class Manga_by_ID_Model // модель для отримуваних даних з публічної АПІ при пошуку повної інформації про обраний тайтл
    {
        public string Title { get; set; }
        public Main_picture Main_Picture { get; set; }
        public Alternative_titles Alternative_Titles { get; set; }
        public int ID { get; set; }
        public double Mean { get; set; }
        public int Rank { get; set; }
        public string Media_type { get; set; }
        public List<Genres> Genres { get; set; }
        public string Status { get; set; }
        public int Num_Volumes { get; set; }
        public int Num_chapters { get; set; }
        public List<Authors> Authors { get; set; }
        public List<Serialization> Serialization { get; set; }
        public List<Related_manga> Related_Manga { get; set; }
        public List<Recommendations> Recommendations { get; set; }
    }
    #endregion


    public class Node_1
    {
        public string First_name { get; set; }
        public string Last_name { get; set; }
    }
    public class Authors
    {
        public Node_1 Node { get; set; }
    }
    public class Node_2
    {
        public string Name { get; set; }
    }
    public class Serialization
    {
        public Node_2 Node { get; set; }
    }
    public class Related_manga
    {
        public Node Node { get; set; }
        public string Relation_type { get; set; }
    }
}
