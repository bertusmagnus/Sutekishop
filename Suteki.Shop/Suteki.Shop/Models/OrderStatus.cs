namespace Suteki.Shop
{
    public partial class OrderStatus
    {
        // these constants must match those in the database
        public static int CreatedId { get { return 1; } }
        public static int DispatchedId { get { return 2; } }
        public static int RejectedId { get { return 3; } }
    }
}
