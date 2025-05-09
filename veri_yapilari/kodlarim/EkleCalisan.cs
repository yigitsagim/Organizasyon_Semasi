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

                var node = new CalisanNode(yeni_id, departman, ad, soyad, "Yönetici", parent_id);
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

                var node = new CalisanNode(yeni_id, departman, ad, soyad, unvan, parent_id);
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

                var node = new CalisanNode(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                calisanlar.Ekle(node);
            }
        }

        private static void SaveToCsv()
        {
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            var lines = new List<string> { "ID;Departman;Ad;Soyad;Ünvan;ParentID" };

            var siraliCalisanlar = calisanlar.TumCalisanlariGetir()
                .OrderBy(x => GetDepartmanSirasi(x.Departman))
                .ThenBy(x => x.Unvan != "Yönetici")
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
                case "Genel": return "9";
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
