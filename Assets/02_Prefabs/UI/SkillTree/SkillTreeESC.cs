using UnityEngine;

public class SkillTreeESC : MonoBehaviour
{
    [SerializeField] private GameObject passivePanel;
    [SerializeField] private GameObject battlePanel;

    public void Open()
    {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX(ESFXType.UI_Open);
    }

    public void Close()
    {
        if (!gameObject.activeSelf) return;

        AudioManager.Instance.PlaySFX(ESFXType.UI_Close);
        gameObject.SetActive(false);
    }

    public void ShowPassive()
    {
        passivePanel.SetActive(true);
        battlePanel.SetActive(false);
        AudioManager.Instance.PlaySFX(ESFXType.UI_ButtonClick);
    }

    public void ShowBattle()
    {
        passivePanel.SetActive(false);
        battlePanel.SetActive(true);
        AudioManager.Instance.PlaySFX(ESFXType.UI_ButtonClick);
    }
}
