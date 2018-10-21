import java.io.*;// Fileなどに必要

public class PlayerData {
	// ルーレットリセット時のセマフォア
	private static boolean semaphore = false;
	// ランクリセット時のセマフォア
	private static boolean rankSemaphore = false;
	// 現在ファイルを書き換えている数
	private static int numSema = 0;
	
	// 各プレイヤーのファイル名の始まり
	protected final static String USER_FILE_NAME_BASE = "../data/user";
	
	// ユーザー情報
	protected int num;
	protected String name;
	protected String password;
	protected String field;
	protected int gold;
	protected int numRoulette;
	protected String skillList;
	protected int car;
	protected String todoList;
	
	// とりあえずインスタンスが必要なログイン用
	PlayerData(){
	}
	PlayerData(int num, String name, String password, String field, int gold, int numRoulette, String skillList, int car, String todoList){
		this.num = num;
		this.name = name;
		this.password = password;
		this.field = field;
		this.gold = gold;
		this.numRoulette = numRoulette;
		this.skillList = skillList;
		this.car = car;
		this.todoList = todoList;
	}
	protected void reset(int num, String name, String password, String field, int gold, int numRoulette, String skillList, int car, String todoList){
		this.num = num;
		this.name = name;
		this.password = password;
		this.field = field;
		this.gold = gold;
		this.numRoulette = numRoulette;
		this.skillList = skillList;
		this.car = car;
		this.todoList = todoList;
	}
	// trueなら++, falseなら--
	private synchronized static void incNumSema(boolean inc){
		if(inc)
			numSema++;
		else
			numSema--;
	}
	protected static int getNumSema(){
		return numSema;
	}
	
	//　ルーレットリセット時にのみ呼ばれるセマフォアをセットする関数
	protected static void setSemaphore(boolean set){
		semaphore = set;
	}
	// ランクもリセット時に呼ばれるセマフォア
	protected static void setRankSemaphore(boolean set){
		rankSemaphore = set;
	}
	protected static boolean getSemaphore(){
		return  semaphore;
	}
	protected static boolean getRankSemaphore(){
		return  rankSemaphore;
	}
	
	
	// 自分のユーザー番号のファイルに書き込む
	protected void fileOut(){
		while(semaphore){
			numRoulette = Server.FIRST_ROULETTE_NUM;
			if(rankSemaphore)
				gold = 0;
		};
		incNumSema(true);
		String fileName = USER_FILE_NAME_BASE + String.valueOf(num) + ".txt";
		try{
			FileWriter fileWrite = new FileWriter(fileName);
			BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
			bufFileWrite.write(name + "\n" + password + "\n" + field + "\n" + gold + "\n" + numRoulette + "\n" + skillList + "\n" + car + "\n" + todoList + "\n");
			bufFileWrite.close();
			fileWrite.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}
		incNumSema(false);
	}
}
