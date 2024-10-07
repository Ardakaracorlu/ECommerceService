namespace Notification.Api.Extension
{
    public static class NotificationExtension
    {
        public static IServiceCollection RegisterService(IServiceCollection services, IConfigurationRoot configuration)
        {

            Notification.Consumer.Extension.ConsumerExtension.RegisterService(services, configuration);
            return services;
        }
    }
}
