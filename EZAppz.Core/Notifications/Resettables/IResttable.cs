using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public interface IResettable
    {
        /// <summary>
        /// Saves the current changes in the object as the current state
        /// </summary>
        void SaveCurrentState();
        /// <summary>
        /// Reverts back all the changes that happened since the lastest save/reset
        /// </summary>
        void Reset();

    }
}
