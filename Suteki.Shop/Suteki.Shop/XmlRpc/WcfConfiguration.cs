using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.ServiceModel.Samples.XmlRpc;
using Suteki.Shop.XmlRpc.Service;

namespace Suteki.Shop.XmlRpc
{
    public static class WcfConfiguration
    {
        public static void ConfigureContainer(IWindsorContainer container)
        {
            var returnFaults = new ServiceDebugBehavior {IncludeExceptionDetailInFaults = true};

            container.AddFacility<WcfFacility>(f => 
            {
                f.Services.AspNetCompatibility = AspNetCompatibilityRequirementsMode.Required;
                f.DefaultBinding = new XmlRpcHttpBinding();
            })
                .Register(
//                    Component.For<IServiceHostBuilder<XmlRpcServiceModel>>().ImplementedBy<XmlRpcServiceHostBuilder>(),
//                    Component.For<XmlRpcServiceModel>(),
                    Component.For<IServiceBehavior>().Instance(returnFaults),
                    Component.For<XmlRpcEndpointBehavior>(),
                    Component.For<IMetaWeblog>().ImplementedBy<MetaWeblogWcf>().Named("metaWebLog").LifeStyle.Transient
                    );

        }
    }
}