using System.Numerics;

public interface IDestructible {
    public void OnDestruct();
    public void OnRepair();
    public void OnHit(BigInteger dmg);
}