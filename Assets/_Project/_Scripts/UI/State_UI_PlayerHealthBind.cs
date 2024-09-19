using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class State_UI_PlayerHealthBind : MonoState
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _healthHealText;
    
    protected override void OnEnter()
    {
        base.OnEnter();
        _healthText.text = $"{ActorRegistry.PlayerActor.GetData<DS_PlayerRuntime>().CurrentHealth}";
        ActorRegistry.PlayerActor.GetData<DS_PlayerRuntime>().onCurrentHealthChanged += OnCurrentHealthChanged;
    }

    protected override void OnExit()
    {
        base.OnExit();
        ActorRegistry.PlayerActor.GetData<DS_PlayerRuntime>().onCurrentHealthChanged -= OnCurrentHealthChanged;
    }

    private void OnCurrentHealthChanged(int arg1,int arg2)
    {
        _healthText.text = $"{arg2}";

        if (arg2 > arg1)// heal
        {
            StartCoroutine(ShowHealText(arg2-arg1));
        }
    }
    
    IEnumerator ShowHealText(int healAmount)
    {
        _healthHealText.gameObject.SetActive(true);
        _healthHealText.text = $"+{healAmount}";
        yield return new WaitForSeconds(2);
        _healthHealText.gameObject.SetActive(false);
    }
}
