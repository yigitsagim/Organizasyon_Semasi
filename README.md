# Kurumsal Organizasyon Şeması Oluşturucu

## Proje Kapsamı ve Amacı

Bu proje, bir kurumun organizasyonel yapısını yazılım aracılığıyla modellemeyi ve yönetmeyi amaçlamaktadır. Proje kapsamında, departmanlara bağlı çalışanlar hiyerarşik bir yapı ile temsil edilir. Kullanıcılar, çalışan ve departman ekleme, silme, güncelleme ve arama gibi işlemleri gerçekleştirebilir. Ayrıca sistem, her çalışanın ait olduğu departmanla olan ilişkisini ve yönetici-alt çalışan bağını ağaç yapısı ile görselleştirir.

Proje tamamen C# diliyle, ASP.NET Web Forms teknolojisi kullanılarak geliştirilmiştir. Veritabanı yerine CSV tabanlı dosya sistemi tercih edilmiştir. Veri işlemleri, Hash Table, Linked List, Tree ve List veri yapıları kullanılarak gerçekleştirilmiştir. Tüm kaynak kodlar ve geliştirici katkıları, açık bir GitHub deposunda ortak bir “main” branch ve grup üyelerinin bireysel branch'ları üzerinden yürütülmüştür.

---

## Grup Üyeleri ve Görev Dağılımı

| Grup Üyesi            | Sorumlu Olduğu Görev        |
|------------------------|------------------------------|
| Ferhat Çelik           | Çalışan Silme                |
| Yiğit Sağım            | Departman Ekleme             |
| Abdullah Eren Korkmaz  | Çalışan Arama                |
| Berkan Kocaman         | Çalışan Ekleme               |
| Berra Akman            | Çalışan Güncelleme           |

---

## Görev İşleyişi ve Analizleri

### 1. Çalışan Silme – Ferhat Çelik

**Teknik Süreç:**
- Çalışanın ID’si alınır.
- Hash tablosunda kayıt bulunur ve silinir.
- Çalışan bir yönetici ise, ilgili departmanın durumu kontrol edilir.
- Güncel çalışan listesi `calisanlar2.csv` dosyasına yazılır.
- Organizasyon şeması güncellenir.

**Kullanılan Veri Yapıları:**
- Hash Table
- Linked List
- Tree

**Algoritma Analizi:**
- Silme: O(1) ortalama, O(n) worst-case
- Yönetici kontrolü: O(k)
- CSV güncelleme ve ağaç kurma: O(n)

---

### 2. Departman Ekleme – Yiğit Sağım

**Teknik Süreç:**
- Yeni departman adı alınır.
- Sistem otomatik departman ID’si üretir.
- Departman ağaç yapısına eklenir.
- Kayıt `departmanlar.csv` dosyasına yazılır.
- Organizasyon yapısı yeniden kurulur.

**Kullanılan Veri Yapıları:**
- Tree
- Linked List

**Algoritma Analizi:**
- ID üretimi: O(d)
- Ağaç güncelleme: O(d)
- Dosya yazma: O(1)

---

### 3. Çalışan Arama – Abdullah Eren Korkmaz

**Teknik Süreç:**
- Arama kutusundan ad/soyad bilgisi alınır.
- Ağaç yapısı kurulup BFS algoritması uygulanır.
- Eşleşme bulunduğunda detay gösterilir.

**Kullanılan Veri Yapıları:**
- Tree
- Queue

**Algoritma Analizi:**
- BFS tarama: O(n)
- Bellek kullanımı: O(w), w = maksimum seviye genişliği

---

### 4. Çalışan Ekleme – Berkan Kocaman

**Teknik Süreç:**
- Formdan alınan bilgilerle ID üretilir.
- İlk çalışansa yönetici atanır.
- Hash tablosuna çalışan eklenir.
- `calisanlar2.csv` güncellenir.
- Şema yeniden kurulur.

**Kullanılan Veri Yapıları:**
- Hash Table
- Linked List
- Tree

**Algoritma Analizi:**
- ID üretimi: O(n)
- Ekleme: O(1) ortalama, O(n) worst-case
- Ağaç kurma: O(n)

---

### 5. Çalışan Güncelleme – Berra Akman

**Teknik Süreç:**
- Güncellenmek istenen çalışanın ID’si bulunur.
- Gerekli bilgiler değiştirilir.
- Departman değişirse eski kayıt silinir, yeni ID ile eklenir.
- Yönetici değişmişse bağlı ilişkiler güncellenir.
- CSV dosyası yeniden yazılır.

**Kullanılan Veri Yapıları:**
- Hash Table
- Tree

**Algoritma Analizi:**
- Arama ve güncelleme: O(1)
- Departman değişimi: O(n)
- Bağlı çalışan güncellemesi: O(k)
- CSV işlemi: O(n)

---

## Kullanılan Veri Yapıları ve Gerekçeleri

**Hash Table**
- Çalışanlara benzersiz ID’lerle hızlı erişim sağlar.
- Sabit diziye göre önemli hız avantajı sunar.
- Ortalama erişim, ekleme ve silme süresi: **O(1)**

**Linked List**
- Hash tablosunda çakışmaları çözmek için zincirleme yapılarda kullanılır.
- Aynı zamanda departmanlar gibi değişken boyutlu yapılar için uygundur.
- Ekleme ve silme işlemleri: **O(1)**

**Tree**
- Organizasyon yapısının doğal temsili.
- Parent-child ilişkisi sayesinde gezinme ve şema kurma kolaylığı sağlar.
- Tüm düğümler üzerinde gezinme: **O(n)**

**List**
- Arama ve geçici veri depolama için kullanılır.
- Sıralı dolaşım: **O(n)**

**Queue**
- Ağaç üzerinde BFS ile arama yapılırken geçici veri tutma amacıyla kullanılır.
- Ekleme/Silme: **O(1)**

---

## Veri Yapısı Seçimi ve Alternatiflerle Karşılaştırma

### Hash Table vs. Sabit Dizi
- Sabit dizide ID’ye göre erişim O(n) iken hash ile O(1).
- Binlerce çalışanda bile anında erişim sağlanır.

### Linked List vs. Dizi
- Silme ve ekleme işlemlerinde dizide veri kaydırmak gerekirken bağlı listede sadece referans değişir.
- Çakışmaların zincirleme çözümünde etkin kullanılır.

### Tree vs. Liste
- Listeyle üst-alt ilişkisi kurmak zahmetlidir ve O(n²) karmaşıklık yaratabilir.
- Ağaç yapısıyla doğrudan ParentID -> Cocuklar ilişkisi tanımlanabilir.

### BFS ile Arama vs. Doğrudan Liste Taraması
- Tree üzerinden katman katman tarama yapılır.
- Hiyerarşik bütünlük korunur, sadece ad/soyad değil, konum bilgisi de analiz edilebilir.
 
---
## Sonuç:

Bu proje, kurumsal organizasyon yapısını hiyerarşik olarak modelleyip yönetilebilir hale getirmiştir. Kullanılan veri yapıları (Tree, Hash Table, Linked List, Queue) işlemlerin hızlı, düzenli ve kalıcı şekilde gerçekleşmesini sağlamıştır.

Veritabanı yerine dosya tabanlı yapı tercih edilerek sistem taşınabilir ve dağıtılabilir hale getirilmiştir. Tüm işlevler algoritma temelli olarak planlanmış, sistem performansı ve veri bütünlüğü ön planda tutulmuştur.

Sonuç olarak proje, veri yapılarının etkin kullanımına dayalı, sürdürülebilir ve işlevsel bir çözüm sunmaktadır.

---
## Projenin Kurulumu ve Çalıştırılması

### Gereksinimler
- Windows işletim sistemi
- Visual Studio 2019 veya üzeri
- .NET Framework 4.7.2+

### Kurulum Adımları

1. Projeyi klonlayın veya ZIP olarak indirin:                                           git clone https://github.com/yigitsagim/Organizasyon_Semasi.git
2. `veri_yapilari.sln` dosyasını Visual Studio ile açın.

3. Gerekli NuGet paketlerini kontrol edin (Web Forms projelerinde genellikle gerekmez).

4. F5 ile projeyi çalıştırın.

5. Tarayıcıda açıldığında:
- Organizasyon şeması otomatik olarak yüklenecektir.
- Menüden çalışan/departman işlemleri yapılabilir.
- Yapılan işlemler `calisanlar2.csv` ve `departmanlar.csv` dosyalarına yansır.

