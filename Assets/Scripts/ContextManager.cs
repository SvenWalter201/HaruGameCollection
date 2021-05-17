using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContextManager : Singleton<ContextManager>
{
    [SerializeField]
    Dictionary<int, Context> availableContexts;

    [SerializeField]
    GameObject topBar, contentHolder;

    [SerializeField]
    TextMeshProUGUI topBarHeader;

    GameObject currentUIHolder;

    Context currentContext;

    public void ChangeContext(Context newContext)
    {
        if(currentContext != null)
        {
            //TODO: terminate and clean up old context
        }
        currentContext = newContext;
        InitializeUIComponents();
    }

    void InitializeUIComponents()
    {
        topBarHeader.SetText(currentContext.title);
        Destroy(currentUIHolder);
        GameObject uiHolder = Instantiate(currentContext.uiHolder);
        uiHolder.transform.SetParent(contentHolder.transform);
        currentUIHolder = uiHolder;
    }
}

