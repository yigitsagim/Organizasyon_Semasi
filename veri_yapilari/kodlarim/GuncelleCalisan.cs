using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
<<<<<<< HEAD
=======
using System.Web.UI;
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb

namespace veri_yapilari.kodlarim //altta csvnin nasil yuklenip tekrar kaydedilecegini gosteriyor
{
    public static class DictionaryExtensions
    {
        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var toRemove = dict.Where(predicate).Select(kvp => kvp.Key).ToList();
            foreach (var key in toRemove)
                dict.Remove(key);
        }
    }
    public static class GuncelleCalisan
    {
        private static string DataFileVirtual = "~/App_Data/calisanlar2.csv";
        private static Dictionary<string, string[]> calisanlar = new Dictionary<string, string[]>();

        public static void Guncelle()
        {
            LoadDataFromCsv();

            string ad = FormVerileri0.GuncelAd.Trim();
            string soyad = FormVerileri0.GuncelSoyad.Trim();
<<<<<<< HEAD
            string new_dep = FormVerileri0.GuncelYeniDepartman;
=======
            string new_dep = FormVerileri0 .GuncelYeniDepartman;
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
            string new_rol = FormVerileri0.GuncelYeniGorev;

            var matches = calisanlar
                .Where(kvp => Normalize(kvp.Value[1]) == Normalize(ad) && Normalize(kvp.Value[2]) == Normalize(soyad))
                .ToList();

            if (matches.Count != 1) return;

            var existing = matches[0];
            string old_id = existing.Key;
            string old_dep = existing.Value[0];
            string old_ad = existing.Value[1];
            string old_soyad = existing.Value[2];
            string old_rol = existing.Value[3];
            string old_parent = existing.Value[4];

            string prefix = GetPrefix(new_dep);
            string new_id;
            string new_parent;

            if (new_rol == "Yönetici")
            {
<<<<<<< HEAD
=======
                // Yeni departmanda zaten başka bir yönetici varsa uyarı ver
                var mevcutYoneticiVar = calisanlar.Any(x =>
                    x.Value[0] == new_dep &&
                    x.Value[3] == "Yönetici" &&
                    !(x.Value[1] == old_ad && x.Value[2] == old_soyad));

                if (mevcutYoneticiVar)
                {
                    if (HttpContext.Current.CurrentHandler is Page hataSayfasi)
                    {
                        hataSayfasi.ClientScript.RegisterStartupScript(
                            hataSayfasi.GetType(),
                            "yoneticiUyari",
                            $"alert('HATA: {new_dep} departmanında zaten bir yönetici bulunmaktadır.');",
                            true
                        );
                    }
                    return;
                }


>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb
                new_id = prefix + "00";
                new_parent = "99";

                calisanlar.RemoveAll(x =>
                    x.Value[0] == new_dep && x.Value[3] == "Yönetici" &&
                    string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]));

                if (new_id != old_id)
                {
                    calisanlar[old_id] = new[] { old_dep, "", "", "Yönetici", "0" };

                    foreach (var key in calisanlar.Keys.ToList())
                    {
                        if (calisanlar[key][4] == old_id)
                            calisanlar[key][4] = "0";
                    }

                    calisanlar[new_id] = new[] { new_dep, old_ad, old_soyad, "Yönetici", new_parent };
                }
                else
                {
                    calisanlar[old_id] = new[] { new_dep, old_ad, old_soyad, "Yönetici", new_parent };
                }
            }
            else
            {
                var yonetici = calisanlar
                    .FirstOrDefault(x => x.Value[0] == new_dep && x.Value[3] == "Yönetici");

                new_parent = string.IsNullOrEmpty(yonetici.Key) ? "" : yonetici.Key;

                var tumIdler = calisanlar.Keys
                    .Where(k => k.StartsWith(prefix))
                    .Select(int.Parse);

                int candidate = tumIdler.Any() ? tumIdler.Max() + 1 : int.Parse(prefix + "01");
                while (calisanlar.ContainsKey(candidate.ToString()))
                    candidate++;

                new_id = candidate.ToString();

                if (new_id != old_id)
                {
                    calisanlar[old_id] = new[] { old_dep, "", "", "Yönetici", "99" };
                    calisanlar[new_id] = new[] { new_dep, old_ad, old_soyad, new_rol, new_parent };
                }
                else
                {
                    calisanlar[old_id] = new[] { new_dep, old_ad, old_soyad, new_rol, new_parent };
                }
            }
<<<<<<< HEAD
=======
            if (HttpContext.Current.CurrentHandler is Page page)
            {
                string mesaj = $"Güncellendi: {old_ad} {old_soyad} → ID: {new_id} / {new_dep} / {new_rol}";
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "updateMsg",
                    $"alert('{mesaj}');",
                    true
                );
            }
>>>>>>> d826f30c784e237daa4b93359e8e7d0384d9e2cb

            SaveToCsv();
        }

        private static void LoadDataFromCsv()
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

        private static string Normalize(string input) =>
            input?.Trim().ToLower(new System.Globalization.CultureInfo("tr-TR", false));

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

        private static void RemoveAll(this Dictionary<string, string[]> dict, Func<KeyValuePair<string, string[]>, bool> predicate)
        {
            var toRemove = dict.Where(predicate).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
                dict.Remove(key);
        }
    }
}
