﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class HomeController : Controller
    {
        private E_CommerceContext _context;

        public HomeController(E_CommerceContext context){
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<Product> Firstfiveproducts = _context.Products.Take(5).ToList();
            ViewBag.Fiveproducts = Firstfiveproducts;
            ViewBag.Recentorders = _context.Orders.Include(Customer=>Customer.customer).Include(Product=>Product.product).OrderByDescending(Order=>Order.created_at).Take(3);
            ViewBag.Recentcustomers = _context.Customers.Include(Product=>Product.orders).OrderByDescending(Customer=>Customer.created_at).Take(3);
            return View("Index");
        }
        [HttpPost]
        [Route("/filter")]
        public IActionResult FilterDashboard(string search){
            List<Product> Firstfiveproducts = _context.Products.Where(product => product.name.Contains(search)).Take(5).ToList();
            ViewBag.Fiveproducts = Firstfiveproducts;
            ViewBag.Recentorders = _context.Orders.Include(Customer=>Customer.customer).Include(Product=>Product.product).Where(order=>order.customer.name.Contains(search)).OrderByDescending(Order=>Order.created_at).Take(3);
            ViewBag.Recentcustomers = _context.Customers.Include(Product=>Product.orders).Where(Customer=>Customer.name.Contains(search)).OrderByDescending(Customer=>Customer.created_at).Take(3);
            return View("Index");
        }

        [HttpGet]
        [Route("/orders")]
        public IActionResult Orders(){
            List<Customer> Allcustomers = _context.Customers.ToList();
            ViewBag.Customers = Allcustomers;
            List<Product> Allproducts = _context.Products.ToList();
            ViewBag.Products = Allproducts;
            List<Order> AllOrders = _context.Orders.Include(Customer=>Customer.customer).Include(Product=>Product.product).ToList();
            ViewBag.Customerorders = AllOrders;
            return View("Orders");
        }
        [HttpPost]
        [Route("/orders/add")]
        public IActionResult AddOrder(Order NewOrder, int quantity, int ProductId){
            
            Product Orderupdate = _context.Products.SingleOrDefault(Product=>Product.ProductId == ProductId);
            if(quantity > Orderupdate.quantity){
                TempData["invalidorder"] = "Cannot Order more than";
                return RedirectToAction("Orders");
            }
            Orderupdate.quantity -= quantity;
            _context.Add(NewOrder);
            _context.SaveChanges();
            return RedirectToAction("Orders");
        }

        [HttpGet]
        [Route("/customers")]
        public IActionResult Customers(){
            List<Customer> Allcustomers = _context.Customers.ToList();
            ViewBag.Customers=Allcustomers;
            return View("Customers");
        }
        [HttpPost]
        [Route("/customers/addcustomer")]
        public IActionResult AddCustomer(Customer NewCustomer){
            _context.Customers.Add(NewCustomer);
            _context.SaveChanges();
            return RedirectToAction("Customers");
        }

        [HttpGet]
        [Route("/products")]
        public IActionResult Products(){
            List<Product> Products = _context.Products.ToList();
            ViewBag.Products = Products;
            return View("Products");
        }

        [HttpPost]
        [Route("/products/addproduct")]
        public IActionResult AddProduct(ProductViewModel model, Product NewProduct){
            if(ModelState.IsValid){
                _context.Products.Add(NewProduct);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }
            // List<Product> Products = _context.Products.ToList();
            // ViewBag.Products = Products;
            return View("Products");
        }
        [HttpPost]
        [Route("/filterproducts")]
        public IActionResult FilterProducts(string productsearch){
            List<Product> Allproducts = _context.Products.Where(Product=>Product.name.Contains(productsearch)).ToList();
            if(Allproducts == null){
                return View("Products");
            }
            ViewBag.Products = Allproducts;
            return View("Products");
        }
        [HttpPost]
        [Route("customers/remove")]
        public IActionResult RemoveCustomer(int customerid){
            Customer Deletecustomer = _context.Customers.SingleOrDefault(Customer=>Customer.CustomerId == customerid);
            _context.Customers.Remove(Deletecustomer);
            _context.SaveChanges();
            return RedirectToAction("Customers");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}