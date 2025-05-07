using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace veri_yapilari.kodlarim
{
    public class DepartmanIslemleri
    {
        private string dosyaYolu;
        public List<Departman> Departmanlar { get; private set; }

        public DepartmanIslemleri()
        {
            dosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/departments.csv");
            Departmanlar = new List<Departman>();
            DosyadanYukle();
        }

        public void DepartmanEkle(int id, string ad, int? ustId)
        {
            if (Departmanlar.Any(d => d.Id == id))
                return;

            Departmanlar.Add(new Departman { Id = id, Ad = ad, UstId = ustId });
            DosyayaKaydet();
        }

        public void DepartmanSil(int id)
        {
            AltDepartmanlariSil(id);
            Departmanlar.RemoveAll(d => d.Id == id);
            DosyayaKaydet();
        }

        private void AltDepartmanlariSil(int ustId)
        {
            var altlar = Departmanlar.Where(d => d.UstId == ustId).ToList();
            foreach (var alt in altlar)
            {
                AltDepartmanlariSil(alt.Id);
                Departmanlar.Remove(alt);
            }
        }

        private void DosyayaKaydet()
        {
            var satirlar = Departmanlar.Select(d => $"{d.Id},{d.Ad},{(d.UstId.HasValue ? d.UstId.ToString() : "")}");
            File.WriteAllLines(dosyaYolu, satirlar);
        }

        private void DosyadanYukle()
        {
            if (!File.Exists(dosyaYolu))
                return;

            var satirlar = File.ReadAllLines(dosyaYolu);
            foreach (var satir in satirlar)
            {
                var parcalar = satir.Split(',');
                int id = int.Parse(parcalar[0]);
                string ad = parcalar[1];
                int? ustId = string.IsNullOrWhiteSpace(parcalar[2]) ? (int?)null : int.Parse(parcalar[2]);

                Departmanlar.Add(new Departman
                {
                    Id = id,
                    Ad = ad,
                    UstId = ustId
                });
            }
        }
    }
}
