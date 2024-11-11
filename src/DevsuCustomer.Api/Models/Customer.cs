using DevsuCustomer.Api.Models.Primitives;

namespace DevsuCustomer.Api.Models;

public class Customer : Person, IAggregateRoot
{
    public Guid CustomerId { get; set; }
    public string Password { get; set; }
    public bool State { get; set; }
    private Customer() {}
    
    private Customer(string personalIdentifier, string name, string gender, int age, string address, string phone, string password, bool state)
        :base(personalIdentifier, name, gender, age, address, phone)
    {
        Password = password;
        State = state;
    }
    
    public static Customer Create(string personIdentifier, string name, string gender, int age, string address,
        string phone, string password, bool state)
    {
        
        var newCustomer = new Customer(personIdentifier, name, gender, age, address, phone, password, state)
        {
            CustomerId = Guid.NewGuid()
        };

        return newCustomer;
    }
    
}