using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    [SerializeField] Toggle FullScreenToggle;
    bool isFullScreen = true;
    string FullScreenPref = "fullScreenPref";

    [SerializeField] TMP_Dropdown RefreshRateDropdown;
    int selectedRefreshRate = 1;
    List<int> RefreshRates = new List<int> { 240, 144, 120, 60, 30 };
    string RefreshRatePref = "refreshRate";

    [SerializeField] TMP_Dropdown ResolutionDropdown;
    List<Vector2Int> Resolution = new List<Vector2Int> {new Vector2Int(3840, 2160),  new Vector2Int(2560, 1440), new Vector2Int(1920, 1080), new Vector2Int(1280, 720) };
    int selectedResolution = 2;
    string ResolutionPref = "resolutionPref";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey(FullScreenPref) && PlayerPrefs.HasKey(RefreshRatePref) && PlayerPrefs.HasKey(ResolutionPref))
        {
            LoadScreenSettings();
        }
        else
        {

            ResolutionDropdown.value = selectedResolution;
            RefreshRateDropdown.value = selectedRefreshRate;
            SetFullscreen();
            SetRefreshRate();
            SetResolution();
        }
    }

    // Update is called once per frame
    public void SetFullscreen() {
        isFullScreen = FullScreenToggle.isOn;
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt(FullScreenPref, isFullScreen ? 1 : 0);
    }

    public void SetResolution() {
        if(ResolutionDropdown.value > Resolution.Count - 1)
        {
            return;
        }
        selectedResolution = ResolutionDropdown.value;
        PlayerPrefs.SetInt(ResolutionPref, selectedResolution);
        Screen.SetResolution(Resolution[selectedResolution].x, Resolution[selectedResolution].y, Screen.fullScreen);
    }
    public void SetRefreshRate() {
        if(RefreshRateDropdown.value > RefreshRates.Count - 1) { return; }
        selectedRefreshRate = RefreshRateDropdown.value;
        Application.targetFrameRate = RefreshRates[selectedRefreshRate];
        PlayerPrefs.SetInt(RefreshRatePref, selectedRefreshRate);
    }
    public void LoadScreenSettings()
    {
        isFullScreen = PlayerPrefs.GetInt(FullScreenPref) == 1 ? true : false;
        FullScreenToggle.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;

        selectedRefreshRate = PlayerPrefs.GetInt(RefreshRatePref);
        Application.targetFrameRate = RefreshRates[selectedRefreshRate];
        RefreshRateDropdown.value = selectedRefreshRate;

        selectedResolution = PlayerPrefs.GetInt(ResolutionPref);
        Screen.SetResolution(Resolution[selectedResolution].x, Resolution[selectedResolution].y, Screen.fullScreen);
        ResolutionDropdown.value = selectedResolution;
    }
    public void SaveScreenSettings()
    {
        SetResolution();
        SetRefreshRate();
        SetFullscreen();
    }
}
