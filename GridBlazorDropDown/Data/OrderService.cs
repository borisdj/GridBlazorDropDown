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
    public class OrderService : IGenericService<Order>
    {
        public ItemsDTO<Order> Get(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query) // GRID
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var q = context.Orders.AsQueryable().Include(a => a.Customer);
            int pageSize = 50;

            var server = new GridServer<Order>(q, new QueryCollection(query), true, "ordersGrid", columns, pageSize)
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

            var q = context.Orders.AsNoTracking();
            if (!String.IsNullOrWhiteSpace(search))
            {
                q = q.Where(a => a.Description.Contains(search));
            }
            var records = q.Take(50).Select(a => new SelectItem(a.OrderId.ToString(), a.Description)).ToList();
            return records;
        }

        // CRUD:

        public async Task<Order> Get(params object[] keys) 
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            int id = Int32.Parse(keys[0].ToString());
            var record = await context.Orders.Include(a => a.Customer).AsNoTracking().SingleOrDefaultAsync(a => a.OrderId == id);
            return record;
        }

        public async Task Insert(Order item)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            await context.Orders.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task Update(Order item)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            var record = await context.Orders.Include(a => a.Customer).SingleOrDefaultAsync(a => a.OrderId == item.OrderId);
            record.Description = item.Description;
            record.IsPriority = item.IsPriority;
            record.Amount = item.Amount;
            record.CustomerId = item.CustomerId;

            await context.SaveChangesAsync();
        }

        public async Task Delete(params object[] keys)
        {
            using var context = new ApplicationDbContext(ApplicationDbContext.GetOptions());

            int id = Int32.Parse(keys[0].ToString());
            var item = await context.Orders.SingleOrDefaultAsync(a => a.OrderId == id);
            context.Orders.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
