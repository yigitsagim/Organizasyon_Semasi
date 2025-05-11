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
        private static Dictionary<string, string[]> calisanlar = new Dictionary<string, string[]>();

        public static void Ekle()
        {
            LoadFromCsv();

            string ad = FormVerileri0.EkleAd.Trim();
            string soyad = FormVerileri0.EkleSoyad.Trim();
            string departman = FormVerileri0.EkleDepartman.Trim();
            string unvan = FormVerileri0.EkleGorev.Trim();

            string prefix = GetPrefix(departman);
            string yeni_id, parent_id;

            if (unvan == "Yönetici")
            {
                yeni_id = prefix + "00";
                parent_id = "99";

                // Boş placeholder varsa sil
                var eskiYoneticiler = calisanlar
                    .Where(x => x.Value[0] == departman && x.Value[3] == "Yönetici" &&
                                string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]))
                    .Select(x => x.Key)
                    .ToList();

                foreach (var key in eskiYoneticiler)
                    calisanlar.Remove(key);

                calisanlar[yeni_id] = new[] { departman, ad, soyad, "Yönetici", parent_id };
            }
            else
            {
                // Yönetici var mı?
                var yonetici = calisanlar
                    .FirstOrDefault(x => x.Value[0] == departman && x.Value[3] == "Yönetici");

                parent_id = string.IsNullOrEmpty(yonetici.Key) ? "" : yonetici.Key;

                // ID üretimi
                var mevcutIdler = calisanlar.Keys
                    .Where(k => k.StartsWith(prefix))
                    .Select(k => int.TryParse(k, out int num) ? num : 0);

                int candidate = mevcutIdler.Any() ? mevcutIdler.Max() + 1 : int.Parse(prefix + "01");
                while (calisanlar.ContainsKey(candidate.ToString()))
                    candidate++;

                yeni_id = candidate.ToString();

                calisanlar[yeni_id] = new[] { departman, ad, soyad, unvan, parent_id };
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
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length != 6) continue;

                calisanlar[parts[0]] = new[]
                {
                    parts[1], parts[2], parts[3], parts[4], parts[5]
                };
            }
        }

        private static void SaveToCsv()
        {
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            var lines = new List<string> { "ID;Departman;Ad;Soyad;Ünvan;ParentID" };

            foreach (var kvp in calisanlar)
            {
                lines.Add($"{kvp.Key};{kvp.Value[0]};{kvp.Value[1]};{kvp.Value[2]};{kvp.Value[3]};{kvp.Value[4]}");
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
                default: return "0";
            }
        }
    }
}