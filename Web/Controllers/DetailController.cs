using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Text.Json.Nodes;
using Web.Models;

namespace Web.Controllers
{
    public class DetailController : BaseController
    {

        private readonly MyDbContext _context;
       

        public DetailController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id)
        {
            ViewBag.Offering = _context.Products.ToList().Take(4);
            
            if (id == null)
            {
                return NotFound();
            }
            _context.Products.Include(x => x.Photos).ToList();
            var p = _context.Products.Find(id);

            return View(p);
        }

        public IActionResult AddToCart(int id, string selectedColor, string selectedSize)
        {
           List<CartItem> cartItems = new List<CartItem>();
            if (!string.IsNullOrEmpty(selectedColor) && !string.IsNullOrEmpty(selectedSize))
            {
                var p = _context.Products.Find(id);
                string json = this.HttpContext.Session.GetString("items");
                CartItem c = new CartItem { Color = selectedColor, Size = selectedSize, Product = p, Quantity = 1, TotalPrice = p.Price };

                if (json == null)
                {
                    cartItems.Add(c);
                    json = cartItems.ToJson();
                }       

                else
                {
                    cartItems = json.FromJson<List<CartItem>>();
                    cartItems.Add(c);
                    json = cartItems.ToJson();
                }

                this.HttpContext.Session.SetString("items", json);
                return RedirectToAction("Index", "Cart");
            }
            string backurl = Request.Headers["Referer"].ToString();
            return Redirect(backurl);
        }

    }
}
