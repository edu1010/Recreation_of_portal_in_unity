using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum GameStates
{
    PLAY=0,
    PAUSE
}
public class GameController : MonoBehaviour
{
    static GameController m_GameController=null;
    private LevelData m_levelData;

    private FPSController m_Player;
    GameObject m_Canvas;
    CanvasGroup m_PlayerHud;
    CanvasGroup m_DeathHud;
    CanvasGroup m_VictoryHud;
    GameStates m_GameState;


    public void Awake() // pq sino en otros starts puede que gameController aun no exista, por eso awake. Sino seria modificar el ExecutionOrder
    {
        if (m_GameController == null)
        {
            m_GameController = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(this); // ya existe, no hace falta crearla
        }
        Application.targetFrameRate = 60;
        m_GameState = GameStates.PLAY;
    }
    public void ResetLevel()
    {
        m_levelData.RestartList();
        ShowPlayerHud();
    }

    static public GameController GetGameController()
    {
        return m_GameController;
    }

    public void SetLevelData(LevelData _LevelData)
    {
        m_levelData = _LevelData;
    }
    public LevelData GetLevelData()
    {
        return m_levelData;
    }
    public void SetPlayer(FPSController _Player)
    {
        m_Player = _Player;
    }
    public FPSController GetPlayer()
    {
        return m_Player;
    }
    public void SetCanvas(GameObject canvas)
    {
        m_Canvas = canvas;
        m_PlayerHud = ((m_Canvas.GetComponentsInChildren<TypeOfCanvas>().Where(x => x.m_type == CanvasType.PlayerHud)).ToArray())[0].GetComponent<CanvasGroup>();
        m_DeathHud = ((m_Canvas.GetComponentsInChildren<TypeOfCanvas>().Where(x => x.m_type == CanvasType.DeathHud)).ToArray())[0].GetComponent<CanvasGroup>();
        m_VictoryHud = ((m_Canvas.GetComponentsInChildren<TypeOfCanvas>().Where(x => x.m_type == CanvasType.WinCanvas)).ToArray())[0].GetComponent<CanvasGroup>();
    }  
    public GameObject GetCanvas()
    {
        return m_Canvas;
    }
    public void ShowDeathHud()
    {
        m_GameState = GameStates.PAUSE;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        HideCanvasGroup(m_PlayerHud);
        ShowCanvasGroup(m_DeathHud);
        HideCanvasGroup(m_VictoryHud);
    }
    public void ShowPlayerHud()
    {
        m_GameState = GameStates.PLAY;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        ShowCanvasGroup(m_PlayerHud);
        HideCanvasGroup(m_DeathHud);
        HideCanvasGroup(m_VictoryHud);
    }

    public void ShowVictoryHud()
    {
        m_GameState = GameStates.PAUSE;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        HideCanvasGroup(m_PlayerHud);
        HideCanvasGroup(m_DeathHud);
        ShowCanvasGroup(m_VictoryHud);
    }
    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public GameStates GetGameStates()
    {
        return m_GameState;
    }
    public void SetGameStates(GameStates gameStates)
    {
        m_GameState = gameStates;
    }
}
