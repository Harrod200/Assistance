using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// DEPRECATED: This patch is no longer used as of v0.6.0.
    /// 
    /// In v0.6.0, assist mission bonuses are no longer added to base attributes.
    /// Instead, bonuses are tracked separately and only applied during contested mission resolution.
    /// Therefore, this CP cap adjustment is no longer necessary.
    /// 
    /// The patch remains in the codebase for reference but is disabled via the [HarmonyPatch] condition.
    /// </summary>
    internal static class TIFactionState_ControlPointMaintenanceCapPatch_Deprecated
    {
        // Patch disabled - bonuses no longer affect CP calculations
    }
}

