using System;
using System.ServiceModel;
using Castle.Core;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel;
using Microsoft.ServiceModel.Samples.XmlRpc;

namespace Suteki.Shop.XmlRpc.Service
{
    public class XmlRpcServiceHostBuilder : AbstractServiceHostBuilder<XmlRpcServiceModel>
    {
        public XmlRpcServiceHostBuilder(IKernel kernel) : base(kernel)
        {
        }

        #region Overrides of AbstractServiceHostBuilder<XmlRpcServiceModel>


        protected override System.ServiceModel.Channels.Binding GetDefaultBinding(ServiceHost serviceHost, string address)
        {
            return new XmlRpcHttpBinding();
        }

        protected override ServiceHost CreateServiceHost(ComponentModel model, XmlRpcServiceModel serviceModel, params Uri[] baseAddresses)
        {
            return CreateXmlRpcServiceHost(model.Implementation,
                GetEffectiveBaseAddresses(serviceModel, baseAddresses));
        }

        protected override ServiceHost CreateServiceHost(ComponentModel model, Uri[] baseAddresses)
        {
            return CreateXmlRpcServiceHost(model.Implementation, baseAddresses);
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return CreateXmlRpcServiceHost(serviceType, baseAddresses);
        }

        #endregion

        private ServiceHost CreateXmlRpcServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new Microsoft.ServiceModel.Samples.XmlRpc.XmlRpcServiceHost(serviceType, baseAddresses);
        }
    }
}