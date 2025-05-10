using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public class CalisanNode
    {
        public string ID;
        public string Departman;
        public string Ad;
        public string Soyad;
        public string Unvan;
        public string ParentID;
        public CalisanNode Next;

        public CalisanNode(string id, string dep, string ad, string soyad, string unvan, string parentId)
        {
            ID = id;
            Departman = dep;
            Ad = ad;
            Soyad = soyad;
            Unvan = unvan;
            ParentID = parentId;
            Next = null;
        }
    }

    public class CalisanHashTable
    {
        private int size = 100;
        private CalisanNode[] table;

        public CalisanHashTable()
        {
            table = new CalisanNode[size];
        }

        private int Hash(string id)
        {
            return Math.Abs(id.GetHashCode()) % size;
        }

        public void Ekle(CalisanNode node)
        {
            int index = Hash(node.ID);
            if (table[index] == null)
            {
                table[index] = node;
            }
            else
            {
                CalisanNode current = table[index];
                while (current.Next != null)
                    current = current.Next;
                current.Next = node;
            }
        }

        public CalisanNode Get(string id)
        {
            int index = Hash(id);
            CalisanNode current = table[index];
            while (current != null)
            {
                if (current.ID == id)
                    return current;
                current = current.Next;
            }
            return null;
        }

        public void Remove(string id)
        {
            int index = Hash(id);
            CalisanNode current = table[index];
            CalisanNode prev = null;

            while (current != null)
            {
                if (current.ID == id)
                {
                    if (prev == null)
                        table[index] = current.Next;
                    else
                        prev.Next = current.Next;
                    return;
                }
                prev = current;
                current = current.Next;
            }
        }

        public List<CalisanNode> TumCalisanlariGetir()
        {
            var list = new List<CalisanNode>();
            foreach (var head in table)
            {
                CalisanNode current = head;
                while (current != null)
                {
                    list.Add(current);
                    current = current.Next;
                }
            }
            return list;
        }

        public bool YoneticiVarMi(string departman)
        {
            return TumCalisanlariGetir().Any(n =>
                n.Departman == departman &&
                n.Unvan == "Yönetici" &&
                !string.IsNullOrWhiteSpace(n.Ad) &&
                !string.IsNullOrWhiteSpace(n.Soyad));
        }

        public CalisanNode GetYonetici(string departman)
        {
            return TumCalisanlariGetir().FirstOrDefault(n =>
                n.Departman == departman && n.Unvan == "Yönetici");
        }

        public void BosYoneticileriSil(string departman)
        {
            var bosYoneticiler = TumCalisanlariGetir()
                .Where(n => n.Departman == departman && n.Unvan == "Yönetici" &&
                            string.IsNullOrWhiteSpace(n.Ad) && string.IsNullOrWhiteSpace(n.Soyad))
                .Select(n => n.ID)
                .ToList();

            foreach (var id in bosYoneticiler)
                Remove(id);
        }

        public void Clear()
        {
            for (int i = 0; i < size; i++)
                table[i] = null;
        }
    }

    public static class DepartmanYardimci
    {
        private static string DataFileVirtual = "~/App_Data/departments.csv";

        public static string GetPrefix(string departmanAdi)
        {
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return "0";

            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 2 &&
                    parts[1].Trim().ToLower() == departmanAdi.Trim().ToLower())
                {
                    return parts[0];
                }
            }

            return "0";
        }


        public static int GetDepartmanSirasi(string departmanAdi)
        {
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (!File.Exists(path)) return 99;

            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 2 && parts[1].Trim() == departmanAdi && int.TryParse(parts[0], out int sira))
                    return sira;
            }

            return 99;
        }

        public static List<string> GetirDepartmanAdlari()
        {
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            var list = new List<string>();

            if (!File.Exists(path)) return list;

            foreach (var line in File.ReadAllLines(path).Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[1]))
                    list.Add(parts[1].Trim());
            }

            return list;
        }
    }

 
        public static class EkleCalisan
        {
            private static string DataFileVirtual = "~/App_Data/calisanlar2.csv";
            public static NodeHashTable calisanlar = new NodeHashTable();

            private static string GetPrefix(string departmanAdi)
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/departmanlar.csv");
                if (!File.Exists(path)) return "-1";

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
                            return csvIdStr;
                    }
                }

                return "-1";
            }

            public static void Ekle()
            {
                LoadFromCsv();

                string ad = FormVerileri0.EkleAd.Trim();
                string soyad = FormVerileri0.EkleSoyad.Trim();
                string departman = FormVerileri0.EkleDepartman.Trim();
                string unvan = FormVerileri0.EkleGorev.Trim();

                string prefix = GetPrefix(departman);
                if (!int.TryParse(prefix, out int pre) || pre == 0)
                {
                    ShowAlert("Departman için geçerli bir ID prefix bulunamadı. Lütfen departmanlar.csv dosyasını kontrol edin.");
                    return;
                }

                int baseId = pre * 100;
                string yeni_id;
                string parent_id;

                if (unvan == "Yönetici")
                {
                    bool mevcutYoneticiVar = calisanlar.Where(x =>
                        x.Value[0] == departman &&
                        x.Value[3] == "Yönetici" &&
                        !string.IsNullOrWhiteSpace(x.Value[1]) &&
                        !string.IsNullOrWhiteSpace(x.Value[2])).Any();

                    if (mevcutYoneticiVar)
                    {
                        ShowAlert("Bu departmanda zaten bir yönetici var. Yeni yönetici eklenemez.");
                        return;
                    }

                    calisanlar.RemoveAll(x =>
                        x.Value[0] == departman && x.Value[3] == "Yönetici" &&
                        string.IsNullOrWhiteSpace(x.Value[1]) && string.IsNullOrWhiteSpace(x.Value[2]));

                    yeni_id = baseId.ToString();
                    parent_id = "99";
                    calisanlar.AddOrUpdate(yeni_id, new[] { departman, ad, soyad, unvan, parent_id });
                }
                else
                {
                    var yonetici = calisanlar.Where(x => x.Value[0] == departman && x.Value[3] == "Yönetici").FirstOrDefault();
                    parent_id = yonetici.Key ?? "";

                    var mevcutIdler = calisanlar.GetAll()
                        .Where(k => k.Key.StartsWith((baseId).ToString()))
                        .Select(k => int.Parse(k.Key));

                    int candidate = mevcutIdler.Any() ? mevcutIdler.Max() + 1 : baseId + 1;
                    while (calisanlar.ContainsKey(candidate.ToString()))
                        candidate++;

                    yeni_id = candidate.ToString();
                    calisanlar.AddOrUpdate(yeni_id, new[] { departman, ad, soyad, unvan, parent_id });
                }

                SaveToCsv();
                ShowAlert($"Eklendi: {ad} {soyad} → ID: {yeni_id} / {departman} / {unvan}");
            }

            private static void LoadFromCsv()
            {
                calisanlar.Clear();
                string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
                if (!File.Exists(path)) return;

                foreach (var line in File.ReadAllLines(path).Skip(1))
                {
                    var parts = line.Split(';');
                    if (parts.Length != 6) continue;
                    string key = parts[0];
                    string[] value = new[] { parts[1], parts[2], parts[3], parts[4], parts[5] };
                    calisanlar.AddOrUpdate(key, value);
                }
            }

            private static void SaveToCsv()
            {
                string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
                var lines = new List<string> { "ID;Departman;Ad;Soyad;Ünvan;ParentID" };

                foreach (var kvp in calisanlar.GetAll())
                {
                    lines.Add($"{kvp.Key};{kvp.Value[0]};{kvp.Value[1]};{kvp.Value[2]};{kvp.Value[3]};{kvp.Value[4]}");
                }

                File.WriteAllLines(path, lines);
            }

            private static string Normalize(string input) =>
                input?.Trim().ToLower(new System.Globalization.CultureInfo("tr-TR", false));

            private static void ShowAlert(string message)
            {
                if (HttpContext.Current.CurrentHandler is Page page)
                {
                    page.ClientScript.RegisterStartupScript(
                        page.GetType(),
                        "alertMsg",
                        $"alert('{message}');",
                        true
                    );
                }
            }
        }
    }

