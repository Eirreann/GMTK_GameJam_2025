using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private bool _isPersistent;
    
    public PlayerController Player;
    [HideInInspector] public InputHandler inputHandler;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        Init(_isPersistent);
    }

    public override void Init(bool isPersist = false)
    {
        base.Init(isPersist);
    }
}
