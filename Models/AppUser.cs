using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MVC_1.Models
{
    public class AppUser: IdentityUser 
    {
          [Column(TypeName = "nvarchar")]
          [StringLength(400)]  
          public string? HomeAdress { get; set; }

          // [Required]       
          [DataType(DataType.DateTime)]
          public DateTime? BirthDate { get; set; }
    }
}
