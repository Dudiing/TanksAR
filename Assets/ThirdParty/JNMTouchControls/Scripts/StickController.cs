using UnityEngine;
using System.Collections;
using System;

public enum EnStickType
{
    Static = 0,
    Dynamic = 1
}

public class StickEventArgs : EventArgs
{
    private Vector2 mPosition;

    internal StickEventArgs(Vector2 position)
    {
        mPosition = position;
    }

    public Vector2 Position
    {
        get { return mPosition; }
    }
}

public class StickController : MonoBehaviour
{
    #region Properties

    public EnStickType StickType;
    public bool AllowX = true;
    public bool AllowY = true;

    #endregion

    #region Protected members

    protected RectTransform _button;
    protected RectTransform _buttonFrame;
    protected float _dragRadius = 0.0f;

    #endregion

    #region Private members

    private int _buttonId = -1;
    private Vector3 _startPos = Vector3.zero;

    #endregion

    public event EventHandler<StickEventArgs> StickChanged;

    protected virtual void Start()
    {
        _button = this.transform.Find("_stick").GetComponent<RectTransform>();
        _buttonFrame = this.transform.Find("_stickFrame").GetComponent<RectTransform>();
        _dragRadius = _buttonFrame.rect.width / 2.0f;

        HideDynamic();
    }

    private void HideDynamic()
    {
        if (StickType == EnStickType.Dynamic)
        {
            _buttonFrame.gameObject.SetActive(false);
            _button.gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        HandleTouchInput();

        #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

        if (Input.GetMouseButtonDown(0))
        {
            if (CheckButtonDown(Input.mousePosition))
            {
                _buttonId = 1;
                ShowDynamic();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ButtonUp();
        }

        #endif
    }

    protected virtual void FixedUpdate()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
        HandleInput(Input.mousePosition);
        #endif
    }

    private void ShowDynamic()
    {
        if (StickType == EnStickType.Dynamic)
        {
            this.transform.position = Input.mousePosition;
            _buttonFrame.gameObject.SetActive(true);
            _button.gameObject.SetActive(true);
        }
    }

    private bool CheckButtonDown(Vector2 input)
    {
        if (StickType == EnStickType.Static)
        {
            float xMid = _buttonFrame.sizeDelta.x / 2.0f;
            float yMid = _buttonFrame.sizeDelta.y / 2.0f;

            Rect rect = new Rect(this.transform.position.x - xMid, this.transform.position.y - yMid, _buttonFrame.sizeDelta.x, _buttonFrame.sizeDelta.y);

            return rect.Contains(input);
        }
        else
        {
            return true;
        }
    }

    private void ButtonUp()
    {
        _buttonId = -1;
        HideDynamic();
    }

    #region User Input

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, 0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (CheckButtonDown(touch.position))
                    {
                        _buttonId = touch.fingerId;
                        ShowDynamic();
                    }
                }

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (_buttonId == touch.fingerId)
                    {
                        HandleInput(touchPos);
                    }
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (_buttonId == touch.fingerId)
                    {
                        _buttonId = -1;
                        ButtonUp();
                    }
                }
            }
        }
    }

    protected virtual void HandleInput(Vector3 input)
    {
        Vector3 differenceVector = Vector3.zero;

        if (_buttonId != -1 && (AllowX || AllowY))
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_buttonFrame, input, null, out var worldInput);

            differenceVector = (worldInput - _buttonFrame.position);

            if (differenceVector.sqrMagnitude > _dragRadius * _dragRadius)
            {
                differenceVector.Normalize();
                _button.position = _buttonFrame.position + differenceVector * _dragRadius;
            }
            else
            {
                _button.position = worldInput;
            }

            if (!AllowY)
            {
                _button.position = new Vector3(_button.position.x, _buttonFrame.position.y, 0);
            }

            if (!AllowX)
            {
                _button.position = new Vector3(_buttonFrame.position.x, _button.position.y, 0);
            }
        }
        else
        {
            _button.position = _buttonFrame.position;
        }

        Vector3 diff = _button.position - _buttonFrame.position;
        float distance = Vector3.Distance(_button.position, _buttonFrame.position) / _dragRadius;
        Vector2 normDiff = (distance < 0.00001f) ? Vector2.zero : new Vector2(diff.x / _dragRadius, diff.y / _dragRadius);

        StickChanged?.Invoke(this, new StickEventArgs(normDiff));
    }

    #endregion
}
