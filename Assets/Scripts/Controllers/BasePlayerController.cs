using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalon.Models.Commands;
using UnityEngine;

namespace Avalon.Controllers
{
    public abstract class BasePlayerController : MonoBehaviour
    {
        public Int32 PlayerId { get; private set; }

        /// <summary>
        /// Végrehajt egy parancsot.
        /// </summary>
        /// <param name="command"></param>
        public abstract void ExecuteCommand(BaseCommand command);

        /// <summary>
        /// Ha végrehajtódik egy parancs, elküldi a parancsot és az új modellt.
        /// </summary>
        public event EventHandler<ExecCommandEventArgs> CommandAdded;
    }
}
