using veri_yapilari.kodlarim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public class Calisan
    {
        public int ID { get; set; }
        public string Departman { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public int? ParentID { get; set; } // Yönetici yoksa (örn. CEO), null olur.

        public List<Calisan> Cocuklar { get; set; }

        public Calisan()
        {
            Cocuklar = new List<Calisan>();
        }

        public string TamAd()
        {
            return Ad + " " + Soyad;
        }

        public override string ToString()
        {
            return $"{Ad} {Soyad} - {Departman} - {Unvan}";
        }
    }
}

