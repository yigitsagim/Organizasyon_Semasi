using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace veri_yapilari.kodlarim
{
    public static class SilCalisan
    {
        private const string DataFileVirtual = "~/App_Data/calisanlar2.csv";

        /// <summary>
        /// Silme işlemini yapar. 
        /// out multipleDepartments: eğer birden fazla departman varsa doldurulur.
        /// </summary>
        public static void Sil(out List<string> multipleDepartments)
        {
            multipleDepartments = null;

            string ad = FormVerileri0.SilAd?.Trim() ?? "";
            string soyad = FormVerileri0.SilSoyad?.Trim() ?? "";
            string dept = FormVerileri0.SilDepartman?.Trim() ?? "";

            // 1) CSV’den oku
            var rows = new List<string[]>();
            var table = new Hashtable();
            string path = HttpContext.Current.Server.MapPath(DataFileVirtual);
            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path, Encoding.UTF8).Skip(1))
                {
                    var parts = line.Split(';');
                    if (parts.Length != 6) continue;
                    rows.Add(parts);
                    string key = $"{parts[1].Trim().ToLower()}_{parts[2].Trim().ToLower()}_{parts[3].Trim().ToLower()}";
                    table[key] = parts;
                }
            }

            // 2) Eşleşmeleri bul
            IEnumerable<string[]> matched;
            if (!string.IsNullOrEmpty(dept))
            {
                matched = rows.Where(p =>
                    string.Equals(p[1].Trim(), dept, StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(p[2].Trim(), ad, StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(p[3].Trim(), soyad, StringComparison.CurrentCultureIgnoreCase)
                );
            }
            else
            {
                matched = rows.Where(p =>
                    string.Equals(p[2].Trim(), ad, StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(p[3].Trim(), soyad, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            var matches = matched.ToList();
            if (matches.Count == 0)
            {
                ShowAlert("Çalışan bulunamadı! Lütfen bilgileri kontrol edin.");
                return;
            }

            // 3) Birden fazla departman kontrolü
            var distinctDeps = matches.Select(p => p[1].Trim()).Distinct().ToList();
            if (distinctDeps.Count > 1 && string.IsNullOrEmpty(dept))
            {
                // Kullanıcıya seçim yaptırmak üzere listeyi döndür
                multipleDepartments = distinctDeps;
                return;
            }

            // 4) Silinecek kaydı seç
            var found = matches[0];
            string lookupKey = $"{found[1].Trim().ToLower()}_{found[2].Trim().ToLower()}_{found[3].Trim().ToLower()}";
            table.Remove(lookupKey);

            // 5) CSV’yi yeniden yaz (ID’ye göre sıralı olacak şekilde)
            var updatedRows = new List<string>();
            updatedRows.Add("ID;Departman;Ad;Soyad;Ünvan;ParentID");

            // Listeyi ID’ye göre sırala
            var sortedRows = table.Values
                .Cast<string[]>()
                .OrderBy(p => int.TryParse(p[0], out int id) ? id : int.MaxValue)
                .ToList();

            foreach (var p in sortedRows)
            {
                updatedRows.Add($"{p[0]};{p[1]};{p[2]};{p[3]};{p[4]};{p[5]}");
            }

            File.WriteAllLines(path, updatedRows, Encoding.UTF8);


            // 6) Başarı mesajı
            ShowAlert($"Silindi: {found[1].Trim()} - {found[2].Trim()} {found[3].Trim()}");
        }

        private static void ShowAlert(string msg)
        {
            if (HttpContext.Current.CurrentHandler is Page page)
            {
                string script = $"alert('{msg.Replace("'", "\\'")}');";
                page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(), script, true);
            }
        }
    }
}
