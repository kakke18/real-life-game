import java.io.*;
import java.util.*;

// ルーレットトランクのリセットを命令するクラス

// 1日以上サーバが起動できない状況はないと考えている
// つまり、日付と曜日でリセットを判断している
// 手動も可
public class Reset extends Thread{
	private static int day;
	private static int dayOfWeek;
	private static Calendar cal;
	
	Reset(){
		getText();
	}
	public void run(){
		while(true){
			int nowDay;
			int nowDayOfWeek;
			
			cal = Calendar.getInstance();
			// 日付が一緒なら関係ない
			nowDay = cal.get(Calendar.DATE);
			if(day == nowDay)
				continue;
			
			// 曜日が一緒ならランキングもリセット
			nowDayOfWeek = cal.get(Calendar.DAY_OF_WEEK);
			if(dayOfWeek == nowDayOfWeek){
				rRank();
				day = nowDay;
				dayOfWeek = nowDayOfWeek;// 念のため
				setText();
			}else{
				rRoulette();
				day = nowDay;
				// dayOfWeekは変更しない
				setText();
			}
		}
	}
	
	
	// 日付ファイルを読み込む
	private static void getText(){
		try{
			FileReader fileRead = new FileReader("../data/DAY.txt");
			BufferedReader bufFileRead = new BufferedReader(fileRead);
			day = Integer.parseInt(bufFileRead.readLine());
			dayOfWeek = Integer.parseInt(bufFileRead.readLine());
			bufFileRead.close();
			fileRead.close();
		}
		catch(Exception e){
			cal = Calendar.getInstance();
			day = cal.get(Calendar.DATE);
			dayOfWeek = cal.get(Calendar.DAY_OF_WEEK);
			setText();
		}
	}
	// 日付ファイルに書き込む
	private static void setText(){
		try{
			FileWriter fileWrite = new FileWriter("../data/DAY.txt");
			BufferedWriter bufFileWrite = new BufferedWriter(fileWrite);
			bufFileWrite.write(day + "\n" + dayOfWeek + "\n");
			bufFileWrite.close();
			fileWrite.close();
		}
		catch(Exception e){
			e.printStackTrace();
		}
	}
	
	// ルーレットをリセットする関数
	private static void rRoulette(){
		Server.resetRoulette();
	}
	// ランキングをリセットする関数（ルーレットリセットも含む）
	private static void rRank(){
		Server.resetRank();
	}
}
