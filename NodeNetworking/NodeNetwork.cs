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
        int MAX_VALIDATORS = 21;
        
        public NodeNetwork()
        {
            Nodes = FindNodes();
            PreviousValidators = FindPreviousValidators();
        }

        public HashSet<Node> Nodes { get; set; }
        public Queue<Node> PreviousValidators { get; set; }

        public void DoConsensusRound()
        {
            FindNodes();
            HashSet<Node> witnesses = GetRoundWitnesses();
            Node validator = GetValidator(witnesses);
            if (isVerifiedBlock(validator, witnesses))
            {
                validator.SignBlock();
            }
            CycleRoundDelegate(validator);
        }

        private HashSet<Node> FindNodes()
        {
            throw new NotImplementedException();
        }

        private Ledger FindLongestLedger()
        {
            // requires consensus
            throw new NotImplementedException();
        }

        private Queue<Node> FindPreviousValidators()
        {
            // go over last 20 blocks
            throw new NotImplementedException();
        }

        private HashSet<Node> GetRoundWitnesses()
        {
            HashSet<Node> candidates = new HashSet<Node>();
            foreach (Node node in Nodes)
            {
                // Fills set until MAX_DELEGATES is reached
                if (candidates.Count < MAX_VALIDATORS)
                    candidates.Add(node);
                else
                {
                    // Replaces node with the least amount of votes with a node which has more votes
                    int minVotes = candidates.Min(n => n.Votes);
                    if (node.Votes > minVotes)
                    {
                        List<Node> minDelegates = candidates.Where(n => n.Votes == minVotes).ToList();

                        candidates.Remove(minDelegates.Last());
                        candidates.Add(node);
                    }
                }
            }
            return candidates;
        }

        private Node GetValidator(HashSet<Node> validatorCandidates)
        {
            if (PreviousValidators != null)
            {
                IEnumerable<Node> delegateOptions = validatorCandidates.Except(PreviousValidators);
                return delegateOptions.First();
            }
            else
                return validatorCandidates.First();
        }

        private bool isVerifiedBlock(Node blockValidator, HashSet<Node> roundValidators)
        {
            int votes = 0;
            foreach (Node n in roundDelegates)
            {
                // if node contains List:blockvalidator.block.transactions
                {
                    votes++;
                }
            }

            throw new NotImplementedException();

            return votes > Math.Ceiling((decimal)MAX_VALIDATORS / 2);
        }

        private void CycleRoundDelegate(Node blockValidator)
        {
            PreviousValidators.Enqueue(blockValidator);
            if (PreviousValidators.Count >= MAX_VALIDATORS)
            {
                PreviousValidators.Dequeue();
            }
        }
    }
}
