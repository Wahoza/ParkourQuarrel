using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameSettingsManager Instance { get; private set; }
    private int GameTime;
    private int PointsPerSecond;
    private int PointsForPlacement;
    private int RequiredPoints;
    private int noPlacedDivider = 3;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create new instance");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public void SetGameTime(int time)
    {
        GameTime = time;
    }
    public void SetPointsPerSecond(int points) 
    { 
        PointsPerSecond = points;
    }
    public void SetPointsForPlacement(int points)
    {
        PointsForPlacement = points;
    }    
    public void SetRequiredPoints(int points)
    {
        RequiredPoints = points;
    }
    public int GetGameMaxTime()
    {
        return GameTime;
    }
    public int GetPointsForPlacement()
    {
        return PointsForPlacement;
    }
    public int GetPointsPerSecond()
    {
        return PointsPerSecond;
    }    
    public int GetRequiredPoints()
    {
        return RequiredPoints;
    }
    public int GetNoPlacedDivider()
    {
        return noPlacedDivider;
    }
}
