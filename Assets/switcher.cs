using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class switcher : MonoBehaviour
{

    public int channel;
    int maxChannel = 15;
    int efficientChannel = 10;
    Texture[] channelImages;
    VideoClip[] channelVideos;
    Sprite[] channelInfos;
    Texture errorTexture;
    Texture blackTexture;
    MeshRenderer meshRenderer;
    VideoPlayer videoPlayer;
    string userInput = "";
    [SerializeField] GameObject nowChannelText;
    [SerializeField] GameObject channelInfo;
    float waitTime = 2f;
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        channel = 0;

        // 初始化裝圖片的陣列
        channelImages = new Texture[efficientChannel]; //不包含[]裡的數字，[3]就是0,1,2
        for (int i = 1; i < efficientChannel; i++)
        {
            channelImages[i] = Resources.Load<Texture>($"channelImages/TV0{i}");
        }
        channelVideos = new VideoClip[efficientChannel];
        for (int i = 1; i < efficientChannel; i++)
        {
            channelVideos[i] = Resources.Load<VideoClip>($"channelVideos/TV0{i}");
        }

        errorTexture = Resources.Load<Texture>("specialImages/error");
        blackTexture = Resources.Load<Texture>("specialImages/blackBG");
        // channelInfos = Resources.Load<Textur>("channelImages/TV02");

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
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            channel--;
            if (channel < 1) channel = maxChannel;
            ChannelStatus(channel);
            timer = 0;
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
                    timer = 0; // 重置計時器
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

        if (nowChannelText.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                nowChannelText.SetActive(false);
                channelInfo.SetActive(false);
                timer = 0f;
            }
        }
    }

    void ChannelStatus(int ch)
    {
        nowChannelText.SetActive(true);
        nowChannelText.GetComponent<Text>().text = ch < 10 ? "0" + ch : ch.ToString();
        videoPlayer.Stop();


        if (ch > 0 && ch < efficientChannel)
        {
            channelInfo.SetActive(true); // 要觀察一下是不是電視切換節目表會一直在，還是會先消失切換才又再出現
            channelInfo.GetComponent<Image>().sprite = Resources.Load<Sprite>("channelInfos/TV00" + ch);

            StartCoroutine(playwithDelay(ch));
        }
        else if (ch >= efficientChannel && ch <= maxChannel)
        {
            channelInfo.SetActive(false);

            meshRenderer.materials[1].mainTexture = errorTexture;
        }
    }

    IEnumerator playwithDelay(int ch)
    {
        meshRenderer.materials[1].mainTexture = blackTexture;
        videoPlayer.clip = channelVideos[ch];

        yield return new WaitForSeconds(0.5f);
        videoPlayer.Play(); // 把指定影片放進去後還要播放才會正式開始 or 打勾play on awake
    }
}