using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DoorSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _hitsRequired = 1;
    public UnityEvent OnSwitchActivate;

    [SerializeField] Gradient _colorGradient;

    [Header("References")]
    private Material _material;
    private Animator _animator;

    [Header("Data")]
    private int _hitCount;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _animator = GetComponent<Animator>();

        _animator.enabled = false;
        _material.color = _colorGradient.Evaluate(0f);
    }

    private void OnDisable()
    {
        DOTween.Kill(_material);
    }

    public void HitSwitch()
    {
        _hitCount++;

        _material.DOColor(_colorGradient.Evaluate((float)_hitCount / (float)_hitsRequired), 1f).SetEase(Ease.OutBounce);

        if (_hitCount >= _hitsRequired)
        {
            ActivateSwitch();
        }
    }

    private void ActivateSwitch()
    {
        _material.DOColor(_colorGradient.Evaluate(1f), 1f).SetEase(Ease.OutBounce);

        _animator.enabled = true;

        OnSwitchActivate.Invoke();

        Destroy(this);
    }
}
