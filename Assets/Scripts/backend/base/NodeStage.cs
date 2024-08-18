using UnityEngine;
using System;

namespace snorri
{
    public class NodeStage
    {
        public Node Origin {get; set;}
        public Map Vars {get; set;}

        public string NodeName { get {return Vars.Get<string>("node", ""); } }
        public string NodeFile { get {return Vars.Get<string>("file", ""); } }
        public bool IsPersisitent { get {return Vars.Get<bool>("is_persistent", false); } }

        public NodeStage(Map vars)
        {
            Vars = vars;
        }

        public void Build()
        {
            LOG.Console("node stage build! " + NodeName + ", " + NodeFile);

            Vars.Log(); 

            Origin = new Node(
                nodeName:NodeName,
                resourceFile:NodeFile 
            );
            Origin.Build();
        }
        public void Terminate()
        {
            if (Origin != null)
            {
                Origin.Terminate();
            }

            Origin = null;
        }

    }
}