using Grand.Api.Commands.Models.Catalog;
using Grand.Api.Commands.Models.Common;
using Grand.Api.Commands.Models.Orders;
using Grand.Api.DTOs.Orders;
using Grand.Api.Models.Common;
using Grand.Business.Core.Interfaces.Customers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grand.Api.Controllers
{
    public class ExternalOrderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICustomerManagerService _customerManagerService;
        public ExternalOrderController(IMediator mediator, ICustomerManagerService customerManagerService)
        {
            _mediator = mediator;
            _customerManagerService = customerManagerService;
        }

        [AllowAnonymous]
        [HttpPost("api/order")]
        [SwaggerOperation(summary: "Create external order", OperationId = "CreateExternalOrder")]
        public async Task<IActionResult> CreateExternalOrder([FromBody] OrderRequestDto model)
        {
            if (ModelState.IsValid)
            {
                var result= await _mediator.Send( new AddExternalOrderCommand(model));
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest(ModelState);
        }
    }

}
