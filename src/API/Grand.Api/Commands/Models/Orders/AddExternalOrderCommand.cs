using Grand.Api.DTOs.Customers;
using Grand.Api.DTOs.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grand.Api.Commands.Models.Orders
{
    public class AddExternalOrderCommand : IRequest<CreateOrderResult>
    {
        public AddExternalOrderCommand(OrderRequestDto model)
        {
            Model = model;
        }
        public OrderRequestDto Model { get; set; }
    }

    public class CreateOrderResult
    {
        public bool IsSuccess { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
    }
}
