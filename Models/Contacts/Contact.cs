using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Contacts
{
  public class Contact
  {
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "nvarchar")]
    [StringLength(50)]
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; }
    public DateTime DateSent { get; set; }
    [Display(Name = "Nội dung")]
    public string Message { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    public string Phone { get; set; }
  }
}