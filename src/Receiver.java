import java.io.*;// InputStreamなどに必要
import java.net.*;// ServerSocketに必要

public class Receiver extends Thread{
	// クライアントから来たデータがどのタイプか
	private final static int NEW_PLAYER = 0;      // 新規登録
	private final static int LOGIN = 1;           // ログイン
	private final static int FIELD = 2;     	  // 盤面
	private final static int GOLD = 3;			  // ゴールド
	private final static int ROULETTE = 4;		  // ルーレット
	private final static int RANK = 5;			  // ランキング
	private final static int SKILL = 6;			  // スキル
	private final static int CAR = 7;			  // 車
	private final static int TO_DO_LIST = 8;	  // todoリスト
	
	private InputStreamReader sisr; //受信データ用文字ストリーム
	private BufferedReader br; //文字ストリーム用のバッファ
	protected PrintWriter printWriter; //データ送信用オブジェクト
	
	private PlayerData myData = new PlayerData();
	
	Receiver(Socket socket){
		try{
			sisr = new InputStreamReader(socket.getInputStream());
			br = new BufferedReader(sisr);
			printWriter = new PrintWriter(socket.getOutputStream(), true);
		} catch (IOException e) {
			System.err.println("データ受信時にエラーが発生しました: " + e);
		}
	}
	
	protected void resetRoulette(){
		myData.numRoulette = Server.FIRST_ROULETTE_NUM;
	}
	protected void resetGold(){
		myData.gold = 0;
	}
	public void run(){
		int type;
		try{
			while(true) {// データを受信し続ける
				String inputLine = br.readLine();//データを一行分読み込む
				if (inputLine != null){ //データを受信したら
					System.out.println(inputLine);
					type = Integer.parseInt("" +inputLine.charAt(0));
					String str = inputLine.substring(1);
					
					// 万が一 ランクリセットと
					// 盤面情報 or ゴールド or ルーレット or スキルが被ったらやめる
					if(PlayerData.getRankSemaphore())
						if(type == FIELD || type == GOLD || type == ROULETTE || type == SKILL)
							continue;
					// ルーレットとルーレットリセットが被ったらやめる
					if(PlayerData.getSemaphore() && type == ROULETTE)
						continue;
					
					// それ以外がルーレットリセットと被ったら待機
					while(PlayerData.getSemaphore());
					
					switch(type){
						case NEW_PLAYER:
							// パスワード
							while((inputLine = br.readLine()) == null);
							// 名前が使われていなければOKで、ユーザー番号が返ってくる
							int judge = Server.isNotUsed(str, inputLine);
							if(judge >= 0){
								myData = new PlayerData(judge, str, inputLine, "",0 ,Server.FIRST_ROULETTE_NUM, "", 0, "");
								myData.fileOut();
								Server.makeRank(myData.name, myData.gold);
								printWriter.println("01");
							}
							else
								printWriter.println("00");
							printWriter.flush();
							break;
						case LOGIN:
							while((inputLine = br.readLine()) == null);
							printWriter.println("1" + Server.login(str, inputLine, myData));
							printWriter.flush();
							break;
						case FIELD:
							if(str.isEmpty()){
								printWriter.println("2" + myData.field);
								printWriter.flush();
							}else{
								myData.field = str;
								myData.fileOut();
							}
							break;
						case GOLD:
							if(str.isEmpty()){
								printWriter.println("3" + myData.gold);
								printWriter.flush();
							}else{
								myData.gold = Integer.parseInt(str);
								Server.makeRank(myData.name, myData.gold);
								myData.fileOut();
								
							}
							break;
						case ROULETTE:
							if(str.isEmpty()){
								printWriter.println("4" + myData.numRoulette);
								printWriter.flush();
							}else{						
								myData.numRoulette = Integer.parseInt(str);
								myData.fileOut();
							}
							break;
						case RANK:
							// str週前ののランキング
							printWriter.println("5" + Server.sendRank(Integer.parseInt(str)));
							printWriter.flush();
							break;
						case SKILL:
							if(str.isEmpty()){
								printWriter.println("6" + myData.skillList);
								printWriter.flush();
							}else{
								myData.skillList = str;
								myData.fileOut();
							}
							break;
						case CAR:
							if(str.isEmpty()){
								printWriter.println("7" + myData.car);
								printWriter.flush();
							}else{
								myData.car = Integer.parseInt(str);
								myData.fileOut();
							}
							break;
						case TO_DO_LIST:
							int subtype = Integer.parseInt("" +str.charAt(0));
							str = str.substring(1);
							if(subtype == 0){
								printWriter.println("8" + myData.todoList);
								printWriter.flush();
							}else if(subtype == 1){
								myData.todoList = str;
								myData.fileOut();
							}
							break;
						default:
							System.out.println("クライアント側からの送信のタイプが不適です");
							break;
					}
				}
			}
		} catch (IOException e){ // 接続が切れたとき
		}
	}
}
