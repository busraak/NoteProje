using Note.BusinessLayer.Abstract;
using Note.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.BusinessLayer
{
    public class CategoryManager:ManagerBase<Category>
    {
     //public override int Delete(Category category)
     //   {
     //       NotManager notManager = new NotManager();
     //       LikedManager likedManager = new LikedManager();
     //       CommentManager commentManager = new CommentManager();


     //       foreach (Not not in category.Nots.ToList())//kategori ile iliskili notlar
     //       {
     //           foreach (Liked like in not.Likes.ToList()) //not ile iliskili like lar
     //           {
     //               likedManager.Delete(like);
     //           }

     //           foreach (Comment comment in not.Comments.ToList()) // not ile iliskili commentler
     //           {
     //               commentManager.Delete(comment);
     //           }

     //           notManager.Delete(not);
     //       }
     //       return base.Delete(category);
     //   }
    }
}
