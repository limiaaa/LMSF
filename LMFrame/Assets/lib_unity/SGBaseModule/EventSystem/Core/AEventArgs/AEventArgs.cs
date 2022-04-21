
namespace SG.EventSystem.EventDispatcher
{
    public abstract class AEventArgs
    {
        public static T Parse<T>(AEventArgs instance) where T:AEventArgs
        {
            return instance as T;
        }

        public int EventType { get; }
    }
}
