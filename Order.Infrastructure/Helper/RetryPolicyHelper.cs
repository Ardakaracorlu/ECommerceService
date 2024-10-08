using Polly;
using Polly.Retry;

namespace Order.Infrastructure.Helper
{
    public static class RetryPolicyHelper
    {
        public static RetryPolicy GetRetryPolicy(int retryCount = 3)
        {
            // Retry politikasını oluştur ve belirli hata tipleri için yeniden dene
            return Policy
                .Handle<Exception>() // Tüm exception'lar için retry yap
                .WaitAndRetry(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Gecikme (exponential backoff)
                    (exception, timeSpan, retryCount, context) =>
                    {
                        // Her yeniden denemede loglama veya farklı bir işlem yapılabilir
                        Console.WriteLine($"Retry {retryCount} due to: {exception.Message}");
                    });
        }

    }
}
