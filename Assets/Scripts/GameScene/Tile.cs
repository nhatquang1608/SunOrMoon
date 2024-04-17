using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public TileCell cell;
    public bool isMoon;
    public bool isSun;
    public bool isDefault;
    public bool isLock;
    public bool isSelected;
    [SerializeField] public GameObject sun;
    [SerializeField] public GameObject sunLock;
    [SerializeField] public GameObject moon;
    [SerializeField] public GameObject moonLock;

    private Vector3 initPosition;
    private Vector3 delta;
    private Transform tileDemo;

    public TileGrid grid;
    public TileBoard board;
    public GameManager gameManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(board.waiting || GameSetting.Instance.playerType != GameSetting.PlayerType.Human) return;

        if(gameManager.turn == GameManager.Turn.Sun && (cell.Empty || isSun) && grid.CanSelected(cell))
        {
            SetSun();
            isSelected = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.touchSound);
        }
        else if(gameManager.turn == GameManager.Turn.Moon && (cell.Empty || isMoon) && grid.CanSelected(cell))
        {
            SetMoon();
            isSelected = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.touchSound);
        }
        else 
        {
            isSelected = false;
            return;
        }

        board.ShowCanTargetCell(cell, true);

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        delta = touchPosition - transform.position;
        initPosition = transform.position;

        tileDemo = Instantiate(transform, grid.transform);
        tileDemo.position = transform.position;
    }

    public void OnDrag (PointerEventData eventData)
    {
        if(!isSelected) return;
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        touchPosition.z = 1;
        transform.position = touchPosition - delta;
        if(tileDemo) tileDemo.position = transform.position;
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        if(!isSelected) return;
        if(tileDemo) Destroy(tileDemo.gameObject);
        board.ShowCanTargetCell(cell, false);

        foreach(TileCell cellTarget in grid.cells)
        {
            if(Vector2.Distance(transform.position, cellTarget.transform.position) < 0.2)
            {
                if(grid.CanSwap(cell, cellTarget)) 
                {
                    board.Swap(cell, cellTarget);
                    isSelected = false;
                    return;
                }
            }
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.releaseSound);
        transform.position = initPosition;
        if(isDefault) SetDefault();
        isSelected = false;
    }

    public void Unlock()
    {
        if(isLock)
        {
            isLock = false;
            if(isSun) SetSun();
            else if(isMoon) SetMoon();
        }
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public IEnumerator Animate(TileCell to, float time)
    {
        transform.DOMove(to.transform.position, time);
        yield return new WaitForSeconds(time);
    }

    public IEnumerator Animate(TileCell to, float time, bool createDemo)
    {
        Transform demo = null;
        if(createDemo)
        {
            demo = Instantiate(transform, grid.transform);
            demo.position = transform.position;
        }
        demo.DOMove(to.transform.position, time);
        transform.DOMove(to.transform.position, time);
        if(time < 1) SoundManager.Instance.PlaySound(SoundManager.Instance.swapSound);

        yield return new WaitForSeconds(1);

        if(time >= 1) SoundManager.Instance.PlaySound(SoundManager.Instance.swapSound);
        Destroy(demo.gameObject);
    }

    public void SetSun()
    {
        SetVisual(true, false, false, false);
    }

    public void SetSunLock()
    {
        SetVisual(true, true, false, false);
    }

    public void SetMoon()
    {
        SetVisual(false, false, true, false);
    }

    public void SetMoonLock()
    {
        SetVisual(false, false, true, true);
    }

    public void SetDefault()
    {
        SetVisual(false, false, false, false);
    }

    private void SetVisual(bool _sun, bool _sunLock, bool _moon, bool _moonLock)
    {
        sun.SetActive(_sun);
        sunLock.SetActive(_sunLock);
        moon.SetActive(_moon);
        moonLock.SetActive(_moonLock);
    }
}
