using System;
using System.Collections.Generic;
using System.IO;
<<<<<<< Updated upstream
using System.Linq;
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        private string dosyaYolu;
        public List<Departman> Departmanlar { get; private set; }

        public DepartmanIslemleri()
        {
            dosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/departments.csv");
            Departmanlar = new List<Departman>();
            DosyadanYukle();
        }

        public void DepartmanEkle(int id, string ad, int? ustId)
        {
            if (Departmanlar.Any(d => d.Id == id))
                return;

            Departmanlar.Add(new Departman { Id = id, Ad = ad, UstId = ustId });
            DosyayaKaydet();
        }

        public void DepartmanSil(int id)
        {
            AltDepartmanlariSil(id);
            Departmanlar.RemoveAll(d => d.Id == id);
            DosyayaKaydet();
        }

        private void AltDepartmanlariSil(int ustId)
        {
            var altlar = Departmanlar.Where(d => d.UstId == ustId).ToList();
            foreach (var alt in altlar)
            {
                AltDepartmanlariSil(alt.Id);
                Departmanlar.Remove(alt);
            }
        }

        private void DosyayaKaydet()
        {
            var satirlar = Departmanlar.Select(d => $"{d.Id},{d.Ad},{(d.UstId.HasValue ? d.UstId.ToString() : "")}");
            File.WriteAllLines(dosyaYolu, satirlar);
        }

        private void DosyadanYukle()
        {
            if (!File.Exists(dosyaYolu))
                return;

            var satirlar = File.ReadAllLines(dosyaYolu);
            foreach (var satir in satirlar)
            {
                var parcalar = satir.Split(',');
                int id = int.Parse(parcalar[0]);
                string ad = parcalar[1];
                int? ustId = string.IsNullOrWhiteSpace(parcalar[2]) ? (int?)null : int.Parse(parcalar[2]);

                Departmanlar.Add(new Departman
                {
                    Id = id,
                    Ad = ad,
                    UstId = ustId
                });
            }
        }
=======
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
            DepartmanTreeNode yeni = new DepartmanTreeNode(
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
            string[] satirlar = File.ReadAllLines(dosyaYolu);

            // 1. Aşama: Her satırdan düğümleri oluştur, ID'ye göre sözlüğe ekle
            foreach (var satir in satirlar)
            {
                var p = satir.Split(';');
                if (p.Length < 6 || !int.TryParse(p[0], out int id)) continue;

                int? parentId = string.IsNullOrWhiteSpace(p[5]) ? (int?)null : int.Parse(p[5]);

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
                else if (tumNodlar.ContainsKey((int)node.ParentId))
                {
                    tumNodlar[(int)node.ParentId].Cocuklar.Add(node); // Ağaca yerleştir
                }
            }
        }

        // Tüm düğümleri CSV formatında kaydeder
        private void DosyayaKaydet()
        {
            List<string> satirlar = new List<string>();
            foreach (var node in tumNodlar.Values)
            {
                satirlar.Add($"{node.Id};{node.DepartmanAdi};{node.Ad};{node.Soyad};{node.Unvan};{(node.ParentId.HasValue ? node.ParentId.ToString() : "")}");
            }
            File.WriteAllLines(dosyaYolu, satirlar);
        }

        // Ağaç yapısının kökünü dışarıya verir (isteğe bağlı)
        public DepartmanTreeNode GetKok() => kok;
>>>>>>> Stashed changes
    }
}
