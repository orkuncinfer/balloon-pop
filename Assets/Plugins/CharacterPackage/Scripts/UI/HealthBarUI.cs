﻿using System;
using System.Collections;
using Sirenix.OdinInspector;
using StatSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Attribute = StatSystem.Attribute;

public class HealthBarUI : MonoState
{
    [SerializeField] private Image _energyFill;
    [SerializeField] private float _fillDuration = 0.5f ;
    
    private Transform m_MainCamera;
    private Stat _healthAttribute;
    private Service_GAS _gas;
    
    private void LateUpdate()
    {
        transform.LookAt(transform.position + m_MainCamera.forward);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _gas = Owner.GetService<Service_GAS>();
        
        _healthAttribute = _gas.StatController.GetStat("Health");
        _healthAttribute.onStatValueChanged += OnCurrentHealthChange;
   
        UpdateHealthBarInstant(); 
    }

    protected override void OnExit()
    {
        base.OnExit();
        _healthAttribute.onStatValueChanged -= OnCurrentHealthChange;
    }


    private void OnCurrentHealthChange()
    {
        if(!gameObject.activeInHierarchy)return;
        UpdateHealthBar();
    }

    private void Awake()
    {
        m_MainCamera = Camera.main.transform;
    }

    void UpdateHealthBar()
    {
        StartCoroutine(UpdateHealthFill(_healthAttribute.Value / (float)_healthAttribute.BaseValue));
    }

    void UpdateHealthBarInstant()
    {
        _energyFill.fillAmount = _healthAttribute.Value / (float)_healthAttribute.BaseValue;
    }

    IEnumerator UpdateHealthFill(float newFillAmount)
    {
        float time = 0f;
        float startFill = _energyFill.fillAmount;

        while (time < _fillDuration)
        {
            _energyFill.fillAmount = Mathf.Lerp(startFill, newFillAmount, time / _fillDuration);
            time += Time.deltaTime;
            yield return null;
        }

        _energyFill.fillAmount = newFillAmount; 
    }
}