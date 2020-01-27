using System;
using System.ServiceModel;
using DataBaseAccessService;

namespace SharedServiceLibrary
{
    public static class SDbService
    {
        public static IDataBaseAccessService Get()
        {
            return ChannelFactory<IDataBaseAccessService>.CreateChannel(
                    new NetTcpBinding(),
                    new EndpointAddress(new Uri(@"net.tcp://localhost:8334/")));
        }
    }
}
