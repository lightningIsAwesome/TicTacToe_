public struct PlayerMove
{
    public PlayerMove(byte pos, CellBehaviour.CellValue value)
    {
        this.pos = pos;
        this.value = value;
    }
    public CellBehaviour.CellValue value;
    public byte pos;
}
