using System;

namespace Suteki.Shop.Binders
{
	public interface IAcceptsAttribute
	{
		void Accept(Attribute attribute);
	}
}