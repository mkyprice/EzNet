

using EzNet.Benchmarks;
using Raylib_cs;

const int WIDTH = 1280, HEIGHT = 720;

GameServer gameServer = new GameServer();
gameServer.Bounds = new Rectangle(0, 0, WIDTH, HEIGHT);
gameServer.Run();
var window = new RpcWindow(WIDTH, HEIGHT, "RPC");
window.Run();
gameServer.Dispose();