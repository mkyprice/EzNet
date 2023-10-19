

using EzNet.Benchmarks;

GameServer gameServer = new GameServer();
gameServer.Run();
var window = new RpcWindow(1280, 720, "RPC");
window.Run();
gameServer.Dispose();