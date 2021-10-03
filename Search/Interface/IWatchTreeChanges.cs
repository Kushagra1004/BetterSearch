using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search.Interface
{
    public interface IWatchTreeChanges
    {
        void WatchChanges(SearchNode node);
    }
}
