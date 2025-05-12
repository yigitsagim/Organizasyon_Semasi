using veri_yapilari.kodlarim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public static class AgacKurucu
    {
        public static Calisan AgaciKur(List<Calisan> liste)
        {
            Dictionary<int, Calisan> idSozluk = new Dictionary<int, Calisan>();
            Calisan kok = null;

            // Tüm düğümleri sözlüğe ekle
            foreach (var calisan in liste)
            {
                calisan.Cocuklar = new List<Calisan>();
                idSozluk[calisan.ID] = calisan;
            }

            // Her düğümü parent'ına bağla
            foreach (var calisan in liste)
            {
                if (calisan.ParentID.HasValue)
                {
                    var parent = idSozluk[calisan.ParentID.Value];
                    parent.Cocuklar.Add(calisan);
                }
                else
                {
                    kok = calisan; // parentID yoksa köktür
                }
            }

            return kok;
        }
    }
}



