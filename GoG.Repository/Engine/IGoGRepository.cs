using System;
using System.Collections.Generic;
using GoG.Infrastructure.Engine;

namespace GoG.Repository.Engine
{
    public interface IGoGRepository
    {
        string GetActiveMessage();

        /// <summary>
        /// Gets the game, returns null if doesn't exist.
        /// </summary>
        GoGameState GetGameState(Guid gameId);

        /// <summary>
        /// Returns true if the game exists.
        /// </summary>
        bool GetGameExists(Guid gameId);

        /// <summary>
        /// Saves game or adds it if it didn't exist.
        /// </summary>
        void SaveGameState(Guid gameId, GoGameState gameState);

        /// <summary>
        /// If gameId exists, removes it.
        /// </summary>
        /// <param name="gameId"></param>
        void RemoveGameIfExists(Guid gameId);

        

    }
}
