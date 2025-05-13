# Kurumsal Organizasyon Şeması Oluşturucu

**GitHub Link:** [Organizasyon Şeması GitHub Repo](https://github.com/yigitsagim/Organizasyon_Semasi.git)

## Proje Kapsamı ve Amacı

Bu proje, bir kurumun organizasyonel yapısını yazılım aracılığıyla modellemeyi ve yönetmeyi amaçlamaktadır. Proje içerisinde departmanlara bağlı çalışanlar hiyerarşik bir yapı ile temsil edilir. Kullanıcılar, çalışan ve departman ekleme, silme, güncelleme ve arama gibi işlemleri gerçekleştirebilir. Ayrıca sistem, her çalışanın ait olduğu departmanla olan ilişkisini ve yönetici-alt çalışan bağını ağaç yapısı ile görselleştirir.

Proje tamamen C# diliyle, ASP.NET Web Forms teknolojisi kullanılarak geliştirilmiştir. Veritabanı yerine CSV tabanlı dosya sistemi tercih edilmiştir. Veri işlemleri, Hash Table, Linked List, Tree ve List veri yapıları kullanılarak gerçekleştirilmiştir. Tüm kaynak kodlar ve geliştirici katkıları, açık bir GitHub deposunda ortak bir “main” branch’ı ve grup üyelerinin kendi gelişmelerini eklediği özel branch’lar üzerinden yürütülmüştür.

## Grup Üyeleri ve Görev Dağılımı

| Grup Üyesi | Sorumlu Olduğu Görev |
|------------|----------------------|
| Ferhat Çelik | Çalışan Silme |
| Yiğit Sağım | Departman Ekleme |
| Abdullah Eren Korkmaz | Çalışan Arama |
| Berkan Kocaman | Çalışan Ekleme |
| Berra Akman | Çalışan Güncelleme |

## Teknik Süreç ve Algoritmalar

### 1. Çalışan Silme – Ferhat Çelik

**Teknik Süreç:**
- Silinecek çalışanın ID’si alınır.
- Hash tablosunda kayıt bulunur ve silinir.
- Eğer çalışan bir yönetici ise, departman boş yöneticiyle kalır veya yeniden atanır.
- Tüm çalışan listesi `calisanlar2.csv` dosyasına yeniden yazılır.
- Organizasyon şeması güncellenir.

**Kullanılan Veri Yapıları:**
- Hash Table: Hızlı silme
- Linked List: Hash zinciri güncelleme
- Tree: Şemanın yeniden oluşturulması

**Algoritma Analizi:**
- Silme işlemi: O(1) ortalama, O(n) worst-case
- Yönetici kontrolü ve bağlantılı kişilerin durumu: O(k)
- CSV güncellemesi ve ağaç yenileme: O(n)

### 2. Departman Ekleme – Yiğit Sağım

**Teknik Süreç:**
- Kullanıcıdan departman adı alınır.
- Departman ID’si sistem tarafından belirlenir.
- Yeni DepartmanTreeNode oluşturularak mevcut ağaç yapısına eklenir.
- Yeni departman bilgileri `departmanlar.csv` dosyasına yazılır.
- Organizasyon yapısı yeniden kurulur.

**Kullanılan Veri Yapıları:**
- Tree: Hiyerarşik departman yapısı
- Linked List: Departman listesi üzerinde gezinme

**Algoritma Analizi:**
- Yeni ID üretimi: O(d), d = departman sayısı
- Ağaç güncelleme ve yerleşim: O(d)
- Dosyaya kayıt işlemi: O(1)

### 3. Çalışan Arama – Abdullah Eren Korkmaz

**Teknik Süreç:**
- Çalışan verileri CSV’den okunur ve geçici listeye alınır.
- Bu liste üzerinden organizasyon yapısı bir Tree (ağaç) olarak inşa edilir.
- BFS (Breadth-First Search) algoritması ile bu ağaç üzerinde ad ve soyad bilgisine göre tarama yapılır.
- İlk eşleşme bulunduğunda sonuç döndürülür.

**Kullanılan Veri Yapıları:**
- Tree: Çalışan nesneleri parent-child ilişkisiyle kurulan düğümlerden oluşur.
- Queue: BFS sırasında kullanılan geçici veri yapısı

**Algoritma Analizi:**
- Tree oluşturma: O(n)
- BFS ile arama: O(n)
- Bellek kullanımı (Queue): O(h) → h: maksimum ağaç seviyesi genişliği
- İlk eşleşmede durma: En iyi durumda O(1), en kötü durumda O(n)

### 4. Çalışan Ekleme – Berkan Kocaman

**Teknik Süreç:**
- Kullanıcıdan ad, soyad, unvan ve departman bilgileri alınır.
- Sistem, ilgili departmana ait çalışanlar arasında kullanılmamış bir ID üretir.
- İlk eklenen çalışansa, varsayılan olarak yönetici atanır.
- Çalışan bilgisi `CalisanNode` nesnesi olarak `CalisanHashTable` veri yapısına eklenir.
- Çalışan bilgileri `calisanlar2.csv` dosyasına yazılır.
- Organizasyon şeması yeniden oluşturulur.

**Kullanılan Veri Yapıları:**
- Hash Table: O(1) erişim ile hızlı kayıt
- Linked List: Hash çakışmalarında zincirleme yapı
- Tree: Organizasyon şemasının kurulumu

**Algoritma Analizi:**
- ID üretimi: O(n)
- Hash tablosuna ekleme: O(1) ortalama, O(n) worst-case
- Organizasyon ağacının güncellenmesi: O(n)

### 5. Çalışan Güncelleme – Berra Akman

**Teknik Süreç:**
- Güncellenecek çalışanın ID’si alınır.
- Hash tablosunda kayıt bulunur ve yeni bilgilerle güncellenir.
- Departman değişmişse eski ID silinerek yeni ID atanır.
- Yönetici değişikliği yapılırsa bağlı çalışanların ParentID değerleri güncellenir.
- Güncel bilgiler `calisanlar2.csv` dosyasına kaydedilir.

**Kullanılan Veri Yapıları:**
- Hash Table: Hızlı erişim ve güncelleme
- Tree: Yönetici ilişkilerinin korunması

**Algoritma Analizi:**
- Hash tablosunda arama ve güncelleme: O(1)
- Departman değişiminde silme/ekleme: O(1) + O(n)
- Yönetici güncellemesi: O(k), k = bağlı çalışan sayısı
- CSV yeniden yazımı: O(n)

## Kullanılan Veri Yapıları ve Gerekçeleri

### Hash Table
- Çalışanlara benzersiz ID’lerle hızlı erişim sağlar.
- Ortalama erişim, ekleme ve silme süresi: O(1)

### Linked List
- Hash tablosunda çakışmaları zincirleme ile çözmek için kullanılır.
- Departmanlar gibi sıralı ve değişken yapılarda da kullanılır.
- Ekleme/silme: O(1)

### Tree
- Organizasyon yapısının (yönetici-alt çalışan) hiyerarşik temsilinde kullanılır.
- Ağaç üzerinde gezinme ve görselleştirme: O(n)

### List
- Arama işlemleri ve geçici veri saklama için kullanılır.
- Dolaşım: O(n)

### Queue
- Breadth-First Search algoritmasında düğümleri geçici olarak tutmak için kullanılır.
- Ekleme/Silme: O(1)

## Veri Yapısı Seçimi ve Alternatiflerle Karşılaştırma

### Hash Table vs. Sabit Dizi
- Sabit dizide ID’ye göre erişim O(n) iken hash ile O(1).
- Binlerce çalışanda bile anında erişim sağlanır.

### Linked List vs. Dizi
- Silme ve ekleme işlemlerinde dizide veri kaydırmak gerekirken bağlı listede sadece referans değişir.
- Çakışmaların zincirleme çözümünde etkin kullanılır.

### Tree vs. Liste
- Listeyle üst-alt ilişkisi kurmak zahmetlidir ve O(n²) karmaşıklık yaratabilir.
- Ağaç yapısıyla doğrudan ParentID -> Çocuklar ilişkisi tanımlanabilir.

### BFS ile Arama vs. Doğrudan Liste Taraması
- Tree üzerinden katman katman tarama yapılır.
- Hiyerarşik bütünlük korunur, sadece ad/soyad değil, konum bilgisi de analiz edilebilir.

## Sonuç

Kurumsal organizasyon yapısı bu projede hiyerarşik olarak modellenip yönetilebilir hale getirilmiştir. Kullanılan veri yapıları (Tree, Hash Table, Linked List, Queue) işlemlerin hızlı, düzenli ve kalıcı şekilde gerçekleşmesini sağlamıştır. Veritabanı yerine dosya tabanlı yapı tercih edilerek sistem taşınabilir ve dağıtılabilir hale getirilmiştir. Tüm işlevler algoritma temelli olarak planlanmış, sistem performansı ve veri bütünlüğü ön planda tutulmuştur.

Proje, veri yapılarının etkin kullanımına dayalı, sürdürülebilir ve işlevsel bir çözüm sunmaktadır.

## Projenin Kurulumu ve Çalıştırılması

### Gereksinimler
- Windows işletim sistemi
- Visual Studio 2019 veya üzeri
- .NET Framework 4.7.2+

### Kurulum Adımları
1. Projeyi klonlayın veya ZIP olarak indirin:
   ```bash
   git clone https://github.com/yigitsagim/Organizasyon_Semasi.git
2.veri_yapilari.sln dosyasını Visual Studio ile açın.
3.	Gerekli NuGet paketlerini kontrol edin (Web Forms projelerinde genellikle gerekmez).
4.	F5 ile projeyi çalıştırın.
5.	Tarayıcıda açıldığında:
	•	Organizasyon şeması otomatik olarak yüklenecektir.
	•	Menüden çalışan/departman işlemleri yapılabilir.