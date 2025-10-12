using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class switcher : MonoBehaviour
{

    public int channel;
    int maxChannel = 150;
    int efficientChannel = 11;
    VideoClip[] channelVideos;
    Sprite[] channelInfos;
    VideoClip errorVideo;
    Texture blackTexture;
    MeshRenderer meshRenderer;
    VideoPlayer videoPlayer;
    string userInput = "";
    [SerializeField] GameObject nowChannelText;
    [SerializeField] GameObject channelInfo;
    [SerializeField] GameObject userIn;
    float waitTime = 2f;
    float timer = 0f;
    bool isTVOn = false;

    // Start is called before the first frame update
    void Start()
    {
        isTVOn = false;

        meshRenderer = GetComponent<MeshRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        // channel = 1;

        channelInfos = new Sprite[maxChannel + 1]; //不包含[]裡的數字，[3]就是0,1,2
        for (int i = 2; i < maxChannel + 1; i++)
        {
            switch (i)
            {
                case < 10:
                    channelInfos[i] = Resources.Load<Sprite>($"channelInfos/TV00{i}");
                    break;
                case < 100:
                    channelInfos[i] = Resources.Load<Sprite>($"channelInfos/TV0{i}");
                    break;
                default:
                    channelInfos[i] = Resources.Load<Sprite>($"channelInfos/TV{i}");
                    break;
            }
        }
        channelVideos = new VideoClip[efficientChannel];
        for (int i = 2; i < efficientChannel; i++)
        {
            if (i < 10)
            {
                channelVideos[i] = Resources.Load<VideoClip>($"channelVideos/TV00{i}");
            }
            else
            {
                channelVideos[i] = Resources.Load<VideoClip>($"channelVideos/TV0{i}");

            }
        }


        errorVideo = Resources.Load<VideoClip>("specialImages/test");
        blackTexture = Resources.Load<Texture>("specialImages/blackBG");
        timer = 0f;
        // Debug.Log("關電視");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTVOn = !isTVOn;
            if (isTVOn)
            {
                if (channel < 2) channel = 2;
                ChannelStatus(channel);
            }
            else
            {
                videoPlayer.Stop();
                meshRenderer.materials[1].mainTexture = blackTexture;
                nowChannelText.SetActive(false);
                channelInfo.SetActive(false);
                userIn.GetComponent<Text>().text = "";
                userInput = "";
                timer = 0f;
            }
        }
        if (!isTVOn)
        {
            return;
        }

        // 因為getkey會頻繁偵測輸入狀態，可能會造成閃爍以及切換亂掉的狀態，所以改用getkeydown
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            channel++;
            if (channel > maxChannel) channel = 2;
            ChannelStatus(channel);
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            channel--;
            if (channel < 2) channel = maxChannel;
            ChannelStatus(channel);
            timer = 0;
        }

        foreach (char c in Input.inputString)
        {
            userInput += c;
            userIn.GetComponent<Text>().text = userInput;
        }
        if (Input.GetKeyDown(KeyCode.Return)) // enter之後才正式輸入
        {
            userIn.GetComponent<Text>().text = "";
            if (int.TryParse(userInput, out int inputChannel)) // 判斷輸入的字串能不能轉換為數字，可以就放進out那邊的變數
            {
                if (inputChannel > 1 && inputChannel <= maxChannel)
                {
                    channel = inputChannel;
                    ChannelStatus(channel);
                    timer = 0;
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
        switch (ch)
        {
            case < 10:
                nowChannelText.GetComponent<Text>().text = "00" + ch;
                break;
            case < 100:
                nowChannelText.GetComponent<Text>().text = "0" + ch;
                break;
            default:
                nowChannelText.GetComponent<Text>().text = ch.ToString();
                break;
        }
        videoPlayer.Stop();

        channelInfo.SetActive(true);
        channelInfo.GetComponent<Image>().sprite = channelInfos[ch];
        meshRenderer.materials[1].mainTexture = blackTexture;
        StartCoroutine(playwithDelay(ch));

    }

    IEnumerator playwithDelay(int ch) //協程等待機制，不會擋住主程式行動
    {
        if (ch > 1 && ch < efficientChannel)
        {
            videoPlayer.clip = channelVideos[ch];

            yield return new WaitForSeconds(0.5f);
            videoPlayer.Play(); // 把指定影片放進去後還要播放才會正式開始 or 打勾play on awake

        }
        else
        {
            videoPlayer.clip = errorVideo;

            yield return new WaitForSeconds(0.5f);
            videoPlayer.Play();

        }
    }
    // 如果不想用協程，也可以在主程式裡搭配async + await Task.Delay(毫秒)來達成等待
}