using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startBtn;
    public GameObject ui;
    public SlenderBehaviour slender;
    public PlayerBehaviour player;

    public void DeadCheck(int h)
    {
        if (h <= 0)
        {
            startBtn.SetActive(true);
            ui.SetActive(false);
            player.health = 100;
            slender.health = 100;
        }
    }
}
