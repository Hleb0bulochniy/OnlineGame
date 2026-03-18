using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FoodSpawner : MonoBehaviour
{
    [Header("FoodParametres")]
    [SerializeField] private GameObject _foodPrefab;
    [SerializeField] private int _foodCountTarget = 100;
    [SerializeField] private int _foodCount = 0;

    [Header("SpawnField")]
    [SerializeField] private int _minX = 0;
    [SerializeField] private int _maxX = 0;
    [SerializeField] private int _minY = 0;
    [SerializeField] private int _maxY = 0;

    [Header("Coroutine")]
    [SerializeField] private Coroutine _refillFoodCountCoroutine;

    [Header("Actions")]
    public UnityAction OnFoodAteAction;

    void Start()
    {
        while (_foodCount < _foodCountTarget)
        {
            CreateFood();
        }
    }

    private void OnEnable() => Food.OnAteFoodAction += OnFoodAte;
    private void OnDisable() => Food.OnAteFoodAction -= OnFoodAte;

    private void CreateFood()
    {
        Vector3 pos = new Vector3(Random.Range(_minX, _maxX), 0, Random.Range(_minY, _maxY));
        GameObject newFood = PhotonNetwork.Instantiate(_foodPrefab.name, pos, Quaternion.identity);
        //GameObject newFood = Instantiate(_foodPrefab, pos, Quaternion.identity);
        _foodCount++;
        //newFood.SetActive(true);
    }

    private void CheckAndRefillFoodCount()
    {
        Debug.Log("CheckAndRefillFoodCount");
        if (_refillFoodCountCoroutine != null)
        {
            Debug.Log("CheckAndRefillFoodCount2");
            StopCoroutine(_refillFoodCountCoroutine);
            _refillFoodCountCoroutine = null;
        }
        if (_foodCount < _foodCountTarget)
        {
            Debug.Log("CheckAndRefillFoodCount1");
            _refillFoodCountCoroutine = StartCoroutine(RefillFoodCountCoroutine());
        }
        
    }

    private IEnumerator RefillFoodCountCoroutine()
    {
        //Debug.Log("Стартует корутина пополнения еды");
        while (_foodCount < _foodCountTarget)
        {
            yield return new WaitForSeconds(1);
            CreateFood();
        }
        _refillFoodCountCoroutine = null;
    }

    public void OnFoodAte(Food food)
    {
        _foodCount--;
        CheckAndRefillFoodCount();
    }
}