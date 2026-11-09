using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode { Normal, Stratagem }

public class InputModeController : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private InputActionReference menuAction;

    public InputMode Mode { get; private set; } = InputMode.Normal;
    public event Action<InputMode> ModeChanged;

    private InputActionMap playerMap;
    private InputActionMap combatMap;
    private InputActionMap stratagemMode;

    private void Awake()
    {
        playerMap = FindMap("Player");
        combatMap = FindMap("Combat");
        stratagemMode = FindMap("Stratagem");
    }

    private InputActionMap FindMap(string mapName)
    {
        InputActionMap map = controls.FindActionMap(mapName);
        if (map == null)
        {
            Debug.LogError($"Action Map '{mapName}'을 찾지 못했습니다. 에셋의 맵 이름을 확인하세요.", this);
        }
        return map;
    }

    private void OnEnable()
    {
        playerMap.Enable();
        combatMap.Enable();
        stratagemMode.Disable();

        menuAction.action.started += OnMenuPressed;
        menuAction.action.canceled += OnMenuReleased;
    }

    private void OnDisable()
    {
        menuAction.action.started -= OnMenuPressed;
        menuAction.action.canceled -= OnMenuReleased;

        playerMap.Disable();
        combatMap.Disable();
        stratagemMode.Disable();
    }

    private void OnMenuPressed(InputAction.CallbackContext context)
    {
        SetMode(InputMode.Stratagem);
    }

    private void OnMenuReleased(InputAction.CallbackContext context)
    {
        SetMode(InputMode.Normal);
    }

    private void SetMode(InputMode next)
    {
        if (Mode == next) return;
        Mode = next;

        combatMap.Enable();
        stratagemMode.Disable();

        if (next == InputMode.Stratagem)
        {
            combatMap.Disable();
            stratagemMode.Enable();
        }

        Debug.Log($"입력 모드: {Mode}");
        ModeChanged?.Invoke(Mode);
    }
}