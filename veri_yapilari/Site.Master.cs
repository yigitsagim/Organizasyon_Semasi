using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using veri_yapilari.kodlarim;

namespace veri_yapilari
{
    public partial class Site : System.Web.UI.MasterPage
    {
        // Departman sınıfı
        public class Departman
        {
            public int Id { get; set; }
            public string DepartmanAdi { get; set; }

            // DepartmanTreeNode'ya dönüşüm fonksiyonu
            public DepartmanTreeNode ToDepartmanTreeNode(int parentId)
            {
                return new DepartmanTreeNode(Id, DepartmanAdi, "", "", "Yönetici", parentId);
            }
        }

        // DepartmanTreeNode sınıfı
        public class DepartmanTreeNode
        {
            public int Id;
            public string DepartmanAdi;
            public string Ad;
            public string Soyad;
            public string Unvan;
            public int? ParentId;
            public List<DepartmanTreeNode> Cocuklar;

            public DepartmanTreeNode(int id, string depAd, string ad, string soyad, string unvan, int? parentId)
            {
                Id = id;
                DepartmanAdi = depAd;
                Ad = ad;
                Soyad = soyad;
                Unvan = unvan;
                ParentId = parentId;
                Cocuklar = new List<DepartmanTreeNode>();
            }

            // DepartmanTreeNode'dan Departman'a dönüşüm
            public Departman ToDepartman()
            {
                return new Departman
                {
                    Id = this.Id,
                    DepartmanAdi = this.DepartmanAdi
                };
            }
        }

        // Sayfa yüklendiğinde çalışacak olan metod
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Gizle();
                // Departmanları CSV dosyasından yükle ve DropDownList'lere bindir
                BindDepartments();
            }
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
            // CSV dosyasının yolu
            string csvFilePath = Server.MapPath("~/App_Data/departmanlar.csv");
            List<Departman> departmanList = LoadDepartmentsFromCSV(csvFilePath);

            if (departmanList == null || departmanList.Count == 0)
            {
                lblBilgi.Text = "Departmanlar yüklenemedi.";
                return;
            }

            // DropDownList'lere departmanları yükle
            ddlDepartman.DataSource = departmanList;
            ddlDepartman.DataTextField = "DepartmanAdi"; // Görünen alan
            ddlDepartman.DataValueField = "Id"; // Seçilen değeri tutacak alan
            ddlDepartman.DataBind();

            // Yeni departman ekleme ekranındaki DropDownList'e de aynı şekilde departmanları yükle
            ddlGuncelleNewDep.DataSource = departmanList;
            ddlGuncelleNewDep.DataTextField = "DepartmanAdi";
            ddlGuncelleNewDep.DataValueField = "Id";
            ddlGuncelleNewDep.DataBind();
        }



        // CSV'den departmanları yükleme fonksiyonu

        private List<Departman> LoadDepartmentsFromCSV(string filePath)
        {
            List<Departman> departments = new List<Departman>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    bool isFirstLine = true;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (isFirstLine)
                        {
                            isFirstLine = false; // İlk satır başlık olduğu için atla
                            continue;
                        }

                        var values = line.Split(';'); // Veriyi ';' ile ayırıyoruz

                        if (values.Length < 2) continue; // Eğer satırda gerekli değerler yoksa geç

                        int id;
                        if (!int.TryParse(values[0], out id)) // Id'yi güvenli bir şekilde dönüştür
                        {
                            continue;
                        }

                        departments.Add(new Departman { Id = id, DepartmanAdi = values[1] }); // Departmanı ekle
                    }
                }
            }
            catch (Exception ex)
            {
                lblBilgi.Text = "Hata: " + ex.Message; // Hata mesajını ekrana yazdır
            }

            return departments;
        }



        // Çalışan ekleme, silme, güncelleme ve arama işlemleri için buton eventleri
        protected void BtnEkle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelEkle.Visible = true;
            BindDepartments();
            SemaYenileIfExists();
        }

        protected void BtnSil_Click(object sender, EventArgs e)
        {
            Gizle();
            panelSil.Visible = true;
            BindDepartments();
            SemaYenileIfExists();
        }

        protected void BtnGuncelle_Click(object sender, EventArgs e)
        {
            Gizle();
            panelGuncelle.Visible = true;
            BindDepartments();
            SemaYenileIfExists();
        }

        protected void BtnAra_Click(object sender, EventArgs e)
        {
            Gizle();
            panelAra.Visible = true;
            BindDepartments();
            SemaYenileIfExists();
        }

        protected void BtnDepartman_Click(object sender, EventArgs e)
        {
            Gizle();
            panelDepartman.Visible = true;
            BindDepartments();
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
        protected void BtnGuncelleCalisan_Click(object sender, EventArgs e)
        {
            FormVerileri0.GuncelAd = txtGuncelAd.Text.Trim();
            FormVerileri0.GuncelSoyad = txtGuncelSoyad.Text.Trim();
            FormVerileri0.GuncelYeniDepartman = ddlGuncelleNewDep.SelectedItem.Text; 
            FormVerileri0.GuncelYeniGorev = ddlGuncelleGorev.SelectedValue;
            GuncelleCalisan.Guncelle();
        }

        protected void BtnKaydet_Click(object sender, EventArgs e)
        {
            FormVerileri0.EkleAd = txtEkleAd.Text.Trim();
            FormVerileri0.EkleSoyad = txtEkleSoyad.Text.Trim();
            FormVerileri0.EkleDepartman = ddlDepartman.SelectedItem.Text;
            FormVerileri0.EkleGorev = ddlEkleGorev.SelectedValue;
            EkleCalisan.Ekle();
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
                Response.Redirect(Request.RawUrl);
            }
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

        protected void BtnDepartmanEkle_Click(object sender, EventArgs e)
        {
            FormVerileri0.YeniDepartman = txtYeniDepartman.Text.Trim();

            DepartmanIslemleri islemler = new DepartmanIslemleri();
            islemler.DepEkle();


        }

    }
}
