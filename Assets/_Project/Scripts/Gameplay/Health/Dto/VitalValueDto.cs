


public class VitalValueDto
{
    public string type;
    public int max;
    public int current;
    public VitalValueDto(string type, int max, int current)
    {
        this.type = type;
        this.max = max;
        this.current = current;
    }
}