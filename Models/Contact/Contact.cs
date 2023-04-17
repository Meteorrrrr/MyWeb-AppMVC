using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_1.Models_Contact
{
    public class Contact{
        [Key]
        public int id{set;get;}
        [Required(ErrorMessage ="Phải nhập tên")]
        [StringLength(50)]
        [Column(TypeName="nvarchar")]
        [Display(Name ="Tên của bạn")]
        public string FullName{set;get;}
        [EmailAddress(ErrorMessage ="Email chưa đúng định dạng")]
        public string Email{set;get;}
        public DateTime DateSend{set;get;}
        [Column(TypeName ="ntext")]
        
        public string Content{set;get;}
         [Phone(ErrorMessage ="Phải nhập số điện thoại")]
        public string Phone{set;get;}
    }
}