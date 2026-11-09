using UnityEngine;
using UnityEngine.InputSystem;

public class StratagemInput : MonoBehaviour
{
    [SerializeField] private InputModeController modeController;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference downAction;
    [SerializeField] private InputActionReference leftAction;
    [SerializeField] private InputActionReference rightAction;
    [SerializeField] private StratagemData[] loadout;

    private CodeMatcher matcher;

    private void Awake()
    {
        matcher = new CodeMatcher(loadout);
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
                Debug.Log($"코드 완성: {matcher.Matched.stratagemName}");
                break;
            case MatchResult.NoMatch:
                Debug.Log("일치 없음, 입력 초기화");
                break;
        }
    }

    private void OnModeChanged(InputMode mode)
    {
        if (mode == InputMode.Normal)
        {
            matcher.Reset();
            Debug.Log("메뉴 닫힘, 입력 초기화");
        }
    }
}