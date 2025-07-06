using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public abstract UIManager.UITypes UIType { get; }
    public virtual void IsActive(bool isActive) => gameObject.SetActive(isActive);
    public virtual void Init() { }
}
