using Assets.Scripts.UI.ControlElements;
using Core;
using UI;
using UnityEngine;

public class GameplayUi : MonoBehaviour
{
    [field: SerializeField] public BasicButton ExitMenuBtn { get; private set; }
    [field: SerializeField] public LinkedTextUI LinkedTextUI { get; private set; }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        LinkedTextUI.Dispose();
    }
}
