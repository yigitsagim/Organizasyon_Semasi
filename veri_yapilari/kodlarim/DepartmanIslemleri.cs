using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace veri_yapilari.kodlarim
{
    public class DepartmanTreeNode
    {
        public int Id;
        public string DepartmanAdi;
        public string Ad;
        public string Soyad;
        public string Unvan;
        public int? ParentId;
        public List<DepartmanTreeNode> Cocuklar;

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

    public class DepartmanIslemleri
    {
        private Dictionary<int, DepartmanTreeNode> tumNodlar;
        private DepartmanTreeNode kok;
        private readonly string dosyaYolu;
        private readonly string departmanlarDosyaYolu;

        public DepartmanIslemleri()
        {
            dosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/calisanlar2.csv");
            departmanlarDosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/departmanlar.csv");
            tumNodlar = new Dictionary<int, DepartmanTreeNode>();
            kok = null;
            DosyadanYukle();
        }

        public void DepEkle()
        {
            string departmanAdi = FormVerileri0.YeniDepartman;
            if (string.IsNullOrWhiteSpace(departmanAdi)) return;

            int maxId = GetMaxId();
            int yeniId = ((maxId / 100) + 1) * 100;

            DepartmanTreeNode yeni = new DepartmanTreeNode(
                yeniId,
                departmanAdi.Trim(),
                "",
                "",
                "Yönetici",
                99
            );

            tumNodlar[yeniId] = yeni;

            if (tumNodlar.ContainsKey(99))
                tumNodlar[99].Cocuklar.Add(yeni);

            DosyayaKaydet();

            var page = HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                string mesaj = $"Yeni departman eklendi: {departmanAdi} departmanı";
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), "alert", $"alert('{mesaj}');", true);
            }
        }

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

        private void DosyadanYukle()
        {
            if (!File.Exists(dosyaYolu)) return;

            string[] satirlar = File.ReadAllLines(dosyaYolu);
            tumNodlar.Clear(); // Eski veriler silinsin

            foreach (var satir in satirlar.Skip(1)) // Başlık satırını atla
            {
                var p = satir.Split(';');
                if (p.Length < 6 || !int.TryParse(p[0], out int id)) continue;

                int? parentId = string.IsNullOrWhiteSpace(p[5]) ? (int?)null : int.Parse(p[5]);

                var node = new DepartmanTreeNode(id, p[1], p[2], p[3], p[4], parentId);
                tumNodlar[id] = node;
            }

            foreach (var node in tumNodlar.Values)
            {
                if (node.ParentId == null || node.ParentId == 0)
                {
                    kok = node;
                }
                else if (tumNodlar.ContainsKey((int)node.ParentId))
                {
                    tumNodlar[(int)node.ParentId].Cocuklar.Add(node);
                }
            }

            // Eğer hala kök atanmadıysa 99'u varsayalım
            if (kok == null && tumNodlar.ContainsKey(99))
            {
                kok = tumNodlar[99];
            }
        }


        private void DosyayaKaydet()
        {
            List<string> satirlarCalisanlar = new List<string>
    {
        "ID;Departman;Ad;Soyad;Ünvan;UstID" // Başlık satırı
    };

            Dictionary<string, int> benzersizDepartmanlar = new Dictionary<string, int>();

            // Çalışanları yaz
            foreach (var node in tumNodlar.Values.OrderBy(n => n.Id))
            {
                satirlarCalisanlar.Add($"{node.Id};{node.DepartmanAdi};{node.Ad};{node.Soyad};{node.Unvan};{(node.ParentId.HasValue ? node.ParentId.ToString() : "")}");

                // Departman adı daha önce eklenmemişse, Id’sini al
                if (!string.IsNullOrWhiteSpace(node.DepartmanAdi) && !benzersizDepartmanlar.ContainsKey(node.DepartmanAdi))
                {
                    benzersizDepartmanlar[node.DepartmanAdi] = node.Id;
                }
            }

            File.WriteAllLines(dosyaYolu, satirlarCalisanlar);

            // Departman dosyasını yaz
            List<string> satirlarDepartmanlar = new List<string> { "Id;DepartmanAdi" };
            foreach (var kvp in benzersizDepartmanlar.OrderBy(k => k.Value))
            {
                satirlarDepartmanlar.Add($"{kvp.Value};{kvp.Key}");
            }

            File.WriteAllLines(departmanlarDosyaYolu, satirlarDepartmanlar);
        }




        public DepartmanTreeNode GetKok() => kok;
    }
}