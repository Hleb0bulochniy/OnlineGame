using UnityEngine;
using UnityEngine.Events;

public class Food : MonoBehaviour
{
    public static UnityAction<Food> OnAteFoodAction;
    public UnityAction OnEatFoodAction;

    private void OnEnable() => OnEatFoodAction += Eat;
    private void OnDisable() => OnEatFoodAction += Eat;

    public void Eat()
    {
        OnAteFoodAction?.Invoke(this);
        Destroy(gameObject);
    }
}
