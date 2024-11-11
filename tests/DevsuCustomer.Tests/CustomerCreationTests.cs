using DevsuCustomer.Api.Models;
using Microsoft.AspNetCore.Http;

namespace DevsuCustomer.Tests;

public class CustomerCreationTests
{
    [Fact]
    public void CreateCustomer_WithNoFailure_ShouldReturnCustomer()
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
        var newCustomerResult = Customer.Create(personIdentifier, name, gender, age, address, phone, password, state);
        var newCustomer = newCustomerResult.Value;
        
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
    
    [Fact]
    public void CreateCustomer_WithPhoneWithSizeBiggerThanExpected_ShouldReturnErrorWithCode422()
    {
        // Arrange
        var personIdentifier = "123456789";
        var name = "John Doe";
        var gender = "Male";
        var age = 30;
        var address = "123 Main St";
        var phone = "55555-55555-55555-55555-55555-55555-55555-55555-55555-55555-55555-55555";
        var password = "password";
        var state = true;

        // Act
        var newCustomerResult = Customer.Create(personIdentifier, name, gender, age, address, phone, password, state);
        var error = newCustomerResult.Error;
        
        // Assert
        Assert.Equal(error.Code, StatusCodes.Status422UnprocessableEntity);
    }
}