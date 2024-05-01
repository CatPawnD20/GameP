!!!Proje tamamlanmıştır.!!!<br>
Demo Video -> https://youtu.be/QXpK1UOJWVM<br>
Bu repository, projenin geliştirilme aşamasındandır, güncel  değildir!!!<br>
# GameP
Bu uygulama, online çok oyunculu bir casino altyapısı oluşturmanın yanında içinde farklı oyun tiplerini barındırabilecek birden çok oyun odası olan bir yapıda  tasarlanmıştır.

Geliştirme aşamasında yazdığım ve pek çok script'in başında bulunan açıklamalar size o element hakkında bir fikir verebilir. Lakin güncel olmayan pek çoğuna rast gelebilirsiniz.

Uygulamanın hiçbir noktasında kullanıcı tarafında önemli bir işlem yapılmamıştır. Tüm işlemler server tarafında hesaplanmış ve kullanıcılara sadece bu hesapların sonucu gönderilmiştir. Kullanıcıdan alınınan veriler kontrol edilmeden kullanılmamıştır.<br>
Dökümantasyon için OneNote ve süreçlerin UML diagramları için Miro kullanılmıştır.<br>
Geliştirme sürecinin iyiliği adına kolay yahut ekstra işlemler sonraya bırakılmıştır.<br>

---Scene Listesi---<br>
OfflineScene, loginScene, lobbyScene ve tableScene olmak üzere 4 scene içermektedir.<br>

---Player.cs---<br>
Tüm Network işlemlerini içinde bulabilirsiniz.<br>

--ISeatList.cs---<br>
Bir masada poker oyununa oturan kişileri ve kişilere ait oyun verilerini tutmak amacı ile seatList yapısı temel olarak kullanılmış. Bir çok işlem için iskelet olan bu yapının tüm işlemleri ISeatList.cs çatısı altında bulabilirsiniz.<br>

----Network Altyapısı---<br>
Unity'nin Mirror Framework'ü üle sağlanmıştır. <br>
Bununla beraber ileride başka bir network altyapısının kullanılabilme ihtimali göz önünde bulundurularak mirror'un sağladığı gelişmiş özellikler nadiren kullanılmıştır.<br>

----Poker Hand Evaluator--- <br>
Poker oyununda kazanan eli belirleyen algoritma CactusDev'in Poker Hand Evaluator algoritmasıdır. C#'a uyumlu hale getirilip kullanılmıştır.<br>

----Login Sistemi---<br>
Ağ üzerinde yollanacak kullanıcı bilgilerinin şifrelenmesinde hangi yöntemi kullanacağıma karar vermediğim için şuan için cypher-decypher algortimalarının gövdeleri boştur.<br>
Loginler ve logoutlar sorunsuzca Database'e kayıt ediliyor.<br>
Oyunu kullanıcının nasıl kapattığından bağımsız şekilde logout kayıt işlemi gerçekleşiyor.<br>

---Online Kullanıcıların Yönetilmesi---<br>
VisitorArea elemanı applicationda online olan oyuncuların yönetilmesinden ve kullanıcı listesinden sorumludur.

---Kullanıcı Oluşturma---<br>
Admin tarafından başlangıç bakiyesi ile birlikte yaratılıyorlar.<br>
Kullanıcıların rakeBack oranı dinamiktir.<br>
Kullanıcıya birisi referans olmuş ise referans olan kişinin rakeBack oranı da dinamiktir. UserCreation aşamasında belirtilirler.<br>
Kullanıcı oluşturma işlemi işlemi yapan admin'in adı ile birlikte database'e kayıt edilir.<br>

---Bakiye Ekleme---<br>
---Bakiye Çekme---<br>

----User Tipleri---<br>
-Admin<br>
-Operator<br>
-Player<br>

----Oyun Oluşturma---<br>
Uygulama aktif edildiğinde aktif bir oyun bulunmaz. Admin bir masa yaratma işlemi gerçekleştirirmek istediğinde;<br>
SpawnManager bu işlemi gerçekleştirir. Oluşturulan masa TableManager elemanına bildirilir ve tüm masa yönetim işlemleri bu eleman tarafından yapılır.<br>
Burada Masa ve üstünde oynanacak oyun birbirinden bağımsız tasarlanmıştır.<br>

Şuan için sadece OmanaEngine bulunuyor lakin ilerde oyun sayısında artış olursa admin bu masalarda o oyunları da oynatabilir.<br>
Oyun yaratıldığında masanın yönetimi için TableGameManager oluşturulur. <br>
Bu eleman oyunun geri kalanı ile masa arasındaki bağlantıyı sağlar ve oyunu yönetmek için 5 eleman yaratır. <br>
1-)TablePlayerManager <br>
2-)GamePlayerManager<br>
3-)GameUISpawnManager<br>
4-)GamePlayerSpawnManager<br>
5-)OmahaEngine<br>
Omaha engine, omaha oyununa ait tüm işlemleri gerçekleştirir. İleride yeni bir oyun eklenmesi için sadece bu eleman'ın değiştirilmesi yetecektir.<br>
Omaha Engine oyunu yönetmek için 3 elemana sahiptir. <br>
1-) TurnManager<br>
2-) ChipManager<br>
3-) Dealer<br>
Oyun oluşturma işlemi işlemi yapan admin'in adı ile birlikte database'e kayıt edilir.<br>

---Masaya'a giriş---<br>
Kullanıcı masaya giriş yaptığında seyirci ekranından masayı izler taki bir sandalyeye oturana kadar. <br>

---Oyuna Giriş---<br>
Minimum giriş limitinden yüksek herhangi bir miktar ile oyuna dahil olunabilir.<br>
Oyuna yatırılan para kullanıcı bakiyesinden düşülür. Ve bu işlem database'e kayıt edilir.<br>

---Oyunun Başlaması---<br>
2 oyuncu masada oturur hale geldiği an oyunculara kağıtlar dağıtılır ve oyun başlar.<br>
PotLimit Omaha kuralları dahilinde turlar oynanır ve turun sonunda tura ait tüm bilgiler kayıt edilir.<br>
Süresi dahilinde karar vermeyen oyuncular için en uygun süre bitiminde verilir.<br>
Kendi turn'ünde olmayan oyuncular preMove yapabilirler.<br>
Yeterli bakiyesi olan en az 2 oyuncu olması dahilinde turlar birbiri ardına dağıtılmaya devam edilir.<br>

---Oyundan ayrılma---<br>
Ayrılma işlemi bakiye'nin bitmesi, kullanıcı isteğiyle yahut kullanıcının düşmesi ile olabilir. <br>
Bu işlem sırasında kullanıcının kalan bakiyesi ve oyunda elde ettiği rakeBack miktarı database'e yazılır.<br>
Oyundan ayrılan kullanıcı izleyici ekranına geri döner.<br>

---Masadan ayrılma---<br>
Güncel lobby'ye geçiş yapılır.<br>

---Stop Deal---(Admin Yetkisinde)<br>
---Dismiss Table---(Admin Yetkisinde)<br>

---Show Casino Earnings--- (Admin Yetkisinde)<br>

İsteiğiniz doğrultusunda detaylı dökümantasyon için OneNote paylaşım linki ve Miro'da projeye ekleme isteği yollayabilirim.<br>
