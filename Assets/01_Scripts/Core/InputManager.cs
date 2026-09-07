using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

/* 
우클릭 이동으로 수정

- UI 위 -> 월드에서 아무 동작도 하지 않음
- 건물 배치 모드 -> 배치만 시도
- 상호작용 가능한 대상 -> 선택
- 이동 가능한 지형 -> 플레이어 이동
- 모두 실패 -> return
 
 */

public class InputManager : MonoBehaviour
{
    [Header("입력 액션")]
    [SerializeField] private InputActionReference primaryPointerAction;

    [Header("레이캐스트 설정")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private Camera mainCamera;

    [Header("ESC로 닫을 UI")]
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private BuildModeController buildModeController;
    [SerializeField] private CharacterPanelController characterPanel;
    [SerializeField] private CarrierCommandPanelView carrierCommandPanel;
    [SerializeField] private SkillTreeESC skillTreeUI;
    [SerializeField] private TeleportUI teleportUI;
    [SerializeField] private BuildingUIRouter buildingUIRouter;
    [SerializeField] private TutorialUI tutorialUI; 

    public event Action OnPrimaryClicked;
    public event Action OnSecondaryClicked;
    public event Action OnCancelPressed;
    public event Action<PlacedBuilding> OnBuildingLongPressed;

    private void Awake()
    {
        if (optionUI == null) optionUI = FindFirstObjectByType<OptionUI>(FindObjectsInactive.Include);
        if (buildModeController == null) buildModeController = FindFirstObjectByType<BuildModeController>(FindObjectsInactive.Include);
        if (characterPanel == null) characterPanel = FindFirstObjectByType<CharacterPanelController>(FindObjectsInactive.Include);
        if (carrierCommandPanel == null) carrierCommandPanel = FindFirstObjectByType<CarrierCommandPanelView>(FindObjectsInactive.Include);
        if (skillTreeUI == null) skillTreeUI = FindFirstObjectByType<SkillTreeESC>(FindObjectsInactive.Include);
        if (teleportUI == null) teleportUI = FindFirstObjectByType<TeleportUI>(FindObjectsInactive.Include);
        if (buildingUIRouter == null) buildingUIRouter = FindFirstObjectByType<BuildingUIRouter>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        if (primaryPointerAction == null) return;

        primaryPointerAction.action.performed += HandlePrimaryPerformed;
        primaryPointerAction.action.Enable();
    }

    private void OnDisable()
    {
        if (primaryPointerAction == null) return;

        primaryPointerAction.action.performed -= HandlePrimaryPerformed;
        primaryPointerAction.action.Disable();
    }

    private void Update()
    {
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasPressedThisFrame && 
            !IsPointerOverUI())
        {
            OnSecondaryClicked?.Invoke();
        }

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscape();
        }
    }

    private void HandleEscape()
    {
        if (tutorialUI != null && tutorialUI.IsOpen)
        {
            tutorialUI.CloseTutorialPopup();
            return;
        }

        if (optionUI != null && optionUI.gameObject.activeInHierarchy)
        {
            optionUI.Hide();
            return;
        }

        if (skillTreeUI != null && skillTreeUI.gameObject.activeInHierarchy)
        {
            skillTreeUI.Close();
            return;
        }

        if (teleportUI != null && teleportUI.IsOpen)
        {
            teleportUI.CloseUI();
            return;
        }

        if (characterPanel != null && characterPanel.IsOpen)
        {
            characterPanel.Hide();
            return;
        }

        if (carrierCommandPanel != null && carrierCommandPanel.IsOpen)
        {
            carrierCommandPanel.Hide();
            return;
        }

        if (buildingUIRouter != null && buildingUIRouter.TryClose())
        {
            return;
        }

        if (buildModeController != null && buildModeController.IsBuildMode)
        {
            OnCancelPressed?.Invoke();
            return;
        }

        if (optionUI != null) optionUI.Show();
    }

    private void HandlePrimaryPerformed(InputAction.CallbackContext context) 
    {
        if (IsPointerOverUI()) return;

        if (context.interaction is HoldInteraction)
        {
            HandleBuildingHold();
            return;
        }

        if (context.interaction is TapInteraction)
        {
            OnPrimaryClicked?.Invoke();
        }
    }

    private void HandleBuildingHold()
    {
        if (!TryGetBuilding(out PlacedBuilding building))
        {
            return;
        }

        OnBuildingLongPressed?.Invoke(building);
    }

    // 마우스가 UI 위에 있다면 false 반환
    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public bool TryGetWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = default;

        if (mainCamera == null || Mouse.current == null)
        {
            return false;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        worldPosition = hit.point;
        return true;
    }

    public bool TryGetBuilding(out PlacedBuilding building)
    {
        building = null;

        if (mainCamera == null || Mouse.current == null)
        {
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, buildingLayer, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        building = hit.collider.GetComponentInParent<PlacedBuilding>();

        return building != null;
    }

    public bool TryGetPlacementHit(out Vector3 worldPosition, out Collider hitCollider)
    {
        worldPosition = default;
        hitCollider = null;

        if (mainCamera == null || Mouse.current == null)
        {
            return false;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementLayer, QueryTriggerInteraction.Ignore))
        {
            worldPosition = hit.point;
            hitCollider = hit.collider;

            return true;
        }

        worldPosition = default;
        hitCollider = null;

        return false;
    }
}
