using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingWindow : BaseWindow
    {
        [field: SerializeField] public CanvasGroup background { get; private set; }
        [field: SerializeField] public Image loadingScreen { get; private set; }
    }
}