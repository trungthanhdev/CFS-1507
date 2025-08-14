using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFS_1507.Domain.Interfaces
{
    public interface IRabbitMQPublisher
    {
        Task Handle(string temp_cart_id);
    }
}