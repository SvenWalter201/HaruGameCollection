using UnityEngine;
using UnityEngine.UI;

public class ChooseModeMenu : MonoBehaviour
{
    [SerializeField]
    int[] buildIndizes;
    [SerializeField]
    string[] stringRes;
    [SerializeField]
    Button buttonPrefab;

    void Start()
    {
        VirtualWorldController.Instance.FlipMainSceneControl();

        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        transform.SetParent(canvas.transform, false);

        for (int i = 0; i < buildIndizes.Length; i++)
        {
            Button btn = Instantiate(buttonPrefab);
            int v = i; //no idea why this is nessessary
            btn.onClick.AddListener(delegate { Choose(buildIndizes[v]); });
            btn.gameObject.GetComponentInChildren<Text>().text = StringRes.Get(stringRes[i]);
            btn.transform.SetParent(transform, false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VirtualWorldController.Instance.FlipMainSceneControl();
            Destroy(gameObject);
        }
    }

    public void Choose(int index)
    {
        VirtualWorldController.Instance.FlipMainSceneControl();

        GameController.Instance.StartGame(index);
        Destroy(gameObject);
    }
}
