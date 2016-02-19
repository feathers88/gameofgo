using System.Collections.Generic;

namespace NovaGS.WinRT.Model
{
    public class Clock
    {
        public int current_player { get; set; }
        public string title { get; set; }
        public int black_player_id { get; set; }
        public long last_move { get; set; }
        public int white_player_id { get; set; }
        public long expiration { get; set; }
        public int game_id { get; set; }
        public int black_time { get; set; }
        public long white_time { get; set; }
    }

    public class Side
    {
        public int stones { get; set; }
        public double total { get; set; }
        public int handicap { get; set; }
        public int prisoners { get; set; }
        public string scoring_positions { get; set; }
        public double komi { get; set; }
        public int territory { get; set; }
    }


    public class White : Side {}
    public class Black : Side {}

    public class Score
    {
        public White white { get; set; }
        public Black black { get; set; }
    }

    public class InitialState
    {
        public string white { get; set; }
        public string black { get; set; }
    }

    public class MalkovichLog
    {
        public int date { get; set; }
        public int player_id { get; set; }
        public int move_number { get; set; }
        public string body { get; set; }
        public string username { get; set; }
    }

    public class ChatLog
    {
        public int date { get; set; }
        public int player_id { get; set; }
        public int move_number { get; set; }
        public object body { get; set; }
        public string username { get; set; }
    }

    public class White2
    {
        public string username { get; set; }
        public string accepted_stones { get; set; }
        public int elo { get; set; }
        public int rank { get; set; }
    }

    public class Black2
    {
        public string username { get; set; }
        public string accepted_stones { get; set; }
        public int elo { get; set; }
        public int rank { get; set; }
    }

    public class Players
    {
        public White2 white { get; set; }
        public Black2 black { get; set; }
    }

    public class GameData
    {
        public bool score_stones { get; set; }
        public int time_canadian_moves { get; set; }
        public bool allow_ko { get; set; }
        public int height { get; set; }
        public string time_control { get; set; }
        public bool free_handicap_placement { get; set; }
        public List<int> meta_groups { get; set; }
        public string moves { get; set; }
        public bool allow_superko { get; set; }
        public int time_fischer_max { get; set; }
        public bool score_passes { get; set; }
        public Clock clock { get; set; }
        public int black_player_id { get; set; }
        public int winner { get; set; }
        public int white_player_id { get; set; }
        public int width { get; set; }
        public Score score { get; set; }
        public InitialState initial_state { get; set; }
        public int time_per_move { get; set; }
        public bool score_territory_in_seki { get; set; }
        public List<MalkovichLog> malkovich_log { get; set; }
        public bool automatic_stone_removal { get; set; }
        public int handicap { get; set; }
        public bool score_prisoners { get; set; }
        public bool allow_self_capture { get; set; }
        public List<ChatLog> chat_log { get; set; }
        public int time_initial { get; set; }
        public double komi { get; set; }
        public int game_id { get; set; }
        public string removed { get; set; }
        public bool opponent_plays_first_after_resume { get; set; }
        public string superko_algorithm { get; set; }
        public bool white_must_pass_last { get; set; }
        public string rules { get; set; }
        public Players players { get; set; }
        public string phase { get; set; }
        public int end_time { get; set; }
        public bool score_territory { get; set; }
        public string outcome { get; set; }
        public string initial_player { get; set; }
        public List<object> history { get; set; }
    }
}
