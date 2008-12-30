using Suteki.Common.Validation;

namespace Suteki.Common.Models
{
    public interface INamedEntity : IEntity
    {
        string Name { get; }
    }

    public abstract class NamedEntity<T> : Entity<T>, INamedEntity where T : class, IEntity
    {
        private string name;
        public virtual string Name
        {
            get { return name; }
            set
            {
                name = value.Label("Name").IsRequired().Value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}