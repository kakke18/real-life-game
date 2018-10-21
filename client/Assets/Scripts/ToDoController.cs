using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;

public class ToDoController : Client {

    bool useServer = StartUp.useServer;

    NetworkStream ns;
    Text titleText;
    Text todoEmptyText;
    Text errorText;
    static Text dialogText;
    Button closeDialogButton;
    GameObject item;
    public GameObject prefab;
    static Canvas dialogCanvas = null;
    string todoString;
    string board;
    string userName;
    int removeToDoNum = 0;
    bool todoEmptyFlag = false;
    bool finishToDoFlag = false;
    float lifeTime = 1.0f;
    float passedTime;
    float passedTime2 = 0;

    // Use this for initialization
    void Start () {
        passedTime = 0f;

        /* ToDo情報をセット */
        /* サーバを使う場合 */
        if (useServer) {
            this.ns = StartUp.ns;
            base.sendMessage(ns, "80"); //サーバにToDo情報を要求
            do {
                base.receiveMessage(ns); //サーバから受信
            } while (base.recMsg == ""); //サーバから文字列が受信できたら
            todoString = base.recMsg.Remove(0, 1); //8を取り除く
            todoString = todoString.Remove(todoString.Length - 1); //\nを取り除く
            if (todoString.Length == 0 ) { //ToDoが空なら
                todoEmptyFlag = true;
            }
        }
        /* サーバを使わない場合 */
        else {
            todoString = "レポート,0,ランニング,1,家庭教師,2,家事,3,試験勉強,0,水泳,1,居酒屋,2,ゴミ出し,3";
            //todoString = "";
        }
            
        /* 部品 */
        titleText = GameObject.Find("TitleText").GetComponent<Text>();
        todoEmptyText = GameObject.Find("ToDoEmptyText").GetComponent<Text>();
        errorText = GameObject.Find("ErrorText").GetComponent<Text>();
        dialogText = GameObject.Find("DialogText").GetComponent<Text>();
        dialogCanvas = GameObject.Find("DialogCanvas").GetComponent<Canvas>();
        closeDialogButton = GameObject.Find("CloseButton").GetComponent<Button>();

        closeDialogButton.onClick.AddListener(OnClickButton);

        if (dialogCanvas != null)
        {
            dialogCanvas.enabled = false;
        }

        dialogText.text = "";

        /* 初期化 */
        /* サーバを使う場合 */
        if (useServer) {
            if (SceneController.regi) {
                userName = Registration.name;
            }
            else {
                userName = Login.name;
            }
            titleText.text = userName + "さんのマイページ";
        }
        /* サーバを使わない場合 */
        else {
            titleText.text = "hoge" +  "さんのマイページ";
        }

        /* ToDoリストを表示 */
        UpdateToDoList();;
	}

    void Update () {
        /* ToDoリストの更新 */
        if (ToDoAdder.changeFlag || ToDoRemover.changeFlag) {
            InitToDoList();
            UpdateToDoList();
        }

        /* ToDoリストが空 */
        todoEmptyText.gameObject.SetActive(todoEmptyFlag);

        /* エラーメッセージ */
        if (ToDoAdder.blankFlag) {
            if (ToDoAdder.toggleFlag) {
                errorText.text = "ToDoが未入力です";
            }
            else {
                errorText.text = "カテゴリが未選択です";
            }
            
        }
        else {
            errorText.text = "";
        }

        /* ToDoが終わったら */
        if (finishToDoFlag) {
            /* サーバを使う場合 */
            if (useServer) {
                if (passedTime2 <= 0 ) {
                    base.sendMessage(ns, "2"); //サーバにボード情報を要求
                    passedTime2 += Time.deltaTime;
                }
                else {
                    passedTime2 += Time.deltaTime;
                    if (passedTime2 > 1.0f) {
                        base.receiveMessage(ns); //サーバからボード情報を受信
                        board = base.recMsg.Remove(0, 1);
                        Debug.Log("before:" + board);
                        board = UpdateBoard();
                        Debug.Log("after:" + board);
                        base.sendMessage(ns, "2"+board);
                        passedTime2 = 0;
                        finishToDoFlag = false;
                    }
                }         
            }
            /* サーバを使わない場合 */
            else {
                board = "0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0";
                Debug.Log("before:" + board);
                board = UpdateBoard();
                Debug.Log("after:" + board);
                finishToDoFlag = false;
            }
        }

        //ダイアログが有効であったなら
        if (dialogCanvas.isActiveAndEnabled)
        {
            passedTime += Time.deltaTime;
            //一定時間開いたのち非表示に
            if (passedTime >= lifeTime)
            {
                closeDialog();
            }
        }
    }

    /* ToDoリストを初期化 */
    private void InitToDoList() { 
        foreach (Transform n in this.transform) { 
            GameObject.Destroy(n.gameObject);
        }
    }

    /* ToDoリストを更新 */
    private void UpdateToDoList() {
        //追加
        if (ToDoAdder.changeFlag) {
            AddToDo();
        }

        //完了or削除
        if (ToDoRemover.changeFlag) {
            RemoveToDo();
        }

        //分割
        string[] todoArray = todoString.Split(',');
        int todoNum = todoArray.Length / 2;

        //ToDo表示
        for (int i = 0; i < todoNum; i++) {
            //プレファブを取得
            item = (GameObject)Instantiate(prefab);
            //親を指定
            item.transform.SetParent(transform);
            item.transform.localScale = Vector3.one;
            //Textを取得
            Text[] itemText = item.GetComponentsInChildren<Text>();
            //Textをセット
            itemText[0].text = todoArray[i * 2];
            itemText[0].color = GetToDoColor(todoArray[i * 2  + 1]);
        }
    }

    /* テキストのカラーを取得 */
    private Color GetToDoColor (string str) {
        int num = int.Parse(str);

        switch (num) {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
        }
        return Color.white;
    }

    private static string GetToDoCategory(Color c)
    {
        if (c.Equals(Color.red))
        {
            return "勉強";
        }
        if (c.Equals(Color.blue))
        {
            return "スポーツ";
        }
        if (c.Equals(Color.green))
        {
            return "バイト";
        }

        return "その他";
    }

    /* ToDoリストの追加 */
    private void AddToDo () {
        if (todoString.Length == 0) { //ToDoが空なら,を取り除く
            ToDoAdder.addToDo = ToDoAdder.addToDo.Remove(0, 1);
        }
        todoString += ToDoAdder.addToDo;
        ToDoAdder.addToDo = "";
        ToDoAdder.changeFlag = false;
        todoEmptyFlag = false;

        /* サーバを使う場合 */
        if (useServer) {
            //サーバにToDo情報を送信
            base.sendMessage(ns, "81" + todoString);
        }
    }

    /* ToDoリストの完了or削除 */
    private void RemoveToDo () {
        //分割
        string[] array = todoString.Split(',');

        //削除する要素を"\n"に置き換える
        for (int i = 0; i < array.Length; i += 2 ) {
            if( array[i].Equals(ToDoRemover.removeToDo)) {
                array[i] = "\n";
                removeToDoNum = int.Parse(array[i+1]);
                array[i + 1] = "\n";
             }
        }

        //todoStringを更新
        todoString = "";
        foreach (string ele in array) {
            if (!ele.Equals("\n")){
                todoString += "," + ele;
            }
        }

        //ToDoリストが空
        if (todoString.Length == 0) {
            todoEmptyFlag = true;
        }
        else {
            todoString = todoString.Remove(0, 1);
        }

        //削除ではなく完了なら
        if (ToDoRemover.finishFlag) {
            finishToDoFlag = true;
            ToDoRemover.finishFlag = false;
        }

        //初期化
        ToDoRemover.removeToDo = "";
        ToDoRemover.changeFlag = false;

        /* サーバを使う場合 */
        if (useServer) {
            //サーバにToDo情報を送信
            base.sendMessage(ns, "81" + todoString);
        }
    }

    //ダイアログを開く
    public static void OpenDialog()
    {
        string s = ToDoRemover.removeToDo;
        if(dialogCanvas != null)
        {
            dialogCanvas.enabled = true;
            //完了なら
            if (ToDoRemover.finishFlag)
            {
                dialogText.text = s + " をチェック\n";
                dialogText.text +=GetToDoCategory(ToDoRemover.category) +  " のマスを取得しました\n";
            }
            //削除なら
            else
            {
                dialogText.text = s + " を消去\n";
            }
        }
    }

    private static object GetColor(int v)
    {
        throw new NotImplementedException();
    }

    //ダイアログを閉じる
    public void closeDialog()
    {
        if(dialogCanvas != null)
        {
            dialogCanvas.enabled = false;
            passedTime = 0;
        }
    }

    //ボタンでダイアログを閉じる
    public void OnClickButton()
    {
        closeDialog();
    }

    /* ボード情報更新 */
    private string UpdateBoard()
    {
        //シャッフルする配列
        int[] ary1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        //シャッフルする
        int[] ary2 = ary1.OrderBy(i => Guid.NewGuid()).ToArray();

        //ボード情報を分割
        string[] boardAry = board.Split(':');

        //removeToDoNumによって書き換えるボード情報を変更
        string changeBoard = "";
        changeBoard += (removeToDoNum + 1).ToString() + ",";
        switch (removeToDoNum)
        {
            case 0:
                int num = UnityEngine.Random.Range(0, 4);
                changeBoard += num.ToString() + ",カード" + (num + 1).ToString();
                break;
            case 1:
                changeBoard += "0,ルーレット券";
                break;
            case 2:
                changeBoard += "0,多めのゴールド";
                break;
            case 3:
                changeBoard += "5000,5000G";
                break;
        }

        //シャッフルした配列をもとにボード情報をランダムに変更
        int j;
        for (j = 0; j < ary2.Length; j++)
        {
            //何もないマスなら
            if (boardAry[ary2[j]].Equals("0"))
            {
                boardAry[ary2[j]] = changeBoard;
                break;
            }
        }

        //何もないマスがない場合，ランダムに変更
        if (j == 20)
        {
            boardAry[ary2[0]] = changeBoard;
        }

        //boardAryからboardへ変換
        board = "";
        foreach (string ele in boardAry)
        {
            board += ":" + ele;
        }
        board = board.Remove(0, 1);

        //ボード情報を返す
        return board;
    }
}