namespace DevsuCustomer.Api.Models.DomainEvents;

public abstract class CustomerDomainEvent;

public class CustomerCreatedDomainEvent : CustomerDomainEvent
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
}

public class CustomerDeletedDomainEvent : CustomerDomainEvent
{
    public Guid CustomerId { get; set; }
}

public class CustomerUpdatedDomainEvent : CustomerDomainEvent
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public bool State { get; set; }
}