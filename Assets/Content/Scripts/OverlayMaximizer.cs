using UnityEngine;

public class OverlayMaximizer : MonoBehaviour
{
    static readonly int IsWindowMax = Animator.StringToHash("IsWindowMax");
    static readonly int WindowState = Animator.StringToHash("WindowState");

    Animator _anim8R;

    void Start()
    {
        _anim8R = GetComponent<Animator>();
    }

    public void ToggleMaximize()
    {
        _anim8R.SetBool(IsWindowMax, !_anim8R.GetBool(IsWindowMax));
    }

    private void HelioButton(Constants.OverlayState stateIfOn)
    {
        int currentState = _anim8R.GetInteger(WindowState);
        Debug.Log($"Current state is {currentState} and it wants to be {(int)stateIfOn}");
        if (currentState == 0)
        {
            _anim8R.SetInteger(WindowState, (int)stateIfOn);
        }
        else
        {
            _anim8R.SetInteger(WindowState, (int)Constants.OverlayState.Minimized);
        }

        Debug.Log($"New state is {_anim8R.GetInteger(WindowState)}");
    }

    public void Button1()
    {
        HelioButton(Constants.OverlayState.Maximized1);
    }

    public void Button2()
    {
        HelioButton(Constants.OverlayState.Maximized2);
    }

    public void Swap(bool isButton1)
    {
        Constants.OverlayState newState =
            isButton1 ? Constants.OverlayState.Maximized2 : Constants.OverlayState.Maximized1;
        _anim8R.SetInteger(WindowState, (int)newState);
    }
}