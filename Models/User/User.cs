using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Transactions;

namespace StockManagementRazorPage.Models.User
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "introduce a email")]
        [EmailAddress(ErrorMessage = "invalid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Introduce the password")]
        public string Password { get; set; }

        [ForeignKey("Role_Id")]
        public int Role_Id { get; set; }

    }

}


