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
    public class CustomerService : ICrudDataService<Customer>
    {
        public ItemsDTO<Customer> Get(Action<IGridColumnCollection<Customer>> columns, QueryDictionary<StringValues> query) // GRID
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Customers.AsQueryable();
            int pageSize = 20;

            var server = new GridServer<Customer>(q, new QueryCollection(query), true, "customersGrid", columns, pageSize)
                .Sortable()
                .Filterable()
                .Searchable()
                .WithMultipleFilters();

            return server.ItemsToDisplay;
        }

        public async Task<IEnumerable<SelectItem>> Get() // DropDown
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Customers.AsNoTracking();
            var records = await q.Select(a => new SelectItem(a.CustomerId.ToString(), a.Name)).ToListAsync();
            return records;
        }

        public async Task<IEnumerable<SelectItem>> Get(string search) // DropDown search
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Customers.AsNoTracking();
            if (String.IsNullOrWhiteSpace(search))
            {
                q = q.Where(a => a.Name.Contains(search));
            }
            var records = await q.Select(a => new SelectItem(a.CustomerId.ToString(), a.Name)).ToListAsync();
            return records;
        }

        // CRUD
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

            var record = await context.Customers.AsNoTracking().SingleOrDefaultAsync(a => a.CustomerId == item.CustomerId);
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
