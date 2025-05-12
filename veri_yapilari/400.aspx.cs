using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace veri_yapilari
{
    public partial class _400 : System.Web.UI.Page
    {
        class Kisi
        {
            public string ID, Ad, Soyad, Unvan, DepartmanID, Departman, UstID;
            public string AdSoyad => $"{Ad} {Soyad}";
        }

        Dictionary<string, string> departmanMap = new Dictionary<string, string>();
        Dictionary<string, List<Kisi>> agac = new Dictionary<string, List<Kisi>>();
        List<Kisi> tumKisiler;

        protected void Page_Load(object sender, EventArgs e)
        {
            
                DepartmanlariYukle();
                string hedefDepartmanId = "400";
                string departmanAdi = departmanMap.ContainsKey(hedefDepartmanId) ? departmanMap[hedefDepartmanId] : "Bilinmeyen";

                Page.Title = departmanAdi + " Departmanı Organizasyon Şeması";
                litBaslik.Text = departmanAdi + " Departmanı Organizasyon Şeması";

                SemaYenile(hedefDepartmanId);
            
        }

        private void DepartmanlariYukle()
        {
            string depYol = Server.MapPath("~/App_Data/departmanlar.csv");
            var satirlar = File.ReadAllLines(depYol).Skip(1);
            foreach (var satir in satirlar)
            {
                var parca = satir.Split(';');
                if (parca.Length >= 2)
                {
                    string id = parca[0].Trim();
                    string ad = parca[1].Trim();
                    departmanMap[id] = ad;
                }
            }
        }

        public void SemaYenile(string hedefDepID)
        {
            string yol = Server.MapPath("~/App_Data/calisanlar2.csv");
            tumKisiler = File.ReadAllLines(yol).Skip(1)
                .Select(l => l.Split(';'))
                .Where(p => p.Length >= 6)
                .Select(p => new Kisi
                {
                    ID = p[0].Trim(),
                    DepartmanID = p[1].Trim(),
                    Departman = departmanMap.ContainsKey(p[1].Trim()) ? departmanMap[p[1].Trim()] : p[1].Trim(),
                    Ad = p[2].Trim(),
                    Soyad = p[3].Trim(),
                    Unvan = p[4].Trim(),
                    UstID = p[5].Trim()
                }).ToList();

            string hedefDepartmanAdi = departmanMap.ContainsKey(hedefDepID) ? departmanMap[hedefDepID] : "Bilinmeyen";

            var aktifAgac = new List<Kisi>();
            var eklendi = new HashSet<string>();

            var yonetici = tumKisiler.FirstOrDefault(k => k.Unvan == "Yönetici" && k.Departman == hedefDepartmanAdi);
            if (yonetici != null)
            {
                TakipEtZincir(aktifAgac, eklendi, yonetici);
                var altlar = tumKisiler.Where(k => k.UstID == yonetici.ID);
                foreach (var alt in altlar)
                {
                    TakipEtZincir(aktifAgac, eklendi, alt);
                }
            }

            var genel = tumKisiler.FirstOrDefault(k => k.ID == "99");
            if (genel != null)
            {
                TakipEtZincir(aktifAgac, eklendi, genel);
            }

            agac.Clear();
            foreach (var kisi in aktifAgac)
            {
                string ust = string.IsNullOrEmpty(kisi.UstID) ? "root" : kisi.UstID;
                if (!agac.ContainsKey(ust))
                    agac[ust] = new List<Kisi>();
                agac[ust].Add(kisi);
            }

            var sb = new StringBuilder();
            sb.Append("<ul class='tree'>");

            if (agac.ContainsKey("99"))
            {
                sb.Append(CizAgac("99"));
            }
            else
            {
                var kok = aktifAgac.FirstOrDefault(k => string.IsNullOrEmpty(k.UstID));
                if (kok != null)
                    sb.Append(CizAgac(kok.ID));
            }

            sb.Append("</ul>");
            litIKSema.Text = sb.ToString();
        }

        private void TakipEtZincir(List<Kisi> hedefListe, HashSet<string> eklendi, Kisi kisi)
        {
            if (string.IsNullOrEmpty(kisi.ID) || kisi.ID == kisi.UstID)
                return;

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
