using BattleShip123Core.Interfaces;
using BattleShip123Core.Model;
using BattleShip123Engine.Models;
using BattleShip123View;

var playerOne = new Player("Player one");
var playerTwo = new Player("Player two");
IView view = new View();
IEngine engine = new Engine(view, playerOne, playerTwo);

engine.StartGame();