# GameP
Bu uygulama, online çok oyunculu bir casino altyapısı oluşturmanın yanında içinde birden çok oyun tipini barındırabilecek şekilde tasarlanmıştır.
Geliştirme aşamasında olan bu oyunun kodlarını okurken scriptlerdeki yorumlar'ı takip ETMEMENİZİ öneririm. Şuan için sadece kendime not olması amacıyla yazılmışlardır.
Uygulamanın hiçbir noktasında kullanıcı tarafında önemli bir işlem yapılmamıştır. Tüm işlemler server tarafında hesaplanmış ve kullanıcılara sadece bu hesapların sonucu gönderilmiştir. Kullanıcıdan alınınan veriler de kontrol edilmeden kullanılmamıştır.
Dökümantasyon için One Note ve processlerin diagramları için Miro teknolojilerinden faydalanılmıştır.
----Network Altyapısı---
Unity'nin Mirror Framework'ü üle sağlanmıştır. 
Bununla beraber ileride başka bir network altyapısının kullanılabilme ihtimali göz önünde bulundurularak mirror'un sağladığı gelişmiş özellikler nadiren kullanılmıştır.
----Poker Hand Evaluator--- 
Poker oyununda kazanan eli belirleyen algoritma CactusDev'in Poker Hand Evaluator algoritmasıdır. C#'a uyumlu hale getirilip kullanılmıştır.
----Login Sistemi---
Ağ üzerinde yollanacak kullanıcı bilgilerinin şifrelenmesinde hangi yöntemi kullanacağıma karar vermediğim için şuan için cypher-decypher algortimalarının gövdeleri boştur.
Loginler ve logoutlar sorunsuzca Database'e kayıt ediliyor.
Oyunu kullanıcının nasıl kapattığından bağımsız şekilde logout kayıt işlemi gerçekleşiyor.
----User Tipleri---
-Admin
-Operator
-Player
----Oyun Oluşturma---
Uygulama aktif edildiğinde aktif bir oyun bulunmaz. Admin bir masa yaratma işlemi gerçekleştirir. Burada Masa ve üstünde oynanacak oyun birbirinden bağımsız tasarlanmıştır. Şuan için sadece OmanaEngine bulunuyor lakin ilerde oyun sayısında artış olursa admin bu masalarda o oyunları da oynatabilir.
Oyun oluşturulduğunda masa için 
TableGameManager oluşturulur. Bu eleman oyunun geri kalanı ile masa arasındaki bağlantıyı sağlar ve oyunu yönetmek için 5 eleman yaratır. 
1-)TablePlayerManager 
2-)GamePlayerManager
3-)GameUISpawnManager
4-)GamePlayerSpawnManager
5-)OmahaEngine
Omaha engine, omaha oyununa ait tüm işlemleri gerçekleştirir. İleride yeni bir oyun eklenmesi için sadece bu eleman'ın değiştirilmesi yetecektir.
Omaha Engine oyunu yönetmek için 3 elemana sahiptir. 
1-) TurnManager
2-) ChipManager
3-) Dealer
Oyun oluşturma işlemi işlemi yapan admin'in adı ile birlikte database'e kayıt edilir.
---Kullanıcı Oluşturma---
Admin tarafından başlangıç bakiyesi ile birlikte yaratılıyorlar.
Kullanıcıların rakeBack oranı dinamiktir.
Kullanıcıya birisi referans olmuş ise referans olan kişinin rakeBack oranı da dinamiktir. UserCreation aşamasında belirtilirler.
Kullanıcı oluşturma işlemi işlemi yapan admin'in adı ile birlikte database'e kayıt edilir.
---Bakiye Ekleme---(şuan mevcut değil)
---Bakiye Çekme---(şuan mevcut değil)
---Masaya'a giriş---
Kullanıcı masaya giriş yaptığında seyirci ekranından masayı izler taki bir sandalyeye oturana kadar. 
---Oyuna Giriş---
Minimum giriş limitinden yüksek herhangi bir miktar ile oyuna dahil olunabilir.
Oyuna yatırılan para kullanıcı bakiyesinden düşülür. Ve bu işlem database'e kayıt edilir.
---Oyunun Başlaması---
2 oyuncu masada oturur hale geldiği an oyunculara kağıtlar dağıtılır ve oyun başlar.
PotLimit Omaha kuralları dahilinde turlar oynanır ve turun sonunda tura ait tüm bilgiler kayıt edilir.
Süresi dahilinde karar vermeyen oyuncular için en uygun süre bitiminde verilir.
Kendi turn'ünde olmayan oyuncular preMove yapabilirler
Yeterli bakiyesi olan en az 2 oyuncu olması dahilinde turlar birbiri ardına dağıtılmaya devam edilir.
---Stop Deal---(şuan mevcut değil (Admin Yetkisinde))
---Oyundan ayrılma---
Ayrılma işlemi bakiye'nin bitmesi, kullanıcı isteğiyle yahut kullanıcının düşmesi ile olabilir. 
Bu işlem sırasında kullanıcının kalan bakiyesi ve oyunda elde ettiği rakeBack miktarı database'e yazılır.
Oyundan ayrılan kullanıcı izleyici ekranına geri döner
---Masadan ayrılma---
Güncel lobby'ye geçiş yapılır.
---Kick Player From Table---(şuan mevcut değil)
---Kick Player From Application--- (şuan mevcut değil)
---Dismiss Table--- (şuan mevcut değil)
---Show Casino Earnings--- (şuan mevcut değil)

İsteiğiniz doğrultusunda detaylı dökümantasyon için OneNote paylaşım linki ve Miro'da projeye ekleme isteği yollayabilirim.
