namespace Suteki.Common.UI.FormTypeBinders
{
    public class ReadonlyFormTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            return !context.Property.CanWrite;
        }
    }
}