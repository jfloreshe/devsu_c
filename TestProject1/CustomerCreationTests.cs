using DevsuCustomer.Api.Models;

namespace TestProject1;

public class CustomerCreationTests
{
    [Fact]
    public void CreateCustomer_WihtSimpleData_ShouldReturnCustomer()
    {
        // Arrange
        var personIdentifier = "123456789";
        var name = "John Doe";
        var gender = "Male";
        var age = 30;
        var address = "123 Main St";
        var phone = "555-1234";
        var password = "password";
        var state = true;

        // Act
        var newCustomer = Customer.Create(personIdentifier, name, gender, age, address, phone, password, state);

        // Assert
        Assert.NotEqual(newCustomer.CustomerId, Guid.Empty);
        Assert.Equal(newCustomer.PersonalIdentifier, personIdentifier);
        Assert.Equal(newCustomer.Name, name);
        Assert.Equal(newCustomer.Gender, gender);
        Assert.Equal(newCustomer.Address, address);
        Assert.Equal(newCustomer.Phone, phone);
        Assert.Equal(newCustomer.Password, password);
        Assert.Equal(newCustomer.State, state);
    }
}