using Cysharp.Threading.Tasks;

namespace LibEngine
{
    public interface ISchemeController<T>
    {
        T GetScheme();

        UniTask<T> GetSchemeAsync();

        void Save();

        void Reset();
    }
}