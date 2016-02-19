using System.Collections.Generic;

namespace GoG.Client.Models.OnlineGoService.GameDetails
{
    public class Related
    {
        public string reviews { get; set; }
    }

    public class Related2
    {
        public string detail { get; set; }
    }

    public class White
    {
        public Related2 related { get; set; }
        public int id { get; set; }
        public string username { get; set; }
        public string country { get; set; }
        public string icon { get; set; }
        public int ranking { get; set; }
        public bool professional { get; set; }
        public string ui_class { get; set; }
    }

    public class Related3
    {
        public string detail { get; set; }
    }

    public class Black
    {
        public Related3 related { get; set; }
        public int id { get; set; }
        public string username { get; set; }
        public string country { get; set; }
        public string icon { get; set; }
        public int ranking { get; set; }
        public bool professional { get; set; }
        public string ui_class { get; set; }
    }

    public class Players
    {
        public White white { get; set; }
        public Black black { get; set; }
    }

    public class TimeControl
    {
        public string time_control { get; set; }
        public int initial_time { get; set; }
        public int max_time { get; set; }
        public int time_increment { get; set; }
    }

    public class PauseControl
    {
    }

    public class BlackTime
    {
        public double thinking_time { get; set; }
        public bool skip_bonus { get; set; }
    }

    public class WhiteTime
    {
        public double thinking_time { get; set; }
        public bool skip_bonus { get; set; }
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

    public class White2
    {
        public string username { get; set; }
        public int egf { get; set; }
        public int rank { get; set; }
        public int id { get; set; }
    }

    public class Black2
    {
        public string username { get; set; }
        public int egf { get; set; }
        public int rank { get; set; }
        public int id { get; set; }
    }

    public class Players2
    {
        public White2 white { get; set; }
        public Black2 black { get; set; }
    }

    public class Gamedata
    {
        public bool score_stones { get; set; }
        public bool allow_ko { get; set; }
        public int height { get; set; }
        public TimeControl time_control { get; set; }
        public bool free_handicap_placement { get; set; }
        public PauseControl pause_control { get; set; }
        public List<object> meta_groups { get; set; }
        public string moves { get; set; }
        public bool allow_superko { get; set; }
        public bool score_passes { get; set; }
        public Clock clock { get; set; }
        public int black_player_id { get; set; }
        public int winner { get; set; }
        public int white_player_id { get; set; }
        public int width { get; set; }
        public InitialState initial_state { get; set; }
        public int end_time { get; set; }
        public bool score_territory_in_seki { get; set; }
        public bool automatic_stone_removal { get; set; }
        public int handicap { get; set; }
        public int start_time { get; set; }
        public bool score_prisoners { get; set; }
        public bool disable_analysis { get; set; }
        public bool allow_self_capture { get; set; }
        public bool ranked { get; set; }
        public double komi { get; set; }
        public int game_id { get; set; }
        public bool strict_seki_mode { get; set; }
        public bool opponent_plays_first_after_resume { get; set; }
        public string superko_algorithm { get; set; }
        public bool white_must_pass_last { get; set; }
        public string rules { get; set; }
        public Players2 players { get; set; }
        public string phase { get; set; }
        public string game_name { get; set; }
        public bool score_territory { get; set; }
        public string outcome { get; set; }
        public string initial_player { get; set; }
        public List<object> history { get; set; }
    }

    public class GameDetails
    {
        public Related related { get; set; }
        public Players players { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int creator { get; set; }
        public string mode { get; set; }
        public string source { get; set; }
        public int black { get; set; }
        public int white { get; set; }
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
        public object tournament { get; set; }
        public int tournament_round { get; set; }
        public object ladder { get; set; }
        public bool pause_on_weekends { get; set; }
        public string outcome { get; set; }
        public bool black_lost { get; set; }
        public bool white_lost { get; set; }
        public bool annulled { get; set; }
        public string started { get; set; }
        public string ended { get; set; }
        public Gamedata gamedata { get; set; }
        public string auth { get; set; }
        public string game_chat_auth { get; set; }
    }
}
