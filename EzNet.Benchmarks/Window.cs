using Raylib_cs;

namespace EzNet.Benchmarks
{
	public abstract class Window
	{
		public Window(int width, int height, string title)
		{
			Raylib.InitWindow(width, height, title);
		}

		public void Run()
		{
			Load();
			while (Raylib.WindowShouldClose() == false)
			{
				Update();
				
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
				
				Render();
				
				Raylib.EndDrawing();
			}
			Unload();
			Raylib.CloseWindow();
		}

		protected virtual void Load() { }
		protected virtual void Unload() { }
		protected virtual void Update() { }
		protected virtual void Render() { }
	}
}
