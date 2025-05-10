using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace veri_yapilari.kodlarim
{
    // Her departmanı temsil eden Tree düğüm sınıfı
    public class DepartmanTreeNode
    {
        public int Id;                     // Departmanın benzersiz ID'si
        public string DepartmanAdi;       // Departman adı
        public string Ad;                 // Yöneticinin adı (boş olabilir)
        public string Soyad;              // Yöneticinin soyadı (boş olabilir)
        public string Unvan;              // Unvan (örneğin "Yönetici")
        public int? ParentId;             // Üst departmanın ID'si (null ise kök)
        public List<DepartmanTreeNode> Cocuklar; // Alt departmanları/kişileri tutan liste

        public DepartmanTreeNode(int id, string depAd, string ad, string soyad, string unvan, int? parentId)
        {
            Id = id;
            DepartmanAdi = depAd;
            Ad = ad;
            Soyad = soyad;
            Unvan = unvan;
            ParentId = parentId;
            Cocuklar = new List<DepartmanTreeNode>();
        }
    }

    // Departman işlemlerini yöneten sınıf (ağaç yapısına dayalı)
    public class DepartmanIslemleri
    {
        private Dictionary<int, DepartmanTreeNode> tumNodlar; // Tüm düğümleri ID ile erişilebilir şekilde tutar
        private DepartmanTreeNode kok; // Kök düğüm (Genel departmanı gibi en üst seviye)
        private readonly string dosyaYolu; // CSV dosya yolu

        public DepartmanIslemleri()
        {
            dosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/calisanlar2.csv");
            tumNodlar = new Dictionary<int, DepartmanTreeNode>();
            kok = null;
            DosyadanYukle(); // Uygulama başlarken tüm veriyi yükle
        }

        // Yeni departman ekler (FormVerileri0.YeniDepartman üzerinden)
        public void DepEkle()
        {
            string departmanAdi = FormVerileri0.YeniDepartman;
            if (string.IsNullOrWhiteSpace(departmanAdi)) return;

            // Mevcut en büyük ID'den bir üst yüzlüğü al
            int maxId = GetMaxId();
            int yeniId = ((maxId / 100) + 1) * 100;

            // Yeni düğüm oluşturulur, yönetici bilgileri boş bırakılır
            var yeni = new DepartmanTreeNode(
                yeniId,
                departmanAdi.Trim(),
                "",
                "",
                "Yönetici", // Varsayılan unvan
                99          // Parent ID sabit: Genel departman
            );

            // Sözlüğe eklenir
            tumNodlar[yeniId] = yeni;

            // 99 ID'li (Genel) düğüm varsa, yeni düğüm onun çocuğu olur
            if (tumNodlar.ContainsKey(99))
                tumNodlar[99].Cocuklar.Add(yeni);

            DosyayaKaydet(); // CSV dosyasına yaz

            // Ekleme sonrası kullanıcıya alert göster
            var page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                string mesaj = $"Yeni departman eklendi: {departmanAdi} departmanı";
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), "alert", $"alert('{mesaj}');", true);
            }
        }

        // Tüm düğümler içinde en büyük ID'yi bulur
        private int GetMaxId()
        {
            int max = 0;
            foreach (var node in tumNodlar.Values)
            {
                if (node.Id > max)
                    max = node.Id;
            }
            return max;
        }

        // CSV dosyasından veriyi okur ve ağaç yapısını kurar
        private void DosyadanYukle()
        {
            if (!File.Exists(dosyaYolu)) return;
            var satirlar = File.ReadAllLines(dosyaYolu).ToList();

            // Başlık satırını atla
            if (satirlar.Count > 0 && satirlar[0].StartsWith("ID;"))
                satirlar.RemoveAt(0);

            // 1. Aşama: Her satırdan düğümleri oluştur, ID'ye göre sözlüğe ekle
            foreach (var satir in satirlar)
            {
                var p = satir.Split(';');
                if (p.Length < 6 || !int.TryParse(p[0], out int id))
                    continue;

                int? parentId = string.IsNullOrWhiteSpace(p[5])
                                ? (int?)null
                                : int.Parse(p[5]);

                var node = new DepartmanTreeNode(id, p[1], p[2], p[3], p[4], parentId);
                tumNodlar[id] = node;
            }

            // 2. Aşama: Parent-Child ilişkilerini kur
            foreach (var node in tumNodlar.Values)
            {
                if (node.ParentId == null)
                {
                    kok = node; // Parent ID yoksa bu kök düğümdür
                }
                else if (tumNodlar.ContainsKey(node.ParentId.Value))
                {
                    tumNodlar[node.ParentId.Value].Cocuklar.Add(node);
                }
            }
        }

        // Tüm düğümleri CSV formatında, başlık satırıyla birlikte kaydeder
        private void DosyayaKaydet()
        {
            var satirlar = new List<string>();

            // 1) Başlık satırını ekle
            satirlar.Add("ID;Departman;Ad;Soyad;Ünvan;UstID");

            // 2) Mevcut düğümleri ekle
            foreach (var node in tumNodlar.Values)
            {
                string ustId = node.ParentId.HasValue ? node.ParentId.ToString() : "";
                satirlar.Add($"{node.Id};{node.DepartmanAdi};{node.Ad};{node.Soyad};{node.Unvan};{ustId}");
            }

            // 3) Dosyayı yaz
            File.WriteAllLines(dosyaYolu, satirlar);
        }

        // Ağaç yapısının kökünü dışarıya verir (isteğe bağlı)
        public DepartmanTreeNode GetKok() => kok;
    }
}
