using System;
using System.Collections.Generic;
using System.Web.UI;
using veri_yapilari.kodlarim;

namespace veri_yapilari
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Gizle();
        }

        private void Gizle()
        {
            foreach (var pnl in new[] { panelEkle, panelSil, panelGuncelle, panelAra, panelDepartman })
            {
                pnl.Visible = false;
                pnl.CssClass = "operationPanel";
            }
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelEkle.Visible = true;
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            Gizle();
            panelSil.Visible = true;
            panelSil.CssClass = "operationPanel active";
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelGuncelle.Visible = true;
        }

        protected void btnAra_Click(object sender, EventArgs e)
        {
            Gizle();
            panelAra.Visible = true;
        }

        protected void btnDepartman_Click(object sender, EventArgs e)
        {
            Gizle();
            panelDepartman.Visible = true;
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            FormVerileri0.EkleAd = txtEkleAd.Text.Trim();
            FormVerileri0.EkleSoyad = txtekleSoyad.Text.Trim();
            FormVerileri0.EkleDepartman = ddlDepartman.SelectedValue;
            FormVerileri0.EkleGorev = ddlEkleGorev.SelectedValue;
            EkleCalisan.Ekle();

            // sayfa yenilenirse ekleme paneli kapansın
            Response.Redirect(Request.RawUrl);
        }

        protected void btnGuncelleCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.GuncelAd = txtGuncelAd.Text.Trim();
            FormVerileri0.GuncelSoyad = txtGuncelSoyad.Text.Trim();
            FormVerileri0.GuncelYeniDepartman = ddlGuncelleNewDep.SelectedValue;
            FormVerileri0.GuncelYeniGorev = ddlGuncelleGorev.SelectedValue;
            GuncelleCalisan.Guncelle();

            // güncelleme sonrası da diyagramı yenile
            Response.Redirect(Request.RawUrl);
        }

        protected void btnSilCalisan_Click(object sender, EventArgs e)
        {
            // form değerlerini al
            FormVerileri0.SilAd = txtSilAd.Text.Trim();
            FormVerileri0.SilSoyad = txtSilSoyad.Text.Trim();
            FormVerileri0.SilDepartman = txtSilDepartman.Text.Trim();

            // silme işlemini yap
            List<string> multipleDeps;
            SilCalisan.Sil(out multipleDeps);

            // eğer birden fazla departman uyarısı geldiyse dropdown göster
            if (multipleDeps != null && multipleDeps.Count > 1)
            {
                lblSilInfo.Text = "Lütfen departman seçin:";
                ddlSilDepartmanSec.DataSource = multipleDeps;
                ddlSilDepartmanSec.DataBind();
                ddlSilDepartmanSec.Visible = true;
            }
            else
            {
                // silme tamamlandıktan sonra sayfayı yeniden yükle
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void btnAraCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.AramaAdSoyad = txtAra.Text.Trim();
            if (string.IsNullOrWhiteSpace(FormVerileri0.AramaAdSoyad)
                || !FormVerileri0.AramaAdSoyad.Contains(" "))
            {
                lblBilgi.Text = "Lütfen 'Ad Soyad' şeklinde girin.";
                return;
            }
            var parts = FormVerileri0.AramaAdSoyad.Split(' ');
            AraCalisan arama = new AraCalisan();
            lblBilgi.Text = arama.Bul(parts[0], parts[1]);
        }

        protected void btnDepartmanEkle_Click(object sender, EventArgs e)
        {
            FormVerileri0.YeniDepartman = txtYeniDepartman.Text.Trim();
            DepartmanIslemleri.DepEkle();

            // departman ekledikten sonra de yenile
            Response.Redirect(Request.RawUrl);
        }
    }
}
