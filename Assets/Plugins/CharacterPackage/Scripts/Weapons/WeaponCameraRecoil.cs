using System;
using Cinemachine;
using Rowlan.Tools.QuickNav;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponCameraRecoil : MonoBehaviour
{
    public Vector2[] RecoilPattern;

    private CinemachineImpulseSource _impulseSource;

    private Gun _gun;
    private MouseLook _mouseLook;

    public float RecoilMultiplier;
    public float CooldownSpeed;
    public float RecoilSpeed;
    public float VerticalRecoil;

    private float _verticalRecoilToAdd;
    
    private int _index;

    private bool _isPlayer;
    private bool _isCooling;
    
    private float _addedPitch;
    private float _reversePitch;
    private float _paidBackPitch;
    private float _totalPitchToPayback;

    private void Start()
    {
        _gun = GetComponent<Gun>();
        _mouseLook = FindAnyObjectByType<MouseLook>();

        _gun.GetComponent<GunFireComponent>().onFire += OnFire;
        _gun.GetComponent<GunFireComponent>().onStopFire += OnStopFire;
    }

    public void OnFire(Gun target)
    {
        _isCooling = false;
        _isPlayer = _gun.GetComponent<Equippable>().Owner.ContainsTag("Player");
        if (!_isPlayer) return;

        //_horizontalRecoil = RecoilPattern[_index].x;
        VerticalRecoil = -RecoilPattern[_index].y * RecoilMultiplier;
        _verticalRecoilToAdd = VerticalRecoil;
        _index = NextIndex(_index);
    }

    void OnStopFire(Gun gun)
    {
        Reset();
    }

    private void Reset()
    {
        _paidBackPitch = 0;
        _index = 0;
    }
    int NextIndex(int index)
    {
        return (index + 1) % RecoilPattern.Length;
    }

    private void Update()
    {
        if (!_isPlayer) return;
        if (Mathf.Abs(_verticalRecoilToAdd) > 0) // on recoil
        {
            _reversePitch += _mouseLook.GetAddedPitchThisFrame();
            float pitch = (VerticalRecoil) * RecoilSpeed * Time.deltaTime;
            if (Mathf.Abs(pitch) > Mathf.Abs(_verticalRecoilToAdd))
                pitch = _verticalRecoilToAdd;

            _addedPitch += pitch;
            _verticalRecoilToAdd -= pitch;

            _mouseLook.AddPitchYaw(pitch, 0);
            return;
        }

        if (Mathf.Abs(_verticalRecoilToAdd) <= 0 && !_isCooling) // recoil finished, calculate payback
        {
            _isCooling = true;
            _totalPitchToPayback = Mathf.Abs(_addedPitch) - _reversePitch; //pozitif olmalı
            if (_totalPitchToPayback < 0)
            {
                _totalPitchToPayback = 0; // eğer recoilden daha çok aşağı çektiysek payback yapma
                _reversePitch = 0;
                _addedPitch = 0;
                _isCooling = false;
            }
        }
        if (Mathf.Abs(_totalPitchToPayback) > 0 && _isCooling)
        {
            float totalToAddPitch = Mathf.Abs(_addedPitch) - _reversePitch; // pozitif değer

            float toAddThisFrame = (totalToAddPitch / 10) * CooldownSpeed * Time.deltaTime;

            if (Mathf.Abs(toAddThisFrame) > Mathf.Abs(_totalPitchToPayback))
            {
                toAddThisFrame = _totalPitchToPayback;
                _reversePitch = 0;
                _addedPitch = 0;
                _totalPitchToPayback = 0;
            }

            _addedPitch += toAddThisFrame;
            _totalPitchToPayback -= toAddThisFrame;
            _mouseLook.AddPitchYaw(toAddThisFrame, 0);
            
            _paidBackPitch += toAddThisFrame;
            if (_totalPitchToPayback < 0) _totalPitchToPayback = 0;
        }
    }

    private void OnGUI()
    {
        if (!_isPlayer) return;
        //shot reverse pitch , aded pitch , paid back pitch on center of screen
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 - 20, 300, 300), "IsCooling:    " + _isCooling);
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 300, 300), "Reverse Pitch    " + _reversePitch);
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 20, 300, 300), "Added Pitch    " + _addedPitch);
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 40, 300, 300),
            "Total Pitch To Pay    " + _totalPitchToPayback);
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 60, 300, 300), "Paid Back Pitch    " + _paidBackPitch);
    }
}