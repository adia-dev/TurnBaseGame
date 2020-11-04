using System;
using System.Collections;
using System.Collections.Generic;
using TurnBaseGame.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBaseGame.Entity
{
    [System.Serializable]
    public struct StatsEvolution
    {
        public int HP;
        public int ATK;
        public int DEF;
        public int SPD;
    }


    public class Stats : MonoBehaviour
    {
        #region STATS
        public bool IsDead => CurrentHP <= 0;

        public int Level;/*{ get; private set; }*/
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }

        public int MaxATK { get; private set; }
        public int CurrentATK { get; private set; }
        public int CurrentATKBAR { get; private set; }

        public int MaxDEF { get; private set; }
        public int CurrentDEF { get; private set; }

        public int BaseSPD { get; private set; }
        public int CurrentSPD { get; private set; }

        public int BaseCritRate { get; private set; }
        public int CurrentCritRate { get; private set; }

        public int CurrentCritDamage { get; private set; }

        #endregion

        [SerializeField] Image _healthBar;
        [SerializeField] Image _attackBar;
        [SerializeField] float _fillSpeed = 1f;
        PlayerController _playerController;

        void Awake()
        {
            _playerController = FindObjectOfType<PlayerController>();
        }

        void OnEnable()
        {
            _playerController.OnTick += TickATB;
        }

        void OnDisable()
        {
            _playerController.OnTick -= TickATB;
        }

        public void InitializeStats(StatsEvolution statsEvolution)
        {
            CurrentHP = MaxHP = statsEvolution.HP;
            CurrentATK = MaxATK = statsEvolution.ATK;
            CurrentDEF = MaxDEF = statsEvolution.DEF;
            CurrentSPD = BaseSPD = statsEvolution.SPD;

        }

        public void TakeDamage(int amount)
        {
            CurrentHP = Mathf.Max(CurrentHP - amount, 0);
            UpdateHealthBar();
        }
        public void Heal(float amount)
        {
            if (amount > 1f)
                CurrentHP = Mathf.Min(CurrentHP + (int)(MaxHP * amount / 100f), MaxHP);
            else if (amount > 0f)
                CurrentHP = Mathf.Min(CurrentHP + (int)(MaxHP * amount), MaxHP);

            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            float targetAmount = (float)CurrentHP / MaxHP;
            StopAllCoroutines();
            StartCoroutine(FillBar(_healthBar, targetAmount));
        }
        public void UpdateATKBar()
        {
            float targetAmount = (float)(CurrentATKBAR / 100f);
            StopAllCoroutines();
            StartCoroutine(FillBar(_attackBar, targetAmount));
        }

        IEnumerator FillBar(Image bar, float amount)
        {
            float t = 0f;
            while (bar.fillAmount != amount)
            {
                bar.fillAmount = Mathf.Lerp(bar.fillAmount, amount, t);
                t += _fillSpeed * Time.deltaTime;
                yield return null;
            }
        }

        public void ResetATB() => CurrentATKBAR = 0;

        void TickATB()
        {
            CurrentATKBAR += (int)(CurrentSPD * 0.07f);
            UpdateATKBar();
        }

    }

}