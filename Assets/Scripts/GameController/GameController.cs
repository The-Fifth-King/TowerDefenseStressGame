using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField] private Tilemap gameField;
    [SerializeField] private Tile gameFieldTile;

    [SerializeField] private TextMeshProUGUI liveText;  //Cooler text für leben
    
    public GameObject[] hotBar;
    private int _currentHotBarIndex;
    private GameObject _silhouette;
    private int CurrentHotBarIndex
    {
        get => _currentHotBarIndex;
        set
        {
            var clampedValue = Mathf.Clamp(value, 0, hotBar.Length - 1);
            if (clampedValue == CurrentHotBarIndex || _silhouette == null) return;
            _currentHotBarIndex = clampedValue;
            HotbarIndexChanged?.Invoke(_currentHotBarIndex);
            var pos = _silhouette.transform.position;
            Destroy(_silhouette);
            CreateSilhouette(pos);
        }
    }
    public event Action<int> HotbarIndexChanged;

    private Spawn _spawn;
    
    private CircuitController _circuitController;
    private Transform _circuitComponents;

    public bool isEnemyPhase = false;
    [SerializeField] private int money = 20;
    [SerializeField] private int lives = 20;
    private int score;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private void Awake()
    {
        _spawn = FindObjectOfType<Spawn>();
        _circuitController = GetComponent<CircuitController>();
        _circuitComponents = GameObject.FindWithTag("EntityController").transform.Find("CircuitComponents");

        liveText.text = "" + lives;
    }

    private bool IsSpaceFilled(Vector3 pos)
    {
        return gameField.HasTile(WorldToCell(pos));
    }
    public void PlaceCircuitComponent(Vector3 pos)
    { 
        if (isEnemyPhase || IsSpaceFilled(pos)) return;

        var towerToPlace = hotBar[_currentHotBarIndex];
        if (!consumeMoney(towerToPlace.GetComponent<CircuitComponent>().towerCost))
        {
            //not enough money to place
            //vielleicht noch irgendein Fail sound
            return;
        }
        var cellPos = WorldToCell(pos);
        gameField.SetTile(cellPos, gameFieldTile);
        var circuitComponent = 
            Instantiate(towerToPlace, WorldToGrid(pos), Quaternion.identity, _circuitComponents);

        var component = circuitComponent.GetComponent<CircuitComponent>();
        component.powerGridPos = cellPos;
        component.placedByPlayer = true;
        _circuitController.AddComponent(component, cellPos);
    }

    public void DeleteCircuitComponent(Vector3 pos)
    {
        if (!IsSpaceFilled(pos)) return;
        
        var cellPos = WorldToCell(pos);
        _circuitController.DeleteComponent(cellPos, gameField);
    }

    public void UpdateSilhouette(Vector3 pos)
    {
        if (isEnemyPhase)
        {
            return;
        }
        
        if (IsSpaceFilled(pos))
        {
            //Debug.Log("space filled");
            DestroySilhouette();
            return;
        }
        
        if (_silhouette == null)
        {
            //Debug.Log("silhouette null");
            CreateSilhouette(pos);
        }
        else
        {
            MoveSilhouette(pos);
        }
    }
    private void MoveSilhouette(Vector3 pos) => _silhouette.transform.position = WorldToGrid(pos);
    private void CreateSilhouette(Vector3 pos)
    {
        _silhouette = Instantiate(hotBar[_currentHotBarIndex], WorldToGrid(pos), Quaternion.identity);
        foreach (var sr in _silhouette.GetComponentsInChildren<SpriteRenderer>())
        {
            var color = Color.black;
            color.a = 0.5f;
            sr.color = color;
        }
    }
    private void DestroySilhouette()
    {
        Destroy(_silhouette);
        _silhouette = null;
    }

    
    public Vector3Int WorldToCell(Vector3 pos) => gameField.WorldToCell(pos);
    public Vector3 CellToGrid(Vector3Int pos) => gameField.CellToWorld(pos);
    public Vector3 WorldToGrid(Vector3 pos) => gameField.CellToWorld(gameField.WorldToCell(pos));

    public void SpawnWave()
    {
        if (isEnemyPhase)
        {
            return;
        }
        
        isEnemyPhase = true;
        DestroySilhouette();
        StartCoroutine(_spawn.SpawnWave());
    }
    public void TakeHit(int damage)
    {
        Debug.Log("Enemy came through! Recieved Damage: " + damage);
        
        lives -= damage;
        liveText.text = "" + lives;
        
        if(lives <= 0)
            OnLose();
        
    }

    private void OnLose()
    {
        Debug.Log("A Noob lost the game!");
        SceneManager.LoadScene("Main Menu(Lydia)");
    }
    
    public void increaseMoney(int amount)
    {
        money += amount;
    }

    public bool consumeMoney(int amount)
    {
        if (money - amount < 0)
        {
            return false;
        }

        money -= amount;
        return true;
    }

    public void increaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    public void SetCurrentHotBarIndex(int value) => CurrentHotBarIndex = value;
    public void IncreaseCurrentHotBarIndex() => CurrentHotBarIndex++;
    public void DecreaseCurrentHotBarIndex() => CurrentHotBarIndex--;
}
