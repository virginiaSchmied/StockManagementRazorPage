using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace StockManagementRazorPage.Models.Product
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Price { get; set; }
        public DateTime UploadDate { get; set; }

        [ForeignKey("ProductCategory_Id")]
        public int IdProductCategory_Id { get; set; }
    }

    public class ProductResponse
    {
        public List<Product> Products { get; set; }
        public object PrevPage { get; set; }
        public object NextPage { get; set; }
        public int TotalPages { get; set; }
    }
}
