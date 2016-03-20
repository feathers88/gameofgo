namespace GoG.Services.Contracts
{
    /// <summary>
    /// The Game Of Go chat service contract.
    /// </summary>
    public interface IChat
    {
        void SendMessage(string message);
    }
}
