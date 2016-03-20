using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace GoG.Services.Contracts
{
    [ServiceContract]
    public interface IMessenger
    {
        [OperationContract]
        string GetActiveMessage();
    }
}