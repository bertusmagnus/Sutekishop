using System.Data.Linq;

namespace Suteki.Shop.Models
{
	public interface ILoadOptions
	{
		void Build(DataLoadOptions options);
	}
}