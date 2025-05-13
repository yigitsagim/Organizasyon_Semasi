using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace veri_yapilari
{
    public partial class Default : System.Web.UI.Page
    {
        class Kisi
        {
            public string ID, Ad, Soyad, Unvan, Departman, UstID;
            public string AdSoyad => $"{Ad} {Soyad}";
        }

        Dictionary<string, List<Kisi>> agac = new Dictionary<string, List<Kisi>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SemaYenile();
            }
        }

        public void SemaYenile()
        {
            string yol = Server.MapPath("~/App_Data/calisanlar2.csv");
            var satirlar = File.ReadAllLines(yol).Skip(1);
            var kisiler = satirlar
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

            agac.Clear();

            foreach (var kisi in kisiler)
            {
                string ust = string.IsNullOrEmpty(kisi.UstID) ? "root" : kisi.UstID;
                if (!agac.ContainsKey(ust))
                    agac[ust] = new List<Kisi>();
                agac[ust].Add(kisi);
            }

            // 🔍 Kontrol: veri var mı?
            if (!kisiler.Any())
            {
                litSema.Text = "<li><div class='kutu'>Hiç çalışan bulunamadı</div></li>";
                return;
            }

            if (!agac.ContainsKey("root") || agac["root"].Count == 0)
            {
                litSema.Text = "<li><div class='kutu'>Root altında hiç kişi yok</div></li>";
                return;
            }

            // ✔️ Şema çizimi
            litSema.Text = CizAgac("root");
        }


        private string CizAgac(string ustID)
        {
            if (!agac.ContainsKey(ustID)) return "";

            var sb = new StringBuilder();
            foreach (var kisi in agac[ustID])
            {
                sb.Append("<li>");
                sb.Append($"<div class='kutu'>{kisi.AdSoyad}<br /><small>{kisi.Unvan} - {kisi.Departman}</small></div>");
                string altlar = CizAgac(kisi.ID);
                if (!string.IsNullOrEmpty(altlar))
                {
                    sb.Append("<ul class='tree'>");
                    sb.Append(altlar);
                    sb.Append("</ul>");
                }
                sb.Append("</li>");
            }
            return sb.ToString();
        }
    }
}
