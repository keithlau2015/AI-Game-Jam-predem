namespace PortalModule
{
    public interface IPortalTeleportable
    {
        bool OnBeforePortalTeleport(PortalTeleportContext context);
        void OnAfterPortalTeleport(PortalTeleportContext context);
    }
}
