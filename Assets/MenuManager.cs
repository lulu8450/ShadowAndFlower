using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Startup sequence panels (order)")]
    public List<GameObject> startupPanels;
    public GameObject panelMainMenu;
    public GameObject panelSettingsRoot;

    [Header("Panels from Main Menu (subpanels)")]
    public GameObject panelSettingEcranMenuP;
    public GameObject panelSettingVolumeMenuP;
    public GameObject panelSettingSoustitreMenuP;

    [Header("Buttons in PannelSettings (Main Menu)")]
    public Button ButtonEcran;
    public Button ButtonVolume;
    public Button ButtonSoustitre;

    // ---------- ECRAN UI - STARTUP ----------
    [Header("Ecran - Startup UI")]
    public TMP_Dropdown resolutionDropdownStartup;
    public Toggle fullscreenToggleStartup;

    // ---------- ECRAN UI - MENU PRINCIPAL ----------
    [Header("Ecran - MenuP UI")]
    public TMP_Dropdown resolutionDropdownMenuP;
    public Toggle fullscreenToggleMenuP;

    private List<Resolution> availableResolutions = new List<Resolution>();

    // ---------- VOLUME UI - STARTUP ----------
    [Header("Volume - Startup UI")]
    public Slider masterSliderStartup;
    public Slider voixSliderStartup;
    public Slider musicSliderStartup;
    public Slider sfxSliderStartup;

    // ---------- VOLUME UI - MENU PRINCIPAL ----------
    [Header("Volume - MenuP UI")]
    public Slider masterSliderMenuP;
    public Slider voixSliderMenuP;
    public Slider musicSliderMenuP;
    public Slider sfxSliderMenuP;

    // ---------- SOUS-TITRE UI - STARTUP ----------
    [Header("Sous-titre - Startup UI")]
    public TMP_Text exampleTextStartup;
    public Button btnLangueFRStartup;      // Bouton Français
    public Button btnLangueENStartup;      // Bouton Anglais
    public Button btnLangueKRStartup;      // Bouton Créole
    public Button btnTextSize50Startup;    // Bouton 50%
    public Button btnTextSize75Startup;    // Bouton 75%
    public Button btnTextSize100Startup;   // Bouton 100%
    public Button btnTextSize125Startup;   // Bouton 125%
    public Slider textSpeedSliderStartup;  // Slider vitesse (0.5 à 2.0)

    // ---------- SOUS-TITRE UI - MENU PRINCIPAL ----------
    [Header("Sous-titre - MenuP UI")]
    public TMP_Text exampleTextMenuP;
    public Button btnLangueFRMenuP;        // Bouton Français
    public Button btnLangueENMenuP;        // Bouton Anglais
    public Button btnLangueKRMenuP;        // Bouton Créole
    public Button btnTextSize50MenuP;      // Bouton 50%
    public Button btnTextSize75MenuP;      // Bouton 75%
    public Button btnTextSize100MenuP;     // Bouton 100%
    public Button btnTextSize125MenuP;     // Bouton 125%
    public Slider textSpeedSliderMenuP;    // Slider vitesse (0.5 à 2.0)

    // ---------- Sequence control ----------
    private int startupIndex = 0;

    // ---------- Save keys ----------
    const string KEY_RES_INDEX = "res_index";
    const string KEY_FULLSCREEN = "fullscreen";
    const string KEY_MASTER = "vol_master";
    const string KEY_VOIX = "vol_voix";
    const string KEY_MUSIC = "vol_music";
    const string KEY_SFX = "vol_sfx";
    const string KEY_LANG = "sub_lang";
    const string KEY_TEXTSIZE = "sub_textsize";
    const string KEY_TEXTSPEED = "sub_textspeed";

    private int baseFontSizeStartup = 36;
    private int baseFontSizeMenuP = 36;

    void Start()
    {
        CloseAllPanels();
        InitResolutions();
        PopulateResolutionDropdowns();
        LoadSettingsToUI_Startup();
        LoadSettingsToUI_MenuP();

        if (exampleTextStartup != null) baseFontSizeStartup = Mathf.RoundToInt(exampleTextStartup.fontSize);
        if (exampleTextMenuP != null) baseFontSizeMenuP = Mathf.RoundToInt(exampleTextMenuP.fontSize);

        // Connecter les boutons de langue - STARTUP
        if (btnLangueFRStartup != null) btnLangueFRStartup.onClick.AddListener(() => OnLanguageButtonStartup(1));
        if (btnLangueENStartup != null) btnLangueENStartup.onClick.AddListener(() => OnLanguageButtonStartup(0));
        if (btnLangueKRStartup != null) btnLangueKRStartup.onClick.AddListener(() => OnLanguageButtonStartup(2));

        // Connecter les boutons de taille de texte - STARTUP
        if (btnTextSize50Startup != null) btnTextSize50Startup.onClick.AddListener(() => OnTextSizeButtonStartup(0));
        if (btnTextSize75Startup != null) btnTextSize75Startup.onClick.AddListener(() => OnTextSizeButtonStartup(1));
        if (btnTextSize100Startup != null) btnTextSize100Startup.onClick.AddListener(() => OnTextSizeButtonStartup(2));
        if (btnTextSize125Startup != null) btnTextSize125Startup.onClick.AddListener(() => OnTextSizeButtonStartup(3));

        // Connecter le slider de vitesse - STARTUP
        if (textSpeedSliderStartup != null)
        {
            textSpeedSliderStartup.minValue = 0.5f;
            textSpeedSliderStartup.maxValue = 2.0f;
            textSpeedSliderStartup.onValueChanged.AddListener(OnTextSpeedSliderStartup);
        }

        // Connecter les boutons de langue - MENU PRINCIPAL
        if (btnLangueFRMenuP != null) btnLangueFRMenuP.onClick.AddListener(() => OnLanguageButtonMenuP(1));
        if (btnLangueENMenuP != null) btnLangueENMenuP.onClick.AddListener(() => OnLanguageButtonMenuP(0));
        if (btnLangueKRMenuP != null) btnLangueKRMenuP.onClick.AddListener(() => OnLanguageButtonMenuP(2));

        // Connecter les boutons de taille de texte - MENU PRINCIPAL
        if (btnTextSize50MenuP != null) btnTextSize50MenuP.onClick.AddListener(() => OnTextSizeButtonMenuP(0));
        if (btnTextSize75MenuP != null) btnTextSize75MenuP.onClick.AddListener(() => OnTextSizeButtonMenuP(1));
        if (btnTextSize100MenuP != null) btnTextSize100MenuP.onClick.AddListener(() => OnTextSizeButtonMenuP(2));
        if (btnTextSize125MenuP != null) btnTextSize125MenuP.onClick.AddListener(() => OnTextSizeButtonMenuP(3));

        // Connecter le slider de vitesse - MENU PRINCIPAL
        if (textSpeedSliderMenuP != null)
        {
            textSpeedSliderMenuP.minValue = 0.5f;
            textSpeedSliderMenuP.maxValue = 2.0f;
            textSpeedSliderMenuP.onValueChanged.AddListener(OnTextSpeedSliderMenuP);
        }

        if (startupPanels != null && startupPanels.Count > 0)
        {
            startupIndex = 0;
            ShowStartupPanel(startupIndex);
        }
        else
        {
            OpenPanel(panelMainMenu);
        }

        if (ButtonEcran != null) ButtonEcran.onClick.AddListener(OnOpenEcranFromSettings_Button);
        if (ButtonVolume != null) ButtonVolume.onClick.AddListener(OnOpenVolumeFromSettings_Button);
        if (ButtonSoustitre != null) ButtonSoustitre.onClick.AddListener(OnOpenSoustitreFromSettings_Button);
    }

    // ---------- STARTUP SEQUENCE ----------
    void ShowStartupPanel(int index)
    {
        if (index < 0 || index >= startupPanels.Count)
        {
            OpenPanel(panelMainMenu);
            return;
        }
        OpenPanel(startupPanels[index]);
    }

    // Bouton "Suivant" pour passer au panel suivant dans la séquence startup
    public void OnStartupNext()
    {
        startupIndex++;
        if (startupIndex < startupPanels.Count)
            ShowStartupPanel(startupIndex);
        else
        {
            OpenPanel(panelMainMenu);
        }
    }

    // ---------- PANEL MANAGEMENT ----------
    public void OpenPanel(GameObject panelToOpen)
    {
        CloseAllPanels();
        if (panelToOpen != null) panelToOpen.SetActive(true);
    }

    public void CloseAllPanels()
    {
        if (startupPanels != null)
            foreach (var p in startupPanels) if (p != null) p.SetActive(false);

        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelSettingsRoot != null) panelSettingsRoot.SetActive(false);
        if (panelSettingEcranMenuP != null) panelSettingEcranMenuP.SetActive(false);
        if (panelSettingVolumeMenuP != null) panelSettingVolumeMenuP.SetActive(false);
        if (panelSettingSoustitreMenuP != null) panelSettingSoustitreMenuP.SetActive(false);
    }

    // ---------- RESOLUTIONS ----------
    void InitResolutions()
    {
        availableResolutions.Clear();
        var resArray = Screen.resolutions;
        var uniq = new HashSet<string>();
        foreach (var r in resArray)
        {
            string label = r.width + " x " + r.height;
            if (!uniq.Contains(label))
            {
                uniq.Add(label);
                availableResolutions.Add(r);
            }
        }
    }

    void PopulateResolutionDropdowns()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < availableResolutions.Count; i++)
            options.Add(availableResolutions[i].width + " x " + availableResolutions[i].height);

        if (resolutionDropdownStartup != null)
        {
            resolutionDropdownStartup.ClearOptions();
            resolutionDropdownStartup.AddOptions(options);
            resolutionDropdownStartup.onValueChanged.AddListener(OnResolutionChangedStartup);
        }

        if (resolutionDropdownMenuP != null)
        {
            resolutionDropdownMenuP.ClearOptions();
            resolutionDropdownMenuP.AddOptions(options);
            resolutionDropdownMenuP.onValueChanged.AddListener(OnResolutionChangedMenuP);
        }

        if (fullscreenToggleStartup != null)
            fullscreenToggleStartup.onValueChanged.AddListener(OnFullscreenToggleStartup);
        if (fullscreenToggleMenuP != null)
            fullscreenToggleMenuP.onValueChanged.AddListener(OnFullscreenToggleMenuP);

        int savedIdx = PlayerPrefs.GetInt(KEY_RES_INDEX, -1);
        bool savedFull = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;

        if (resolutionDropdownStartup != null) resolutionDropdownStartup.value = Mathf.Clamp(savedIdx, 0, availableResolutions.Count - 1);
        if (resolutionDropdownMenuP != null) resolutionDropdownMenuP.value = Mathf.Clamp(savedIdx, 0, availableResolutions.Count - 1);

        if (fullscreenToggleStartup != null) fullscreenToggleStartup.isOn = savedFull;
        if (fullscreenToggleMenuP != null) fullscreenToggleMenuP.isOn = savedFull;
    }

    public void OnResolutionChangedStartup(int idx) { ApplyResolutionIndex(idx); PlayerPrefs.SetInt(KEY_RES_INDEX, idx); }
    public void OnResolutionChangedMenuP(int idx) { ApplyResolutionIndex(idx); PlayerPrefs.SetInt(KEY_RES_INDEX, idx); }

    void ApplyResolutionIndex(int idx)
    {
        if (idx < 0 || idx >= availableResolutions.Count) return;
        var r = availableResolutions[idx];
        bool isFull = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
        Screen.SetResolution(r.width, r.height, isFull);
    }

    public void OnFullscreenToggleStartup(bool isOn) { ApplyFullscreen(isOn); PlayerPrefs.SetInt(KEY_FULLSCREEN, isOn ? 1 : 0); }
    public void OnFullscreenToggleMenuP(bool isOn) { ApplyFullscreen(isOn); PlayerPrefs.SetInt(KEY_FULLSCREEN, isOn ? 1 : 0); }

    void ApplyFullscreen(bool isFull)
    {
        int idx = PlayerPrefs.GetInt(KEY_RES_INDEX, 0);
        if (idx < 0 || idx >= availableResolutions.Count) idx = 0;
        var r = availableResolutions[idx];
        Screen.SetResolution(r.width, r.height, isFull);
    }

    // ---------- VOLUME ----------
    public void OnMasterVolumeChangedStartup(float v) { PlayerPrefs.SetFloat(KEY_MASTER, v); }
    public void OnVoixVolumeChangedStartup(float v) { PlayerPrefs.SetFloat(KEY_VOIX, v); }
    public void OnMusicVolumeChangedStartup(float v) { PlayerPrefs.SetFloat(KEY_MUSIC, v); }
    public void OnSfxVolumeChangedStartup(float v) { PlayerPrefs.SetFloat(KEY_SFX, v); }

    public void OnMasterVolumeChangedMenuP(float v) { PlayerPrefs.SetFloat(KEY_MASTER, v); }
    public void OnVoixVolumeChangedMenuP(float v) { PlayerPrefs.SetFloat(KEY_VOIX, v); }
    public void OnMusicVolumeChangedMenuP(float v) { PlayerPrefs.SetFloat(KEY_MUSIC, v); }
    public void OnSfxVolumeChangedMenuP(float v) { PlayerPrefs.SetFloat(KEY_SFX, v); }

    // ---------- SOUS-TITRE - LANGUE (via Boutons) ----------
    // idx: 0=EN, 1=FR, 2=KRÉOL
    public void OnLanguageButtonStartup(int idx)
    {
        PlayerPrefs.SetInt(KEY_LANG, idx);
        UpdateExampleTextStartup();
    }

    public void OnLanguageButtonMenuP(int idx)
    {
        PlayerPrefs.SetInt(KEY_LANG, idx);
        UpdateExampleTextMenuP();
    }

    // ---------- SOUS-TITRE - TAILLE TEXTE (via Boutons) ----------
    // idx: 0=50%, 1=75%, 2=100%, 3=125%
    public void OnTextSizeButtonStartup(int idx)
    {
        PlayerPrefs.SetInt(KEY_TEXTSIZE, idx);
        UpdateExampleTextStartup();
    }

    public void OnTextSizeButtonMenuP(int idx)
    {
        PlayerPrefs.SetInt(KEY_TEXTSIZE, idx);
        UpdateExampleTextMenuP();
    }

    // ---------- SOUS-TITRE - VITESSE (via Slider) ----------
    public void OnTextSpeedSliderStartup(float speed)
    {
        PlayerPrefs.SetFloat(KEY_TEXTSPEED, speed);
        UpdateExampleTextStartup();
    }

    public void OnTextSpeedSliderMenuP(float speed)
    {
        PlayerPrefs.SetFloat(KEY_TEXTSPEED, speed);
        UpdateExampleTextMenuP();
    }

    void UpdateExampleTextStartup()
    {
        if (exampleTextStartup == null) return;
        int lang = PlayerPrefs.GetInt(KEY_LANG, 1);
        int sizeIdx = PlayerPrefs.GetInt(KEY_TEXTSIZE, 2);
        float speed = PlayerPrefs.GetFloat(KEY_TEXTSPEED, 1f);

        string example = "Exemple de sous-titre (FR)";
        if (lang == 0) example = "Example subtitle (EN)";
        else if (lang == 2) example = "Egzanp soustit (KRÉOL)";

        exampleTextStartup.text = example + "\n\n[Vitesse: " + speed.ToString("0.00") + "]";

        float scale = MapSizeIndexToScale(sizeIdx);
        exampleTextStartup.fontSize = Mathf.RoundToInt(baseFontSizeStartup * scale);
    }

    void UpdateExampleTextMenuP()
    {
        if (exampleTextMenuP == null) return;
        int lang = PlayerPrefs.GetInt(KEY_LANG, 1);
        int sizeIdx = PlayerPrefs.GetInt(KEY_TEXTSIZE, 2);
        float speed = PlayerPrefs.GetFloat(KEY_TEXTSPEED, 1f);

        string example = "Exemple de sous-titre (FR)";
        if (lang == 0) example = "Example subtitle (EN)";
        else if (lang == 2) example = "Egzanp soustit (KRÉOL)";

        exampleTextMenuP.text = example + "\n\n[Vitesse: " + speed.ToString("0.00") + "]";

        float scale = MapSizeIndexToScale(sizeIdx);
        exampleTextMenuP.fontSize = Mathf.RoundToInt(baseFontSizeMenuP * scale);
    }

    float MapSizeIndexToScale(int idx)
    {
        switch (idx)
        {
            case 0: return 0.5f;
            case 1: return 0.75f;
            case 2: return 1.0f;
            case 3: return 1.25f;
            default: return 1.0f;
        }
    }

    void LoadSettingsToUI_Startup()
    {
        float master = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        float voix = PlayerPrefs.GetFloat(KEY_VOIX, 1f);
        float music = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 1f);

        if (masterSliderStartup != null) masterSliderStartup.value = master;
        if (voixSliderStartup != null) voixSliderStartup.value = voix;
        if (musicSliderStartup != null) musicSliderStartup.value = music;
        if (sfxSliderStartup != null) sfxSliderStartup.value = sfx;

        // Charger la vitesse de texte sauvegardée
        float speed = PlayerPrefs.GetFloat(KEY_TEXTSPEED, 1f);
        if (textSpeedSliderStartup != null) textSpeedSliderStartup.value = speed;

        UpdateExampleTextStartup();
    }

    void LoadSettingsToUI_MenuP()
    {
        float master = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        float voix = PlayerPrefs.GetFloat(KEY_VOIX, 1f);
        float music = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 1f);

        if (masterSliderMenuP != null) masterSliderMenuP.value = master;
        if (voixSliderMenuP != null) voixSliderMenuP.value = voix;
        if (musicSliderMenuP != null) musicSliderMenuP.value = music;
        if (sfxSliderMenuP != null) sfxSliderMenuP.value = sfx;

        // Charger la vitesse de texte sauvegardée
        float speed = PlayerPrefs.GetFloat(KEY_TEXTSPEED, 1f);
        if (textSpeedSliderMenuP != null) textSpeedSliderMenuP.value = speed;

        UpdateExampleTextMenuP();
    }

    // ---------- MAIN MENU BUTTONS ----------
    public void OnPlayButton() { SceneManager.LoadScene("FirstMap"); }
    public void OnOpenSettingsFromMain() { OpenPanel(panelSettingsRoot); }
    public void OnCreditsButton() { SceneManager.LoadScene("Credits"); }
    public void OnQuitButton() { Application.Quit(); }

    void OnOpenEcranFromSettings_Button()
    {
        if (panelSettingEcranMenuP == null) return;
        panelSettingVolumeMenuP?.SetActive(false);
        panelSettingSoustitreMenuP?.SetActive(false);
        panelSettingEcranMenuP.SetActive(true);
    }

    void OnOpenVolumeFromSettings_Button()
    {
        if (panelSettingVolumeMenuP == null) return;
        panelSettingEcranMenuP?.SetActive(false);
        panelSettingSoustitreMenuP?.SetActive(false);
        panelSettingVolumeMenuP.SetActive(true);
    }

    void OnOpenSoustitreFromSettings_Button()
    {
        if (panelSettingSoustitreMenuP == null) return;
        panelSettingEcranMenuP?.SetActive(false);
        panelSettingVolumeMenuP?.SetActive(false);
        panelSettingSoustitreMenuP.SetActive(true);
    }

    public void OnOpenEcranFromSettings() => OnOpenEcranFromSettings_Button();
    public void OnOpenVolumeFromSettings() => OnOpenVolumeFromSettings_Button();
    public void OnOpenSoustitreFromSettings() => OnOpenSoustitreFromSettings_Button();

    public void OnCloseSettingsToMain() { OpenPanel(panelMainMenu); }
}