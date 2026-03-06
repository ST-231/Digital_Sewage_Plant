using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    public GameObject optionPrefab;
    public GameObject Tip;
    public Transform optionGroup;
    Transform[] options;
    [Range(0,20)]
    public int optionNum;
    [Range(0, 100)]
    public float yOffset;

    private Slider loadingBar;


    float halfNum;
    Dictionary<Transform,Vector3> OptionPos=new Dictionary<Transform,Vector3>();
    Dictionary<Transform,int> OptionIndex=new Dictionary<Transform,int>();
    Dictionary<Transform,string> OptionTips=new Dictionary<Transform,string>();
    Dictionary<Transform,string> OptionScenes=new Dictionary<Transform,string>();       //场景名

    Vector3 center = Vector3.zero;
    float R = 300f;
    [Range(1f, 10f)]
    public float speed;
    [Range (0f, 1f)]
    public float minAlpha;

    private Vector3 frontPos;

    Coroutine current;          //当前协程，防止冲突


    private void Awake()
    {
        //生成场景选项
        for (int i = 0; i < optionNum; i++) {
            GameObject go = GameObject.Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, optionGroup);
            go.name =i.ToString();
        }

        halfNum = optionNum / 2;
        options=new Transform[optionNum];

        for (int i = 0; i < optionNum; i++)
        {
            options[i]=optionGroup.GetChild(i);
        }

        frontPos=new Vector3(0f, 0f, -R);

        InitPos();
        InitIndex();
        InitTip();

        setAlpha();
        setTip();

    }

    private void InitTip()
    {
        OptionTips.Add(options[0], "first");
        OptionTips.Add(options[1], "second");
        OptionTips.Add(options[2], "third");
        OptionScenes.Add(options[0], "SewagePlant");
        OptionScenes.Add(options[1], "SewagePlant");
        OptionScenes.Add(options[2], "SewagePlant");
    }

    private void InitPos()
    {
        float angle = 0f;

        for (int i = 0; i < optionNum; i++)
        {
            angle=(360.0f/(float)optionNum)*i*Mathf.Rad2Deg;

            float x=Mathf.Sin(angle)*R;
            float z=-Mathf.Cos(angle)*R;
            float y = yOffset * i;
            if (i > halfNum)
            {
                x = Mathf.Cos(angle) * R;
                y =yOffset*(optionNum-i);
                z= Mathf.Sin(angle) * R;
            }
            

            options[i].localPosition=new Vector3(x,y,z);
            OptionPos.Add(options[i], options[i].localPosition);
        }
    }

    private void InitIndex()
    {
        for (int i = 0; i < optionNum; i++)
        {
            if (i <= halfNum)
            {
                if (optionNum % 2 == 0)
                {
                    options[i].SetSiblingIndex((int)halfNum - i);
                }
                else
                {
                    options[i].SetSiblingIndex((int)((optionNum - 1) / 2) - i);
                }
            }
            else
            {
                options[i].SetSiblingIndex(options[optionNum - i].GetSiblingIndex());
            }

        }

        for (int i = 0;i < optionNum;i++)
        {
            OptionIndex.Add(options[i], options[i].GetSiblingIndex());
            Debug.Log("index"+OptionIndex[options[i]]);
        }

    }

    private void setAlpha()
    {
        float startZ = center.z - R;
        foreach (var option in OptionPos)
        {
            float value=1-Mathf.Abs(option.Value.z-startZ)/(2*R)*(1-minAlpha);

            Image[] img=option.Key.GetComponentsInChildren<Image>();

            for (int i = 0;i<img.Length;i++)
            {
                Color color=img[i].color;
                img[i].color=new Color(color.r,color.g,color.b,value);
            }
        }
    }

    public void clickLeft()
    {
        Debug.Log("left");
        StartCoroutine(MoveLeft());
    }

    public void clickRight()
    {
        StartCoroutine(MoveRight());
    }

    public void setTip()
    {
        Text tip=Tip.GetComponentInChildren<Text>();

        foreach (Transform tf in options)
        {
            if(tf.localPosition==frontPos)
            {
                tip.text = OptionTips[tf];
            }
        }
     
    }

    IEnumerator MoveLeft()
    {
        if(current!=null)               //等待上一协程运行完毕
        {
            yield return current;
        }

        Vector3 pos = OptionPos[options[0]];
        int index = OptionIndex[options[0]];
        Vector3 target;

        for (int i = 0; i < optionNum; i++)
        {
            if(i==optionNum - 1)
            {
                target = pos;
                OptionIndex[options[i]] = index;
            }
            else
            {
                target = options[(i+1)%optionNum].localPosition;
                OptionIndex[options[i]] = OptionIndex[options[(i + 1) % optionNum]];
            }

            options[i].SetSiblingIndex(OptionIndex[options[i]]);
            current=StartCoroutine(MoveToTarget(options[i], target));
        }
        yield return null;

    }

    IEnumerator MoveRight()
    {
        if (current != null)
        {
            yield return current;
        }

        Vector3 pos = OptionPos[options[optionNum - 1]];
        int index = OptionIndex[options[optionNum - 1]];
        Vector3 target;

        for (int i = optionNum-1; i >=0 ; i--)
        {
            if (i == 0)
            {
                target = pos;
                OptionIndex[options[i]] = index;
            }
            else
            {
                target = options[(i - 1) % optionNum].localPosition;
                OptionIndex[options[i]] = OptionIndex[options[(i - 1) % optionNum]];
            }

            options[i].SetSiblingIndex(OptionIndex[options[i]]);
            current=StartCoroutine(MoveToTarget(options[i], target));
        }
        yield return null;
    }

    IEnumerator MoveToTarget(Transform tf, Vector3 target)
    {
        float tempSpeed = (tf.localPosition - target).magnitude * speed;
        while (tf.localPosition != target)
        {
            tf.localPosition=Vector3.MoveTowards(tf.localPosition, target, tempSpeed*Time.deltaTime);
            yield return null;
        }
        OptionPos[tf] = target;

        setAlpha();
        setTip();

        yield return null;
    }


    public void EntryClick()
    {
        foreach (Transform tf in options)
        {
            if (tf.localPosition == frontPos)
            {
                LoadingScreenManager.nextSceneName = OptionScenes[tf];

                if (OptionTips[tf]=="first")
                {
                    LoadingScreenManager.nextSceneIndex = 0;
                }else if(OptionTips[tf] == "second")
                {
                    LoadingScreenManager.nextSceneIndex = 1;
                }else if(OptionTips[tf] == "third")
                {
                    LoadingScreenManager.nextSceneIndex = 2;
                }

                SceneManager.LoadScene("LoadingScene");
            }
        }
    }

}
