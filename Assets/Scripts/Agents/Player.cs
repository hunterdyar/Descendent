using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : Pawn
    {
        public static Action<int> OnPlayerExperienceChange;
        public static Action<Vector2Int> OnPlayerPositionChange;
        
        public void Move(Vector2Int dir)
        {
            bool moved = false;
            var c = CurrentPos;
            var test = RuntimeLevel.GetFirstAgentOrWall(c, dir);
            if (test.agent == null)
            {
                if (test.Item2 != c)
                {
                   MoveAgent(test.Item2);
                   moved = true;
                }
            }
            else
            {
                //hit the agent.
                var agentWeHit = test.agent;
                
                //move to next to it.
                if (test.Item2-dir != c)
                {
                    MoveAgent(test.Item2 - dir);
                    moved = true;
                }

                //FIGHT
                if (agentWeHit is Pawn pawn)
                {
                    var duel = new Duel(this, pawn);
                    duel.Resolve();
                }else if (agentWeHit is Exit exit)
                {
                    //yes, we can overlap this one!
                    exit.Remove();
                    MoveAgent(test.Item2);
                    moved = true;
                }
            }

            if (moved)
            {
                OnPlayerPositionChange?.Invoke(CurrentPos);
            }
        }

        private void OnEnable()
        {
            Stats.OnExperienceChange += OnPlayerExperienceChange;
        }

        private void OnDisable()
        {
            Stats.OnExperienceChange -= OnPlayerExperienceChange;
        }

        protected override void Start()
        {
            OnPlayerExperienceChange?.Invoke(Stats.Experience);
            base.Start();
        }

        public override void OnVictory(Pawn pawnB)
        {
            Stats.GainExperience(pawnB.Stats.Experience);
            base.OnVictory(pawnB);
        }
    }
}