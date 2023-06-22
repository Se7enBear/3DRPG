using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class move : Single<move>
{

    RaycastHit hitinfo;
    public event Action<Vector3> OnmouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hitinfo))
        {

        }
    }
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null)
        {
            if (hitinfo.collider.gameObject.CompareTag("ground"))
                {
                OnmouseClicked?.Invoke(hitinfo.point);
            }
            if (hitinfo.collider.gameObject.CompareTag("Portal"))
            {
                OnmouseClicked?.Invoke(hitinfo.point);
            }
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);
            }
            if (hitinfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);
            }
        }
    }
}
