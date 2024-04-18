using UnityEngine;
using Cinemachine;

namespace snorri
{
    public class CameraModuleCine : Module
    {
        /*
            VARS:
                --> mode: perspective, orthographic
                --> fov
                --> orthographic_size
                --> near_clip
                --> far_clip
                --> bg_color
        */

        CinemachineVirtualCamera vCamera;

        public float ArmDistance
        {
            get 
            { 
                var thirdPersonFollow = vCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                return thirdPersonFollow.CameraDistance;
            }
            set 
            { 
                var thirdPersonFollow = vCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                thirdPersonFollow.CameraDistance = value;
            }
        }
        public float ArmHeight
        {
            get 
            { 
                var thirdPersonFollow = vCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                return thirdPersonFollow.VerticalArmLength;
            }
            set 
            { 
                var thirdPersonFollow = vCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                thirdPersonFollow.VerticalArmLength = value;
            }
        }

        protected override void AddClasses()
        {
            LOG.Console("camera module cine add classes");
            vCamera = ComponentCheck<CinemachineVirtualCamera>();
        }

        protected override void Setup()
        {
            base.Setup();

            LOG.Console("camera module cine setup");

            Configure();
        }

        protected override void Launch()
        {
            base.Launch();

            LOG.Console("camera module cine launch");

            string target = Vars.Get<string>("target", "stage_overworld.player");
            Node n = NODE.Tree.Get<Node>(target, null);
            if (n == null)
                return;

            Point p = n.Point;

            vCamera.LookAt = p.transform;

            bool isFollow = Vars.Get<bool>("is_follow", false);
            if (isFollow)
                vCamera.Follow = p.transform;
        }

        void Configure()
        {

        }
    }
}