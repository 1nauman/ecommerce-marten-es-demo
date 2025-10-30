using Domain.SharedKernel;

namespace Domain.Customer;

public class Customer : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public Address? ShippingAddress { get; private set; }

    private Customer()
    {
    }

    protected override void Dispatch(DomainEvent @event) => Apply((dynamic)@event);

    public static Customer Register(string name, string email)
    {
        var customer = new Customer();
        var @event = new CustomerRegistered(Guid.NewGuid(), name, email);
        customer.Raise(@event);
        return customer;
    }

    public void UpdateShippingAddress(Address newAddress)
    {
        var @event = new ShippingAddressUpdated(Id, newAddress);
        Raise(@event);
    }

    private void Apply(CustomerRegistered @event)
    {
        Id = @event.CustomerId;
        Name = @event.Name;
        Email = @event.Email;
    }

    private void Apply(ShippingAddressUpdated @event)
    {
        ShippingAddress = @event.ShippingAddress;
    }
}