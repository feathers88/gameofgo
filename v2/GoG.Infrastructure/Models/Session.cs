using System;

namespace GoG.Client.Models
{
    public enum SessionType
    {
        Microsoft
    }

    public class Session
    {
        public SessionType SessionType { get; set; }
        
        public UserProfile UserProfile { get; set; }
        public UserDetails UserDetails { get; set; }
        public UserSettings UserSettings { get; set; }

        public string AccessToken { get; set; }
        public string AuthenticationToken { get; set; }
        public DateTime? TokenExpires { get; set; }
    }
}
