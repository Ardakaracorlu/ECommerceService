namespace Order.Api.Extension
{
    public static class OrderExtension
    {
        public static  IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            Order.Service.Extension.ServiceExtension.RegisterService(services, configuration);
            Order.Consumer.Extension.ConsumerExtension.RegisterService(services, configuration);    
            return services;
        }
    }
}
