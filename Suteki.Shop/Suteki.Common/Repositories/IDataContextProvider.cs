using System;
using System.Data.Linq;

namespace Suteki.Common.Repositories
{
    public interface IDataContextProvider : IDisposable
    {
        DataContext DataContext { get; }
    }
}
