using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class switcher : MonoBehaviour
{

    public int channel;
    int maxChannel = 15;
    int efficientChannel = 10;
    Texture[] channelImages;
    Texture errorTexture;
    MeshRenderer meshRenderer;
    string userInput = "";
    [SerializeField] GameObject nowChannelText;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        channel = 0;

        // 初始化裝圖片的陣列
        channelImages = new Texture[efficientChannel]; //不包含[]裡的數字，[3]就是0,1,2
        for (int i = 1; i < efficientChannel; i++)
        {
            channelImages[i] = Resources.Load<Texture>($"channelImages/TV0{i}");
        }

        errorTexture = Resources.Load<Texture>("specialImages/error");

        // specialImages = new Texture[1];
        // specialImages[0] = Resources.Load<Texture>("channelImages/error");
        // Debug.Log("關電視");

        // meshRenderer.materials[1].mainTexture = channelImages[0];
    }

    // Update is called once per frame
    void Update()
    {
        // 因為getkey會頻繁偵測輸入狀態，可能會造成閃爍以及切換亂掉的狀態，所以改用getkeydown
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            channel++;

            if (channel > maxChannel) channel = 1;
            ChannelStatus(channel);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            channel--;

            if (channel < 1) channel = maxChannel;
            ChannelStatus(channel);
        }

        foreach (char c in Input.inputString)
        {
            userInput += c;
        }
        if (Input.GetKeyDown(KeyCode.Return)) // enter之後才正式輸入
        {
            if (int.TryParse(userInput, out int inputChannel)) // 判斷輸入的字串能不能轉換為數字，可以就放進out那邊的變數
            {
                if (inputChannel > 0 && inputChannel <= maxChannel)
                {
                    channel = inputChannel;
                    ChannelStatus(channel);
                }
                else
                {
                    Debug.Log($"輸入頻道{inputChannel}不存在");
                }
            }
            else
            {
                Debug.Log($"輸入{userInput}非數字");
            }
            userInput = "";
        }
    }

    void ChannelStatus(int ch)
    {
        if (ch > 0 && ch < efficientChannel)
        {
            nowChannelText.SetActive(true);
            meshRenderer.materials[1].mainTexture = channelImages[ch];
            nowChannelText.GetComponent<Text>().text = "0" + ch;
        }
        else if (ch >= efficientChannel && ch <= maxChannel)
        {
            nowChannelText.SetActive(true);
            meshRenderer.materials[1].mainTexture = errorTexture;
            nowChannelText.GetComponent<Text>().text = ch.ToString();
        }
    }
}