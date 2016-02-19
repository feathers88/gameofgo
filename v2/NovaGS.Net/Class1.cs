using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NovaGS.Net
{
    public class TimeControl
    {
        public string time_control { get; set; }
        public int period_time { get; set; }
        public int main_time { get; set; }
        public int periods { get; set; }
    }

    public class PauseControl
    {
    }

    public class BlackTime
    {
        public double thinking_time { get; set; }
        public int period_time { get; set; }
        public int periods { get; set; }
    }

    public class WhiteTime
    {
        public double thinking_time { get; set; }
        public int period_time { get; set; }
        public int periods { get; set; }
    }

    public class Clock
    {
        public int current_player { get; set; }
        public string title { get; set; }
        public int paused_since { get; set; }
        public int black_player_id { get; set; }
        public long last_move { get; set; }
        public int white_player_id { get; set; }
        public long expiration { get; set; }
        public int game_id { get; set; }
        public long now { get; set; }
        public BlackTime black_time { get; set; }
        public WhiteTime white_time { get; set; }
    }

    public class InitialState
    {
        public string white { get; set; }
        public string black { get; set; }
    }

    [DataContract]
    public class Player
    {
        public string username { get; set; }
        public int egf { get; set; }
        public int rank { get; set; }
    }

    [DataContract]
    public class Players
    {
        public Player white { get; set; }
        public Player black { get; set; }
    }

    public class RootObject
    {
        public bool score_stones { get; set; }
        public bool allow_ko { get; set; }
        public int height { get; set; }
        public TimeControl time_control { get; set; }
        public bool free_handicap_placement { get; set; }
        public PauseControl pause_control { get; set; }
        public List<int> meta_groups { get; set; }
        public string moves { get; set; }
        public bool allow_superko { get; set; }
        public bool score_passes { get; set; }
        public Clock clock { get; set; }
        public int black_player_id { get; set; }
        public int white_player_id { get; set; }
        public int width { get; set; }
        public int handicap { get; set; }
        public InitialState initial_state { get; set; }
        public int start_time { get; set; }
        public bool score_territory_in_seki { get; set; }
        public bool automatic_stone_removal { get; set; }
        public string rules { get; set; }
        public double komi { get; set; }
        public bool score_prisoners { get; set; }
        public bool disable_analysis { get; set; }
        public bool allow_self_capture { get; set; }
        public bool ranked { get; set; }
        public string phase { get; set; }
        public int game_id { get; set; }
        public bool opponent_plays_first_after_resume { get; set; }
        public string superko_algorithm { get; set; }
        public bool white_must_pass_last { get; set; }
        public Players players { get; set; }
        public string game_name { get; set; }
        public bool score_territory { get; set; }
        public string initial_player { get; set; }
        public List<object> history { get; set; }
    }
}
