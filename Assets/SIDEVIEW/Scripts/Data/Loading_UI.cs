using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading_UI : MonoBehaviour
{
    public Data_Manager data_Manager;

    public GameObject loadingUI;
    public Slider progressBar;
    public CanvasGroup loadingUI_Group;
    public CanvasGroup progressBar_Group;
    public CanvasGroup pressAnyKeyGroup;
    public float fadeSpeed = 1f;

    private bool loadingDone = false;
    private AsyncOperation loadOperation;

    private Canvas canvas;
    public Game_Manager game_Manager;

    public int loading;
    public bool Data_Load;
    void Start()
    {
        data_Manager = GameObject.FindWithTag("Data_Manager").GetComponent<Data_Manager>();
        if (GameObject.FindWithTag("Game_Manager") != null && game_Manager == null)
        {
            game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();
        }
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        DontDestroyOnLoad(this.gameObject);
    }
    public void GameStart()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        loadingUI.SetActive(true);
        pressAnyKeyGroup.alpha = 0;
        pressAnyKeyGroup.gameObject.SetActive(false);
        loading = 1;

        // 페이크 로딩 1초
        float fakeTime = 1f;
        float timer = 0f;
        while (timer < fakeTime)
        {
            timer += Time.deltaTime;
            if (progressBar != null)
                progressBar.value = timer / fakeTime;
            yield return null;
        }

        loadOperation = SceneManager.LoadSceneAsync("Main");

        Data_Load = true;

        while (loadOperation.progress < 0.9f)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Lerp(0f, 1f, loadOperation.progress / 0.9f);
            yield return null;
        }

        // 로딩 완료 표시
        if (progressBar != null)
            progressBar.value = 1f;

        // "아무 키나 누르세요" 텍스트 깜빡임 시작
        loadingDone = true;
        loading = 2;
        pressAnyKeyGroup.gameObject.SetActive(true);
        StartCoroutine(BlinkText());

        // 아무 키 입력 대기
        yield return StartCoroutine(WaitForAnyKey());

        // 씬 전환
        game_Manager.Data_Load = true;
        loading = 0;
        game_Manager.iscutSceneEnd = true;
        data_Manager.Data_Main = true;
        yield return StartCoroutine(PadeOut());
    }

    IEnumerator BlinkText()
    {
        float minAlpha = 0f;
        float maxAlpha = 1f;
        float duration = 1f;
        bool Active = false;
        while (true)
        {
            // Fade In
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                pressAnyKeyGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
                if (!Active)
                {
                    progressBar_Group.alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
                }
                else
                {
                    progressBar_Group.alpha = minAlpha;
                }
                
                yield return null;
            }

            // Fade Out
            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                pressAnyKeyGroup.alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
                Active = true;
                yield return null;
            }
        }
    }

    IEnumerator PadeOut()
    {
        float minAlpha = 0f;
        float maxAlpha = 1f;
        float duration = 1f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            loadingUI_Group.alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
            yield return null;
        }

        if (loadingUI_Group.alpha == 0)
        {
            loadingUI.SetActive(false);
        }
        
    }

    IEnumerator WaitForAnyKey()
    {
        // 키 입력 대기
        while (!Input.anyKeyDown)
            
            yield return null;
    }

    void Update()
    {
        canvas.worldCamera = Camera.main;
        if (GameObject.FindWithTag("Game_Manager") != null && game_Manager == null)
        {
            game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();
            game_Manager.Loading = true;
        }

        if (game_Manager != null && Data_Load)
        {
            game_Manager.Data_Loading = true;
            Data_Load = false;
        }

    } 
}
