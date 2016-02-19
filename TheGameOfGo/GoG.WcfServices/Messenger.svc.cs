using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GoG.Fuego;
using GoG.Repository.Engine;
using GoG.ServerCommon.Logging;
using GoG.Services.Contracts;
using GoG.Services.Logging;

namespace GoG.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Messenger" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Messenger.svc or Messenger.svc.cs at the Solution Explorer and start debugging.
    public class Messenger : IMessenger
    {
        #region Data
        private readonly ILogger _logger;
        private readonly IGoGRepository _goGRepository;
        #endregion Data

        #region Ctor

        public Messenger() : this(new Logger(), new DbGoGRepository(new Logger())) { }

        public Messenger(ILogger logger, IGoGRepository goGRepository)
        {
            logger.LogGameInfo(Guid.Empty, "Entering Fuego (service) constructor.");

            _logger = logger;
            _goGRepository = goGRepository;
            
            // Initialize the singleton fuego engine using the repository of choice.
            FuegoEngine.Init(logger, _goGRepository); // Spins up the fuego.exe instances.
        }

        #endregion Ctor

        #region IMessenger Members

        public string GetActiveMessage()
        {
            try
            {
                var msg = _goGRepository.GetActiveMessage();
                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogServerError(Guid.Empty, ex, null);
                return null;
                throw;
            }
        }

        #endregion
    }
}
