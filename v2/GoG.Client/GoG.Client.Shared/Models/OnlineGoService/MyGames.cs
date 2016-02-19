// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace GoG.Client.Models.OnlineGoService.MyGames
{
    public class Related
    {
        public string detail { get; set; }
    }

    public class Player
    {
        public string username { get; set; }
        public int ranking { get; set; }
        public bool professional { get; set; }
        public Related related { get; set; }
        public int? id { get; set; }
        public string country { get; set; }
        public string icon { get; set; }
        public string ui_class { get; set; }
    }

    public class Players
    {
        public Player white { get; set; }
        public Player black { get; set; }
    }

    public class Game
    {
        public Related related { get; set; }
        public Players players { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int creator { get; set; }
        public string mode { get; set; }
        public string source { get; set; }
        public int? black { get; set; }
        public int? white { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string rules { get; set; }
        public bool ranked { get; set; }
        public int handicap { get; set; }
        public string komi { get; set; }
        public string time_control { get; set; }
        public int time_per_move { get; set; }
        public string time_control_parameters { get; set; }
        public bool disable_analysis { get; set; }
        public int? tournament { get; set; }
        public int tournament_round { get; set; }
        public object ladder { get; set; }
        public bool pause_on_weekends { get; set; }
        public string outcome { get; set; }
        public bool black_lost { get; set; }
        public bool white_lost { get; set; }
        public bool annulled { get; set; }
        public string started { get; set; }
        public string ended { get; set; }
        public object sgf_filename { get; set; }
    }

    public class GamesPager
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<Game> results { get; set; }
    }





    
}
