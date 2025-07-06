using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonobehaviorSingleton<UIManager>
{
    [SerializeField] private Transform[] uiContainers;
    private Dictionary<UITypes, UIBase> registeredUI = new();
    public enum UITypes 
    {
        None,
        MainMenu,
        Lobby,
        Gameplay,
        Gameover,

    }
    public void OpenUI(UITypes uiType) => registeredUI[uiType].IsActive(true);
    public void CloseUI(UITypes uiType) => registeredUI[uiType].IsActive(false);
    public void SetUI(UITypes uiType)
    {
        foreach (KeyValuePair<UITypes, UIBase> kvp in registeredUI)
            kvp.Value.IsActive(kvp.Key == uiType);
    }
    protected override void Awake()
    {
        foreach (Transform transform in uiContainers)
            foreach (UIBase uiBase in transform.GetComponentsInChildren<UIBase>(true))
            {
                registeredUI.Add(uiBase.UIType, uiBase);
                uiBase.Init();
            }
        SetUI(UITypes.MainMenu);
    }
}
