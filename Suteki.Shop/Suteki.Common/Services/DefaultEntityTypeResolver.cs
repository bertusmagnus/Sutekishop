using System;
using Suteki.Common.Models;

namespace Suteki.Common.Services
{
    public class DefaultEntityTypeResolver : IEntityTypeResolver
    {
        public Type GetTypeOf(IEntity entity)
        {
            return entity.GetType();
        }

        public Type GetRealTypeOf(Type type)
        {
            return type;
        }
    }
}