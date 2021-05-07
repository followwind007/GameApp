
namespace GameApp.DebugConsole
{
    public interface ICommand
    {
        string Name { get; }
        string Describe { get; }
        void DoCommand();
    }
}