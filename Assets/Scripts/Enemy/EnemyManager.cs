using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private EnemyEffects _enemyEffects;
    private EnemyWeaponBehavior _enemyWeaponBehavior;
    private AudioSource _audioSource;


    [SerializeField] private string _enemyName;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private GameObject _enemyInfos;
    private Slider _enemyHpSlider;
    private TextMeshProUGUI _enemyNameTxt;
    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _enemyEffects = GetComponent<EnemyEffects>();
        _enemyWeaponBehavior = GetComponentInChildren<EnemyWeaponBehavior>();
        _audioSource = GetComponent<AudioSource>();

        _enemyHpSlider = GetComponentInChildren<Slider>();
        _enemyNameTxt = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if(_enemyInfos != null)
        {
            _enemyNameTxt.text = _enemyName;
            _enemyHpSlider.maxValue = _enemyStats.maxHp;
            _enemyInfos.SetActive(false);
        }  
    }

    private void Update()
    {
       ControlEnemyCanvas();
    }

    private void ControlEnemyCanvas()
    {
        if (_enemyInfos != null)
            _enemyHpSlider.value = _enemyStats.currentHp;


        if (PlayerManager.Instance != null && _enemyInfos != null)
        {
            float distance = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

            if (isBoss)
            {
                if (distance <= 50f)
                    _enemyInfos.SetActive(true);
                else
                    _enemyInfos.SetActive(false);
            }
            else
            {
                if (distance <= 15f)
                {
                    _enemyInfos.SetActive(true);
                    Vector3 direction = PlayerManager.Instance.transform.position - _enemyInfos.transform.position;
                    direction.y = 0;

                    _enemyInfos.transform.rotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    _enemyInfos.SetActive(false);
                }
            }
        }
    }

    public void ActiveWeaponCollider()
    {
        _enemyWeaponBehavior.CallCollider();
    }

    public void AttackSwoosh()
    {
        _audioSource.PlayOneShot(_enemyEffects.attackSound);
    }

    public void ReableBlockedAttack()
    {
        EnemyWeaponBehavior[] weaponBehaviors = GetComponentsInChildren<EnemyWeaponBehavior>();

        foreach (var weaponBehavior in weaponBehaviors)
        {
            weaponBehavior.attackBlockedByPlayer = false;
        }
    }

    public void CallProjectile()
    {
        _enemyWeaponBehavior.CallProjectile();
    }

    public void DropXp()
    {
        for (int i = 0; i < _enemyStats.xpCarryed; i++)
        {
            GameObject xpOrb = Instantiate(_enemyEffects.xpOrb, transform.position, Quaternion.identity);
        }
    }
}
