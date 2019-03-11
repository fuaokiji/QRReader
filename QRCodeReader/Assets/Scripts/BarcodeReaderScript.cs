using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

public class BarcodeReaderScript : MonoBehaviour
{
    public Text resultText; //読み取り結果

    //カメラの解像度を知りたい場合はコメントを外して「Resolution」のつくオブジェクトをアクティブ化
    //public Text resolutionText;

    public Button btnRegist;
    public Button btnCancel;
    public RawImage cameraPanel;

    private WebCamTexture webcamTexture;
    private BarcodeReader reader = new BarcodeReader();
    private Color32[] _data;

    private int width = 800;
    private int height = 1280;
    private string foundString = null;

    private string codeResult = null;

    void Awake()
    {
        RectTransform textRect = cameraPanel.GetComponent<RectTransform>();

        // 開発している画面を元に縦横比取得 (縦画面) 
        float developAspect = 800.0f / 1280.0f;
        // 横画面で開発している場合は以下と切り替える
        //float developAspect = 1280.0f / 800.0f;

        // 実機のサイズを取得し縦横比を計算
        float deviceAspect = (float)Screen.width / Screen.height;
        //DeviceText.text = "<color=#111111>" + Screen.width + " " + Screen.height + " " + "</color>";

        // 実機と開発画面との対比
        float scale = deviceAspect / developAspect;

        Camera mainCamera = Camera.main;

        // カメラに設定していたorthographicSizeを実機との対比でスケール
        float deviceSize = mainCamera.orthographicSize;
        // scaleの逆数
        float deviceScale = 1.0f / scale;
        // orthographicSizeを計算し直す
        mainCamera.orthographicSize = deviceSize * deviceScale;

        //CameraPanelのサイズを実際のサイズと同じ大きさに変更する
        textRect.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    private IEnumerator Start()
    {
        // ボタンは最初は非表示
        ChangeBtnVisible(false);
        Debug.Log("68");

        //カメラの有無や許可の確認
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogFormat("カメラがありません");
            yield break;
        }
        Debug.Log("76");

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Debug.Log("79");
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogFormat("カメラ利用が許可されていません");
            yield break;
        }
        Debug.Log("85");

        WebCamDevice userCameraDevice = WebCamTexture.devices[0];
        Debug.Log("88");

        if (WebCamTexture.devices.Length > 0)
        {
            webcamTexture = new WebCamTexture(userCameraDevice.name, width, height);
            //webcamTexture = new WebCamTexture(userCameraDevice.name);
            Debug.Log("94");

            var euler = transform.localRotation.eulerAngles;
            Debug.Log("97");

            //iPhone,Androidの場合はカメラの向きを縦にする
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                cameraPanel.transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z - 90);
                Debug.Log("103");

                float AfterW = (float)Screen.height / Screen.width;
                Debug.Log("106");
                float AfterH = (float)Screen.width / Screen.height;
                Debug.Log("108");
                cameraPanel.transform.localScale = new Vector3(AfterW, AfterH, 1);
                //AlterationText.text = "<color=#111111>" + AfterW + " " + AfterH + "</color>";
                Debug.Log("111");

                //さらにiOSであった場合WebCamTextureを映しているRaw画像を反転させる
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    cameraPanel.transform.localScale = new Vector3(-1, 1, 1);
                    Debug.Log("117");
                }
                Debug.Log("119");
            }
        }
        Debug.Log("122");

        //パネルにカメラから取得した映像を反映
        cameraPanel.texture = webcamTexture;
        Debug.Log("126");
        webcamTexture.Play();
        Debug.Log("128");

        Debug.Log(webcamTexture.width + " " + webcamTexture.height + " " + webcamTexture.requestedFPS);
        Debug.Log("131");

        //カメラの解像度を知りたい場合はコメントを外して「Resolution」のつくオブジェクトをアクティブ化
        //resolutionText.text = "<color=#111111>" + webcamTexture.width + " " + webcamTexture.height + "</color>";
    }

    // Update is called once per frame
    void Update()
    {
        //Escキーを押した場合アプリを終了
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            return;
        }

        //if (codeResult != null && btnRegist.gameObject.activeSelf)

        if (foundString == null)
        {
            // コード発見前
            // バーコード認識

            Color32[] data = webcamTexture.GetPixels32();
            //reader.SetData(data);
            if (_data == null)
            {
                _data = data;
            }

            var task = TaskAsync();
            //Debug.Log("Task終了");

            if (codeResult == null)
            {
                resultText.text = "<color=#111111>scanning...</color>";
            }
            else
            {
                foundString = codeResult;
                resultText.text = "<color=#111111>" + codeResult + "</color>";
            }

        }
        else
        {
            //コード発見後
            ChangeBtnVisible(true);
        }
    }

    public async Task TaskAsync()
    {
        int Wid = webcamTexture.width;
        int Het = webcamTexture.height;
        //Debug.Log("Task起動成功");

        Result result = await Task.Run(() => reader.Decode(_data, Wid, Het));
        //Debug.Log("Task通過");
        if (result != null)
        {
            codeResult = result.Text;
            Debug.Log(result.Text);
            Debug.Log("read success");
        }
        else
        {
            codeResult = null; //エラー時
        }
        _data = null;
        Debug.Log("read error");
    }

    //ブラウザ起動やキャンセルのボタンを可視化
    void ChangeBtnVisible(bool state)
    {
        btnRegist.gameObject.SetActive(state);
        btnCancel.gameObject.SetActive(state);
    }

    //読み取ったバーコードがURL(Httpから始まる)場合URLを直接開き、そうでない場合はGoogle検索の結果を表示
    private void OpenBarcode(string str)
    {
        if (str.StartsWith("http"))
        {
            Application.OpenURL(str);
        }
        else
        {
            Application.OpenURL("http://www.google.com/search?q=" + str);
        }
    }

    //ブラウザ起動を選択した場合上記OpenBarcode関数をコール
    public void OnBtnRegistClick()
    {
        OpenBarcode(foundString);
        foundString = null;
        codeResult = null;
        ChangeBtnVisible(false);
        _data = null;
        Debug.Log("btnRegist");
    }

    //キャンセルを選択した場合読み取った内容をNullに初期化しボタンを不可視化、再度読み取りを開始
    public void OnBtnCancelClick()
    {
        foundString = null;
        codeResult = null;
        ChangeBtnVisible(false);
        _data = null;
        Debug.Log("BtnCancel");
    }

    //左上の「終了する」ボタンを押すとアプリが終了、読み取り結果を持ち越したくないので念の為初期化する
    public void OnBtnExitClick()
    {
        foundString = null;
        codeResult = null;
        ChangeBtnVisible(false);
        _data = null;
        Application.Quit();
    }
}