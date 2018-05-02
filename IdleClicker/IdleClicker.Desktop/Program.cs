using Urho;

namespace IdleClicker.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            new MyGame(new ApplicationOptions("Data")).Run();
            // For a console app Urho will create a Windows/macOS/Linux window using SDL 
        }
    }
}
