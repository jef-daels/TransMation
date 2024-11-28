using System;
using System.Collections;
using TransMation;
using UnityEngine;

public class TestTransMation : MonoBehaviour
{
    [SerializeField] private float _duration = 3.0f;
    [SerializeField] private float _spirals = 3.0f;
    [SerializeField] private TransMationReverseMode _reverseMode = TransMationReverseMode.None;
    private TransMation<Vector3> _moveTransMation;
    private TransMation<Quaternion> _rotateQTransMation;
    private TransMation<float> _rotateEulerYTransMation;
    private TransMation<Color> _colorTransMation;
    private SpiralTransMation _spiralTransMation;
    [SerializeField] private GameObject _spiralCenter;
    [SerializeField] private GameObject _spiralEnd;

    [SerializeField] private Renderer _renderer;

    private Color _initColor;
    // Start is called before the first frame update
    void Start()
    {
        _initColor = _renderer.material.color;
    }


    private void SpiralAnimation()
    {
        _spiralTransMation = (SpiralTransMation)(new SpiralTransMation(_spiralCenter.transform.position,
            _spiralEnd.transform.position
            , 0.1f, _spirals
            )
            .SetDuration(3)
            .SetReverseMode(TransMationReverseMode.DoubleDuration)
            .SetMaxIterations(1))
            ;
        _spiralTransMation.Progressed += (s, e) => transform.localPosition = _spiralTransMation.CurrentValue;
        //ABC();
        StartCoroutine(_spiralTransMation.Animate());
    }

    //private void ABC()
    //{
    //    Vector3 test = _spiralTransMation.LerpFunction(_spiralTransMation.From, _spiralTransMation.To, 0);
    //    Debug.Log($"From: {_spiralTransMation.From}, To: {_spiralTransMation.To}, 0: {test}");
    //    test = _spiralTransMation.LerpFunction(_spiralTransMation.From, _spiralTransMation.To, 1);
    //    Debug.Log($"From: {_spiralTransMation.From}, To: {_spiralTransMation.To}, 1: {test}");
    //}

    private void MoveToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 toPosition = ray.GetPoint(10);

        _moveTransMation = new TransMation<Vector3>(
            //Vector3.Lerp
            TransMation<Vector3>.TechAnimationUtilities.SinLerpFunction(Vector3.Lerp)
            )
            .SetFrom(transform.position)
            .SetTo(toPosition)
            .SetDuration(_duration)
            .SetDelay(0)
            .SetReverseMode(_reverseMode)
            .SetMaxIterations(3)
            ;
        _moveTransMation.Progressed += (s, e)
            => transform.position = _moveTransMation.CurrentValue;
        StartCoroutine(_moveTransMation.Animate());

        Quaternion fromQ = transform.rotation;
        transform.Rotate(Vector3.up, 180);  //Quaternion rotations are modulo 360°
        Quaternion toQ = transform.rotation;
        transform.rotation = fromQ;
        _rotateQTransMation = new TransMation<Quaternion>(Quaternion.Lerp
            )
            .SetFrom(fromQ)
            .SetTo(toQ)
            .SetDuration(_duration)
            .SetReverseMode(_reverseMode)
            .SetMaxIterations(3)
            ;
        _rotateQTransMation.Progressed += (s, e)
            => transform.rotation = _rotateQTransMation.CurrentValue;
        //StartCoroutine(_rotateQTransMation.Animate());

        float fromEulerY = transform.eulerAngles.y;
        float toEulerY = fromEulerY + 360;
        _rotateEulerYTransMation = new TransMation<float>(TransMation<float>.TechAnimationUtilities.SinLerpFunction(Mathf.Lerp)
            )
            .SetFrom(fromEulerY)
            .SetTo(toEulerY)
            .SetDuration(_duration)
            .SetReverseMode(_reverseMode)
            .SetMaxIterations(3);
            ;
        _rotateEulerYTransMation.Progressed += (s, e)
            => transform.eulerAngles = new Vector3(0, _rotateEulerYTransMation.CurrentValue, 0);
        StartCoroutine(_rotateEulerYTransMation.Animate());

        Color toColor = new Color(1, 0, 0, 0);
        Color fromColor = _renderer.material.color;
        if (toColor == fromColor)
        {
            toColor = _initColor;
        }
        _colorTransMation = new TransMation<Color>
            (TransMation<Color>.TechAnimationUtilities.SinLerpFunction(Color.Lerp)
            )
            .SetFrom(fromColor)
            .SetTo(toColor)
            .SetDuration(_duration)
            //.SetOnProgress(() => _renderer.material.color = _colorTransMation.CurrentValue)
            .SetReverseMode(_reverseMode)
            .SetMaxIterations(3);
        ;
        _colorTransMation.Progressed += (s, e)
            => _renderer.material.color = _colorTransMation.CurrentValue;
        StartCoroutine(_colorTransMation.Animate());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_moveTransMation == null || _moveTransMation.State.State == TransMationStates.Ended)
                MoveToMouse();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (_spiralTransMation == null || _spiralTransMation.State.State == TransMationStates.Ended)
                SpiralAnimation();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _moveTransMation.TogglePause();
        }
    }
}
