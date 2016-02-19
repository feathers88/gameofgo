using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace GoG.BackgroundTasks
{
    public sealed class TileBackgroundTask : IBackgroundTask
    {
        #region IBackgroundTask Members

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the taks from closing prematurely while 
            // async code is running.
            var deferral = taskInstance.GetDeferral();

            // 
        }

        #endregion
    }
}
