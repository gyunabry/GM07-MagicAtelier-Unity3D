using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class _TestSystem : MonoBehaviour
{
    [Header("개발자 UI 표시")]
    [SerializeField] private Key toggleKey = Key.F11;
    [SerializeField] private bool showOnStart;
    [Tooltip("개발자 UI 전체를 감싸는 패널의 CanvasGroup. 비워두면 이 오브젝트에 생성합니다.")]
    [SerializeField] private CanvasGroup uiCanvasGroup;

    [SerializeField] private int testGold;
    [SerializeField] private int testEXP;

    [Header("아이템 지급 테스트")]
    [SerializeField] private ItemDatabaseSO itemDatabase;
    [SerializeField] private ItemDataSO selectedItem;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private IntegratedTransmitter integratedTransmitter;

    [Header("공통 아이템 선택")]
    [SerializeField] private TMP_Dropdown itemDropdown;
    [Tooltip("선택 사항: 지급 결과를 표시할 텍스트")]
    [SerializeField] private TMP_Text itemResultText;

    [Header("플레이어 인벤토리 버튼")]
    [SerializeField] private Button playerAdd10Button;
    [SerializeField] private Button playerAdd1Button;

    [Header("통합 전송기 버튼")]
    [SerializeField] private Button transmitterAdd10Button;
    [SerializeField] private Button transmitterAdd1Button;

    [Header("판매대 버튼")]
    [SerializeField] private Button salesCounterAdd10Button;
    [SerializeField] private Button salesCounterAdd1Button;

    private readonly List<ItemDataSO> dropdownItems = new();
    public bool IsVisible { get; private set; }
    public string LastItemResult { get; private set; }

    private void Awake()
    {
        if (uiCanvasGroup == null) uiCanvasGroup = GetComponent<CanvasGroup>();
        if (uiCanvasGroup == null) uiCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        SetVisible(showOnStart);
    }

    private void Update()
    {
        if (toggleKey != Key.None && Keyboard.current != null &&
            Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    public void ToggleUI() => SetVisible(!IsVisible);

    public void SetVisible(bool visible)
    {
        if (uiCanvasGroup == null) return;

        if (!visible && itemDropdown != null) itemDropdown.Hide();
        IsVisible = visible;

        // GameObject를 끄면 Update도 멈추므로 UI 표시와 입력만 차단한다.
        uiCanvasGroup.alpha = visible ? 1f : 0f;
        uiCanvasGroup.interactable = visible;
        uiCanvasGroup.blocksRaycasts = visible;
    }

    private void OnEnable()
    {
        RefreshItemDropdown();
        if (itemDropdown != null)
            itemDropdown.onValueChanged.AddListener(OnItemSelected);
        BindButtons(true);
    }

    private void OnDisable()
    {
        if (itemDropdown != null)
            itemDropdown.onValueChanged.RemoveListener(OnItemSelected);
        BindButtons(false);
    }

    private void BindButtons(bool bind)
    {
        BindButton(playerAdd10Button, OnClickPlayerItem10, bind);
        BindButton(playerAdd1Button, OnClickPlayerItem1, bind);
        BindButton(transmitterAdd10Button, OnClickTransmitterItem10, bind);
        BindButton(transmitterAdd1Button, OnClickTransmitterItem1, bind);
        BindButton(salesCounterAdd10Button, OnClickSalesCounterItem10, bind);
        BindButton(salesCounterAdd1Button, OnClickSalesCounterItem1, bind);
    }

    private void BindButton(Button button, UnityAction action, bool bind)
    {
        if (button == null) return;
        button.onClick.RemoveListener(action);
        if (!bind) return;

        // 이미 Inspector의 On Click에 연결한 경우에는 중복 지급을 방지한다.
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            if (button.onClick.GetPersistentTarget(i) == this &&
                button.onClick.GetPersistentMethodName(i) == action.Method.Name &&
                button.onClick.GetPersistentListenerState(i) != UnityEventCallState.Off)
                return;
        }
        button.onClick.AddListener(action);
    }

    public void RefreshItemDropdown()
    {
        dropdownItems.Clear();
        if (itemDropdown == null)
        {
            RefreshItemButtons();
            return;
        }

        // 0번은 안내 문구로 두어 명시적으로 선택한 아이템만 지급한다.
        dropdownItems.Add(null);
        var options = new List<string> { "아이템 종류 선택" };
        if (itemDatabase != null && itemDatabase.Items != null)
        {
            foreach (ItemDataSO item in itemDatabase.Items)
            {
                if (item == null) continue;
                dropdownItems.Add(item);
                options.Add($"{item.ItemName} ({item.ItemId})");
            }
        }

        itemDropdown.ClearOptions();
        itemDropdown.AddOptions(options);
        itemDropdown.interactable = dropdownItems.Count > 1;
        int index = Mathf.Max(0, dropdownItems.IndexOf(selectedItem));
        itemDropdown.SetValueWithoutNotify(index);
        OnItemSelected(index);
        itemDropdown.RefreshShownValue();
    }

    private void OnItemSelected(int index)
    {
        selectedItem = index >= 0 && index < dropdownItems.Count ? dropdownItems[index] : null;
        RefreshItemButtons();
    }

    private void RefreshItemButtons()
    {
        bool canAdd = itemDatabase != null && itemDatabase.Items != null &&
            selectedItem != null && itemDatabase.Items.Contains(selectedItem);
        SetButtonInteractable(playerAdd10Button, canAdd);
        SetButtonInteractable(playerAdd1Button, canAdd);
        SetButtonInteractable(transmitterAdd10Button, canAdd);
        SetButtonInteractable(transmitterAdd1Button, canAdd);
        SetButtonInteractable(salesCounterAdd10Button, canAdd);
        SetButtonInteractable(salesCounterAdd1Button, canAdd);
    }

    private static void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }

    // Button.onClick에서 수량 인수 없이 연결할 수 있는 함수들.
    public void OnClickPlayerItem1() => AddPlayerItem(1);
    public void OnClickPlayerItem10() => AddPlayerItem(10);
    public void OnClickTransmitterItem1() => AddTransmitterItem(1);
    public void OnClickTransmitterItem10() => AddTransmitterItem(10);
    public void OnClickSalesCounterItem1() => AddSalesCounterItem(1);
    public void OnClickSalesCounterItem10() => AddSalesCounterItem(10);

    private void AddPlayerItem(int amount)
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        AddItem(playerInventory != null ? playerInventory.Inventory : null, "플레이어", amount);
    }

    private void AddTransmitterItem(int amount)
    {
        if (integratedTransmitter == null)
            integratedTransmitter = FindFirstObjectByType<IntegratedTransmitter>();
        AddItem(integratedTransmitter != null ? integratedTransmitter.Inventory : null, "통합 전송기", amount);
    }

    private void AddSalesCounterItem(int amount)
    {
        AddItem(CounterInventory.Instance != null ? CounterInventory.Instance.Inventory : null, "판매대", amount);
    }

    private void AddItem(ItemInventory inventory, string targetName, int amount)
    {
        if (!Application.isPlaying)
        {
            ReportItemResult("아이템 지급은 Play 모드에서만 가능합니다.", true);
            return;
        }

        if (itemDatabase == null || itemDatabase.Items == null ||
            selectedItem == null || !itemDatabase.Items.Contains(selectedItem))
        {
            ReportItemResult("Item Database를 지정하고 데이터베이스의 아이템을 선택하세요.", true);
            return;
        }

        if (inventory == null)
        {
            ReportItemResult($"{targetName} 인벤토리를 찾을 수 없습니다.", true);
            return;
        }

        int added = inventory.Add(selectedItem, amount);
        string result = $"{targetName}: {selectedItem.ItemName} {added}/{amount}개 추가";
        if (added < amount) result += " (인벤토리 용량 부족)";
        ReportItemResult(result, added < amount);
    }

    private void ReportItemResult(string message, bool warning)
    {
        LastItemResult = message;
        if (itemResultText != null) itemResultText.text = message;
        if (warning) Debug.LogWarning(message, this);
        else Debug.Log(message, this);
    }

    public void OnClickGold()
    {
        CurrencySystem.Instance.GrantMoney(testGold);
    }

    public void OnClickEXP()
    {
        CurrencySystem.Instance.GrantExperience(testEXP);
    }
}