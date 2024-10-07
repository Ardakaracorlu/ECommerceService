namespace Stock.Api.Extension
{
    public static class StockExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {
            Stock.Consumer.Extension.ConsumerExtension.RegisterService(services, configuration);
            return services;
        }
    }
}
