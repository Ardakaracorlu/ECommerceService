 # Özet
RabbitMQ ve PostgreSQL kullanarak .NET 8 ile Microservice mimarisini uyguladım. RabbitMQ, servisler arası asenkron mesajlaşma için kullanılırken, PostgreSQL her servis için ayrı veritabanı yönetimi sağladı. Uygulama, bir e-ticaret platformu geliştirilmesi üzerine kurulu olup, Sipariş Servisi siparişleri aldı ve işledi, Stok Servisi ürün stoklarını yönetti, ve Bildirim Servisi kullanıcılara e-posta ve SMS bildirimleri gönderdi. Mesajlaşma için RabbitMQ'da exchange modeli kullanılarak servisler arası iletişim sağlandı
-----------------------------------------------------------------------------------------------------------------------------------------------
# Çalıştır
# C:\ altında, ECommerceService adında bir klasör oluşturup GitHub'taki klon linki üzerinden proje dosyalarını indirip oluşturunuz.
# Terminal üzerinden projenin bulunduğu dizine gidip  "C:\ECommerceService>"  "docker-compose up" komutu ile Projeye ait tüm conteinerları ayağa kaldırıyoruz.

# Containerlar başarıyla ayağa kalktıktan sonra ulaşmak için aşağıdaki URL'leri kullanabilirsiniz.

RabbitMQUI : http://localhost:15672/
RabbitMQ   : http://localhost:5672/
  Username: user
  Password: password

OrderPostgres   : localhost:5432
  Username : orders_user
  Password : orders_password
  Database : ordersdb

NotificationPostgres   : localhost:5433
  Username : notifications_user
  Password : notifications_password
  Database : notificationsdb

StockPostgres   : localhost:5434
  Username : stocks_user
  Password : stocks_password
  Database : stocksdb

Order.Api : http://localhost:5000/
Order.Api.Swagger : http://localhost:5000/swagger/index.html

Notification.Api : http://localhost:5001/
Notification.Api.Swagger : http://localhost:5001/swagger/index.html

Stock.Api : http://localhost:5002/
Stock.Api.Swagger : http://localhost:5002/swagger/index.html

-----------------------------------------------------------------------------------------------------------------------------------------------

Api Endpointleri Ve RabbitMQ Queue bilgileri aşağıdaki gibidir.

# APİ
Url : api/Order/CreateOrder
Http Method : Post
Description : Yeni bir sipariş kaydı oluşturulur ve stok kontrolü için stock_queue kuyruğuna mesaj bırakılır.

Sample Request Body:
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", - > 
  "quantity": 2,
  "customerName": "arda",
  "customerSurname": "karaçorlu",
  "adress": "istanbul",
  "phone": "5554448899",
  "email": "arda.karacorlu@gmail.com"
}

# RabbitMQ 
Her bir consumer, ilgili API içinde bir Background Service olarak çalışır ve RabbitMQ kuyruğundaki mesajları sürekli olarak dinleyip işlemektedir. Bu yapı sayesinde, kuyruktan gelen her mesajın asenkron olarak işlenir   Aynı zamanda, bu consumer'lar diğer servislerle RabbitMQ üzerinden haberleşerek mesaj bazlı bir iletişim mekanizması kurar. Böylece her servis, görevini tamamladığında ilgili kuyruklara mesajlar bırakır ve bu mesajlar, diğer servisler tarafından işlenerek sistemin haberleşmesi sağlanır.

Api : Stock.Api
QueueName : stock_queue
ExchangeName: stock.direct
ExchangeType: direct
Description : Sipariş kaydı oluşturulduktan sonra, consumer üzerinde productId ile stok kontrolü yapılır. Stok kontrolü sonucuna göre, order_status_queue kuyruğuna mesaj bırakılır.

Api : Order.Api
QueueName : order_status_queue
ExchangeName: order_status_direct
ExchangeType: direct
Description : Sipariş kaydı güncellenir ve gerekli bildirim mesajları hazırlanır. Bildirim mesajları, notification_email ve notification_sms kuyruklarına gönderilir.

Api: Notification.Api
QueueName : notification_email
ExchangeName: notification_topic
ExchangeType: topic
Description : Müşteriye email gönderimi yapılır ve bu işlem Notification veritabanına loglanır.

Api : Notification.Api
QueueName : notification_sms
ExchangeName: notification_topic
ExchangeType: topic
Description : Müşteriye SMS gönderimi yapılır ve bu işlem Notification veritabanına loglanır.





