using Core;
using NodeRepository.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking
{
    internal class NodeNetwork
    {
        public NodeNetwork()
        {
            HashSet<Node> Nodes = FindNodes();
        }

        public void DoRound()
        {
            // find all nodes
            // get validator candidates
            // get previous validators
            // get validator
            // vote on block
            // add validator to previous validator queue
        }

        public HashSet<Node> FindNodes()
        {
            throw new NotImplementedException();
        }
    }
}
