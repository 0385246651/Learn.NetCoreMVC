
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using App.Areas.Product.Models;


namespace App.Areas.Product.Controllers
{
    [Area("Product")]
    public class ViewProductController : Controller
    {


        private readonly ILogger<ViewProductController> _logger;
        private readonly AppDbContext _context;

        private readonly CartService _cartService;

        public ViewProductController(ILogger<ViewProductController> logger, AppDbContext context, CartService cartService)
        {
            _cartService = cartService;
            _logger = logger;
            _context = context;
        }

        // /post/
        // /post/{categoryslug?}
        [Route("/product/{categoryslug?}")]
        public ActionResult Index(string categoryslug, [FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categorySlug = categoryslug;

            CategoryProduct category = null;
            if (!string.IsNullOrEmpty(categoryslug))
            {
                // lấy ra chuyên mục theo slug
                category = _context.CategoryProduct.Where(c => c.Slug == categoryslug)
                .Include(c => c.CategoryChildren)
                .FirstOrDefault();

                if (category == null)
                {
                    return NotFound("Không thấy Category");
                }
            }

            var products = _context.Product
            .Include(p => p.Author)
            .Include(p => p.Photos)
            .Include(p => p.ProductCategoryProducts)
            .ThenInclude(pc => pc.Category)
            .AsQueryable();

            products.OrderByDescending(p => p.DateUpdated);

            if (category != null)
            {
                // lấy ra tất cả các ID của chuyên mục con
                List<int> ids = new List<int>();
                category.ChildCategoryIDs(null, ids);
                ids.Add(category.Id); // thêm cả ID của chuyên mục hiện tại

                products = products.Where(p => p.ProductCategoryProducts.Where(pc => ids.Contains(pc.CategoryID)).Any());
            }

            // phân trang
            int totalProducts = products.Count();
            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalProducts / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };

            var productsInPage = products.Skip((currentPage - 1) * pagesize)
                             .Take(pagesize);


            ViewBag.pagingModel = pagingModel;
            ViewBag.totalProducts = totalProducts;

            ViewBag.category = category;
            return View(productsInPage.ToList());

        }

        [Route("/product/{productslug}.html")]
        public ActionResult Details(string productslug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;

            var product = _context.Product
            .Where(p => p.Slug == productslug)
            .Include(p => p.Author)
            .Include(p => p.Photos)
            .Include(p => p.ProductCategoryProducts)
            .ThenInclude(pc => pc.Category)
            .FirstOrDefault();


            if (product == null)
            {
                return NotFound("Không thấy Bài viết");
            }

            CategoryProduct category = product.ProductCategoryProducts.FirstOrDefault()?.Category;
            ViewBag.category = category;


            // lấy ra 5 bài viết gần nhất
            var otherProducts = _context.Product.Where(p => p.ProductCategoryProducts.Any(c => c.Category.Id == category.Id))
            .Where(p => p.ProductID != product.ProductID)
            .OrderByDescending(p => p.DateUpdated)
            .Take(5);
            ViewBag.otherProducts = otherProducts;

            return View(product);
        }

        private List<CategoryProduct> GetCategories()
        {
            var categories = _context.CategoryProduct
            .Include(c => c.CategoryChildren)
            .AsEnumerable()
            .Where(c => c.ParentCategory == null)
            .ToList()
            ;

            return categories;
        }

        /// Thêm sản phẩm vào cart
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _context.Product
                .Where(p => p.ProductID == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            _cartService.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(_cartService.GetCartItems());
        }

        /// xóa item trong cart
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            _cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }


        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductID == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            _cartService.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        [Route("/checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartItems();

            // ....
            _cartService.ClearCart();

            return Content("Đã gửi đơn hàng !");

        }
    }
}
