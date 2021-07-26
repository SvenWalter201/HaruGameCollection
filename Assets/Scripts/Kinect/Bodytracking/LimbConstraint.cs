using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LimbConstraint : MonoBehaviour
{
    [SerializeField]
    Button removeBtn;

    [SerializeField]
    TextMeshProUGUI constraintName;

    Limb constraint;

    public void Init(Limb constraint)
    {
        this.constraint = constraint;
        constraintName.text = constraint.ToString();
        removeBtn.onClick.AddListener(() => RemoveConstraint());
    }


    void RemoveConstraint() => 
        UIController.Instance.RemoveLimbConstraint(constraint);
}
