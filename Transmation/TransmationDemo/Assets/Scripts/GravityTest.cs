using System;
using System.Collections;
using System.Collections.Generic;
using TransMation;
using UnityEngine;

public class GravityTest : MonoBehaviour
{
    [SerializeField] private float _duration = 3.0f;
    [SerializeField] private TransMationReverseMode _reverseMode = TransMationReverseMode.None;
    private GravityHeightTransMation _gravityAnimation;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_gravityAnimation == null || 
                _gravityAnimation.State.State == TransMationStates.Ended)
                TestGravityAnimation();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _gravityAnimation.TogglePause();
        }
    }

    private void TestGravityAnimation()
    {
        Vector3 fromPosition = transform.position;
        _gravityAnimation = 
            new GravityHeightTransMation(fromPosition.y, fromPosition.y, _duration, 0.0f
            , null
            //, () => transform.position = new Vector3(fromPosition.x, _gravityAnimation.CurrentValue, fromPosition.z)
            );

        _gravityAnimation.SetReverseMode( _reverseMode);
        _gravityAnimation.Progressed += (s, e)
            => transform.position = new Vector3(fromPosition.x, _gravityAnimation.CurrentValue, fromPosition.z);
        StartCoroutine(_gravityAnimation.Animate());
    }
}
