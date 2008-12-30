namespace Suteki.Common.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        bool IsNew { get; }
    }
}