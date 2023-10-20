

using EzNet.Benchmarks;
using EzNet.Benchmarks.Serializers;
using EzRpc.Serialization;
using Raylib_cs;
using System.Numerics;

const int WIDTH = 1280, HEIGHT = 720;

Rpc.RegisterTypeSerializer<Vector2Serializer>(typeof(Vector2));

GameServer gameServer = new GameServer();
gameServer.Bounds = new Rectangle(0, 0, WIDTH, HEIGHT);
gameServer.Run();
var window = new RpcWindow(WIDTH, HEIGHT, "RPC");
window.Run();
gameServer.Dispose();