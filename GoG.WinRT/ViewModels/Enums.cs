namespace GoG.WinRT.ViewModels
{
    

    /// <summary>
    /// Defines values that indicate the visual state of a board space.
    /// </summary>
    public enum BoardSpaceState
    {
        None,
        PlayerOne,
        PlayerTwo,
        PlayerOneHint, // not using yet
        PlayerTwoHint, // not using yet
        PlayerOneNewPiece,
        PlayerTwoNewPiece,
        PlayerOneNewCapture,
        PlayerTwoNewCapture
    }

    
}
