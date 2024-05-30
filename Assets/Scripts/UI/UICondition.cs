using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}