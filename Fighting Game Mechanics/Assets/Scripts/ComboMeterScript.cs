using System.Collections;
using TMPro;
using UnityEngine;

public class ComboMeterScript : MonoBehaviour
{
    [Header("Combo Meter Settings")] [SerializeField, Tooltip("Set the Combo Text Component.")]
    private TMP_Text comboText;
    [SerializeField, Tooltip("Set the Damage Text Component.")]
    private TMP_Text damageText;
    [SerializeField, Tooltip("Set the time that the combo will be shown for before it disappears.")]
    private float timeUntilComboTransparent = 5;
    [SerializeField, Tooltip("Set the player who the combo meter will be based on.")]
    private GameObject playersComboMeter = null;
    [SerializeField, Tooltip("Set the player who the combo meter will be based on.")]
    private CanvasGroup cg;

    private int _combo = 0;
    private int _damage = 0;
    private Coroutine _coroutineReference;
    private bool _comboActive = false;

    private void Awake()
    {
        if (cg == null)
            cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        SetComboText(0, 0);
    }

    /* Update Combo Meter adds 1 to the combo total and adds the damage to the damage total. It makes sure that these values are visible.
     * pDamage tells us how much damage the player took.
     */
    public void UpdateComboMeter(int pDamage)
    {
        if (playersComboMeter != null)
        {
            _comboActive = true;
            if (_coroutineReference != null)
            {
                StopCoroutine(_coroutineReference);
                _coroutineReference = null;
            }
            cg.alpha = 1;
            
            _combo++;
            _damage += pDamage;
            
            SetComboText(_combo, _damage);
        }
    }

    /* Ends the combo. Resets the values and turns on the coroutine that makes the combo meter invisible. */
    public void EndCombo()
    {
        if (_comboActive)
        {
            _combo = 0;
            _damage = 0;
            _coroutineReference = StartCoroutine(ShowComboBeforeTransparency());
        }
    }

    /* Set the Combo and Damage text.
     * pCombo tells us how far the combo is.
     * pDamage tells us how much damage the player took.
     */
    private void SetComboText(int pCombo, int pDamage)
    {
        if (comboText != null)
            comboText.SetText($"{pCombo} HIT COMBO!");
        if (damageText != null)
            damageText.SetText($"{pDamage} DAMAGE!");
    }

    /* Turn the combo meter invisible. */
    IEnumerator ShowComboBeforeTransparency()
    {
        yield return new WaitForSeconds(timeUntilComboTransparent);
        if (cg != null)
        {
            while (cg.alpha > 0)
            {
                cg.alpha -= 0.1f;
                yield return null;
            }
        }
        _coroutineReference = null;
        _comboActive = false;
    }
}