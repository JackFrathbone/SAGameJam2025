using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] List<EnemyController> _linkedEnemies = new();
    [SerializeField] List<DoorSwitch> _linkedSwitches = new();
    [SerializeField] bool _bossDoor = false;

    [Header("References")]
    [SerializeField] GameObject _doorModelBreakable;
    [SerializeField] GameObject _doorModelRed;
    [SerializeField] GameObject _doorModelGreen;
    [SerializeField] GameObject _doorModelGold;

    [Header("Data")]
    private bool _breakable;

    private void Start()
    {
        _doorModelBreakable.SetActive(false);
        _doorModelRed.SetActive(false);
        _doorModelGreen.SetActive(false);
        _doorModelGold.SetActive(false);

        if (_linkedEnemies.Count != 0)
        {
            _doorModelGreen.SetActive(true);
        }
        else if (_linkedSwitches.Count != 0)
        {
            _doorModelRed.SetActive(true);
        }
        else if (_bossDoor)
        {
            _doorModelGold.SetActive(true);
        }
        else
        {
            _breakable = true;
            _doorModelBreakable.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        _linkedEnemies.RemoveAll(item => item == null);
        _linkedSwitches.RemoveAll(item => item == null);

        if (_linkedEnemies.Count == 0 && _linkedSwitches.Count == 0 && !_breakable)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    public void TryBreak()
    {
        if (_breakable)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
