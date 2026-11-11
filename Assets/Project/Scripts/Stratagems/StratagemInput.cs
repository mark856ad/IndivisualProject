using UnityEngine;
using UnityEngine.InputSystem;

public class StratagemInput : MonoBehaviour
{
    [SerializeField] private InputModeController modeController;
    [SerializeField] private StratagemInventory inventory;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference downAction;
    [SerializeField] private InputActionReference leftAction;
    [SerializeField] private InputActionReference rightAction;
    [SerializeField] private BeaconThrower thrower;
    private CodeMatcher matcher;
    
    public event System.Action InputChanged;

    public System.Collections.Generic.IReadOnlyList<Direction> CurrentInput
    {
        get
        {
            if (matcher == null) return System.Array.Empty<Direction>();
            return matcher.Current;
        }
    }
    private void Awake()
    {
        matcher = new CodeMatcher(inventory.Loadout);
    }

    private void OnEnable()
    {
        modeController.ModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        modeController.ModeChanged -= OnModeChanged;
    }

    private void Update()
    {
        if (upAction.action.WasPressedThisFrame()) Push(Direction.Up);
        if (downAction.action.WasPressedThisFrame()) Push(Direction.Down);
        if (leftAction.action.WasPressedThisFrame()) Push(Direction.Left);
        if (rightAction.action.WasPressedThisFrame()) Push(Direction.Right);
    }

    private void Push(Direction direction)
    {
        MatchResult result = matcher.Push(direction);

        switch (result)
        {
            case MatchResult.Partial:
                Debug.Log($"입력 중: {matcher.Current.Count}칸");
                break;
            case MatchResult.Complete:
                TryActivate(matcher.Matched);
                break;
            case MatchResult.NoMatch:
                Debug.Log("일치 없음, 입력 초기화");
                break;
        }
        InputChanged?.Invoke();
    }

    private void TryActivate(StratagemData data)
    {
        if (!inventory.CanUse(data))
        {
            Debug.Log($"{data.stratagemName} 사용 불가 (쿨타임 {inventory.GetCooldownRemaining(data):F1}초, 남은 횟수 {inventory.GetUsesRemaining(data)})");
            return;
        }

        thrower.Hold(data);
    }

    private void OnModeChanged(InputMode mode)
    {
        if (mode == InputMode.Normal)
        {
            matcher.Reset();
            InputChanged?.Invoke();
            Debug.Log("메뉴 닫힘, 입력 초기화");
        }
    }
}