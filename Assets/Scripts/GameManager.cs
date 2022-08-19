using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public const string BULLET_BUFF_KEY = "Bullet";
    public const string HEALTH_BUFF_KEY = "Health";
    public const string DAMAGE_BUFF_KEY = "Damage";

    public enum GameState
    {
        Normal,
        Selecting,
        Lose
    }

    public List<GameObject> allEnemy = new List<GameObject>();
    public List<Weapon> allWeapon = new List<Weapon>();
    public List<GameObject> allDrop = new List<GameObject>();

    public List<GameObject> gameUI = new List<GameObject>();
    public GameObject selectionMenu;

    public static GameManager instance;

    public static Action onselectStart;
    public static Action onselectEnd;
    public static Action<bool,float> onBuffTake;
    public static Action<int> onEnemyDie;

    public List<Enemy> allEnemies = new List<Enemy>();

    public GameState currentState = GameState.Normal;

    public static bool unlimitedAmmo = false;

    public static int attackingCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        attackingCount = 0;
        unlimitedAmmo = PlayerPrefs.GetInt("Ammo") >= 1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
