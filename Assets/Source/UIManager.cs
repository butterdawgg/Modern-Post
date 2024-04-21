using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject introductionMenu;
    [SerializeField] Button quitToMenuButton;
    [SerializeField] Button startGameButton;
    [SerializeField] TextMeshProUGUI introductionText;
    [SerializeField] string[] phrases;
    [SerializeField] float phraseDelay;

    public static bool IsPaused { get; private set; } = false;
    public static bool IsInIntroduction { get; private set; } = false;

    void Awake()
    {
        quitToMenuButton.onClick.AddListener(QuitToMenu);
        startGameButton.onClick.AddListener(Resume);

        if (SerializeManager.Instance.GetBool(BoolType.FirstPlay))
            StartCoroutine(IntroductionCoroutine());
        else
            Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) & !IsInIntroduction)
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    void Resume()
    {
        IsInIntroduction = false;
        introductionMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    void Pause()
    {
        introductionMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private IEnumerator IntroductionCoroutine()
    {
        IsInIntroduction = true;
        startGameButton.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 0f;

        foreach (string phrase in phrases)
        {
            for (int i = 0; i < phrase.ToCharArray().Length; i++)
            {
                introductionText.text += phrase.ToCharArray()[i];

                AudioManager.Instance.PlaySound("Text");

                yield return new WaitForSecondsRealtime(0.05f);
            }

            if (phrase != phrases[phrases.Length - 1])
            {
                introductionText.text += "\n\n";
            }

            yield return new WaitForSecondsRealtime(phraseDelay);
        }

        SerializeManager.Instance.SetBool(BoolType.FirstPlay, false);
        startGameButton.gameObject.SetActive(true);
    }
}