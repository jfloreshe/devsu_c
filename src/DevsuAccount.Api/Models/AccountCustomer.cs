namespace DevsuAccount.Api.Models;

public class AccountCustomer
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public bool State { get; set; }
}