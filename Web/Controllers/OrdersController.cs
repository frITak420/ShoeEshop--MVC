using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Z.EntityFramework.Extensions;


namespace Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public IActionResult Index()
        {
            var myDbContext = _context.Orders.Include(o => o.Customer);
            return View(myDbContext);
        }
        
        public IActionResult DeleteOrder(int id)
        {
            if (OrderExists(id))
            {
                Order o = _context.Orders.Find(id);
                _context.Orders.SingleDelete(o);
            }

            return RedirectToAction("Index");
        }
       
        public IActionResult OrderIsDone(int id)
        {
            if (OrderExists(id)) 
            { 
                Order o = _context.Orders.Find(id);
                o.IsCompleted = true;
                _context.SingleUpdate(o);
            }
            return RedirectToAction("Index");
        }

        public bool OrderExists(int id)
        {
          return _context.Orders.Any(e => e.OrderNumber == id);
        }
    }
}
