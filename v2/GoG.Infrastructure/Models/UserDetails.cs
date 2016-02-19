using System;

namespace GoG.Client.Models
{
    // Generated from JSON:
    // {
    //   "username":"cbordeman",
    //   "rating":-800,
    //   "ranking":0,
    //   "about":"",
    //   "settings":"/api/v1/me/settings",
    //   "friends":"/api/v1/me/friends",
    //   "games":"/api/v1/me/games",
    //   "challenges":"/api/v1/me/challenges",
    //   "groups":"/api/v1/me/groups",
    //   "mail":"/api/v1/me/mail",
    //   "tournaments":"/api/v1/me/tournaments",
    //   "vacation":"/api/v1/me/vacation",
    //   "notifications":"/api/v1/me/notifications"
    // }

    public class UserDetails
    {
        public int rating { get; set; }
        public int ranking { get; set; }
        public string about { get; set; }
        public string settings { get; set; }
        public string friends { get; set; }
        public string games { get; set; }
        public string challenges { get; set; }
        public string groups { get; set; }
        public string mail { get; set; }
        public string tournaments { get; set; }
        public string vacation { get; set; }
        public string notifications { get; set; }
    }
}
