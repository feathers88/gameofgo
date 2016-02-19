namespace NovaGS.WinRT
{
    public class GameClockUpdate
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
        public long now { get; set; }

    }
}
