using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public record OrderCreatedEvent(
        Guid OrderId,
        string UserId,
        OrderAddress Address);

    public record OrderAddress(
        string City,
        string Street,
        string Country,
        string ZipCode);
}
