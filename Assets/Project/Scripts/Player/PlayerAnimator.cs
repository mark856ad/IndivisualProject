using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponSwitcher switcher;
    [SerializeField] private float dampTime = 0.1f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float reloadClipLength = 2f;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveZHash = Animator.StringToHash("MoveZ");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int ReloadingHash = Animator.StringToHash("Reloading");
    private static readonly int ReloadSpeedHash = Animator.StringToHash("ReloadSpeed");

    private Rigidbody rb;
    private Weapon current;
    private float lastLogTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator를 찾지 못했습니다. Player 아래 Model에 Animator를 붙이세요.", this);
            enabled = false;
            return;
        }

        if (rb == null)
        {
            Debug.LogError("Player에 Rigidbody가 없습니다.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (switcher != null) switcher.WeaponEquipped += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        if (switcher != null) switcher.WeaponEquipped -= OnWeaponEquipped;

        if (current != null)
        {
            current.Fired -= OnFired;
            current = null;
        }
    }

    private void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        Vector3 local = transform.InverseTransformDirection(velocity) / Mathf.Max(0.1f, maxSpeed);

        animator.SetFloat(MoveXHash, local.x, dampTime, Time.deltaTime);
        animator.SetFloat(MoveZHash, local.z, dampTime, Time.deltaTime);

        bool reloading = current != null && current.IsReloading;

        if (reloading)
        {
            animator.SetFloat(ReloadSpeedHash, reloadClipLength / Mathf.Max(0.1f, current.ReloadTime));
        }

        if (Time.time - lastLogTime >= 1f)
        {
            lastLogTime = Time.time;
        }
    }

    private void OnWeaponEquipped(Weapon weapon)
    {
        if (current != null)
        {
            current.Fired -= OnFired;
        }

        current = weapon;
        current.Fired += OnFired;
    }

    private void OnFired()
    {
        animator.SetTrigger(ShootHash);
    }
}