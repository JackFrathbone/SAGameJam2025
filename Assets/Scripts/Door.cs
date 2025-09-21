using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] List<EnemyController> _linkedEnemies = new();
    [SerializeField] List<DoorSwitch> _linkedSwitches = new();

    [Header("References")]
    [SerializeField] GameObject _doorModel;
    [SerializeField] GameObject _doorModelLocked;

    [Header("Data")]
    private bool _breakable;

    private void FixedUpdate()
    {
        _linkedEnemies.RemoveAll(item => item == null);
        _linkedSwitches.RemoveAll(item => item == null);

        if (_linkedEnemies.Count == 0 && _linkedSwitches.Count == 0)
        {
            _doorModel.SetActive(true);
            _doorModelLocked.SetActive(false);

            _breakable = true;
        }
        else
        {
            _doorModel.SetActive(false);
            _doorModelLocked.SetActive(true);

            _breakable = false;
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
