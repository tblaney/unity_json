using UnityEngine;

namespace snorri
{
    public class MeshColliderModule : ColliderModule
    {
        MeshCollider mesh;

        protected override void AddClasses()
        {
            base.AddClasses();

            mesh = ComponentCheck<MeshCollider>();
            this.collider = mesh as Collider;
        }

        protected override void Setup()
        {
            base.Setup();

            bool isTrigger = Vars.Get<bool>("is_trigger", false);
            mesh.isTrigger = isTrigger;

            bool isConvex = Vars.Get<bool>("is_convex", false);
            mesh.convex = isConvex;
        }

        public Mesh Mesh {
            get {
                return mesh.sharedMesh;
            }
            set {
                LOG.Console($"mesh collider module: {value}");
                //mesh.sharedMesh = null;
                mesh.sharedMesh = value;
            }
        }

        public void Refresh() {
            LOG.Console("heightmap mesh collider module destroy! " + this.gameObject.name);
            DestroyImmediate(this.mesh);

            ComponentCheck<MeshCollider>();
        }
    }
}