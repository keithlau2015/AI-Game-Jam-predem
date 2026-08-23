namespace Model
{
    public class ProjectileModel : Model<ProjectileModel>
    {
        public string entity { get; protected set; }
        public float spd { get; protected set; }
        public float hp { get; protected set; }
        public float lifeTime { get; protected set; }
        public string explosionEntityKey { get; protected set; }
        public float explosionRadius { get; protected set; }
        public float explosionDamage { get; protected set; }
        public int isTracker { get; protected set; }
        public string icon { get; protected set; }
        public string name { get; protected set; }
        public string description { get; protected set; }

        public ProjectileModel(object key) : base(key)
        {

        }
    }
}