using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class switcher : MonoBehaviour
{

    public int channel;
    int maxChannel = 3;
    Texture[] channelImages;
    MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        channel = 0;

        // 初始化裝圖片的陣列
        channelImages = new Texture[maxChannel];
        for (int i = 0; i < maxChannel; i++)
        {
            channelImages[i] = Resources.Load<Texture>($"channelImages/TV0{i}");
        }
        // Debug.Log("關電視");

        meshRenderer.materials[1].mainTexture = channelImages[0];


    }

    // Update is called once per frame
    void Update()
    {

        if (channel >= 0 && channel < maxChannel)
        {
            meshRenderer.materials[1].mainTexture = channelImages[channel];
        }

        // 因為getkey會頻繁偵測輸入狀態，可能會造成閃爍以及切換亂掉的狀態，所以改用getkeydown
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            channel++;
            if (channel >= maxChannel)
            {
                channel = 0;
            }

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            channel--;
            if (channel < 0)
            {
                channel = maxChannel - 1;
            }

        }
    }
}
