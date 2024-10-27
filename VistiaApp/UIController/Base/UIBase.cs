using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public virtual async UniTask OnShow(UIType previousUI, params object[] message)
    {
        await UniTask.CompletedTask;
    }

    public virtual async UniTask OnHide(UIType nextUI)
    {
        await UniTask.CompletedTask;
    }
}
