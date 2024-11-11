namespace DevsuCustomer.Api.Models;

public class Person
{
    public long PersonId { get; set; }
    public string PersonalIdentifier { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }

    protected Person(){}

    protected Person(string personalIdentifier, string name, string gender, int age, string address, string phone)
    {
        PersonalIdentifier = personalIdentifier;
        Name = name;
        Gender = gender;
        Age = age;
        Address = address;
        Phone = phone;
    }
}