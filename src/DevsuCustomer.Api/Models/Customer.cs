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
    
    public static Result<Customer> Create(string personIdentifier, string name, string gender, int age, string address,
        string phone, string password, bool state)
    {
        if(password.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("contrasenia", 50));
        }
        if(personIdentifier.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("identificador", 50));
        }
        if(name.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("nombres", 200));
        }
        if(gender.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("genero", 20));
        }
        if(address.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("direccion", 200));
        }
        if(phone.Length > 50)
        {
            return Result<Customer>.Failure(CustomerUnprocessableErrors.PropertyBiggerThanExpected("telefono", 50));
        }
        
        var newCustomer = new Customer(personIdentifier, name, gender, age, address, phone, password, state)
        {
            CustomerId = Guid.NewGuid()
        };

        return Result<Customer>.Success(newCustomer);
    }
    
}

public static class CustomerUnprocessableErrors
{
    public const string CustomerCreation = "Creacion cliente";
    
    public static Error PropertyBiggerThanExpected(string property, int size) => new(
        CustomerCreation,
        $"{property} no puede exceder {size} caracteres",
        StatusCodes.Status422UnprocessableEntity);
}