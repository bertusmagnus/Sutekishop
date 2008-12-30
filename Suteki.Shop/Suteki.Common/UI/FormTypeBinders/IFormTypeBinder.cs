namespace Suteki.Common.UI.FormTypeBinders
{
    public interface IFormTypeBinder<T>
    {
        bool TryBind(FormTypeBindingContext<T> context);
    }
}