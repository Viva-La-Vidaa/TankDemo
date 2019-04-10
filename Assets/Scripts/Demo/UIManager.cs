using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //玩家UI
    public Image playerIco;
    public Text txtPlayerName;
    public Text txtPlayerLevel;
    public Slider playerHpSlider;
    public Slider playerExpSlider;
    public Text txtPlayerHp;
    public Text txtPlayerExp;

    public GameObject playerUI;

    //敌人UI
    public Image enemyIco;
    public Text txtEnemyName;
    public Text txtEnemyLevel;
    public Slider enemyHpSlider;
    public Text txtEnemyHp;
    public Slider enemyExpSlider;
    public Text txtEnemyExp;

    public GameObject enemyUI;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideEnemyUI()
    {
        if (enemyUI.activeSelf)
        {
            enemyUI.SetActive(false);
        }
    }

    public void ShowPlayerUI(string username, int level, int hp, int exp)
    {
        if (!playerUI.activeSelf)
        {
            txtPlayerName.text = username;
            playerUI.SetActive(true);
        }
        txtPlayerLevel.text = level.ToString();
        int tempHpTotal = 100 + 50 * (level - 1);
        playerHpSlider.value = (float)hp / tempHpTotal;
        txtPlayerHp.text = hp + "/" + tempHpTotal;
        txtPlayerExp.text = exp + "/100";
        playerExpSlider.value = exp / 100.0f;
    }

    /// <summary>
    /// 击中时，显示敌人信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="level"></param>
    /// <param name="hp"></param>
    /// <param name="exp"></param>
    public void ShowEnemyUI(string username,int level,int hp,int exp)
    {
        if (!enemyUI.activeSelf) {
            txtEnemyName.text = username;
            enemyUI.SetActive(true);
        }
        txtEnemyLevel.text = level.ToString();
        int tempHpTotal = 100 + 50 * (level - 1);
        enemyHpSlider.value = (float)hp / tempHpTotal;
        txtEnemyHp.text = hp + "/" + tempHpTotal;
        txtEnemyExp.text = exp + "/100";
        enemyExpSlider.value = exp / 100.0f;
    }
}
