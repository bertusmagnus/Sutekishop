using System;
using NHibernate.Proxy;
using Suteki.Common.Models;

namespace Suteki.Common.Services
{
    public class NHibernateEntityTypeResolver : IEntityTypeResolver
    {
        public Type GetTypeOf(IEntity entity)
        {
            return NHibernateProxyHelper.GuessClass(entity);
        }

        public Type GetRealTypeOf(Type type)
        {
            var nhibernateProxyInterface = type.GetInterface("INHibernateProxy");
            if(nhibernateProxyInterface == null)
            {
                return type;
            }

            return type.BaseType;
        }
    }
}