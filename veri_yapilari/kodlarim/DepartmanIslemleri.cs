using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public class DepartmanIslemleri
    {
        // CSV dosyanızın tam yolu (App_Data altında)
        private readonly string dosyaYolu =
            HttpContext.Current.Server.MapPath("~/App_Data/calisanlar2.csv");

        /// <summary>
        /// FormVerileri0.YeniDepartman'dan gelen ismi alır,
        /// bir Calisan nesnesi olarak listeye ekler,
        /// AgacKurucu ile ağacı yeniden kurar ve CSV'ye yazar.
        /// </summary>
        public void DepEkle()
        {
            // 1) Form’dan gelen departman adı
            string yeniDep = FormVerileri0.YeniDepartman?.Trim();
            if (string.IsNullOrEmpty(yeniDep))
                return;

            // 2) Mevcut Calisan listesini yükle
            List<Calisan> liste = YukleListe();

            // 3) Ağacı kur (istersen burayı atlayıp sadece listeyi güncelleyeceksin)
            Calisan kok = AgacKurucu.AgaciKur(liste);

            // 4) Yeni ID hesapla (yüksek ID'nin bir sonraki yüzlüğü)
            int maxId = liste.Any() ? liste.Max(c => c.ID) : 0;
            int yeniId = ((maxId / 100) + 1) * 100;

            // 5) Yeni Calisan (departman) nesnesini oluştur
            var yeniNode = new Calisan
            {
                ID = yeniId,
                Departman = yeniDep,
                Ad = string.Empty,
                Soyad = string.Empty,
                Unvan = "Yönetici",
                ParentID = 99,                  // Genel kök departman ID’si
                Cocuklar = new List<Calisan>()
            };

            // 6) Listeye ekle
            liste.Add(yeniNode);

            // 7) Ağacı tekrar kur (eğer ek görselleştirme yapacaksan)
            kok = AgacKurucu.AgaciKur(liste);

            // 8) Güncellenen listeyi CSV’ye yaz
            KaydetListe(liste);

            // 9) Kullanıcıyı bilgilendir
            var page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                string mesaj = $"Yeni departman eklendi: {yeniDep}";
                ScriptManager.RegisterStartupScript(
                    page, page.GetType(),
                    "depEkleAlert",
                    $"alert('{mesaj}');",
                    true);
            }
        }

        /// <summary>
        /// CSV'den Calisan listesini okur
        /// </summary>
        private List<Calisan> YukleListe()
        {
            var liste = new List<Calisan>();
            if (!File.Exists(dosyaYolu)) return liste;

            foreach (var satir in File.ReadAllLines(dosyaYolu))
            {
                var p = satir.Split(';');
                if (p.Length < 6 || !int.TryParse(p[0], out int id))
                    continue;

                liste.Add(new Calisan
                {
                    ID = id,
                    Departman = p[1],
                    Ad = p[2],
                    Soyad = p[3],
                    Unvan = p[4],
                    ParentID = string.IsNullOrWhiteSpace(p[5])
                                ? (int?)null
                                : int.Parse(p[5]),
                    Cocuklar = new List<Calisan>()
                });
            }
            return liste;
        }

        /// <summary>
        /// Calisan listesini CSV formatında yazar
        /// </summary>
        private void KaydetListe(List<Calisan> liste)
        {
            var satirlar = liste
                .Select(c =>
                    $"{c.ID};{c.Departman};{c.Ad};{c.Soyad};{c.Unvan};" +
                    $"{(c.ParentID.HasValue ? c.ParentID.Value.ToString() : "")}");
            File.WriteAllLines(dosyaYolu, satirlar);
        }
    }
}