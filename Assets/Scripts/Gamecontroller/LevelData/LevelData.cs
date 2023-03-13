using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public List<GameObject> m_RestartObjects;
    List<IRestart> m_ObjectsWithRestart;
    public List<Transform> m_CheckPoints;

    

    private void Awake()
    {
        GameController.GetGameController().SetLevelData(this);
    }
    public void RestartList()
    {
        for (int i = 0; i < m_RestartObjects.Count; i++)
        {
            IRestart l_RestartObject = m_RestartObjects[i].GetComponent<IRestart>();
            if (l_RestartObject != null)
            {
                l_RestartObject.Restart();
            }
            m_RestartObjects[i].SetActive(true);
        }
    }
    public List<IRestart> GetRestartList()
    {
        return m_ObjectsWithRestart;
    }
    public List<Transform> GetCheckpointsList()
    {
        return m_CheckPoints;
    }
}
