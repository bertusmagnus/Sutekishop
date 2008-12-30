using Suteki.Common.UI;

namespace Suteki.Common.Services
{
    public interface IFormBuilder<T>
    {
        SelectListCollection GetSelectLists(T entity);
    }
}