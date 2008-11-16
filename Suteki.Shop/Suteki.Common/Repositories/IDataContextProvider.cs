using System.Data.Linq;

namespace Suteki.Common.Repositories
{
    public interface IDataContextProvider
    {
        DataContext DataContext { get; }
    }
}
