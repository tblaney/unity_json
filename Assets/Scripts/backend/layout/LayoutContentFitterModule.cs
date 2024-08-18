using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    using UnityEngine.UI;

    public class LayoutContentFitterModule : Module
    {
        ContentSizeFitter fitter;

        protected override void AddClasses()
        {
            fitter = ComponentCheck<ContentSizeFitter>();
        }
        protected override void Setup()
        {
            base.Setup();

            Configure();
        }

        void Configure()
        {
            switch (Vars.Get<string>("horizontal", "preferred")) {
                case "preferred":
                    fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case "unconstrained":
                    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    break;
                case "minsize":
                    fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                    break;
            }
            switch (Vars.Get<string>("vertical", "preferred")) {
                case "preferred":
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case "unconstrained":
                    fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    break;
                case "minsize":
                    fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                    break;
            }
        }
    }
}