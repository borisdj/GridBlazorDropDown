using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GridBlazorDropDown.Data
{
    public interface IDbInitializer
    {
        void SeedData();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    SeedCustomerOrder(context);
                    context.SaveChanges();
                }
            }
        }

        private void SeedCustomerOrder(ApplicationDbContext context)
        {
            if (context.Customers.Count() == 0)
            {
                context.Customers.Add(new Customer() { Name = "Customer First", Remark = "info" });
                context.Customers.Add(new Customer() { Name = "Customer Second", Remark = "data" });

                var customers = new List<Customer>();
                for (int i = 1; i < 100; i++)
                {
                    customers.Add(new Customer() { Name = "Buyer " + i, Remark = "text " + i/2 });
                }
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
        }
    }
}