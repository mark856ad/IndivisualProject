using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private InputModeController modeController;
    [SerializeField] private MissionController mission;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainSceneName = "Main";

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        pausePanel.SetActive(false);
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();

        if (IsPaused)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (mission.IsEnded) return;

        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        Time.timeScale = 0f;
        modeController.enabled = false;
        pausePanel.SetActive(true);

        Debug.Log("일시정지");
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;

        pausePanel.SetActive(false);
        modeController.enabled = true;
        Time.timeScale = 1f;

        Debug.Log("재개");
    }

    public void GoToMain()
    {
        if (!Application.CanStreamedLevelBeLoaded(mainSceneName))
        {
            Debug.LogWarning($"'{mainSceneName}' 씬이 빌드 설정에 없습니다. File > Build Profiles에서 등록하세요.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainSceneName);
    }
}