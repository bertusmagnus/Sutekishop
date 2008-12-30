using Iesi.Collections.Generic;

namespace Suteki.Common.Models
{
    public interface IComposite<T>
    {
        T Parent { get; }
        ISet<T> Children { get; }
    }
}