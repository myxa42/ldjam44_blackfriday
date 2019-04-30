
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TMPro;

public sealed class Global : MonoBehaviour
{
    public const long InitialMoney = 10;
    public const long InitialMaxHealth = 500;
    const int ProgressBarFillerWidth = 302;

    public static Global Instance { get; private set; }

    public GameObject LoadingUI;
    public RectTransform ProgressBarFiller;
    public TextMeshProUGUI LoadingText;

    public InventoryItemWeaponSpec InitialWeapon;

    [Header("Internals")]
    public InventoryItem EquippedWeapon;
    public Inventory PlayerInventory = new Inventory();
    public int PlayerLevel = 0;
    public long LevelBaseExperience = 0;
    public long PlayerTargetExperience;
    public long PlayerTargetMoney = InitialMoney;
    public long PlayerTargetHealth = InitialMaxHealth;
    public long MaxHealth = InitialMaxHealth;

    public bool ShouldLoadGame;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        ResetPlayer();
    }

    void Update()
    {
        if (ShouldLoadGame) {
            ShouldLoadGame = false;
            LoadScene("Park");
        }
    }

    public void ResetPlayer()
    {
        PlayerInventory = new Inventory();
        EquippedWeapon = PlayerInventory.AddItem(InitialWeapon);
        PlayerLevel = 0;
        LevelBaseExperience = 0;
        PlayerTargetExperience = 0;
        PlayerTargetMoney = InitialMoney;
        PlayerTargetHealth = InitialMaxHealth;
        MaxHealth = InitialMaxHealth;
    }

    void SetProgress(float progress)
    {
        var off = ProgressBarFiller.offsetMax;
        off.x = -Mathf.Clamp((1.0f - progress) * ProgressBarFillerWidth, 0, ProgressBarFillerWidth);
        ProgressBarFiller.offsetMax = off;
        LoadingText.text = String.Format(Language.Current.Loading, (int)(progress * 100.0f));
    }

    public void LoadScene(string name, Action callback = null)
    {
        StartCoroutine(SceneLoadCoroutine(name, callback));
    }

    IEnumerator SceneLoadCoroutine(string name, Action callback)
    {
        LoadingUI.SetActive(true);

        SetProgress(0.0f);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        while (!asyncOperation.isDone) {
            SetProgress(asyncOperation.progress * 0.5f);
            yield return null;
        }

        SetProgress(0.5f);

        asyncOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!asyncOperation.isDone) {
            SetProgress(0.5f + asyncOperation.progress * 0.5f);
            yield return null;
        }

        SetProgress(1.0f);
        yield return null;

        try {
            FindObjectOfType<GameController>().BeginGameplay();
        } catch (Exception e) {
            Debug.LogException(e);
        }

        try {
            callback?.Invoke();
        } catch (Exception e) {
            Debug.LogException(e);
        }

        LoadingUI.SetActive(false);
    }
}
