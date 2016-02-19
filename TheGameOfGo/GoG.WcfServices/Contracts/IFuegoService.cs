using System;
using System.ServiceModel;
using GoG.Infrastructure.Engine;
using GoG.Infrastructure.Services.Engine;

namespace GoG.Services.Contracts
{
    /// <summary>
    /// A WCF service contract for a Fuego AI player.
    /// </summary>
    [ServiceContract]
    public interface IFuegoService
    {
        // Checks that a game exists.
        [OperationContract]
        GoResponse GetGameExists(Guid gameid);

        // Gets the game state from server so the client can sync up.
        [OperationContract]
        GoGameStateResponse GetGameState(Guid gameid);

        // Creates a new game.  Fails if a game with this gameid already exists.
        [OperationContract]
        GoGameStateResponse Start(Guid gameid, GoGameState state);

        // Used to setup handicap or other initial board position.  Only works before the first move is made.
        //GoResponse Place(Guid gameid, GoGameState clientState, GoMove[] moves);

        // Asks the engine to generate its own move.
        [OperationContract]
        GoMoveResponse GenMove(Guid gameid, GoColor color);

        // Client is making move.
        [OperationContract]
        GoMoveResponse Play(Guid gameid, GoMove move);

        // Undo one or more moves.
        [OperationContract]
        GoGameStateResponse Undo(Guid gameid);

        
        //// Returns the name of the engine or remote opponent (e.g. "Fuego", "Rahul").
        //string Name();

        //// Returns the version of the engine (e.g. "1.1").
        //string Version();

        //// Scores the game.
        //GoScoreResponse Score(Guid gameid, GoGameState clientState);

        //// Returns which stones are 'dead'.
        //GoPositionsResponse Dead(Guid gameid, GoGameState clientState);

        //// Returns the territory currently held by a given color.
        //GoPositionsResponse Territory(Guid gameid, GoGameState clientState, GoColor color);

        // Returns possible moves for a given color.
        [OperationContract]
        GoHintResponse Hint(Guid gameid, GoColor color);

        [OperationContract]
        GoSaveSVGResponse SaveSGF(Guid gameid);

        [OperationContract]
        GoResponse LoadSGF(Guid gameid, string sgf);
    }
}