using System;
using System.Collections.Generic;
using Turtinia.Combat;
using Turtinia.Player;
using UnityEngine;

namespace Turtinia.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject _heartPrefab;
        private List<HealthHeart> _hearts = new List<HealthHeart>();

        public void OnHealthChanged(ResourceChange change)
        {
            int heartsNeeded = Mathf.CeilToInt(change.Max / 2f);
            if (_hearts.Count != heartsNeeded)
            {
                DrawHearts(heartsNeeded);
            }
            UpdateHearts(change.Current);
        }
        
        private void DrawHearts(int count)
        {
            ClearHearts();
            for (int i = 0; i < count; i++)
            {
                CreateEmptyHeart();
            }
        }
        
        private void UpdateHearts(float current)
        {
            for (int i = 0; i < _hearts.Count; i++)
            {
                float healthInThisHeart = current - i * 2;
                HeartStatus status = healthInThisHeart >= 2 ? HeartStatus.Full
                    : healthInThisHeart >= 1 ? HeartStatus.Half
                    : HeartStatus.Empty;
                _hearts[i].SetHeartImage(status);
            }
        }

        private void CreateEmptyHeart()
        {
            GameObject heart = Instantiate(_heartPrefab, transform);
            HealthHeart heartComponent = heart.GetComponent<HealthHeart>();
            heartComponent.SetHeartImage(HeartStatus.Empty);
            _hearts.Add(heartComponent);
        }

        private void ClearHearts()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            _hearts = new List<HealthHeart>();
        }
    }
}