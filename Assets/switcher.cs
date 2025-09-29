using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class switcher : MonoBehaviour
{

    public int channel;
    int maxChannel = 3;
    Texture[] channelImages;
    // Texture[] specialImages;
    Texture errorTexture;
    MeshRenderer meshRenderer;
    string userInput="";


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        channel = 0;

        // 初始化裝圖片的陣列
        channelImages = new Texture[maxChannel]; //不包含[]裡的數字，[3]就是0,1,2
        for (int i = 0; i < maxChannel; i++)
        {
            channelImages[i] = Resources.Load<Texture>($"channelImages/TV0{i}");
        }

        errorTexture = Resources.Load<Texture>("specialImages/test");
        if (errorTexture == null)
        {
            Debug.LogError("錯誤圖片載入失敗");
        }

        // specialImages = new Texture[1];
        // specialImages[0] = Resources.Load<Texture>("channelImages/error");
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

        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                userInput += c;
            }


            if (Input.GetKeyDown(KeyCode.Return)) // enter之後才正式輸入
            {
                if (int.TryParse(userInput, out int inputChannel)) // 判斷輸入的字串能不能轉換為數字，可以就放進out那邊的變數
                {
                    if (inputChannel >= 0 && inputChannel < maxChannel)
                    {
                        channel = inputChannel;
                        meshRenderer.materials[1].mainTexture = channelImages[channel];
                        userInput = "";
                    }
                    else
                    {
                        meshRenderer.materials[1].mainTexture = errorTexture;

                        Debug.Log($"輸入頻道{inputChannel}不存在");
                        userInput = "";
                    }
                }
                else
                {
                    Debug.Log("輸入非數字");
                    userInput = "";
                }
            }
        }
    }
}
