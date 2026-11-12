using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private MissionStats stats;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private string mainSceneName = "Main";

    private void Awake()
    {
        resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        stats.StatsReady += Show;
    }

    private void OnDisable()
    {
        stats.StatsReady -= Show;
    }

    private void Show(bool success)
    {
        titleText.text = success ? "MISSION SUCCESS" : "MISSION FAILED";

        int seconds = Mathf.RoundToInt(stats.ElapsedSeconds);

        statsText.text =
            $"TIME  {seconds / 60:00}:{seconds % 60:00}\n" +
            $"KILLS  {stats.KillCount}\n" +
            $"STRATAGEMS USED  {stats.StratagemUses}";

        resultPanel.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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