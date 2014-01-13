using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSP.Models;
using CSP.Data;

namespace CSP.Models
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
