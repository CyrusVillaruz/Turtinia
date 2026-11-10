using System.Collections.Generic;
using Turtinia.Combat;
using UnityEngine;

namespace Turtinia.UI
{
    public class PlayerManaBar : MonoBehaviour
    {
        [SerializeField] private GameObject _crystalPrefab;
        [SerializeField] private float _unitsPerCrystal = 15f;
        private List<ManaCrystal> _crystals = new List<ManaCrystal>();

        public void OnManaChanged(Resource change)
        {
            int crystalsNeeded = Mathf.CeilToInt(change.Max / _unitsPerCrystal);
            if (_crystals.Count != crystalsNeeded)
            {
                DrawCrystals(crystalsNeeded);
            }
            UpdateCrystals(change.Current);
        }
        
        private void DrawCrystals(int count)
        {
            ClearCrystals();
            for (int i = 0; i < count; i++)
            {
                CreateEmptyCrystal();
            }
        }
        
        private void UpdateCrystals(float current)
        {
            for (int i = 0; i < _crystals.Count; i++)
            {
                float inThisCrystal = current - i * _unitsPerCrystal;
                CrystalStatus status = inThisCrystal >= _unitsPerCrystal ? CrystalStatus.Full
                    : inThisCrystal >= _unitsPerCrystal / 2f ? CrystalStatus.Half
                    : CrystalStatus.Empty;
                _crystals[i].SetCrystalImage(status);
            }
        }

        private void CreateEmptyCrystal()
        {
            GameObject crystal = Instantiate(_crystalPrefab, transform);
            ManaCrystal crystalComponent = crystal.GetComponent<ManaCrystal>();
            crystalComponent.SetCrystalImage(CrystalStatus.Empty);
            _crystals.Add(crystalComponent);
        }

        private void ClearCrystals()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            _crystals = new List<ManaCrystal>();
        }
    }
}