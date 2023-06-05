using SolidEdu.Shared; //Customer
namespace Ecommerce.WebApi.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> CreateAsync(Customer c); //POST
    Task<IEnumerable<Customer>> RetrieveAllAsyc(); //GET
    Task<Customer?> RetrieveAsync(string id);//Get by Id
    Task<Customer?> UpdateAsync(string id, Customer c);
    Task<bool?> DeleteAynsc(string id);
}
