
public class MoveClass 
{
	public MoveBaseData data { get; }
	public int pp { get; set; }

	public MoveClass(MoveBaseData _data)
	{
		data = _data;
		pp = data.pp;
	}
}
