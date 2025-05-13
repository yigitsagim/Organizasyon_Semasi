
# 🏢 Organizasyon Şeması Yönetim Sistemi

Bu proje, farklı departmanlara ait çalışanların hiyerarşik olarak görüntülendiği bir **organizasyon şeması yönetim sistemidir**. Web Forms (.NET Framework) kullanılarak geliştirilmiştir ve veri yapıları (ağaç, hash, dictionary vb.) ile desteklenmiştir.

---

## 📌 Özellikler

- ✅ Çalışan ekleme, silme, güncelleme, arama
- ✅ Departman ekleme ve departman ağacı gösterimi
- ✅ CSV dosyaları üzerinden veri yönetimi
- ✅ Hiyerarşik ağaç yapısıyla organizasyon görselleştirme
- ✅ Her çalışan için görev ve ID bilgisi takibi

---

## 🛠️ Kurulum

### Gerekli:
- Visual Studio 2019/2022
- .NET Framework 4.7.2 veya üstü

### Adımlar:
1. Bu repoyu klonlayın:
   ```bash
   git clone https://github.com/yigitsagim/Organizasyon_Semasi.git
   ```
2. `veri_yapilari.sln` dosyasını Visual Studio ile açın.
3. Gerekirse `.NET Framework` yüklü değilse Visual Studio Installer ile ekleyin.
4. Projeyi çalıştırın (F5).

---

## 📁 Dosya Yapısı

```
Organizasyon_Semasi/
├── veri_yapilari.sln
├── veri_yapilari/
│   ├── veri_yapilari.csproj
│   ├── kodlarim/
│   ├── App_Data/
│   └── WebForm1.aspx, Site.master, ...
```

---

## 🧠 Kullanılan Veri Yapıları

- ✅ Dictionary (sözlük) ile çalışan bilgileri
- ✅ HashTable ile hızlı erişim
- ✅ Tree yapısı ile organizasyon şeması
- ✅ List<T> ve diğer koleksiyon sınıfları

---

## 🧪 Test & Demo

Her bir form ekranı Site.master üzerinden erişilir. Tüm formlar aynı layout üzerinden dinamik olarak açılır.

---

## 👥 Katkı

Katkıda bulunmak isterseniz:
1. Fork alın
2. Yeni bir branch oluşturun (`feature/xyz`)
3. Değişikliklerinizi yapın
4. Pull Request gönderin

---

## 📄 Lisans

MIT Lisansı altında dağıtılmaktadır.
