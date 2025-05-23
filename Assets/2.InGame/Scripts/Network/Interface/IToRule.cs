public interface IToRule
{
    public int PassPlayer(SteppingTile tile);

    public bool CheckTail(int tailCount);

    public bool OpenTile(SteppingTile tile, SelectingTile selectTileInfo);
}
