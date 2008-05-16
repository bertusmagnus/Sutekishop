namespace Suteki.Shop
{
    public partial class Size : IActivatable
    {
        public string NameAndStock
        {
            get
            {
                return Name + (IsInStock ? "" : " Out of Stock");
            }
        }
    }
}
