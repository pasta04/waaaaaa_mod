using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI; // <---- 追加1

public class ShowHeartRate : MonoBehaviour
{
    // シリアルポート通信
    public SerialHandler serialHandler;

    // 心拍数表示テキストオブジェクト
    public Text heartRateText;

    // ハートオブジェクト
    public Text heartText;

    private string fontFamily = "Arial";

    private int portNum = 1;

    public TitleBarSetter Instance;

    private string[] fontnames;
    private int fontIndex = 0;

    async void Start()
    {
        Debug.Log("[ShowHeartRate][Start] start");
        ReadConfig();
        serialHandler.OnDataReceived += OnDataReceived;

        fontnames = Font.GetOSInstalledFontNames();
        // for (int i = 0; i < fontnames.Length; i++)
        // {
        //     Debug.Log(fontnames[i]);
        // }

        await Task.Delay(2000);
        Debug.Log("[ShowHeartRate][Start] end");
    }

    // 毎フレームの処理
    void Update()
    {
        int baseSize = 150;
        // 心拍数に応じて鼓動させる
        int heartrate = 0;
        // Debug.Log(heartRateText.text);
        bool result = int.TryParse(heartRateText.text, out heartrate);
        if (result)
        {
            int freq = int.Parse(heartRateText.text);
            float omega = 2 * Mathf.PI * freq / 120;
            float t = Time.time;
            float emphasis = freq > 140 ? 1.5f : 1f;
            float x = emphasis * baseSize * (Mathf.Abs(Mathf.Sin(omega * t)) + 0.2f);
            heartText.fontSize = Mathf.FloorToInt(x);
            heartText.color = Color.red;
        }

        DownKeyCheck();
    }

    void ReadConfig()
    {
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + "/settings.ini");
        Debug.Log(ini.iniString);
        fontFamily = ini.ReadValue("Settings", "FontFamily", "Arial");
        Font font = Font.CreateDynamicFontFromOSFont(fontFamily, 120);
        heartRateText.font = font;
        ini.WriteValue("Settings", "FontFamily", fontFamily);
        ini.Close();
    }

    // 受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        Debug.Log("[ShowHeartRate] message: " + message);

        try
        {
            int heartrate = 0;
            // Debug.Log(heartRateText.text);
            bool result = int.TryParse(message, out heartrate);
            if (result)
            {
                heartRateText.text = heartrate.ToString("D3");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    // キー入力を検知
    void DownKeyCheck()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            // ポート番号変更(戻る)
            portNum = portNum - 1;
            if (portNum < 1) portNum = 1;
            serialHandler.portName = "COM" + portNum;

            Instance.SetTitleBar("COM" + portNum);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // ポート番号変更(進む)
            // ポート番号変更(戻る)
            portNum = portNum + 1;
            serialHandler.portName = "COM" + portNum;

            Instance.SetTitleBar("COM" + portNum);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // テキスト色の変更(黒)
            heartRateText.color = new Color(0 / 255f, 0 / 255f, 0 / 255f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // テキスト色の変更(白)
            heartRateText.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            // フォント(戻る)
            fontIndex = fontIndex - 1;
            if (fontIndex < 0) fontIndex = 0;
            fontFamily = fontnames[fontIndex];
            Font font = Font.CreateDynamicFontFromOSFont(fontFamily, 120);
            heartRateText.font = font;
            // heartText.font = font;
            Debug.Log(fontIndex + fontFamily);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            // フォント(進む)
            fontIndex = fontIndex + 1;
            if (fontIndex > fontnames.Length - 1) fontIndex = fontnames.Length - 1;
            fontFamily = fontnames[fontIndex];
            Font font = Font.CreateDynamicFontFromOSFont(fontFamily, 120);
            heartRateText.font = font;
            // heartText.font = font;
            Debug.Log(fontIndex + fontFamily);
        }
    }
}