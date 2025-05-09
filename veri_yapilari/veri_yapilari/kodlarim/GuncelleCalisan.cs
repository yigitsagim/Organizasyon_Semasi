﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace veri_yapilari.kodlarim 
{
    public static class DictionaryExtensions //şartı sağlayan tüm elemanları silen metot
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

        // Çalışanlar için sözlük(dictionary) veri yapısı ID -> [Departman, Ad, Soyad, Ünvan, ParentID]
        private static Dictionary<string, string[]> calisanlar = new Dictionary<string, string[]>();

        public static void Guncelle() // Ana metod: Çalışan bilgilerini güncelle

        {
            LoadDataFromCsv(); //var olan çalışanların bilgisi dict e atildi

            // Formdan gelen yeni bilgiler
            string ad = FormVerileri0.GuncelAd.Trim();
            string soyad = FormVerileri0.GuncelSoyad.Trim();
            string new_dep = FormVerileri0 .GuncelYeniDepartman;
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
