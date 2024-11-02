using Cysharp.Threading.Tasks;

namespace Code.States
{
    public interface IGameState
    {
        UniTask Enter();
        UniTask Exit();
    }
}