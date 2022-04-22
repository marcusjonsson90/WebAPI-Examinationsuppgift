using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Examinationsuppgift.Entities;
using WebAPI_Examinationsuppgift.Filters;
using WebAPI_Examinationsuppgift.Models;

namespace WebAPI_Examinationsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UseApiKey]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder(OrderFormModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);

            if (user == null) return NotFound();

            if (model.Cart.Count == 0) return BadRequest();

            var order = new Order
            {
                CustomerName = user.FirstName + " " + user.LastName,
                Address = user.Address + ", " + user.PostalCode + ", " + user.City,
                OrderDate = model.OrderDate,
                OrderStatus = model.OrderStatus,
            };

            var orderRows = new List<OrderRow>();

            foreach (var item in model.Cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null) return BadRequest();
                orderRows.Add(new OrderRow
                {
                    ProductName = product.Name,
                    ProductNumber = product.ProductId,
                    ProductPrice = product.Price,
                    Quantity = item.Quantity,
                    OrderId = order.OrderId,
                });
            }

            order.OrderRows = orderRows;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return new OkObjectResult(order);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllOrders() => new OkObjectResult(await _context.Orders.Include(x => x.OrderRows).ToListAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {
           var order = await _context.Orders.Include(x => x.OrderRows).FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null) return NotFound();

            return new OkObjectResult(order);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrderById(int id, UpdateOrderModel model)
        {
            var order = await _context.Orders.Include(x => x.OrderRows).FirstOrDefaultAsync(x => x.OrderId == id);

            if(order == null) return NotFound();

            order.OrderDate = model.OrderDate;
            order.Address = model.Address;
            order.OrderStatus = model.OrderStatus;
            order.CustomerName = model.CustomerName;

            var orderRows = new List<OrderRow>(order.OrderRows);
            foreach (var item in model.OrderRows)
            {
                var orderRow = orderRows.FirstOrDefault(x => x.ProductNumber == item.ProductNumber);
                if( orderRow == null)
                {
                    orderRows.Add(new OrderRow 
                    { 
                        OrderId = order.OrderId, 
                        ProductNumber = item.ProductNumber, 
                        ProductName = item.ProductName, 
                        ProductPrice = item.ProductPrice, 
                        Quantity = item.Quantity,
                    });
                }
                else
                {
                    orderRow.Quantity = item.Quantity;
                    orderRow.ProductNumber = item.ProductNumber;
                    orderRow.ProductName = item.ProductName;
                    orderRow.ProductPrice = item.ProductPrice;
                    _context.Entry(orderRow).State = EntityState.Modified;
                }
            }

            order.OrderRows = orderRows;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new OkObjectResult(order);
          
          
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();


        }



        


           
        
    }
}
