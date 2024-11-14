namespace DevsuCustomer.Api.Infrastructure.Integration.RabbitMq;

public class RabbitMqSettings(string Host, ushort Port, string VirtualHost, string Username, string Password)
{
    public string Host { get; set; }
    public ushort Port { get; set; }
    public string VirtualHost { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public RabbitMqSettings() : this("localhost", 5672, "/", "guest", "guest") { }
}


public static class RabbitMqConstants
{
    public const string CustomerExchange = "customers";
    public const string ConsumerCustomerCreatedRoutingKey = "customer-created";
    public const string ConsumerCustomerUpdatedRoutingKey = "customer-updated";
    public const string ConsumerCustomerDeletedRoutingKey = "customer-deleted";
    public const string ExchangeTypeDirect = "direct";
}
