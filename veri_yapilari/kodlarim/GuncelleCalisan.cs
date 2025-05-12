using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public class Node
    {
        public string Key;
        public string[] Value;
        public Node Next;

        public Node(string key, string[] value)
        {
            Key = key;
            Value = value;
            Next = null;
        }
    }

    public class NodeHashTable
    {
        private const int size = 100;
        private Node[] buckets;

        public NodeHashTable()
        {
            buckets = new Node[size];
        }

        private int GetIndex(string key)
        {
            return Math.Abs(key.GetHashCode()) % size;
        }

        public void AddOrUpdate(string key, string[] value)
        {
            int index = GetIndex(key);
            Node current = buckets[index];

            while (current != null)
            {
                if (current.Key == key)
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            Node newNode = new Node(key, value);
            newNode.Next = buckets[index];
            buckets[index] = newNode;
        }

        public bool ContainsKey(string key)
        {
            int index = GetIndex(key);
            Node current = buckets[index];

            while (current != null)
            {
                if (current.Key == key)
                    return true;
                current = current.Next;
            }
            return false;
        }

        public string[] Get(string key)
        {
            int index = GetIndex(key);
            Node current = buckets[index];

            while (current != null)
            {
                if (current.Key == key)
                    return current.Value;
                current = current.Next;
            }
            return null;
        }

        public IEnumerable<KeyValuePair<string, string[]>> GetAll()
        {
            for (int i = 0; i < size; i++)
            {
                Node current = buckets[i];
                while (current != null)
                {
                    yield return new KeyValuePair<string, string[]>(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }

        public List<KeyValuePair<string, string[]>> Where(Func<KeyValuePair<string, string[]>, bool> predicate)
        {
            List<KeyValuePair<string, string[]>> results = new List<KeyValuePair<string, string[]>>();

            for (int i = 0; i < size; i++)
            {
                Node current = buckets[i];
                while (current != null)
                {
                    var kvp = new KeyValuePair<string, string[]>(current.Key, current.Value);
                    if (predicate(kvp))
                        results.Add(kvp);
                    current = current.Next;
                }
            }

            return results;
        }

        public void Remove(string key)
        {
            int index = GetIndex(key);
            Node current = buckets[index];
            Node previous = null;

            while (current != null)
            {
                if (current.Key == key)
                {
                    if (previous == null)
                        buckets[index] = current.Next;
                    else
                        previous.Next = current.Next;
                    return;
                }
                previous = current;
                current = current.Next;
            }
        }

        public void RemoveAll(Func<KeyValuePair<string, string[]>, bool> predicate)
        {
            for (int i = 0; i < size; i++)
            {
                Node current = buckets[i];
                Node previous = null;

                while (current != null)
                {
                    var kvp = new KeyValuePair<string, string[]>(current.Key, current.Value);
                    if (predicate(kvp))
                    {
                        if (previous == null)
                            buckets[i] = current.Next;
                        else
                            previous.Next = current.Next;

                        current = previous == null ? buckets[i] : previous.Next;
                        continue;
                    }

                    previous = current;
                    current = current.Next;
                }
            }
        }

        public void Clear()
        {
            buckets = new Node[size];
        }
    }
   
        public static class GuncelleCalisan
        {
            private static string DataFileVirtual = "~/App_Data/calisanlar2.csv";
            public static NodeHashTable calisanlar = new NodeHashTable();

        private static string GetPrefix(string departmanAdi)
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/departmanlar.csv");
            if (!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine("CSV dosyası bulunamadı: " + path);
                return "-1";
            }

            string aranacak = Normalize(departmanAdi);
            var lines = File.ReadAllLines(path).Skip(1);

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    string csvIdStr = parts[0].Trim();
                    string csvDepAdi = parts[1].Trim();

                    if (Normalize(csvDepAdi) == aranacak)
                    {
                        return csvIdStr;
                    }
                }
            }

            return "-1";
        }


        public static void Guncelle()
            {
                LoadDataFromCsv();

                string ad = FormVerileri0.GuncelAd.Trim();
                string soyad = FormVerileri0.GuncelSoyad.Trim();
                string new_dep = FormVerileri0.GuncelYeniDepartman;
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
                if (prefix == "-1")
                {
                    ScriptManager.RegisterStartupScript(
                        HttpContext.Current.CurrentHandler as Page,
                        typeof(Page),
                        "departmanHata",
                        $"alert('Geçersiz departman adı!');",
                        true
                    );
                    return;
                }

                int baseId = int.Parse(prefix);
                string new_id;
                string new_parent;

                if (new_rol == "Yönetici")
                {
                    bool mevcutYoneticiVar = calisanlar.Where(x =>
                        x.Value[0] == new_dep &&
                        x.Value[3] == "Yönetici" &&
                        !(x.Value[1] == old_ad && x.Value[2] == old_soyad) &&
                        !(string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]))
                    ).Any();

                    if (mevcutYoneticiVar)
                    {
                        ScriptManager.RegisterStartupScript(
                            HttpContext.Current.CurrentHandler as Page,
                            typeof(Page),
                            "yoneticiUyari",
                            $"alert('HATA: {new_dep} departmanında zaten bir yönetici bulunmaktadır.');",
                            true
                        );
                        return;
                    }

                    new_id = (baseId).ToString(); // Örn: 2 → 200
                    new_parent = "99";

                    calisanlar.RemoveAll(x =>
                        x.Value[0] == new_dep && x.Value[3] == "Yönetici" &&
                        string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]));

                    if (new_id != old_id)
                    {
                        calisanlar.Remove(old_id);

                        foreach (var kvp in calisanlar.GetAll())
                        {
                            if (kvp.Value[4] == old_id)
                                kvp.Value[4] = "0";
                        }

                        calisanlar.AddOrUpdate(new_id, new[] { new_dep, old_ad, old_soyad, new_rol, new_parent });
                    }
                    else
                    {
                        calisanlar.AddOrUpdate(old_id, new[] { new_dep, old_ad, old_soyad, new_rol, new_parent });
                    }
                }
                else
                {
                    var yonetici = calisanlar
                        .Where(x => x.Value[0] == new_dep && x.Value[3] == "Yönetici")
                        .FirstOrDefault();

                    new_parent = yonetici.Key ?? "";

                    var tumIdler = calisanlar.GetAll()
                        .Where(k => k.Key.StartsWith((baseId).ToString()))
                        .Select(k => int.Parse(k.Key));

                    int candidate = tumIdler.Any() ? tumIdler.Max() + 1 : baseId + 1;
                    while (calisanlar.ContainsKey(candidate.ToString()))
                        candidate++;

                    new_id = candidate.ToString();

                    if (new_id != old_id)
                    {
                        calisanlar.Remove(old_id);

                        foreach (var kvp in calisanlar.GetAll())
                        {
                            if (kvp.Value[4] == old_id)
                                kvp.Value[4] = "0";
                        }

                        calisanlar.AddOrUpdate(new_id, new[] { new_dep, old_ad, old_soyad, new_rol, new_parent });
                    }
                    else
                    {
                        calisanlar.AddOrUpdate(old_id, new[] { new_dep, old_ad, old_soyad, new_rol, new_parent });
                    }
                }

                SaveToCsv();

                ScriptManager.RegisterStartupScript(
                    HttpContext.Current.CurrentHandler as Page,
                    typeof(Page),
                    "guncellemeBasarili",
                    $"alert(\"{ad} {soyad} başarıyla güncellendi.\\nEski: {old_dep} ({old_rol})\\nYeni: {new_dep} ({new_rol})\");",
                    true
                );
            }

        private static void LoadDataFromCsv()
        {
            calisanlar.Clear();
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');

                // Eksik sütun varsa atla
                if (parts.Length < 6) continue;

                string key = parts[0];
                string[] value = new string[5];
                for (int i = 0; i < 5; i++)
                {
                    value[i] = parts[i + 1].Trim(); // boşsa da "" olur
                }

                calisanlar.AddOrUpdate(key, value);
            }
        }


        private static void SaveToCsv()
        {
            var path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            var lines = new List<string> { "ID;Departman;Ad;Soyad;Ünvan;UstID" };

            var all = calisanlar.GetAll()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Önce genel yönetici (ID: 99) yazılır
            if (all.TryGetValue("99", out string[] genelYonetici))
            {
                lines.Add($"99;{genelYonetici[0]};{genelYonetici[1]};{genelYonetici[2]};{genelYonetici[3]};{genelYonetici[4]}");
            }

            // Yöneticileri bul
            var yoneticiler = all
                .Where(kvp => kvp.Value[3] == "Yönetici" && kvp.Key != "99")
                .OrderBy(kvp => int.Parse(kvp.Key)) // ID'ye göre sırala
                .ToList();

            foreach (var yonetici in yoneticiler)
            {
                string yoneticiId = yonetici.Key;
                string[] val = yonetici.Value;

                // Yöneticiyi yaz
                lines.Add($"{yoneticiId};{val[0]};{val[1]};{val[2]};{val[3]};{val[4]}");

                // O yöneticinin çalışanlarını bul ve yaz
                var calisanlarAltinda = all
                    .Where(x => x.Value[4] == yoneticiId && x.Value[3] != "Yönetici")
                    .OrderBy(x => int.Parse(x.Key));

                foreach (var c in calisanlarAltinda)
                {
                    var v = c.Value;
                    lines.Add($"{c.Key};{v[0]};{v[1]};{v[2]};{v[3]};{v[4]}");
                }
            }

            File.WriteAllLines(path, lines, System.Text.Encoding.UTF8);
        }




        private static string Normalize(string input) =>
                input?.Trim().ToLower(new System.Globalization.CultureInfo("tr-TR", false));
        }
    }



