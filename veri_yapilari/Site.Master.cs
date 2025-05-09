using System;
using System.Web.UI;
using veri_yapilari;
using veri_yapilari.kodlarim; //hata veren kod



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
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            Gizle();
            panelSil.Visible = true;
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

        protected void btnGuncelleCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.GuncelAd = txtGuncelAd.Text.Trim();
            FormVerileri0.GuncelSoyad = txtGuncelSoyad.Text.Trim();
            FormVerileri0.GuncelYeniDepartman = ddlGuncelleNewDep.SelectedValue;
            FormVerileri0.GuncelYeniGorev = ddlGuncelleGorev.SelectedValue;
            GuncelleCalisan.Guncelle();
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            FormVerileri0.EkleAd = txtEkleAd.Text.Trim();
            FormVerileri0.EkleSoyad = txtekleSoyad.Text.Trim();
            FormVerileri0.EkleDepartman = ddlDepartman.SelectedValue;
            FormVerileri0.EkleGorev = ddlEkleGorev.SelectedValue;
            EkleCalisan.Ekle();
        }

        protected void btnSilCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.SilAd = txtSilAd.Text.Trim();
            FormVerileri0.SilSoyad = txtSilSoyad.Text.Trim();
            SilCalisan.Sil();
        }

       
            protected void btnAraCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.AramaAdSoyad = txtAra.Text.Trim();

            // Boş kontrolü
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
        }



        protected void btnDepartmanEkle_Click(object sender, EventArgs e)
        {
            var islem = new DepartmanIslemleri();
            islem.DepEkle();

            // Gerekirse ağacı veya dropdown'ı yenileyin:
            // BindTree();  
            // BindDropdown();
        }
    }
}
