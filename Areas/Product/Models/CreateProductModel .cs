using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Models.Product;

namespace App.Areas.Product.Models
{
    public class CreateProductModel : ProductModel
    {
        [Display(Name = "Danh mục sản phẩm")]
        public int[] CategoryIDs { get; set; }
    }
}