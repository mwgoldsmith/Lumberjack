using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medidata.Lumberjack.Core.Data.Collections
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void ItemUpdatedHandler<T>(object source, CollectionItemEventArgs<T> e);

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void ItemAddedHandler<T>(object source, CollectionItemEventArgs<T> e);

    /// <summary>
    ///
    /// </summary>
    /// <param name="source">Object which is sending the message.</param>
    /// <param name="e">Event arguments for the event handler.</param>
    public delegate void ItemRemovedHandler<T>(object source, CollectionItemEventArgs<T> e);

}
