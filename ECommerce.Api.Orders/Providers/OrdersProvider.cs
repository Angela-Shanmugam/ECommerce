using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }
        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Order()
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = DateTime.Now,
                    Items = new List<OrderItem>()
                    {
                        new OrderItem() { OrderId = 1, ProductId = 1, Quantity = 49, UnitPrice = 50 },//$2450
                        new OrderItem() { OrderId = 1, ProductId = 2, Quantity = 13, UnitPrice = 70 },//$910
                        new OrderItem() { OrderId = 1, ProductId = 3, Quantity = 32, UnitPrice = 80 },//$2560
                    },
                    Total = 5920
                });
                dbContext.Orders.Add(new Order()
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = DateTime.Now.AddDays(-1),
                    Items = new List<OrderItem>()
                    {
                        new OrderItem() { OrderId = 1, ProductId = 1, Quantity = 7, UnitPrice = 60 },//$420
                        new OrderItem() { OrderId = 1, ProductId = 2, Quantity = 14, UnitPrice = 40 },//$560
                        new OrderItem() { OrderId = 1, ProductId = 3, Quantity = 2, UnitPrice = 120 },//$240
                       
                    },
                    Total = 1220
                });
                dbContext.Orders.Add(new Order()
                {
                    Id = 3,
                    CustomerId = 3,
                    OrderDate = DateTime.Now,
                    Items = new List<OrderItem>()
                    {
                        new OrderItem() { OrderId = 1, ProductId = 1, Quantity = 5, UnitPrice = 90 },//$450
                        new OrderItem() { OrderId = 2, ProductId = 2, Quantity = 15, UnitPrice = 100 },//$1500
                        new OrderItem() { OrderId = 3, ProductId = 3, Quantity = 3, UnitPrice = 200 }//$600
                    },
                    Total = 2550
                });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders
                   .Where(o => o.CustomerId == customerId)
                   .Include(o => o.Items)
                   .ToListAsync();
               
                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>,IEnumerable<Models.Order>>(orders);
                    
                    return (true, result, null);
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {

                logger?.LogError(ex.ToString());

                return (false, null, ex.Message);
            }
        }
    }
}
