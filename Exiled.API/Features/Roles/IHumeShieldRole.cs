namespace Exiled.API.Features.Roles
{
    using PlayerRoles.PlayableScps.HumeShield;

    /// <summary>
    /// Represents a role that supports a hume shield.
    /// </summary>
    public interface IHumeShieldRole
    {
        /// <summary>
        /// Gets a reference to the role's <see cref="HumeShieldModuleBase"/>.
        /// </summary>
        HumeShieldModuleBase HumeShieldBase { get; }
    }
}
