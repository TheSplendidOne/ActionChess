# ActionChess
### Chess based real-time multiplayer strategy
This game is a rethinking of classic chess.
Two players fight on a regular chessboard using pieces moving according to the classical rules.

The main feature - in the game there isn't turn order.
White doesn't go first and black doesn't go second.
Players can make any number of moves between opponent moves.

Pieces don't move instantly.
After giving the order, they move at a speed of 1 cell per second.
At the moment the movement ends, the figure becomes inactive for 12 seconds.
At this time it cannot be moved, so it becomes vulnerable.

Pieces may collide while moving.
When a moving piece collides with a immovable one, the immovable piece is captured, and the moving one takes its place.
When two moving pieces collide, the piece that started moving later is captured.
It becomes possible to defend your piece from an already moving enemy piece, leading it out of combat, or putting another figure in the way.

There is no check in the game.
The game ends when one of the kings is captured.

These rules make ActionChess a game completely different from classic chess.

### Implemented Features
- Sign Up / Sign In
- Game search
- Playing up to two victories
- View statistics in a profile
- Ability to view replay of all games

### About implementation
The project is based on microservice architecture.
There is a client with a graphical interfsace running on Windows.
There are three services communicating via TCP on the server side:
- Game Service,
- Authentication Service,
- Database Access Service.

Game service is an intermediate node between clients during the game.
It creates a game party, processes requests for the movement of pieces, tracks collisions of pieces, turns pawns into queens, records the game’s progress in JSON and notifies players of the end of the game.
The authentication service processes sign up and sign in requests and supports active sessions.
Database Access Service has exclusive access to the database, provides data to the clients and other services.

### Technology used
- C# 8
- .Net Framework 4.8
- Windows Presetation Foundation (WPF)
- Windows Communication Foundation (WCF)
- Microsoft SQL Server
- Entity Framework

### Screenshots
<img src="https://a.radikal.ru/a03/2102/dc/11c6e6927fc7.png" width="600">
<img src="https://b.radikal.ru/b10/2102/c9/dcc8e9e57fdd.png" width="600">
<img src="https://a.radikal.ru/a17/2102/72/fa5069c154f9.png" width="600">
<img src="https://d.radikal.ru/d23/2102/6f/fb04c338cfa9.png" width="600">
<img src="https://c.radikal.ru/c11/2102/e4/0eb7db5e91c9.png" width="600">
<img src="https://c.radikal.ru/c41/2102/db/c7f74368d5e2.png" width="600">

### Overview
<a href="https://youtu.be/OZS5Vog1Log">Gameplay video</a><br>
<a href="https://youtu.be/P3OiQbvXd0Q">Main menu</a>
