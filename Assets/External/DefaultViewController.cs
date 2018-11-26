using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultViewController : MonoBehaviour {
    public GameObject parentViewController;
	// Use this for initialization
	virtual protected void Awake () {
        //gameObject.SetActive(false);
	}

    // Update is called once per frame
    virtual protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape)|| Input.GetMouseButtonDown(1))
        {
            Back();
            return;
        }
#elif UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
                return;
            }
#endif
    }

    public virtual void Back() {
        //SFXManager.Instance.ButtonClick();
        Destroy(gameObject);
        if (ShouldUnhideParent())
        {
            parentViewController.SetActive(true);
        }
    }

    protected virtual bool ShouldUnhideParent()
    {
        return parentViewController!=null;
    }

}
