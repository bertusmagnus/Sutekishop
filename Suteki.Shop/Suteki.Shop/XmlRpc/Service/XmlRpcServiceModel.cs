using System;
using Castle.Facilities.WcfIntegration;

namespace Suteki.Shop.XmlRpc.Service
{
    public class XmlRpcServiceModel : WcfServiceModel<XmlRpcServiceModel>
    {
		public XmlRpcServiceModel()
		{
            Console.WriteLine("XmlRpcServiceModel.ctor");
		}

		public XmlRpcServiceModel(string baseAddress)
		{
			AddBaseAddresses(baseAddress);
		}

        public XmlRpcServiceModel(Uri baseAddress)
		{
			AddBaseAddresses(baseAddress);
		}
    }
}