using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Attribute = StatSystem.Attribute;

public class HealthBarUI : MonoState
{
    [SerializeField] private Image _energyFill;
    [SerializeField] private float _fillDuration = 0.5f ;
    
    private Transform m_MainCamera;
    private Attribute _healthAttribute;
    private Data_Character _characterData;
    
    private void LateUpdate()
    {
        transform.LookAt(transform.position + m_MainCamera.forward);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _characterData = Owner.GetData<Data_Character>();
        
        _healthAttribute = _characterData.StatController.GetAttribute("Health");
        _healthAttribute.onCurrentValueChanged += OnCurrentHealthChange;
   
        UpdateHealthBarInstant(); 
    }


    private void OnCurrentHealthChange()
    {
        UpdateHealthBar();
    }

    private void Awake()
    {
        m_MainCamera = Camera.main.transform;
    }

    void UpdateHealthBar()
    {
        StartCoroutine(UpdateHealthFill(_healthAttribute.CurrentValue / (float)_healthAttribute.BaseValue));
    }

    void UpdateHealthBarInstant()
    {
        _energyFill.fillAmount = _healthAttribute.CurrentValue / (float)_healthAttribute.BaseValue;
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