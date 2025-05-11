# Kurumsal Organizasyon Şeması Oluşturucu

## Proje Kapsamı ve Amacı

Bu proje, bir kurumun organizasyonel yapısını yazılım aracılığıyla modellemeyi ve yönetmeyi amaçlamaktadır. Proje kapsamında, departmanlara bağlı çalışanlar hiyerarşik bir yapı ile temsil edilir. Kullanıcılar, çalışan ve departman ekleme, silme, güncelleme ve arama gibi işlemleri gerçekleştirebilir. Ayrıca sistem, her çalışanın ait olduğu departmanla olan ilişkisini ve yönetici-alt çalışan bağını ağaç yapısı ile görselleştirir.

Proje tamamen C# diliyle, ASP.NET Web Forms teknolojisi kullanılarak geliştirilmiştir. Veritabanı yerine CSV tabanlı dosya sistemi tercih edilmiştir. Veri işlemleri, Hash Table, Linked List, Tree ve List veri yapıları kullanılarak gerçekleştirilmiştir. Tüm kaynak kodlar ve geliştirici katkıları, açık bir GitHub deposunda ortak bir “main” branch’ı ve grup üyelerinin kendi gelişmelerini eklediği özel branch’lar üzerinden yürütülmüştür.

---

## Grup Üyeleri ve Görev Dağılımı

| Grup Üyesi           | Sorumlu Olduğu Görev        |
|----------------------|------------------------------|
| Ferhat Çelik         | Çalışan Silme                |
| Yiğit Sağım          | Departman Ekleme             |
| Abdullah Eren Korkmaz| Çalışan Arama                |
| Berkan Kocaman       | Çalışan Ekleme               |
| Berra Akman          | Çalışan Güncelleme           |

---

## Görev İşleyişi ve Analizleri

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

---

### 2. Departman Ekleme – Yiğit Sağım

**Teknik Süreç:**
- Kullanıcıdan departman adı alınır.
- Departman ID’si sistem tarafından belirlenir.
- Yeni `DepartmanTreeNode` oluşturularak mevcut ağaç yapısına eklenir.
- Yeni departman bilgileri `departmanlar.csv` dosyasına yazılır.
- Organizasyon yapısı yeniden kurulur.

**Kullanılan Veri Yapıları:**
- Tree: Hiyerarşik departman yapısı
- Linked List: Departman listesi üzerinde gezinme

**Algoritma Analizi:**
- Yeni ID üretimi: O(d), d = departman sayısı
- Ağaç güncelleme ve yerleştirme: O(d)
- Dosyaya kayıt işlemi: O(1)

---

### 3. Çalışan Arama – Abdullah Eren Korkmaz

**Teknik Süreç:**
- Arama kutusuna girilen ad/soyad alınır.
- Hash tablosundaki tüm kayıtlar gezilerek eşleşme kontrolü yapılır.
- Eşleşen kayıtlar kullanıcıya listelenir.

**Kullanılan Veri Yapıları:**
- Hash Table: Tüm kayıtları dolaşma
- List: Geçici arama sonucu listesi

**Algoritma Analizi:**
- Kayıtlar üzerinde tam tarama: O(n)
- Listeleme: O(k), k = eşleşen kayıt sayısı
- Toplam karmaşıklık: O(n + k)

---

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

---

### 5. Çalışan Güncelleme – Berra Akman

**Teknik Süreç:**
- Güncellenecek çalışanın ID’si alınır.
- Hash tablosunda kayıt bulunur ve yeni bilgilerle güncellenir.
- Departman değişmişse eski ID silinerek yeni ID atanır.
- Yönetici değişikliği yapılırsa bağlı çalışanların `ParentID` değerleri güncellenir.
- Güncel bilgiler `calisanlar2.csv` dosyasına kaydedilir.

**Kullanılan Veri Yapıları:**
- Hash Table: Hızlı erişim ve güncelleme
- Tree: Yönetici ilişkilerinin korunması

**Algoritma Analizi:**
- Hash tablosunda arama ve güncelleme: O(1)
- Departman değişiminde silme/ekleme: O(1) + O(n)
- Yönetici güncellemesi: O(k), k = bağlı çalışan sayısı
- CSV yeniden yazımı: O(n)

---

## Kullanılan Veri Yapıları ve Gerekçeleri

**Hash Table**
- Çalışanlara benzersiz ID’lerle hızlı erişim sağlar.
- Ortalama erişim, ekleme ve silme süresi: O(1)

**Linked List**
- Hash tablosunda çakışmaları zincirleme ile çözmek için kullanılır.
- Departmanlar gibi sıralı ve değişken yapılarda da kullanılır.
- Ekleme/silme: O(1)

**Tree**
- Organizasyon yapısının (yönetici-alt çalışan) hiyerarşik temsilinde kullanılır.
- Ağaç üzerinde gezinme ve görselleştirme: O(n)

**List**
- Arama işlemleri ve geçici veri saklama için kullanılır.
- Dolaşım: O(n)

**Queue**
- Breadth-First Search algoritmasında düğümleri geçici olarak tutmak için kullanılır.
- Ekleme/Silme: O(1)

---

## Genel Proje Algoritma ve Yapı Analizi

Sistem genelinde veri yapıları, gerçekleştirilmek istenen işlevin performans ve sürdürülebilirlik gereksinimlerine göre seçilmiştir. Özellikle Tree veri yapısı, organizasyonun doğal yapısını modellemek için kullanılmış; Hash Table yapısı ise hızlı erişim, ekleme ve silme operasyonlarını mümkün kılmıştır. Arama işlemlerinde Tree üzerinde BFS kullanılması, isim tabanlı sorgulamalarda sistemin hiyerarşik bütünlüğünü koruyarak arama yapılmasını sağlamıştır.

Her işlem sonrası sistemin durumunun kalıcı olarak CSV dosyalarına yazılması, veri güvenliğini artırırken; bu işlemin zaman karmaşıklığı proje gereksinimleri dahilinde kabul edilebilir düzeyde kalmıştır. 

Sonuç olarak bu yazılım, veri yapıları ve algoritmaların entegre biçimde kullanıldığı, dağıtılabilir, sürdürülebilir ve analizlenebilir nitelikte bir çözüm sunmaktadır.
