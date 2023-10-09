using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Web.Models;

namespace Web.Controllers
{
    public class CartController : BaseController
    {

        private readonly MyDbContext _context;


        public CartController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string json = this.HttpContext.Session.GetString("items");
            if (json == "[]" || json.IsNullOrEmpty())
            {
                ViewBag.Empty = true;
                return View();
            }
            ViewBag.CartItems = json.FromJson<List<CartItem>>();
            ViewBag.Total = InTotal(json.FromJson<List<CartItem>>());
            ViewBag.Photos = _context.Photos.ToList();
            return View();
        } 

        public decimal InTotal(List<CartItem> cartItems)
        {
            int total = 0;

            foreach (CartItem item in cartItems)
            {
                total += item.TotalPrice;
            }
            return total;
        }

        public IActionResult RemoveFromCart(int id)
        {
            
            string json = this.HttpContext.Session.GetString("items");
            var list = json.FromJson<List<CartItem>>();
            CartItem del = list.Find(c => c.Product.Id == id);
            list.Remove(del);
            json = list.ToJson();
            this.HttpContext.Session.SetString("items", json);
            return RedirectToAction("Index");
        }

        public IActionResult ConfirmOrder([Bind("Id,FirstName,LastName,Email,Mobile,City,Street,Country,Zip")] Customer customer, int total)
        {
            if (TryValidateModel(customer))
            {
                string json = this.HttpContext.Session.GetString("items");
                List<CartItem> cartItems = json.FromJson<List<CartItem>>();
                AddToDb(cartItems, customer, total);

                return RedirectToAction("Index", "ConfirmOrder");
            }
            return new EmptyResult();
        }

        public void AddToDb(List<CartItem> cartItems, Customer customer, int total)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            Order order = new Order { Created = DateTime.Now, CustomerId = customer.Id, Total =  total};
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (CartItem item in cartItems)
            {
                item.OrderId = order.OrderNumber;
                item.ProductId = item.Product.Id;
                item.Product = null;
                 
                _context.CartItems.Add(item);
                _context.SaveChanges();
            }

            this.HttpContext.Session.Remove("items");
        }

        public IActionResult QtityChange(int value, int id)
        {
            string json = this.HttpContext.Session.GetString("items");
            List<CartItem> cartItems = json.FromJson<List<CartItem>>();
            CartItem c = cartItems.Find(x => x.Product.Id == id);
            c.QtityChange(value);

            json = cartItems.ToJson();
            this.HttpContext.Session.SetString("items", json);
            return RedirectToAction("Index");
        }

        
    }
}
