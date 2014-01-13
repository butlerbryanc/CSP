using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSP.Data;
using CSP.Models;


namespace CSP.Controllers
{
    public class ShoppingCartController : Controller
    {

        CSPEntities storeContext = new CSPEntities();
        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //Bryan! this is the view model

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        public ActionResult AddToCart (int id)
        {
            var newItem = storeContext.Products
                .Single(prod => prod.product_id == id);

            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(newItem);


            return RedirectToAction("Index", "ShoppingCart");
        }

        //AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            string product = storeContext.Carts.Single(i => i.Id == id).Product.product_name;

            int itemCount = cart.RemoveFromCart(id);

            var outcome = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(product) + " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(outcome);
        }

        // ShoppingCartSummary

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();

            return PartialView("CartSummary");
        }

    }
}
