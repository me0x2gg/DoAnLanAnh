using Microsoft.AspNetCore.Mvc; //[Route], [ApiController], ControllerBase
using SolidEdu.Shared; //Customer
using Ecommerce.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.IdentityJWT.Authentication;
namespace Ecommerce.WebApi.Controllers;


//url base: api/customers => json
//[Authorize(Roles = UserRoles.AdminRole)]
[Route("api/[controller]")]
[ApiController]
public class CustomersController:ControllerBase
{
    private readonly ICustomerRepository repo;
    // constructor inject repository in startup
    public CustomersController(ICustomerRepository repo)
    {
        this.repo = repo;
    }
    //GET: api/customers
    //GET: api/customers/?country=[country]
    
    [Authorize("customerApiReadPolicy")]
    [HttpGet]
    [ProducesResponseType(200,Type = typeof(IEnumerable<Customer>))]
    public async Task<IEnumerable<Customer>> GetCustomers(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return await repo.RetrieveAllAsyc();
        }
        else
        {
            //using LinQ to return 
            return (await repo.RetrieveAllAsyc())
                .Where(c=>c.Country == country);
        }
    }

    //GET: api/customers/[id]
    
    [Authorize("customerApiReadPolicy")]
    [HttpGet("{id}",Name =nameof(GetCustomer))] // named route
    [ProducesResponseType(200,Type=typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id)
    {
        Customer? c = await repo.RetrieveAsync(id);
        if(c == null)
        {
            return NotFound();// return 404
        }
        else
        {
            return Ok(c);//customer exists
        }
    }

    //POST: api/customers (json, xml)
    [Authorize("customerApiWritePolicy")]
    [HttpPost]
    [ProducesResponseType(201,Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Customer c)
    {
        if(c == null)
        {
            return BadRequest();//400 code
        }
        Customer? addedCustomer  = await repo.CreateAsync(c);
        if(addedCustomer == null)
        {
            return BadRequest("Repository faild to create customer " + c.CustomerId);
        }
        else
        {
            //Problem details (redirectly)
            return CreatedAtRoute(
             routeName:nameof(GetCustomer),
             routeValues: new {id = addedCustomer.CustomerId.ToLower()},//annoymous type
             value: addedCustomer
            );
        }
    }
    //PUT (update): api/customers/[id]
    [Authorize("customerApiWritePolicy")]
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] Customer c)
        {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();
        if(c == null || c.CustomerId != id)
        {
            return BadRequest(); //400 Bad request
        }
        Customer? existing = await repo.RetrieveAsync(id);

        if (existing == null)
        {
            return NotFound(); // 404 code error
        }
        await repo.UpdateAsync(id, c);
        return new NoContentResult(); // 204 no contents
    }
    //DELETE: api/customers/[id]
    [Authorize("customerApiWritePolicy")]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {  
        Customer? existing = await repo.RetrieveAsync(id);
        if(existing == null)
        {
            if(id == "bad")
            {
                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://localhost:5001/customers/failed-to-delete",
                    Title = $"Customer ID {id} found but failed to delete.",
                    Detail = "More details like Company Name, Country and so on.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails); //400 Bad Request
            }
            return NotFound();
        }
        //if found
        bool? deleted = await repo.DeleteAynsc(id);

        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult(); // 204 no contents
        }
        else
        {
            return BadRequest($"Customer {id} was not found but failed to delete..."); //400 code
        }
    }

}


