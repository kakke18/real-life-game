import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
// ServerSocketに必要
import java.net.ServerSocket;
import java.net.Socket;
// ArrayListに必要
import java.util.ArrayList;

public class Server {
	//-----------------------ここから定数--------------------------------------------//

	// ランキングのファイル名の始まり
	private final static String RANK_FILE_NAME_BASE = "../data/rank";
	// プレイヤー名の一覧（行数がプレイヤー名と一致）
	private final static String ALL_NAME_FILE = "../data/allUser.txt";

	// リセットされたときのルーレットの回数
	public final static int FIRST_ROULETTE_NUM = 10;
	// PlayerDataで何行目に書き込むか
	private final static int FIELD_POS = 3;
	private final static int GOLD_POS = 4;
	private final static int ROULETTE_POS = 5;
	private final static int SKILL_POS = 6;

	//-----------------------ここから変数--------------------------------------------//

	// 日付
	//private static
	// ランキングのテキストの番号
	private static int numRankFile = 1;

	// ALL_NAME_FILEのセマフォア
	private static boolean userSemaphore = false;
	// RANK_FILEのセマフォア
	private static boolean rankSemaphore = false;

	// Receiverの配列
	private static ArrayList<Receiver> receiver = new ArrayList<Receiver>();
	// 全ユーザー名の配列
	private static ArrayList<String> userName = new ArrayList<String>();
	// ランキングの配列
	private static ArrayList<Rank> rankList = new ArrayList<Rank>();

	public static void main(String[] args) {
		//new Demo();
		//　全ユーザー名を読み込む
		Server.readAllNameFile();
		// ランキングを読み込む
		ReadRank();
		// リセット用のスレッドを開始
		Reset re = new Reset();
		re.start();
		try{
			System.out.println("サーバを起動しました");
			// ポートを取得
	        int port = Integer.parseInt(args[0]);
			// サーバーソケットを作成
	        ServerSocket ss = new ServerSocket(port);
	        while (true) {
	        	// 新規接続を受け付ける(ここで止まる)
	        	Socket socket = ss.accept();
	        	System.out.println("クライアントから接続要求がありました");
	        	receiver.add(new Receiver(socket));
         		receiver.get(receiver.size() - 1).start();
		   }
		}
		catch (Exception e) {
			e.printStackTrace();
		}
	}

	private synchronized static void setUserSemaphore(boolean s){
		if(s)
			while(userSemaphore);// 同時に入ろうとして、次に来たセマフォアが止まるタイミング
		userSemaphore = s;
	}
	private synchronized static void setRankSemaphore(boolean s){
		if(s)
			while(rankSemaphore);// 同時に入ろうとして、次に来たセマフォアが止まるタイミング
		rankSemaphore = s;
	}

	// 新規登録（名前が被ってなければ戻り値は登録順の番号、被ってれば-1）
	protected synchronized static int isNotUsed(String name, String password){
		if(userName.contains(name))
			return -1;
		userName.add(name);
		addAllNameFile(name);
		return userName.size() - 1;
	}
	// ＜戻り値＞
	// ログイン失敗：
	//			"01":ユーザー名が存在しない
	//			"02":ユーザー名とパスワードが一致しない
	//			"03":サーバ側の異常
	// ログイン成功："1"
	protected static String login(String name, String password, PlayerData myData){
		int num = userName.indexOf(name);
		if(num == -1)
			return "01";
		try{
			FileReader fileRead = new FileReader("../data/" + PlayerData.USER_FILE_NAME_BASE + String.valueOf(num) + ".txt");
			BufferedReader bufFileRead = new BufferedReader(fileRead);
			// 名前かパスワードが違ったらダメ
			if(!bufFileRead.readLine().equals(name)){
				bufFileRead.close();
				fileRead.close();
				// ここは本来なら来ないはず
				System.out.println("サーバ側において、配列userNameの内容と対応する番号に異常があります");
				System.out.println("num:" + num + ", name:" + name);
				return "03";
			}
			if(!bufFileRead.readLine().equals(password)){
				bufFileRead.close();
				fileRead.close();
				return "02";
			}
			String field = bufFileRead.readLine();
			int gold = Integer.parseInt(bufFileRead.readLine());
			int numRoulette = Integer.parseInt(bufFileRead.readLine());
			String skillList = bufFileRead.readLine();
			int car = Integer.parseInt(bufFileRead.readLine());
			String todoList = bufFileRead.readLine();
			myData.reset(num, name, password, field, gold, numRoulette, skillList, car, todoList);
			bufFileRead.close();
			fileRead.close();
		}
		catch(Exception e){
			e.printStackTrace();
			return "03";
		}
		return "1";
	}
	// 全ユーザー名ファイルから読み込む
	private static void readAllNameFile(){
		while(userSemaphore);
		setUserSemaphore(true);
		try{
			FileReader fileRead = new FileReader(ALL_NAME_FILE);
			BufferedReader bufFileRead = new BufferedReader(fileRead);
			while(true){
				if(bufFileRead.ready())
					userName.add(bufFileRead.readLine());
				else
					break;
			}
			bufFileRead.close();
			fileRead.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}
		setUserSemaphore(false);
	}
	// 全ユーザー名ファイルに追加
	private synchronized static void addAllNameFile(String name){
		while(userSemaphore);
		setUserSemaphore(true);
		try{
			FileWriter fileWrite = new FileWriter(ALL_NAME_FILE, true);
			BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
			bufFileWrite.write(name + "\n");
			bufFileWrite.close();
			fileWrite.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}
		setUserSemaphore(false);
	}
	// ルーレットのみをリセット
	protected static void resetRoulette(){
		PlayerData.setSemaphore(true);

		// 各ファイルの作業が終わるまで待つ
		while(PlayerData.getNumSema() != 0);
		allResetRoulette(false);

		PlayerData.setSemaphore(false);
	}
	// ランキングを初期化
	protected static void resetRank(){
		PlayerData.setRankSemaphore(true);
		PlayerData.setSemaphore(true);
		while(rankSemaphore);
		setRankSemaphore(true);
		rankList.clear();
		numRankFile++;
		// あらかじめからファイルを作っておく
		try{
			FileWriter fileWrite = new FileWriter(RANK_FILE_NAME_BASE + String.valueOf(numRankFile)+ ".txt");
			BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
			// ゴールド初期値でもユーザーは全員ランキングに入れる
			for(int i = 0; i < userName.size(); i++){
				bufFileWrite.write(userName.get(i) + "\n" + "0" + "\n");
				rankList.add(new Rank(userName.get(i), 0));
			}
			bufFileWrite.close();
			fileWrite.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}
		// 各ファイルの作業が終わるまで待つ
		while(PlayerData.getNumSema() != 0);
		allResetRoulette(true);
		setRankSemaphore(false);
		PlayerData.setSemaphore(false);
		PlayerData.setRankSemaphore(false);
	}
	// allがtrueならゴールドもリセット
	private static void allResetRoulette(boolean all){
		for(int i = 0; i < receiver.size(); i++){
			receiver.get(i).resetRoulette();
			if(all)
				receiver.get(i).resetGold();
		}

		String fileName;
		ArrayList<String> strList = new ArrayList<String>();
		try{
			for(int i = 0; true; i++){
				strList.clear();
				fileName = "../data/" + PlayerData.USER_FILE_NAME_BASE + String.valueOf(i)+".txt";
				File file = new File(fileName);
				// 最後まで行ったら
				if(!file.exists())
					break;
				//-------------------------------ここで読み込んで--------------------------
				FileReader fileRead = new FileReader(fileName);
				BufferedReader bufFileRead = new BufferedReader(fileRead);
				while(true){
					if(bufFileRead.ready()){
						strList.add(bufFileRead.readLine());
					}else
						break;
				}
				strList.set(ROULETTE_POS - 1, String.valueOf(FIRST_ROULETTE_NUM));
				if(all){
					//strList.set(FIELD_POS - 1, "");
					strList.set(GOLD_POS - 1, "0");
					//strList.set(SKILL_POS - 1, "");
				}
				bufFileRead.close();
				fileRead.close();
				//-------------------------------ここで書き込む---------------------------
				FileWriter fileWrite = new FileWriter(fileName);
				BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
				for(int j = 0; j < strList.size(); j++)
					bufFileWrite.write(strList.get(j) + "\n");
				bufFileWrite.close();
				fileWrite.close();
			}
		}
		catch(Exception e){
			e.printStackTrace();
		}
	}
	// ランクファイルを読み込む
	private static void ReadRank(){
		String fileName;
		// 現在一番新しいランクファイルを読み込む
		for(int i = 1; true; i++){
			fileName = RANK_FILE_NAME_BASE + String.valueOf(i)+ ".txt";
			File file = new File(fileName);
			if(!file.exists()){
				if(i > 1)
					fileName = RANK_FILE_NAME_BASE + String.valueOf(i - 1)+ ".txt";
				numRankFile = i - 1;
				break;
			}
		}
		// まだファイルがないときは読み込めない
		if(numRankFile == 0){
			numRankFile = 1;
			// 一応作っておく
			try{
				FileWriter fileWrite = new FileWriter(fileName);
				fileWrite.close();
			}
			catch(Exception e){
				e.printStackTrace();
			}
		}else{
			// 実際に読み込む
			try{
				FileReader fileRead = new FileReader(fileName);
				BufferedReader bufFileRead = new BufferedReader(fileRead);
				while(true){
					if(!bufFileRead.ready())
						break;
					String name = bufFileRead.readLine();
					int money = Integer.parseInt(bufFileRead.readLine());
					rankList.add(new Rank(name, money));
				}
				bufFileRead.close();
				fileRead.close();
			}
			catch(Exception e){
				e.printStackTrace();
			}
		}
	}
	protected synchronized static void makeRank(String name, int money){
		while(rankSemaphore);
		setRankSemaphore(true);

		boolean isWritten = false;
		boolean isRemoved = false;

		for(int i = 0; i < rankList.size(); i++){
			if(rankList.get(i).getName().equals(name)){
				rankList.remove(i);
				isRemoved = true;
			}
			// もし自分のほうが金額が大きければ
			if(!isWritten && money > rankList.get(i).getMoney()){
				// その位置に追加
				rankList.add(i, new Rank(name, money));
				// 同じものを2回通過する必要はない
				i++;
				isWritten = true;
			}
			// どちらの作業も終わったら抜ける
			if(isWritten && isRemoved)
				break;
		}
		// 書き込んでいないときは自分を書き込む
		if(!isWritten){
			rankList.add(new Rank(name, money));
		}
		// ファイルを実際に書き込む部分
		try{
			FileWriter fileWrite = new FileWriter(RANK_FILE_NAME_BASE + String.valueOf(numRankFile)+ ".txt");
			BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
			for(int i = 0; i < rankList.size();i++)
				bufFileWrite.write(rankList.get(i).getName() + "\n" + rankList.get(i).getMoney() + "\n");
			bufFileWrite.close();
			fileWrite.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}

		setRankSemaphore(false);
	}
	// クライアントからのランキング要求
	protected synchronized static String sendRank(int num){
		while(rankSemaphore);
		setRankSemaphore(true);
		String retStr = "";
		// 今週ならファイルを読み込まないほうが早い
		if(num == 0){
			for(int i = 0; i < rankList.size(); i++){
				retStr += rankList.get(i).out();
			}
		}else{
			// そもそも範囲内か
			if(numRankFile > num){
				// これがファイルの数字になる
				num = numRankFile - num;
				try{
					FileReader fileRead = new FileReader(RANK_FILE_NAME_BASE + String.valueOf(num) + ".txt");
					BufferedReader bufFileRead = new BufferedReader(fileRead);
					while(true){
						if(!bufFileRead.ready())
							break;
						retStr += bufFileRead.readLine() + ",";
					}
					bufFileRead.close();
					fileRead.close();
				}
				catch(Exception e){
					e.printStackTrace();
				}
			}
		}
		setRankSemaphore(false);
		// 最後の,を消す
		if(retStr.length() > 0)
			retStr = retStr.substring(0, retStr.length() - 1);
		return retStr;
	}
}
