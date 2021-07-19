using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpSwitch : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private RectTransform bgRectTrans;
    [SerializeField] private RectTransform leftRectTrans;
    [SerializeField] private RectTransform rightRectTrans;

    [SerializeField] private TextColorAnimationController leftColorController;
    [SerializeField] private TextColorAnimationController rightColorController;

    private enum BUTTON_SELECTED { LEFT, RIGHT }
    private BUTTON_SELECTED currentBtn = BUTTON_SELECTED.LEFT;

    private void Awake()
    {
        leftButton.onClick.AddListener(delegate { OnButtonClick(BUTTON_SELECTED.LEFT); } );
        rightButton.onClick.AddListener(delegate { OnButtonClick(BUTTON_SELECTED.RIGHT); } );
    }

    private void OnButtonClick(BUTTON_SELECTED selected)
    {
        currentBtn = selected;
        switch (selected)
        {
            case BUTTON_SELECTED.LEFT:
                MoveActiveBackground(bgRectTrans, new Vector3(-288f, 0f, 0f));
                rightColorController.SetIdleState();
                leftColorController.SetActiveState();
                break;
            case BUTTON_SELECTED.RIGHT:
                MoveActiveBackground(bgRectTrans, new Vector3(-12f, 0f, 0f));
                leftColorController.SetIdleState();
                rightColorController.SetActiveState();
                break;
        }
    }

    private void MoveActiveBackground(Transform origin, Vector3 target)
    {
        StartCoroutine(origin.ChangeLocalPosition(target, 0.1f));
    }
}
