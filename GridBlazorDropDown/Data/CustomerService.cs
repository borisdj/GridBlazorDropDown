using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorDropDown.Data
{
    public class CustomerService : IGenericService<Customer>
    {
        public ItemsDTO<Customer> Get(Action<IGridColumnCollection<Customer>> columns, QueryDictionary<StringValues> query) // GRID
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Customers.AsQueryable();
            int pageSize = 50;

            var server = new GridServer<Customer>(q, new QueryCollection(query), true, "customersGrid", columns, pageSize)
                .Sortable()
                .Filterable()
                .Searchable()
                .WithMultipleFilters();

            return server.ItemsToDisplay;
        }

        public IEnumerable<SelectItem> GetSelect()
        {
            return GetSelect(null);
        }

        public IEnumerable<SelectItem> GetSelect(string search) // DropDown
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Customers.AsNoTracking();
            if (!String.IsNullOrEmpty(search))
            {
                q = q.Where(o => o.Name.Contains(search));
            }
            var result = q.Take(50).Select(a => new SelectItem(a.CustomerId.ToString(), a.Name)).ToList();
            return result;
        }

        // CRUD:

        public async Task<Customer> Get(params object[] keys)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());
            
            int id = Int32.Parse(keys[0].ToString());
            var record = await context.Customers.AsNoTracking().SingleOrDefaultAsync(a => a.CustomerId == id);
            return record;
        }

        public async Task Insert(Customer item)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            await context.Customers.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task Update(Customer item)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var record = await context.Customers.SingleOrDefaultAsync(a => a.CustomerId == item.CustomerId);
            record.Name = item.Name;
            record.Remark = item.Remark;

            await context.SaveChangesAsync();
        }

        public async Task Delete(params object[] keys)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            int id = Int32.Parse(keys[0].ToString());
            var item = await context.Orders.SingleOrDefaultAsync(a => a.CustomerId == id);
            context.Orders.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
