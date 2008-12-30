using System;
using Suteki.Common.Models;

namespace Suteki.Common.Services
{
    public interface IEntityTypeResolver
    {
        Type GetTypeOf(IEntity entity);
        Type GetRealTypeOf(Type type);
    }
}