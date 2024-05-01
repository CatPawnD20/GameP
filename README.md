!!!The project has been completed.!!!<br>
Demo Video -> https://youtu.be/QXpK1UOJWVM<br>
This repository is from the development phase of the project and is not up to date.!!!<br>
# GameP
In addition to creating an online multi-player casino infrastructure, this application has been designed in a structure with multiple game rooms that can accommodate different game types.<br>

The explanations I wrote during the development phase and at the beginning of many scripts can give you an idea about that element. However, you may come across many that are out of date.<br>

No significant "user side" action was taken at any point in the application. All operations are calculated "server side" and only the results of these calculations are sent to users. The data received from the user was not used without checking.<br>
OneNote was used for documentation and Miro was used for UML diagrams of processes..<br>

---Scene Lists---<br>
It contains 4 scenes: offlineScene, loginScene, lobbyScene and tableScene.<br>

---Player.cs---<br>
You can find all Network transactions here.<br>

--ISeatList.cs---<br>
The SeatList structure is basically used to protect the people sitting at the table and the playing queues in the poker game. You can find all the processes of these processes, which are the skeleton of many processes, under the umbrella of ISeatList.cs<br>

----Network Infrastructure---<br>
It is provided by Unity's Mirror Framework. <br>
However, considering the possibility of using another network infrastructure in the future, the advanced features provided by the mirror were rarely used.<br>

----Poker Hand Evaluator--- <br>
The algorithm that determines the winning hand in the poker game is CactusDev's Poker Hand Evaluator algorithm. It was adapted and used in C#.<br>

----Login System---<br>
User information to be sent over the network is encrypted<br>
Logins and logouts are recorded to the Database without any problems.<br>
Logout recording takes place regardless of how the user closes the game.<br>

---Managing Online Users---<br>
The Visitor Area element is responsible for managing the players who are online in the application and the user list.<br>

---Kullanıcı Oluşturma---<br>
Users' rakeBack rate is dynamic.<br>
If someone has referred the user, the rakeBack rate of the referred person is also dynamic. They are specified during the UserCreation phase.<br>
The user creation process is recorded in the database along with the name of the admin who created the user.<br>

---User Deposit---<br>
---User Withdrawal---<br>

----User Types---<br>
-Admin<br>
-Operator<br> (Not Active)
-Player<br>

----Table Creation---<br>
When the application is activated, there is no active table. When the admin wants to create a table;<br>
SpawnManager performs this operation. The created table is reported to the TableManager element and all table management operations are performed by this element.<br>
Here, the table and the game to be played on it are designed independently of each other.<br>

Currently, only OmanaEngine is available, but if the number of games increases in the future, the admin can also play those games on these tables.<br>
When the game is created, TableGameManager is created to manage the table. <br>
This element provides the connection between the rest of the game and the table and creates 5 elements to manage the game. <br>
1-)TablePlayerManager <br>
2-)GamePlayerManager<br>
3-)GameUISpawnManager<br>
4-)GamePlayerSpawnManager<br>
5-)OmahaEngine<br>
Omaha engine performs all operations of the Omaha game. To add a new game in the future, just changing this element will be enough.<br>
Omaha Engine has 3 elements to manage the game. <br>
1-) TurnManager<br>
2-) ChipManager<br>
3-) Dealer<br>
It is recorded in the database with the name of the admin who created the table.<br>

---Join Table---<br>
When the user enters the table, he watches the table from the audience screen until he sits on a chair. <br>

---Join Game---<br>
You can join the game with any amount higher than the minimum entry limit.<br>
The money deposited into the game is deducted from the user balance. And this transaction is recorded in the database.<br>

---Turn Start---<br>
As soon as 2 players sit at the table, cards are distributed to the players and the game begins.<br>
Rounds are played within PotLimit Omaha rules and all information about the round is recorded at the end of the round.<br>
For players who do not make a decision within the time limit, the most appropriate decision is made at the end of the time period. And these players are tagged as afk. They are removed from the table and switched to the spectator screen before the next turn<br>
Players can move in advance via preMoves<br>
If there are at least 2 players with sufficient funds, the rounds continue to be distributed one after the other.<br>

---Leave Game---<br>
LeaveGame may occur if the balance runs out, the user makes a request, or the user leaves the application. <br>
During this process, the user's remaining balance and the rakeBack amount obtained in the game are written to the database.<br>
The user who leaves the game returns to the viewer screen.<br>

---Leave Table---<br>
The user returns to the lobby screen.<br>

---Stop Deal---(Under Admin Authority)<br>
---Dismiss Table---(Under Admin Authority)<br>

---Show Casino Earnings--- (Under Admin Authority)<br>

İsteiğiniz doğrultusunda detaylı dökümantasyon için OneNote paylaşım linki ve Miro'da projeye ekleme isteği yollayabilirim.<br>
