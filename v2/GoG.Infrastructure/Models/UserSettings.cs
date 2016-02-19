using System;

namespace GoG.Client.Models
{
    // Generated from JSON:
    // {
    //   "profile":{
    //      "username":"cbordeman",
    //      "website":"",
    //      "last_name":"",
    //      "rating":-800,
    //      "vacation_left":1910661.0,
    //      "ranking":0,
    //      "on_vacation":false,
    //      "about":"",
    //      "professional":false,
    //      "id":156,
    //      "icon":"http://www.gravatar.com/avatar/d894a427bea90911ed86252888c579dd?s=32&d=http%3A%2F%2Fcdn.online-go.com%2F1.18%2Fimg%2Fdefault-user-32.png",
    //      "first_name":"",
    //      "country":"un",
    //      "real_name_is_private":true,
    //      "email":"cbordeman@outlook.com"
    //   },
    //   "notifications":{
    //      "groupInvitation":{
    //         "type":"notification",
    //         "key":"groupInvitation",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"I receive an invitation to a group"
    //      },
    //      "tournamentStarted":{
    //         "type":"notification",
    //         "key":"tournamentStarted",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"A tournament you have joined has started"
    //      },
    //      "gameResumedFromStoneRemoval":{
    //         "type":"notification",
    //         "key":"gameResumedFromStoneRemoval",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"Game has been resumed from the stone removal phase"
    //      },
    //      "groupRequest":{
    //         "type":"notification",
    //         "key":"groupRequest",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"Someone requests to join my group"
    //      },
    //      "groupNews":{
    //         "type":"notification",
    //         "key":"groupNews",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"Group news is sent out"
    //      },
    //      "challenge":{
    //         "type":"notification",
    //         "key":"challenge",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"Someone challenges me to a game"
    //      },
    //      "yourMove":{
    //         "notes":"Only sent if you're not online when your opponent moved",
    //         "type":"notification",
    //         "key":"yourMove",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"It is my turn to move"
    //      },
    //      "gameStarted":{
    //         "type":"notification",
    //         "key":"gameStarted",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"A game starts"
    //      },
    //      "gameEnded":{
    //         "type":"notification",
    //         "key":"gameEnded",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"A game ends"
    //      },
    //      "timecop":{
    //         "type":"notification",
    //         "key":"timecop",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"I am running out of time to make a move"
    //      },
    //      "mail":{
    //         "type":"notification",
    //         "key":"mail",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"A user sends you a mail message"
    //      },
    //      "tournamentInvitation":{
    //         "type":"notification",
    //         "key":"tournamentInvitation",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"I receive an invitation to a tournament"
    //      },
    //      "tournamentEnded":{
    //         "type":"notification",
    //         "key":"tournamentEnded",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"A tournament you have joined has finished"
    //      },
    //      "friendRequest":{
    //         "type":"notification",
    //         "key":"friendRequest",
    //         "value":{
    //            "mobile":true,
    //            "email":true
    //         },
    //         "description":"Someone sends me a friend request"
    //      }
    //   },
    //   "site_preferences":{
    //      "hide_recently_finished_games":false,
    //      "profanity_filter":true,
    //      "language":"auto",
    //      "theme":"default",
    //      "auto_advance_after_submit":false,
    //      "show_game_list_view":false,
    //      "font":"default"
    //   }
    // }
    
    public class UserProfile
    {
        // Basic properties from Live.
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Link { get; set; }
        public object Gender { get; set; }
        public string Locale { get; set; }
        public DateTime? UpdatedTime { get; set; }

        // GoG properties.
        public int Rating { get; set; }
        public double VacationLeft { get; set; }
        public int Ranking { get; set; }
        public bool IsOnVacation { get; set; }
        public string About { get; set; }
        public bool IsProfessional { get; set; }
        public string Icon { get; set; }
        public string Country { get; set; }
        public bool IsRealNamePrivate { get; set; }
        public string Email { get; set; }
    }

    public class ContactMethods
    {
        public bool mobile { get; set; }
        public bool email { get; set; }
    }

    public class GroupInvitation
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class TournamentStarted
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class GameResumedFromStoneRemoval
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class GroupRequest
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class GroupNews
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class Challenge
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class YourMove
    {
        public string notes { get; set; }
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class GameStarted
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class GameEnded
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class Timecop
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class Mail
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class TournamentInvitation
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class TournamentEnded
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class FriendRequest
    {
        public string type { get; set; }
        public string key { get; set; }
        public ContactMethods value { get; set; }
        public string description { get; set; }
    }

    public class Notifications
    {
        public GroupInvitation groupInvitation { get; set; }
        public TournamentStarted tournamentStarted { get; set; }
        public GameResumedFromStoneRemoval gameResumedFromStoneRemoval { get; set; }
        public GroupRequest groupRequest { get; set; }
        public GroupNews groupNews { get; set; }
        public Challenge challenge { get; set; }
        public YourMove yourMove { get; set; }
        public GameStarted gameStarted { get; set; }
        public GameEnded gameEnded { get; set; }
        public Timecop timecop { get; set; }
        public Mail mail { get; set; }
        public TournamentInvitation tournamentInvitation { get; set; }
        public TournamentEnded tournamentEnded { get; set; }
        public FriendRequest friendRequest { get; set; }
    }

    public class SitePreferences
    {
        public bool hide_recently_finished_games { get; set; }
        public bool profanity_filter { get; set; }
        public string language { get; set; }
        public string theme { get; set; }
        public bool auto_advance_after_submit { get; set; }
        public bool show_game_list_view { get; set; }
        public string font { get; set; }
    }

    public class UserSettings
    {
        public Notifications notifications { get; set; }
        public SitePreferences site_preferences { get; set; }
    }
}
