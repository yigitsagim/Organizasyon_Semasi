using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace veri_yapilari
{
    public partial class BT : System.Web.UI.Page
    {
        class Kisi
        {
            public string ID, Ad, Soyad, Unvan, Departman, UstID;
            public string AdSoyad => $"{Ad} {Soyad}";
        }

        Dictionary<string, List<Kisi>> agac = new Dictionary<string, List<Kisi>>();
        List<Kisi> tumKisiler;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string yol = Server.MapPath("~/App_Data/calisanlar2.csv");

                // CSV'den tüm çalışanları oku
                tumKisiler = File.ReadAllLines(yol).Skip(1)
                    .Select(l => l.Split(';'))
                    .Where(p => p.Length >= 6)
                    .Select(p => new Kisi
                    {
                        ID = p[0].Trim(),
                        Departman = p[1].Trim(),
                        Ad = p[2].Trim(),
                        Soyad = p[3].Trim(),
                        Unvan = p[4].Trim(),
                        UstID = p[5].Trim()
                    }).ToList();

                var btAgac = new List<Kisi>();
                var eklendi = new HashSet<string>();

                // BT departmanından başlayarak üstleriyle birlikte zinciri kur
                foreach (var kisi in tumKisiler.Where(k => k.Departman == "BT"))
                {
                    TakipEtZincir(btAgac, eklendi, kisi);
                }

                // Genel Yönetici'yi ve onun zincirini de mutlaka dahil et
                var genel = tumKisiler.FirstOrDefault(k => k.ID == "99");
                if (genel != null)
                    TakipEtZincir(btAgac, eklendi, genel);

                // Ağaç yapısını oluştur
                agac.Clear();
                foreach (var kisi in btAgac)
                {
                    string ust = string.IsNullOrEmpty(kisi.UstID) ? "root" : kisi.UstID;
                    if (!agac.ContainsKey(ust))
                        agac[ust] = new List<Kisi>();
                    agac[ust].Add(kisi);
                }

                // Ağaç HTML'ini üret
                var sb = new StringBuilder();
                sb.Append("<ul class='tree'>");

                if (agac.ContainsKey("99"))
                {
                    sb.Append(CizAgac("99")); // Genel Yönetici kök
                }
                else
                {
                    var kok = btAgac.FirstOrDefault(k => string.IsNullOrEmpty(k.UstID));
                    if (kok != null)
                        sb.Append(CizAgac(kok.ID));
                }

                sb.Append("</ul>");
                Session["btSemaHTML"] = sb.ToString();
            }
        }

        private void TakipEtZincir(List<Kisi> hedefListe, HashSet<string> eklendi, Kisi kisi)
        {
            if (!eklendi.Contains(kisi.ID))
            {
                hedefListe.Add(kisi);
                eklendi.Add(kisi.ID);
            }

            if (!string.IsNullOrEmpty(kisi.UstID))
            {
                var parent = tumKisiler.FirstOrDefault(p => p.ID == kisi.UstID);
                if (parent != null)
                    TakipEtZincir(hedefListe, eklendi, parent);
            }
        }

        private string CizAgac(string ustID)
        {
            if (!agac.ContainsKey(ustID)) return "";

            var sb = new StringBuilder();
            foreach (var kisi in agac[ustID])
            {
                sb.Append("<li>");
                sb.Append($"<div class='kutu'>{kisi.AdSoyad}<br /><small>{kisi.Unvan} - {kisi.Departman}</small></div>");
                string alt = CizAgac(kisi.ID);
                if (!string.IsNullOrEmpty(alt))
                {
                    sb.Append("<ul class='tree'>");
                    sb.Append(alt);
                    sb.Append("</ul>");
                }
                sb.Append("</li>");
            }
            return sb.ToString();
        }
    }
}
