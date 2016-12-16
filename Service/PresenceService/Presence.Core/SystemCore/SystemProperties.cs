using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presence.Core.SystemCore
{
    /// <summary>
    ///---------------------------------------------------------------------------------------------------------
    ///   Namespace:      Presence.Core.SystemCore
    ///   Class:          SystemProperties
    ///   Description:    Retains all properties and parameters for the correct functioning of 
    ///                   the entire system, including tasks.
    ///   Author:         Henrique Dal Bello Batista                 Date: 13/10/2015
    ///   Notes:          
    ///---------------------------------------------------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///---------------------------------------------------------------------------------------------------------
    /// </summary>
    public class SystemProperties
    {
        private string logPath;

        public SystemProperties()
        {
            this.getProperties();
        }

        /// <summary>
        /// Reload Properties
        /// </summary>
        public void ReloadProperties()
        {
            this.getProperties();
        }

        /// <summary>
        /// Get properties from UCETB_SYSTEM_PARAMETER
        /// </summary>
        private void getProperties()
        {
            this.logPath = @"C:\ApplicationTests\PresenceService\LOGS";
        }

        public string LogPath
        {
            get { return logPath; }
        }

    }
}
