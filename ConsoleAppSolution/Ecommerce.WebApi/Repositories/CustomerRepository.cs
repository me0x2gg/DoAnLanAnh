using Microsoft.EntityFrameworkCore.ChangeTracking;
using SolidEdu.Shared; //Customer
using System.Collections.Concurrent; //ConcurrentDictionary

namespace Ecommerce.WebApi.Repositories;


public class CustomerRepository : ICustomerRepository
{
    //use a static dictionary field to cache the customers
    private static ConcurrentDictionary<string, Customer> customerCache;
    //use a instance data context field
    private SolidStoreContext db;

    public CustomerRepository(SolidStoreContext injectedContext)
    {
        this.db = injectedContext;
        if(customerCache == null)
        {
            customerCache = new ConcurrentDictionary<string, Customer>(
                db.Customers.ToDictionary(c => c.CustomerId));
        }

    }

    public async Task<Customer?> CreateAsync(Customer c)
    {
        c.CustomerId = c.CustomerId.ToUpper();
        //add to database via EF Core
        EntityEntry<Customer> added = await db.Customers.AddAsync(c);
        int affected = await db.SaveChangesAsync();
        if(affected == 1)
        {
            if (customerCache is null) return c;
            //if the customer is new, add it to cached, else call updateCache method
            return customerCache.AddOrUpdate(c.CustomerId, c, UpdateCached);
        }
        else
        {
            return null;
        }
    }

    public async Task<bool?> DeleteAynsc(string id)
    {
        id = id.ToUpper();
        //remove from database 
        Customer? c = db.Customers.Find(id); // out, ref, params
        if (c is null) return null;
        db.Customers.Remove(c);
        int affected = await db.SaveChangesAsync();
        if(affected == 1)
        {
            if(customerCache is null) return null;
            //remove from cached
            return customerCache.TryRemove(id, out c);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsyc()
    {
        //for excellent performance, get from cached
        return Task.FromResult(
            customerCache is null 
                ? Enumerable.Empty<Customer>() : customerCache.Values
            );    
    }

    public Task<Customer?> RetrieveAsync(string id)
    {
        // for excellent performance, get from cached
        id = id.ToUpper();
        if(customerCache is null) return null!;
        customerCache.TryGetValue(id, out Customer? c);//compared between out vs ref
        return Task.FromResult(c);
    }

    public async Task<Customer?> UpdateAsync(string id, Customer c)
    {
        id = id.ToUpper();//old
        c.CustomerId = c.CustomerId.ToUpper();
        //update in database by EF Core
        db.Customers.Update(c);
        int effected = await db.SaveChangesAsync();
        if(effected == 1)
        {
            //update in cached
            return UpdateCached(id, c);
        }
        return null;
    }

    private Customer UpdateCached(string id, Customer c)
    {
        Customer? old;
        if(customerCache is not null)
        {
            if(customerCache.TryGetValue(id, out old))
            {
                if (customerCache.TryUpdate(id, c, old))
                {
                    return c;
                }
            }
        }
        return null;
    }


}
