using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeSequence : MonoBehaviour
{
    [SerializeField] private ObjectiveManager manager;
    [SerializeField] private InputModeController modeController;
    [SerializeField] private StratagemInput stratagemInput;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject shuttle;
    [SerializeField] private InputActionReference upAction;
    [SerializeField] private InputActionReference downAction;
    [SerializeField] private InputActionReference leftAction;
    [SerializeField] private InputActionReference rightAction;
    [SerializeField] private float radius = 5f;
    [SerializeField] private int codeLength = 6;
    [SerializeField] private float countdownSeconds = 90f;
    [SerializeField] private float boardSeconds = 20f;
    [SerializeField] private float boardRadius = 4f;

    public event Action MissionSucceeded;
    public event Action MissionFailed;

    private enum State { Locked, WaitingForCode, Countdown, Landed, Finished }

    private State state = State.Locked;
    private CodeMatcher matcher;
    private Transform player;
    private float arrivalTime;
    private float boardDeadline;
    private int lastLoggedSecond = -1;

    public float RemainingSeconds => state switch
    {
        State.Countdown => Mathf.Max(0f, arrivalTime - Time.time),
        State.Landed => Mathf.Max(0f, boardDeadline - Time.time),
        _ => 0f
    };

    private void Awake()
    {
        visual.SetActive(false);
        shuttle.SetActive(false);
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        manager.AllCompleted += Activate;
        modeController.ModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        manager.AllCompleted -= Activate;
        modeController.ModeChanged -= OnModeChanged;
    }

    [ContextMenu("테스트: 탈출 활성화")]
    private void Activate()
    {
        if (state != State.Locked) return;

        StratagemData escapeCode = CreateRandomCode(codeLength);
        matcher = new CodeMatcher(new[] { escapeCode });

        visual.SetActive(true);
        state = State.WaitingForCode;

        Debug.Log($"탈출 지점 활성화. 코드: {CodeToText(escapeCode.code)}");
    }

    private static StratagemData CreateRandomCode(int length)
    {
        StratagemData data = ScriptableObject.CreateInstance<StratagemData>();
        data.stratagemName = "CALL SHUTTLE";
        data.code = new Direction[length];

        for (int i = 0; i < length; i++)
        {
            data.code[i] = (Direction)UnityEngine.Random.Range(0, 4);
        }

        return data;
    }

    private static string CodeToText(Direction[] code)
    {
        string text = "";
        foreach (Direction d in code)
        {
            text += d == Direction.Up ? "↑" : d == Direction.Down ? "↓" : d == Direction.Left ? "←" : "→";
        }
        return text;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingForCode:
                UpdateWaiting();
                break;
            case State.Countdown:
                UpdateCountdown();
                break;
            case State.Landed:
                UpdateLanded();
                break;
        }
    }

    private void UpdateWaiting()
    {
        bool inside = (player.position - transform.position).sqrMagnitude <= radius * radius;
        stratagemInput.enabled = !inside;
        if (!inside) return;

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
                Debug.Log($"탈출 코드 입력 중: {matcher.Current.Count}/{codeLength}");
                break;
            case MatchResult.Complete:
                StartCountdown();
                break;
            case MatchResult.NoMatch:
                Debug.Log("탈출 코드 틀림, 처음부터");
                break;
        }
    }

    private void StartCountdown()
    {
        state = State.Countdown;
        arrivalTime = Time.time + countdownSeconds;
        stratagemInput.enabled = true;

        if (spawner != null) spawner.SetMissionPressure(1f);

        Debug.Log($"탈출선 호출! {countdownSeconds:F0}초 뒤 도착");
    }

    private void UpdateCountdown()
    {
        float remaining = arrivalTime - Time.time;

        int second = Mathf.CeilToInt(remaining);
        if (second != lastLoggedSecond && second % 10 == 0)
        {
            lastLoggedSecond = second;
            Debug.Log($"SHUTTLE ARRIVING {second / 60:00}:{second % 60:00}");
        }

        if (remaining <= 0f)
        {
            Land();
        }
    }

    private void Land()
    {
        state = State.Landed;
        boardDeadline = Time.time + boardSeconds;
        shuttle.SetActive(true);

        Debug.Log($"탈출선 착륙! {boardSeconds:F0}초 안에 탑승하세요");
    }

    private void UpdateLanded()
    {
        Vector3 toShuttle = player.position - shuttle.transform.position;
        toShuttle.y = 0f;

        if (toShuttle.sqrMagnitude <= boardRadius * boardRadius)
        {
            Finish(true);
            return;
        }

        if (Time.time >= boardDeadline)
        {
            Finish(false);
        }
    }

    private void Finish(bool success)
    {
        state = State.Finished;

        if (success)
        {
            Debug.Log("탑승 성공! 임무 성공");
            MissionSucceeded?.Invoke();
        }
        else
        {
            Debug.Log("탈출선이 떠났습니다. 임무 실패");
            MissionFailed?.Invoke();
        }
    }

    private void OnModeChanged(InputMode mode)
    {
        if (mode == InputMode.Normal)
        {
            matcher?.Reset();
        }
    }
}