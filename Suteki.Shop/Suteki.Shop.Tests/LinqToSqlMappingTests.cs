using System;
using System.Linq;
using NUnit.Framework;

namespace Suteki.Shop.Tests
{
	[TestFixture, Explicit, Category("Database")]
	public class LinqToSqlMappingTests
	{
		[Test]
		public void AllColumnsShouldBeMapped()
		{
			string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SutekiShop;Integrated Security=True";

			using (var context = new ShopDataContext(connectionString))
			{
				context.Log = Console.Out;

				foreach (var mappedTable in context.Mapping.GetTables().Select(x => x.RowType.Type)) 
				{
					context.GetTable(mappedTable).Cast<object>().Take(0).ToList();
				}
			}
		}
	}
}