
namespace CGameDevToolkit.Framework
{
    public interface IPool<T>
    {
        T Respawn();

        void Despawn(T obj);
    }
}