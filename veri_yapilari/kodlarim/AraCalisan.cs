using veri_yapilari.kodlarim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;




namespace veri_yapilari.kodlarim
{
    public class AraCalisan
    {
        public string Bul(string ad, string soyad)
        {
            // Önce CSV'den verileri oku
            List<Calisan> calisanListesi = CsvOkuyucu.CsvdenCalisanlariOku();
            if (calisanListesi == null || calisanListesi.Count == 0)
                return "Veri bulunamadı.";

            // Ağaç yapısını kur
            Calisan kok = AgacKurucu.AgaciKur(calisanListesi);
            if (kok == null)
                return "Ağaç oluşturulamadı.";

            // Kuyruk kullanarak genişlik öncelikli arama (BFS)
            Queue<Calisan> kuyruk = new Queue<Calisan>();
            kuyruk.Enqueue(kok);

            while (kuyruk.Count > 0)
            {
                Calisan mevcut = kuyruk.Dequeue();

                // Eşleşme kontrolü (büyük/küçük harf duyarsız)
                if (mevcut.Ad.Equals(ad, StringComparison.OrdinalIgnoreCase) &&
                    mevcut.Soyad.Equals(soyad, StringComparison.OrdinalIgnoreCase))
                {
                    return $"Ad: {mevcut.Ad}, Soyad: {mevcut.Soyad}, Departman: {mevcut.Departman}, Ünvan: {mevcut.Unvan}";
                }

                // Çocukları kuyruğa ekle
                foreach (var cocuk in mevcut.Cocuklar)
                {
                    kuyruk.Enqueue(cocuk);
                }
            }

            return "Aranan çalışan bulunamadı.";
        }
    }
}

