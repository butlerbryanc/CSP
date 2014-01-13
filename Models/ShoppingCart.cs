using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSP.Data;
namespace CSP.Models
{
    public class ShoppingCart
    {
        CSPEntities storeContext = new CSPEntities();
        string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";


        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);

            return cart;
        }

        //helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Product prod)
        {
            //get matching cart and product instances

            var cartItem = storeContext.Carts.SingleOrDefault(
                c => c.cart_id == ShoppingCartId && c.product_id == prod.product_id);

            if (cartItem == null)
            {
                //create new cart item if none exists
                cartItem = new Cart
                {
                    product_id = prod.product_id,
                    cart_id = ShoppingCartId,
                    count = 1,
                    date_created = DateTime.Now
                };
                storeContext.Carts.Add(cartItem);
            }

            else
            {
                //add one to quantity
                cartItem.count++;
            }

            storeContext.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            var cartItem = storeContext.Carts.Single(
                cart => cart.cart_id == ShoppingCartId && cart.Id == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.count > 1)
                {
                    cartItem.count--;
                    itemCount = (int)cartItem.count;
                }

                else
                {
                    storeContext.Carts.Remove(cartItem);
                }

                storeContext.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeContext.Carts.Where(
                cart => cart.cart_id == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeContext.Carts.Remove(cartItem);
            }

            storeContext.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return storeContext.Carts.Where(
                cart => cart.cart_id == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            //get the total quantity of all items in cart summed.
            int? count = (from cartItems in storeContext.Carts
                          where cartItems.cart_id == ShoppingCartId
                          select (int?)cartItems.count).Sum();
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            //multiply price by count then sum for total price of cart.
            decimal? total = (from cartItems in storeContext.Carts
                              where cartItems.cart_id == ShoppingCartId
                              select (int?)cartItems.count * cartItems.Product.product_price).Sum();

            return total ?? decimal.Zero;

        }

        public int PlaceOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var newOrder = new OrderDetail
                {
                    product_id = item.product_id,
                    order_id = order.order_id,
                    product_price = item.Product.product_price,
                    quantity = item.count
                };
                orderTotal +=  (decimal)(item.count * item.Product.product_price);

                storeContext.OrderDetails.Add(newOrder);
            
            }

            order.total = orderTotal;

            storeContext.SaveChanges();

            EmptyCart();

            return order.order_id;
        }

        public string GetCartId(HttpContextBase context)
        {
            if(context.Session[CartSessionKey]==null)
            {
                if(!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }

                else
                {
                    Guid tempCartId = Guid.NewGuid();

                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        public void MigrateCart(string userName)
        {
            var shoppingCart = storeContext.Carts.Where(
                c => c.cart_id == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.cart_id = userName;
            }
            storeContext.SaveChanges();
        }


    }
}