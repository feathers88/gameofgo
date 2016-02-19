namespace GoG.Client.ViewModels
{
    public interface ICurrentUserUserControlViewModel : IViewModelBase
    {
        string UserId { get; set; }
        string Gravatar { get; set; }
    }
}
