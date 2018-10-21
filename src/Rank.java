public class Rank {
	private String name;
	private int money;
	Rank(String name, int money){
		this.name = name;
		this.money = money;
	}
	protected String getName(){
		return name;
	}
	protected int getMoney(){
		return money;
	}
	// 名前とゴールドを,でつないだものを返す
	protected String out(){
		return name + "," + money + ",";
	}
}
