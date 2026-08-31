using System;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Condition that restricts a mission to only player-controlled factions.
    /// AI factions cannot use missions with this condition.
    /// </summary>
    public class TIMissionCondition_PlayerFactionOnly : TIMissionCondition
    {
        public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
        {
            if (councilor == null || councilor.faction == null)
                return TIMissionCondition.fail;

            // Only allow player-controlled factions
            // Check if faction's player is AI. If it is AI, return fail; if it's player-controlled, return pass
            bool isAI = councilor.faction.player != null && councilor.faction.player.isAI;
            string result = isAI ? TIMissionCondition.fail : TIMissionCondition.pass;

            // Log for debugging
            if (Main.mod != null)
            {
                Main.mod.Logger.Log(string.Format("[TIMissionCondition_PlayerFactionOnly] Checking councilor '{0}' in faction '{1}' (isAI={2}) -> {3}", 
                    councilor.displayName, councilor.faction.displayName, isAI, result));
            }

            return result;
        }
    }
}
