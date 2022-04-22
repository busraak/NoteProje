using Note.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.DataAccessLayer.EF
{
    public class RepositoryBase
    {
        // bu islem ile ilk cagırmada databasecontext newlenir, sonrasında newlenmez.
        protected static DatabaseContext context; 
        private static object _lockSync = new object();

        protected RepositoryBase() // artık newlenemez.
        {
            CreateContext();
        }
        private static void CreateContext()
        {
            //ornegin comment calıstı,category calısacagı zaman db nin null olmadıgını goruyor buradan ve var olan db yi geri donduruyor.
            //boylece her defasında aynı nesne ile calısıp her defasında olusan repository classımızın aynı db yi aldı,aynı db uzerinden islerini yuruttu.
            if(context == null)
            {
                lock (_lockSync) //ortamı kitler, aynı anda iki parcacıgın calısmasını engeller.
                {
                    if (context == null) // dısarıda yapılan kontrol tekrar yapılıyor, amac garantiye almak.
                    {
                        context = new DatabaseContext();
                    }
                    
                }
               
            }
            
        }
    }
}
