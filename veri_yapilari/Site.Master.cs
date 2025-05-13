using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using veri_yapilari.kodlarim;

namespace veri_yapilari
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public class Departman
        {
            public int Id { get; set; }
            public string DepartmanAdi { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DinamikNavbarOlustur();
            if (!IsPostBack)
            {
                Gizle();
                BindDepartments();
               
            }
            SemaYenileIfExists();
        }

        private void DinamikNavbarOlustur()
        {
            string csvYol = Server.MapPath("~/App_Data/departmanlar.csv");
            if (!File.Exists(csvYol)) return;

            var satirlar = File.ReadAllLines(csvYol).Skip(2);
            var sb = new StringBuilder();

            foreach (var satir in satirlar)
            {
                var parca = satir.Split(';');
                if (parca.Length >= 2)
                {
                    string id = parca[0].Trim();
                    string ad = parca[1].Trim();
                    sb.Append($"<li class='nav-item'><a class='nav-link' href='{id}.aspx'>{ad}</a></li>");
                }
            }

            phNavbarLinks.Controls.Clear();
            phNavbarLinks.Controls.Add(new Literal { Text = sb.ToString() });
        }

        private void Gizle()
        {
            panelEkle.Visible = false;
            panelSil.Visible = false;
            panelGuncelle.Visible = false;
            panelAra.Visible = false;
            panelDepartman.Visible = false;
        }

        private void BindDepartments()
        {
            string csvFilePath = Server.MapPath("~/App_Data/departmanlar.csv");
            List<Departman> departmanList = File.ReadAllLines(csvFilePath)
                .Skip(2)
                .Select(l => l.Split(';'))
                .Where(p => p.Length >= 2)
                .Select(p => new Departman
                {
                    Id = int.Parse(p[0].Trim()),
                    DepartmanAdi = p[1].Trim()
                }).ToList();

            ddlDepartman.DataSource = departmanList;
            ddlDepartman.DataTextField = "DepartmanAdi";
            ddlDepartman.DataValueField = "Id";
            ddlDepartman.DataBind();

            ddlGuncelleNewDep.DataSource = departmanList;
            ddlGuncelleNewDep.DataTextField = "DepartmanAdi";
            ddlGuncelleNewDep.DataValueField = "Id";
            ddlGuncelleNewDep.DataBind();
        }

        private void SemaYenileIfExists()
        {
            try
            {
                string sayfaAdi = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                string hedefDepartmanId = new string(sayfaAdi.Where(char.IsDigit).ToArray());

                if (!string.IsNullOrEmpty(hedefDepartmanId))
                {
                    Page.GetType().InvokeMember("SemaYenile",
                        System.Reflection.BindingFlags.InvokeMethod |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public,
                        null, Page, new object[] { hedefDepartmanId });
                }
            }
            catch { }
        }



        protected void BtnEkle_Click(object sender, EventArgs e) { Gizle(); 
            panelEkle.Visible = true; BindDepartments(); SemaYenileIfExists(); }
        protected void BtnSil_Click(object sender, EventArgs e) { Gizle(); panelSil.Visible = true; BindDepartments(); SemaYenileIfExists(); }
        protected void BtnGuncelle_Click(object sender, EventArgs e) { Gizle(); panelGuncelle.Visible = true; BindDepartments(); SemaYenileIfExists(); }
        protected void BtnAra_Click(object sender, EventArgs e) { Gizle(); panelAra.Visible = true; BindDepartments(); SemaYenileIfExists(); }
        protected void BtnDepartman_Click(object sender, EventArgs e) { Gizle(); panelDepartman.Visible = true; BindDepartments(); SemaYenileIfExists(); }
        protected void BtnKaydet_Click(object sender, EventArgs e)
        {
            FormVerileri0.EkleAd = txtEkleAd.Text.Trim();
            FormVerileri0.EkleSoyad = txtEkleSoyad.Text.Trim();
            FormVerileri0.EkleDepartman = ddlDepartman.SelectedItem.Text;
            FormVerileri0.EkleGorev = ddlEkleGorev.SelectedValue;
            EkleCalisan.Ekle();
            SemaYenileIfExists();
        

        }
        protected void BtnGuncelleCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.GuncelAd = txtGuncelAd.Text.Trim();
            FormVerileri0.GuncelSoyad = txtGuncelSoyad.Text.Trim();
            FormVerileri0.GuncelYeniDepartman = ddlGuncelleNewDep.SelectedItem.Text;
            FormVerileri0.GuncelYeniGorev = ddlGuncelleGorev.SelectedValue;
            GuncelleCalisan.Guncelle();
            SemaYenileIfExists();
           

        }
        protected void btnAraCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.AramaAdSoyad = txtAra.Text.Trim();
            if (!FormVerileri0.AramaAdSoyad.Contains(" "))
            {
                lblBilgi.Text = "Lütfen 'Ad Soyad' şeklinde girin.";
                return;
            }
            var parcala = FormVerileri0.AramaAdSoyad.Split(' ');
            string ad = parcala[0];
            string soyad = parcala[1];
            AraCalisan arama = new AraCalisan();
            string sonuc = arama.Bul(ad, soyad);
            lblBilgi.Text = sonuc;
            SemaYenileIfExists();

        }
        protected void btnSilCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.SilAd = txtSilAd.Text.Trim();
            FormVerileri0.SilSoyad = txtSilSoyad.Text.Trim();
            FormVerileri0.SilDepartman = txtSilDepartman.Text.Trim();

            List<string> multipleDeps;
            SilCalisan.Sil(out multipleDeps);

            if (multipleDeps != null && multipleDeps.Count > 1)
            {
                lblSilInfo.Text = "Lütfen departman seçin:";
                ddlSilDepartmanSec.DataSource = multipleDeps;
                ddlSilDepartmanSec.DataBind();
                ddlSilDepartmanSec.Visible = true;
            }
            else
            {
            }
            SemaYenileIfExists();

        }
        protected void BtnDepartmanEkle_Click(object sender, EventArgs e)
        {
            FormVerileri0.YeniDepartman = txtYeniDepartman.Text.Trim();
            DepartmanIslemleri islemler = new DepartmanIslemleri();
            islemler.DepEkle();
            SemaYenileIfExists();
        }
    }
}
