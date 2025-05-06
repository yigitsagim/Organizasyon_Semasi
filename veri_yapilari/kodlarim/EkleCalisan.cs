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
<<<<<<< HEAD
            string yeni_id = "";
            string parent_id = "";

            if (unvan == "Yönetici")
            {
                // Aktif yönetici var mı kontrol et
                bool mevcutYoneticiVar = calisanlar.Any(x =>
                    x.Value[0] == departman &&
                    x.Value[3] == "Yönetici" &&
                    !string.IsNullOrWhiteSpace(x.Value[1]) &&
                    !string.IsNullOrWhiteSpace(x.Value[2]));

                if (mevcutYoneticiVar)
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

                // Boş (placeholder) yöneticiler varsa sil
                var eskiBosYoneticiler = calisanlar
=======
            string yeni_id, parent_id;

            if (unvan == "Yönetici")
            {
                yeni_id = prefix + "00";
                parent_id = "99";

                // Boş placeholder varsa sil
                var eskiYoneticiler = calisanlar
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                    .Where(x => x.Value[0] == departman && x.Value[3] == "Yönetici" &&
                                string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]))
                    .Select(x => x.Key)
                    .ToList();

<<<<<<< HEAD
                foreach (var key in eskiBosYoneticiler)
                    calisanlar.Remove(key);

                yeni_id = prefix + "00";
                parent_id = "99";

=======
                foreach (var key in eskiYoneticiler)
                    calisanlar.Remove(key);

>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                calisanlar[yeni_id] = new[] { departman, ad, soyad, "Yönetici", parent_id };
            }
            else
            {
<<<<<<< HEAD
                // Departmanın yöneticisi varsa parent_id olarak belirle
=======
                // Yönetici var mı?
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                var yonetici = calisanlar
                    .FirstOrDefault(x => x.Value[0] == departman && x.Value[3] == "Yönetici");

                parent_id = string.IsNullOrEmpty(yonetici.Key) ? "" : yonetici.Key;

<<<<<<< HEAD
                // Yeni ID oluştur
=======
                // ID üretimi
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                var mevcutIdler = calisanlar.Keys
                    .Where(k => k.StartsWith(prefix))
                    .Select(k => int.TryParse(k, out int num) ? num : 0);

                int candidate = mevcutIdler.Any() ? mevcutIdler.Max() + 1 : int.Parse(prefix + "01");
                while (calisanlar.ContainsKey(candidate.ToString()))
                    candidate++;

                yeni_id = candidate.ToString();

                calisanlar[yeni_id] = new[] { departman, ad, soyad, unvan, parent_id };
            }
<<<<<<< HEAD

=======
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
            if (HttpContext.Current.CurrentHandler is Page page)
            {
                string mesaj = $"Eklendi: {ad} {soyad} → ID: {yeni_id} / {departman} / {unvan}";
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "addMsg",
                    $"alert('{mesaj}');",
                    true
                );
<<<<<<< HEAD

=======
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                SaveToCsv();
            }
        }

        private static void LoadFromCsv()
        {
            calisanlar.Clear();
<<<<<<< HEAD
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return;

            foreach (string line in File.ReadAllLines(path).Skip(1))
=======
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path).Skip(1))
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
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
<<<<<<< HEAD
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
=======
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
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
<<<<<<< HEAD

=======
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
