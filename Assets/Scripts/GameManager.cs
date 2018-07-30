using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject startBtn;
    public GameObject ui;
    public void DeadCheck(int h)
    {
        if (h <= 0)
        {
            startBtn.SetActive(true);
            ui.SetActive(false);
        }
    }
}
