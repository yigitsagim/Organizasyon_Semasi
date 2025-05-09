using System;
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
            panelEkle.Visible = false;
            panelSil.Visible = false;
            panelGuncelle.Visible = false;
            panelAra.Visible = false;
            panelDepartman.Visible = false;
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelEkle.Visible = true;
            SemaYenileIfExists();
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            Gizle();
            panelSil.Visible = true;
            SemaYenileIfExists();
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelGuncelle.Visible = true;
            SemaYenileIfExists();
        }

        protected void btnAra_Click(object sender, EventArgs e)
        {
            Gizle();
            panelAra.Visible = true;
            SemaYenileIfExists();
        }

        protected void btnDepartman_Click(object sender, EventArgs e)
        {
            Gizle();
            panelDepartman.Visible = true;
            SemaYenileIfExists();
        }

        protected void btnGuncelleCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.GuncelAd = txtGuncelAd.Text.Trim();
            FormVerileri0.GuncelSoyad = txtGuncelSoyad.Text.Trim();
            FormVerileri0.GuncelYeniDepartman = ddlGuncelleNewDep.SelectedValue;
            FormVerileri0.GuncelYeniGorev = ddlGuncelleGorev.SelectedValue;
            GuncelleCalisan.Guncelle();
            SemaYenileIfExists();
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            FormVerileri0.EkleAd = txtEkleAd.Text.Trim();
            FormVerileri0.EkleSoyad = txtekleSoyad.Text.Trim();
            FormVerileri0.EkleDepartman = ddlDepartman.SelectedValue;
            FormVerileri0.EkleGorev = ddlEkleGorev.SelectedValue;
            EkleCalisan.Ekle();
            SemaYenileIfExists();
        }

        protected void btnSilCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.SilAd = txtSilAd.Text.Trim();
            FormVerileri0.SilSoyad = txtSilSoyad.Text.Trim();
            SilCalisan.Sil();
            SemaYenileIfExists();
        }

        protected void btnAraCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.AramaAdSoyad = txtAra.Text.Trim();
            

            if (string.IsNullOrWhiteSpace(FormVerileri0.AramaAdSoyad) || !FormVerileri0.AramaAdSoyad.Contains(" "))
            {
                lblBilgi.Text = "Lütfen 'Ad Soyad' şeklinde girin.";
                return;
            }

            string[] parcalar = FormVerileri0.AramaAdSoyad.Split(' ');
            string ad = parcalar[0];
            string soyad = parcalar[1];

            

            AraCalisan arama = new AraCalisan();
            string sonuc = arama.Bul(ad, soyad);
            lblBilgi.Text = sonuc;
            SemaYenileIfExists();
        }

        protected void btnDepartmanEkle_Click(object sender, EventArgs e)
        {
            FormVerileri0.YeniDepartman = txtYeniDepartman.Text.Trim();
            DepartmanIslemleri islemler = new DepartmanIslemleri();
            islemler.DepEkle();
            SemaYenileIfExists();
        }

        private void SemaYenileIfExists()
        {
            try
            {
                // Eğer aktif sayfa "Default" gibi şema çizen bir sayfaysa onun SemaYenile metodunu çağır
                Page.GetType().InvokeMember("SemaYenile",
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public,
                    null, Page, null);
            }
            catch
            {
                // sayfada SemaYenile() yoksa sessizce geç
            }
        }
    }
}
