using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode { Normal, Stratagem, Throw }

public class InputModeController : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private InputActionReference menuAction;

    public InputMode Mode { get; private set; } = InputMode.Normal;
    public event Action<InputMode> ModeChanged;

    private InputActionMap playerMap;
    private InputActionMap combatMap;
    private InputActionMap stratagemMap;
    private InputActionMap throwMap;

    private void Awake()
    {
        if (controls == null || menuAction == null)
        {
            Debug.LogError("Controls 또는 Menu Action 칸이 비어 있습니다.", this);
            enabled = false;
            return;
        }

        playerMap = FindMap("Player");
        combatMap = FindMap("Combat");
        stratagemMap = FindMap("Stratagem");
        throwMap = FindMap("Throw");

        if (playerMap == null || combatMap == null || stratagemMap == null || throwMap == null)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Mode = InputMode.Normal;
        playerMap.Enable();
        ApplyMaps(Mode);

        menuAction.action.started += OnMenuPressed;
        menuAction.action.canceled += OnMenuReleased;
    }

    private void OnDisable()
    {
        if (menuAction != null)
        {
            menuAction.action.started -= OnMenuPressed;
            menuAction.action.canceled -= OnMenuReleased;
        }

        playerMap?.Disable();
        combatMap?.Disable();
        stratagemMap?.Disable();
        throwMap?.Disable();
    }

    private void OnMenuPressed(InputAction.CallbackContext context)
    {
        if (Mode != InputMode.Normal) return;
        SetMode(InputMode.Stratagem);
    }

    private void OnMenuReleased(InputAction.CallbackContext context)
    {
        if (Mode != InputMode.Stratagem) return;
        SetMode(InputMode.Normal);
    }

    public void SetMode(InputMode next)
    {
        if (Mode == next) return;

        Mode = next;
        ApplyMaps(next);

        Debug.Log($"입력 모드: {Mode}");
        ModeChanged?.Invoke(Mode);
    }

    private void ApplyMaps(InputMode mode)
    {
        SetEnabled(combatMap, mode == InputMode.Normal);
        SetEnabled(stratagemMap, mode == InputMode.Stratagem);
        SetEnabled(throwMap, mode == InputMode.Throw);
    }

    private static void SetEnabled(InputActionMap map, bool on)
    {
        if (on) map.Enable();
        else map.Disable();
    }

    private InputActionMap FindMap(string mapName)
    {
        InputActionMap map = controls.FindActionMap(mapName);
        if (map == null)
        {
            Debug.LogError($"Action Map '{mapName}'을 찾지 못했습니다. 현재 Controls 에셋: {controls.name}", this);
        }
        return map;
    }
}