using System;

namespace GoG.Client.Services.Popups
{
    public class DialogCommand
    {
        public object Id { get; set; }
        public string Label { get; set; }
        public Action OnInvoked { get; set; }
        public bool WasInvoked { get; set; }
    }
}
