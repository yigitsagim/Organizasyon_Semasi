using veri_yapilari.kodlarim;
using System;
using System.Collections.Generic;

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
                    if (idSozluk.ContainsKey(calisan.ParentID.Value))
                    {
                        var parent = idSozluk[calisan.ParentID.Value];
                        parent.Cocuklar.Add(calisan);
                    }
                    else
                    {
                        // Hatalı ParentID varsa log veya uyarı üretilebilir
                        // Örnek: loglama yapılabilir veya sistemde işlenmeden bırakılabilir
                        // Console.WriteLine($"Uyarı: {calisan.TamAd()} için geçersiz ParentID: {calisan.ParentID.Value}");
                    }
                }
                else
                {
                    kok = calisan; // parentID yoksa kök düğümdür
                }
            }

            return kok;
        }
    }
}




