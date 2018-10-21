package server;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;


// デモ用
// すべてprivateで書く
public class Demo extends JFrame implements ActionListener{
	private final static int BUTTON_NUM = 2;
	private final static String BUTTON_STR[] = {"1日が終了", "1週間が終了"};
	Demo(){
		super("デモ用");
		
		setLayout(new FlowLayout());
		JPanel panel[] = new JPanel[BUTTON_NUM];
		JButton button[] = new JButton[BUTTON_NUM];
		for(int i = 0; i < BUTTON_NUM; i++){
			panel[i] = new JPanel();
			button[i] = new JButton(BUTTON_STR[i]);
			button[i].addActionListener(this);
			
			panel[i].add(button[i]);
			add(panel[i]);
		}
		
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setSize(200, 200);
		setVisible(true);
	}
	
	public void actionPerformed(ActionEvent e){
		String str = e.getActionCommand();
		System.out.println(str);

		// 7日たっても一週間とは動作しないので注意
		// １日
		if(str.equals(BUTTON_STR[0])){
			Server.resetRoulette();
		}
		// １週間
		else if(str.equals(BUTTON_STR[1])){
			Server.resetRank();
		}
		else
			System.out.println("ボタンに無い言葉が押されました");
	}
	
}
