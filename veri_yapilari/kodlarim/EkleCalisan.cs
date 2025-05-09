using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public static class EkleCalisan
    {
        private static string DataFileVirtual = "~/App_Data/calisanlar2.csv";
        private static CalisanHashTable calisanlar = new CalisanHashTable();

        public static void Ekle()
        {
            LoadFromCsv();

            string ad = FormVerileri0.EkleAd.Trim();
            string soyad = FormVerileri0.EkleSoyad.Trim();
            string departman = FormVerileri0.EkleDepartman.Trim();
            string unvan = FormVerileri0.EkleGorev.Trim();

            string prefix = GetPrefix(departman);
            string yeni_id = "";
            string parent_id = "";

            if (unvan == "Yönetici")
            {
                if (calisanlar.YoneticiVarMi(departman))
                {
                    if (HttpContext.Current.CurrentHandler is Page page1)
                    {
                        string mesaj = $"Bu departmanda zaten bir yönetici var. Yeni yönetici eklenemez.";
                        page1.ClientScript.RegisterStartupScript(
                            page1.GetType(),
                            "yoneticiVarMsg",
                            $"alert('{mesaj}');",
                            true
                        );
                    }
                    return;
                }

                calisanlar.BosYoneticileriSil(departman);
                yeni_id = prefix + "00";
                parent_id = "99";

                var node = new Node(yeni_id, departman, ad, soyad, "Yönetici", parent_id);
                calisanlar.Ekle(node);
            }
            else
            {
                var yonetici = calisanlar.GetYonetici(departman);
                parent_id = yonetici != null ? yonetici.ID : "";

                var mevcutIdler = calisanlar.TumCalisanlariGetir()
                    .Where(x => x.ID.StartsWith(prefix))
                    .Select(x => int.TryParse(x.ID, out int num) ? num : 0);

                int candidate = mevcutIdler.Any() ? mevcutIdler.Max() + 1 : int.Parse(prefix + "01");
                while (calisanlar.Get(candidate.ToString()) != null)
                    candidate++;

                yeni_id = candidate.ToString();

                var node = new Node(yeni_id, departman, ad, soyad, unvan, parent_id);
                calisanlar.Ekle(node);
            }

            if (HttpContext.Current.CurrentHandler is Page page)
            {
                string mesaj = $"Eklendi: {ad} {soyad} → ID: {yeni_id} / {departman} / {unvan}";
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "addMsg",
                    $"alert('{mesaj}');",
                    true
                );

                SaveToCsv();
            }
        }

        private static void LoadFromCsv()
        {
            calisanlar.Clear();
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return;

            foreach (string line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length != 6) continue;

                var node = new Node(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                calisanlar.Ekle(node);
            }
        }

        private static void SaveToCsv()
        {
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            var lines = new List<string> { "ID;Departman;Ad;Soyad;Ünvan;ParentID" };

            var siraliCalisanlar = calisanlar.TumCalisanlariGetir()
                .OrderBy(x => GetDepartmanSirasi(x.Departman))
                .ThenBy(x => x.Unvan != "Yönetici") // önce yöneticiler
                .ThenBy(x => int.TryParse(x.ID, out int id) ? id : int.MaxValue);

            foreach (var node in siraliCalisanlar)
            {
                lines.Add($"{node.ID};{node.Departman};{node.Ad};{node.Soyad};{node.Unvan};{node.ParentID}");
            }

            File.WriteAllLines(path, lines);
        }

        private static string GetPrefix(string departman)
        {
            switch (departman)
            {
                case "Finans": return "1";
                case "BT": return "2";
                case "Satış": return "3";
                case "Pazarlama": return "4";
                case "İK":
                case "İnsan Kaynakları": return "5";
                case "Genel": return "9"; // Genel yönetici için özel prefix
                default: return "0";
            }
        }

        private static int GetDepartmanSirasi(string departman)
        {
            switch (departman)
            {
                case "Genel": return 0;
                case "Finans": return 1;
                case "BT": return 2;
                case "Satış": return 3;
                case "Pazarlama": return 4;
                case "İK":
                case "İnsan Kaynakları": return 5;
                default: return 99;
            }
        }
    }
}

