using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models.Product
{
    [Table("ProductPhoto")]
    public class ProductPhoto
    {
        [Key]
        public int ID { get; set; }

        // VD FileName: image1.jpg
        // => /contents/products/image1.jpg
        public string FileName { get; set; }

        public string PhotoURL { get; set; }

        //tham chiếu đến ProductModel
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
    }
}