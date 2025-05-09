using System;
using System.Collections.Generic;
using System.IO;
using System.Web;


namespace veri_yapilari.kodlarim
{
    public static class CsvOkuyucu
    {
        public static List<Calisan> CsvdenCalisanlariOku()
        {
            string dosyaYolu = HttpContext.Current.Server.MapPath("~/App_Data/calisanlar2.csv");
            List<Calisan> calisanlar = new List<Calisan>();

            if (!File.Exists(dosyaYolu))
                return calisanlar;

            string[] satirlar = File.ReadAllLines(dosyaYolu);
            for (int i = 1; i < satirlar.Length; i++) // Başlık satırını atla
            {
                string[] parcalar = satirlar[i].Split(';');
                if (parcalar.Length == 6)
                {
                    calisanlar.Add(new Calisan
                    {
                        ID = int.Parse(parcalar[0]),
                        Departman = parcalar[1],
                        Ad = parcalar[2],
                        Soyad = parcalar[3],
                        Unvan = parcalar[4],
                        ParentID = string.IsNullOrWhiteSpace(parcalar[5]) ? (int?)null : int.Parse(parcalar[5])
                    });
                }
            }

            return calisanlar;
        }
    


    }
}



