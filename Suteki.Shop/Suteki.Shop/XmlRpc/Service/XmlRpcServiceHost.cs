using System;
using Castle.Core;
using Castle.Facilities.WcfIntegration;

namespace Suteki.Shop.XmlRpc.Service
{
    public class XmlRpcServiceHost : DefaultServiceHost
    {
        public XmlRpcServiceHost(ComponentModel model, params Uri[] baseAddresses) : base(model, baseAddresses)
        {
        }

        public XmlRpcServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }
    }
}