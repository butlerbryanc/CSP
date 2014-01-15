using CSP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSP.Models;


namespace CSP.Controllers
{
    public class StoreController : Controller
    {
        //
        // GET: /Store/
        
        

        public ActionResult Index(int category=0, int menuId = 0)
        {

            CSPEntities context = new CSPEntities();
            if (category == 0 && menuId == 0)
            {
                var products = context.Products.ToList();

                return View(products);
            }
            if (menuId == 0 && category != 0)
            {
                IEnumerable<Product> customQuery =
                  from prod in context.Products
                  where prod.category_id == category
                  orderby prod.product_name ascending
                  select prod;

                return View(customQuery);
            }
            else
            {
                IEnumerable<Product> customQuery =
                    from prod in context.Products
                    where prod.menu_id == menuId
                    orderby prod.product_name ascending
                    select prod;
                return View(customQuery);
            }
        }

        public ActionResult Search(string searchTerm)
        {
                CSPEntities context = new CSPEntities();
                IEnumerable<Product> searchQuery =
                  from prod in context.Products
                  where prod.product_name.Contains(searchTerm)
                  orderby prod.product_name ascending
                  select prod;

                return View(searchQuery);
        }
        
        public ActionResult Details(int id)
        {
            CSPEntities context = new CSPEntities();
            var prod = context.Products.SingleOrDefault(
                p => p.product_id == id);

            return View(prod);
        }

        
    }
}
