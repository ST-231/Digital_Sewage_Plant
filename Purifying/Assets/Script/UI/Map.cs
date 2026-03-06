using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Map : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Image myPos;
    public Button[] btns;
    public Transform player;

    public XiGeShan xsg;
    public GeShan sg;
    public wuNiTuoShui wnts;
    public ziWaiXiaoDu zwxd;
    public gaoMiDuChenDian gmdcd;
    public shenChuangLvChi sclc;
    public erChenChi ecc;
    public AAO aao;


    private RectTransform mapRect;
    private RectTransform containerRect;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;

    private float zoomSpeed = 0.1f;
    private float curZoom=1f;
    private float minZoom = 1f;
    private float maxZoom = 2f;

    private bool isDragging;
    private const float dragThreshold = 5f; // 像素阈值

    private Color origColor;

    void Awake()
    {

        mapRect = GetComponent<RectTransform>();
        containerRect = transform.parent.GetComponent<RectTransform>();
        btns[0].transform.localPosition = getPos(ecc.pos);
        btns[1].transform.localPosition = getPos(aao.pos);
        btns[2].transform.localPosition = getPos(sclc.pos);
        btns[3].transform.localPosition = getPos(sg.pos);
        btns[4].transform.localPosition = getPos(xsg.pos);
        btns[5].transform.localPosition = getPos(zwxd.pos);
        btns[6].transform.localPosition = getPos(gmdcd.pos);
        btns[7].transform.localPosition = getPos(wnts.pos);
        Image img = btns[0].transform.GetChild(1).GetComponent<Image>();
        origColor =img.color;

    }

    void OnEnable()
    {
        myPos.transform.localPosition = getPos(player.localPosition);
        Vector3 playerRot = player.localRotation.eulerAngles;
        myPos.transform.localRotation=Quaternion.Euler(new Vector3(0, 0, -playerRot.y));
        Vector3 pos = mapRect.localPosition;           //坐标开始在图中
        pos.x =-myPos.transform.localPosition.x ;
        pos.y = -myPos.transform.localPosition.y;
        mapRect.localPosition = pos;

        ClampMapPosition();


        //显示错误
        Image img0 = btns[0].transform.GetChild(1).GetComponent<Image>();
        if (ecc.Eerr || ecc.Herr || ecc.SRTerr || ecc.MLSSerr || ecc.Vserr)
        {
            if (img0 != null)
            {
                img0.color = Color.red;
            }
        }
        else
        {
            img0.color = origColor;
        }

        Image img1 = btns[1].transform.GetChild(1).GetComponent<Image>();
        if (aao.Eerr||aao.tnerr||aao.tperr)
        {
            
            if (img1 != null)
            {
                img1.color = Color.red;
            }
        }
        else
        {
            img1.color = origColor;
        }

        Image img2 = btns[2].transform.GetChild(1).GetComponent<Image>();
        if (sclc.Eerr||sclc.Rserr)
        {
            if (img2 != null)
            {
                img2.color = Color.red;
            }
        }
        else
        {
            img2.color = origColor;
        }

        Image img3 = btns[3].transform.GetChild(1).GetComponent<Image>();
        if (sg.herr||sg.verr)
        {
            
            if (img3 != null)
            {
                img3.color = Color.red;
            }
        }
        else
        {
            img3.color = origColor;
        }

        Image img4 = btns[4].transform.GetChild(1).GetComponent<Image>();
        if (xsg.herr||xsg.verr)
        {
            if(img4 != null )
            {
                img4.color = Color.red;
            }
        }
        else
        {
            img4.color = origColor;
        }

        Image img5 = btns[5].transform.GetChild(1).GetComponent<Image>();
        if (zwxd.Eerr||zwxd.Merr)
        {
            if (img5 != null)
            {
                img5.color = Color.red;
            }
        }
        else
        {
            img5.color = origColor;
        }

        Image img6 = btns[6].transform.GetChild(1).GetComponent<Image>();
        if (gmdcd.Eerr||gmdcd.Rserr||gmdcd.Rtperr)
        {
            if (img6 != null)
            {
                img6.color = Color.red;
            }
        }
        else
        {
            img6.color = origColor;
        }

        Image img7 = btns[7].transform.GetChild(1).GetComponent<Image>();
        if (wnts.Eerr||wnts.Perr)
        {
            if (img7 != null)
            {
                img7.color = Color.red;
            }
        }
        else
        {
            img7.color = origColor;
        }

    }

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y*0.01f;
        //Debug.Log("scroll=" + scroll);
        if (scroll != 0)
        {
            Vector2 mouseBeforeZoom = GetLocalMousePosition();
            curZoom = Mathf.Clamp(mapRect.localScale.x + scroll * zoomSpeed, minZoom, maxZoom);
            Debug.Log("newZoom=" + curZoom);
            mapRect.localScale = Vector3.one * curZoom;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (!isDragging) // 需要添加拖动判断逻辑
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            {
                Debug.Log($"Clicked at map coordinates: {localPoint}");
            }
        }
    }

    Vector2 getPos(Vector3 pos)
    {
        Vector2 mapPos=new Vector2(pos.x*1800f/320f-900f, pos.z*900f/160f-450f);
        return mapPos;
    }

    Vector2 GetLocalMousePosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRect,
            Input.mousePosition,
            null,
            out localPoint);
        return localPoint;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = false;

        originalPanelLocalPosition = mapRect.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerRect,
            eventData.position,
            eventData.pressEventCamera,
            out originalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging && eventData.delta.magnitude > dragThreshold)
        {
            isDragging = true;
        }

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            Vector3 offset = localPointerPosition - originalLocalPointerPosition;
            mapRect.localPosition = originalPanelLocalPosition + offset;
        }

        ClampMapPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    void ClampMapPosition(Vector2? zoomCenter = null)    //默认值为null
    {
        Vector3 pos = mapRect.localPosition;

        // 计算边界限制
        Vector3 minPosition = containerRect.rect.min - mapRect.rect.min;
        Vector3 maxPosition = containerRect.rect.max - mapRect.rect.max;

        pos.x = Mathf.Clamp(pos.x, (maxPosition.x)*curZoom, (minPosition.x)*curZoom);
        pos.y = Mathf.Clamp(pos.y, (maxPosition.y)*curZoom, (minPosition.y)*curZoom);


        mapRect.localPosition = pos;

    }
}
