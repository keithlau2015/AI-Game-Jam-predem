using UnityEngine;

/// <summary>
/// Central color-coding module for the Portal Escort prototype.
///
/// This is the SINGLE source of truth for the project's visual language. Every other agent
/// (Gameplay, Transition, UI/UX, Music) should read colors from here rather than hardcoding
/// hex/Color values, so the palette stays consistent and tweakable in one place.
///
/// Meaning of each color (matches the intent described in docs/Contracts.md and
/// docs/Agents/Graphics.md):
///   - SpeedUp    : green  -> floor tile that increases escort move speed.
///   - SlowDown   : blue   -> floor tile that decreases escort move speed.
///   - Turn       : yellow -> floor tile that redirects the escort's direction.
///   - Goal       : gold   -> the rescue destination the escort must reach.
///   - Invalid    : red    -> an illegal / disallowed portal placement preview.
///   - Portal     : cyan   -> portal endpoints (entrance/exit) and their link.
///   - Escort     : white  -> the moving entity the player is escorting.
///   - Turret     : dark red -> stationary hazard that fires projectiles.
///   - Projectile : orange -> the projectile a turret fires (lethal to escort).
///
/// Note: contracts fix Direction arrow ROTATION (DirectionUtility.GetRotationZ) but leave the
/// COLOR coding as a Graphics-owned decision — documented here.
/// </summary>
public static class Palette
{
    /// <summary>Speed-up floor tile. Increases escort move speed.</summary>
    public static readonly Color SpeedUp = new Color(0.20f, 0.80f, 0.25f);   // green

    /// <summary>Slow-down floor tile. Decreases escort move speed.</summary>
    public static readonly Color SlowDown = new Color(0.20f, 0.45f, 0.95f);  // blue

    /// <summary>Turn floor tile. Redirects the escort's movement direction.</summary>
    public static readonly Color Turn = new Color(0.95f, 0.85f, 0.15f);      // yellow

    /// <summary>Goal / rescue destination. Must read clearly as "the destination".</summary>
    public static readonly Color Goal = new Color(1.00f, 0.80f, 0.10f);      // gold

    /// <summary>Invalid / illegal state, e.g. disallowed portal placement preview.</summary>
    public static readonly Color Invalid = new Color(0.90f, 0.15f, 0.15f);   // red

    /// <summary>Portal endpoints and their connecting link.</summary>
    public static readonly Color Portal = new Color(0.15f, 0.85f, 0.90f);    // cyan

    /// <summary>The escort entity being guided to the goal.</summary>
    public static readonly Color Escort = new Color(1.00f, 1.00f, 1.00f);    // white

    /// <summary>Turret — stationary hazard that fires projectiles.</summary>
    public static readonly Color Turret = new Color(0.55f, 0.05f, 0.05f);    // dark red

    /// <summary>Projectile fired by a turret; lethal to the escort.</summary>
    public static readonly Color Projectile = new Color(1.00f, 0.55f, 0.10f); // orange
}
