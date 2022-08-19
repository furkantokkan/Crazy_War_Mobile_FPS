using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image circleBarFill;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;

    private GameObject player;

    public List<Button> selectButtons;


    private int score;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }
    void Start()
    {
        GameManager.onEnemyDie += OnScoreChange;
        GameManager.onselectStart += OnSelectStart;
    }
    private void OnDisable()
    {
        GameManager.onEnemyDie -= OnScoreChange;
        GameManager.onselectStart -= OnSelectStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        circleBarFill.fillAmount = (float)player.GetComponent<Health>().GetCurrentHealth() / player.GetComponent<Health>().maxHealth;
        ammoText.text = player.GetComponent<FireController>().currentWeapon.GetMaxAmmo.ToString();
    }

    private void OnScoreChange(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    private void OnSelectStart()
    {
        foreach (var item in GameManager.instance.gameUI)
        {
            if (GameManager.instance == null)
            {
                return;
            }
            if (item.gameObject == null)
            {
                return;
            }
            item.gameObject.SetActive(false);
        }

        GameManager.instance.selectionMenu.gameObject.SetActive(true);
    }

    public void OnClickSelectButton(string id)
    {
        if (id == "Weapon")
        {
            int selectedIndex = 0;
            do
            {
                selectedIndex = Random.Range(0, GameManager.instance.allWeapon.Count);
            } while (GameManager.instance.allWeapon[selectedIndex] == player.GetComponent<FireController>().currentWeapon);

            player.GetComponent<FireController>().EquipWeapon(GameManager.instance.allWeapon[selectedIndex]);
                
        }
        else if (id == "Health")
        {
            player.GetComponent<Health>().SetHealth(player.GetComponent<Health>().maxHealth);
        }
        else if (id == "Special")
        {
            if (PlayerPrefs.GetInt("Ammo") >= 1)
            {
                return;
            }
            IAP.Instance.BuyProductID("unlimited.bullet");
        }

        WaveManager.currentSpawnState = WaveManager.SpawnState.Counting;

        foreach (var item in GameManager.instance.gameUI)
        {
            item.gameObject.SetActive(true);
        }

        GameManager.instance.selectionMenu.gameObject.SetActive(false);
    }
}
