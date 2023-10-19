

using EzNet.Benchmarks;

Server server = new Server();
server.Run();
var window = new RpcWindow(1280, 720, "RPC");
window.Run();
server.Dispose();