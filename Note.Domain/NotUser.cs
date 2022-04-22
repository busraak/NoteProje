using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain
{
    [Table("NotUsers")]
    public class NotUser:MyEntityBase
    {
        [DisplayName("İsim"), 
            StringLength(25, ErrorMessage ="{0} alanı en fazla {1} karakter olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyisim"), 
            StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı"), 
            Required(ErrorMessage ="{0} alanı gereklidir."), 
            StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("E-Posta"), 
            Required(ErrorMessage = "{0} alanı gereklidir."), 
            StringLength(70, ErrorMessage = "{0} alanı en fazla {1} karakter olmalıdır.")]
        public string Email { get; set; }


        [DisplayName("Şifre"), 
            Required(ErrorMessage = "{0} alanı gereklidir."), 
            StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olmalıdır.")]
        public string Password { get; set; }

        [StringLength(30),ScaffoldColumn(false)]
        public string ProfileImageFilename { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }

        [Required,ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }


        public virtual List<Not> Nots { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }

    }
}
