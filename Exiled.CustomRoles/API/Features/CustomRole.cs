// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;

    using MEC;

    using Mirror;

    using NorthwoodLib.Pools;
    using PlayerRoles;
    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// The custom role base class.
    /// </summary>
    public abstract class CustomRole
    {
        /// <summary>
        /// Gets a list of all registered custom roles.
        /// </summary>
        public static HashSet<CustomRole> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the custom RoleID of the role.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> to spawn this role as.
        /// </summary>
        public abstract RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets or sets the max <see cref="Player.Health"/> for the role.
        /// </summary>
        public abstract int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the name of this role.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this role.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the CustomInfo of this role.
        /// </summary>
        public abstract string CustomInfo { get; set; }

        /// <summary>
        /// Gets all of the players currently set to this role.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> TrackedPlayers { get; } = new();

        /// <summary>
        /// Gets or sets a list of the roles custom abilities.
        /// </summary>
        public virtual List<CustomAbility> CustomAbilities { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting inventory for the role.
        /// </summary>
        public virtual List<string> Inventory { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting ammo for the role.
        /// </summary>
        public virtual Dictionary<AmmoType, ushort> Ammo { get; set; } = new();

        /// <summary>
        /// Gets or sets the possible spawn locations for this role.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current inventory when gaining this role.
        /// </summary>
        public virtual bool KeepInventoryOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players die when this role is removed.
        /// </summary>
        public virtual bool RemovalKillsPlayer { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether players keep this role when they die.
        /// </summary>
        public virtual bool KeepRoleOnDeath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/>'s size.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier.
        /// </summary>
        public virtual Dictionary<RoleTypeId, float> CustomRoleFFMultiplier { get; set; } = new();

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by ID.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole Get(int id) => Registered?.FirstOrDefault(r => r.Id == id);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole Get(Type t) => Registered.FirstOrDefault(r => r.GetType() == t);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomRole Get(string name) => Registered?.FirstOrDefault(r => r.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by <inheritdoc cref="Id"/>.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        public static bool TryGet(int id, out CustomRole customRole)
        {
            customRole = Get(id);

            return customRole is not null;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all registered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object overrideClass = null)
        {
            List<CustomRole> roles = new();

            Log.Warn("Registering roles...");

            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomRole) || type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null)
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true))
                {
                    CustomRole customRole = null;

                    if (!skipReflection && Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                            break;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all registered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> RegisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object overrideClass = null)
        {
            List<CustomRole> roles = new();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomItem) ||
                    type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) ||
                    (!isIgnored && !targetTypes.Contains(type)))
                {
                    continue;
                }

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true))
                {
                    CustomRole customRole = null;

                    if (!skipReflection && Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                          plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            customRole = property.GetValue(overrideClass ?? plugin.Config) as CustomRole;
                        }
                    }

                    customRole ??= (CustomRole)Activator.CreateInstance(type);

                    if (customRole.Role == RoleTypeId.None)
                        customRole.Role = ((CustomRoleAttribute)attribute).RoleTypeId;

                    if (customRole.TryRegister())
                        roles.Add(customRole);
                }
            }

            return roles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles()
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomRole> unregisteredRoles = new();

            foreach (CustomRole customRole in Registered)
            {
                if ((targetTypes.Contains(customRole.GetType()) && isIgnored) || (!targetTypes.Contains(customRole.GetType()) && !isIgnored))
                    continue;

                customRole.TryUnregister();
                unregisteredRoles.Add(customRole);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRole"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetRoles">The <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> containing the target roles.</param>
        /// <param name="isIgnored">A value indicating whether the target roles should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRole"/> which contains all unregistered <see cref="CustomRole"/>'s.</returns>
        public static IEnumerable<CustomRole> UnregisterRoles(IEnumerable<CustomRole> targetRoles, bool isIgnored = false) => UnregisterRoles(targetRoles.Select(x => x.GetType()), isIgnored);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <param name="customRole">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomRole customRole)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customRole = int.TryParse(name, out int id) ? Get(id) : Get(name);

            return customRole is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="IReadOnlyCollection{T}"/> of the specified <see cref="Player"/>'s <see cref="CustomRole"/>s.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="customRoles">The custom roles the player has.</param>
        /// <returns>True if the player has custom roles.</returns>
        /// <exception cref="ArgumentNullException">If the player is <see langword="null"/>.</exception>
        public static bool TryGet(Player player, out IReadOnlyCollection<CustomRole> customRoles)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            List<CustomRole> tempList = ListPool<CustomRole>.Shared.Rent();
            tempList.AddRange(Registered?.Where(customRole => customRole.Check(player)) ?? Array.Empty<CustomRole>());

            customRoles = tempList.AsReadOnly();
            ListPool<CustomRole>.Shared.Return(tempList);

            return customRoles?.Count > 0;
        }

        /// <summary>
        /// ResyncCustomRole Friendly Fire with Player (Append, or Overwrite).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole"/> to sync with player. </param>
        /// <param name="player"> <see cref="Player"/> Player to add custom role to. </param>
        /// <param name="overwrite"> <see cref="bool"/> whether to force sync (Overwriting previous information). </param>
        public static void SyncPlayerFriendlyFire(CustomRole roleToSync, Player player, bool overwrite = false)
        {
            if (overwrite)
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier, overwrite);
                player.UniqueRole = roleToSync.Name;
            }
            else
            {
                player.TryAddCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
            }
        }

        /// <summary>
        /// Force sync CustomRole Friendly Fire with Player (Set to).
        /// </summary>
        /// <param name="roleToSync"> <see cref="CustomRole"/> to sync with player. </param>
        /// <param name="player"> <see cref="Player"/> Player to add custom role to. </param>
        public static void ForceSyncSetPlayerFriendlyFire(CustomRole roleToSync, Player player)
        {
            player.TrySetCustomRoleFriendlyFire(roleToSync.Name, roleToSync.CustomRoleFFMultiplier);
        }

        /// <summary>
        /// Checks if the given player has this role.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player has this role.</returns>
        public virtual bool Check(Player player) => TrackedPlayers.Contains(player);

        /// <summary>
        /// Initializes this role manager.
        /// </summary>
        public virtual void Init() => SubscribeEvents();

        /// <summary>
        /// Destroys this role manager.
        /// </summary>
        public virtual void Destroy() => UnsubscribeEvents();

        /// <summary>
        /// Handles setup of the role, including spawn location, inventory and registering event handlers and add FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add the role to.</param>
        public virtual void AddRole(Player player)
        {
            Vector3 oldPos = player.Position;

            Log.Debug($"{Name}: Adding role to {player.Nickname}.", CustomRoles.Instance.Config.Debug);

            if (Role != RoleTypeId.None)
                player.SetRole(Role, SpawnReason.ForceClass);

            Timing.CallDelayed(
                1.5f,
                () =>
                {
                    Vector3 pos = GetSpawnPosition();

                    Log.Debug($"{nameof(AddRole)}: Found {pos} to spawn {player.Nickname}", CustomRoles.Instance.Config.Debug);

                    // If the spawn pos isn't 0,0,0, We add vector3.up * 1.5 here to ensure they do not spawn inside the ground and get stuck.
                    player.Position = oldPos;
                    if (pos != Vector3.zero)
                    {
                        Log.Debug($"{nameof(AddRole)}: Setting {player.Nickname} position..", CustomRoles.Instance.Config.Debug);
                        player.Position = pos + (Vector3.up * 1.5f);
                    }

                    if (!KeepInventoryOnSpawn)
                    {
                        Log.Debug($"{Name}: Clearing {player.Nickname}'s inventory.", CustomRoles.Instance.Config.Debug);
                        player.ClearInventory();
                    }

                    foreach (string itemName in Inventory)
                    {
                        Log.Debug($"{Name}: Adding {itemName} to inventory.", CustomRoles.Instance.Config.Debug);
                        TryAddItem(player, itemName);
                    }

                    foreach (AmmoType ammo in Ammo.Keys)
                    {
                        Log.Debug($"{Name}: Adding {Ammo[ammo]} {ammo} to inventory.", CustomRoles.Instance.Config.Debug);
                        player.SetAmmo(ammo, Ammo[ammo]);
                    }

                    Log.Debug($"{Name}: Setting health values.", CustomRoles.Instance.Config.Debug);
                    player.Health = MaxHealth;
                    player.MaxHealth = MaxHealth;
                    player.Scale = Scale;
                });

            Log.Debug($"{Name}: Setting player info", CustomRoles.Instance.Config.Debug);
            player.CustomInfo = CustomInfo;
            player.InfoArea &= ~PlayerInfoArea.Role;
            if (CustomAbilities is not null)
            {
                foreach (CustomAbility ability in CustomAbilities)
                    ability.AddAbility(player);
            }

            ShowMessage(player);
            RoleAdded(player);
            TrackedPlayers.Add(player);
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFFMultiplier);
        }

        /// <summary>
        /// Removes the role from a specific player and FF rules.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove the role from.</param>
        public virtual void RemoveRole(Player player)
        {
            Log.Debug($"{Name}: Removing role from {player.Nickname}", CustomRoles.Instance.Config.Debug);
            TrackedPlayers.Remove(player);
            player.CustomInfo = string.Empty;
            player.InfoArea |= PlayerInfoArea.Role;
            player.Scale = Vector3.one;
            if (RemovalKillsPlayer)
                player.SetRole(RoleTypeId.Spectator);
            foreach (CustomAbility ability in CustomAbilities)
            {
                ability.RemoveAbility(player);
            }

            RoleRemoved(player);
            player.UniqueRole = string.Empty;
            player.TryRemoveCustomeRoleFriendlyFire(Name);
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                CustomRoleFFMultiplier[roleToAdd] = ffMult;
            }
            else
            {
                CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            }
        }

        /// <summary>
        /// Wrapper to call <see cref="SetFriendlyFire(RoleTypeId, float)"/>.
        /// </summary>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetFriendlyFire(KeyValuePair<RoleTypeId, float> roleFF)
        {
            SetFriendlyFire(roleFF.Key, roleFF.Value);
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFFMultiplier.ContainsKey(roleToAdd))
            {
                return false;
            }

            CustomRoleFFMultiplier.Add(roleToAdd, ffMult);
            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="pairedRoleFF"> Role FF multiplier to add. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(KeyValuePair<RoleTypeId, float> pairedRoleFF) => TryAddFriendlyFire(pairedRoleFF.Key, pairedRoleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to CustomRole FriendlyFire rules.
        /// </summary>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether to overwrite current values if they exist. </param>
        /// <returns> Whether the item was able to be added. </returns>
        public bool TryAddFriendlyFire(Dictionary<RoleTypeId, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleTypeId, float> temporaryFriendlyFireRules = new();
            foreach (KeyValuePair<RoleTypeId, float> roleFF in ffRules)
            {
                if (overwrite)
                {
                    SetFriendlyFire(roleFF);
                }
                else
                {
                    if (!CustomRoleFFMultiplier.ContainsKey(roleFF.Key))
                    {
                        temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                    }
                    else
                    {
                        // Contained Key but overwrite set to false so we do not add any.
                        return false;
                    }
                }
            }

            if (!overwrite)
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFF in temporaryFriendlyFireRules)
                {
                    TryAddFriendlyFire(roleFF);
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to register this role.
        /// </summary>
        /// <returns>True if the role registered properly.</returns>
        internal bool TryRegister()
        {
            if (!CustomRoles.Instance.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same Role ID as another role: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.", CustomRoles.Instance.Config.Debug);

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) [{Role}] as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this role.
        /// </summary>
        /// <returns>True if the role is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}) [{Role}], it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to add an item to the player's inventory by name.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to try giving the item to.</param>
        /// <param name="itemName">The name of the item to try adding.</param>
        /// <returns>Whether or not the item was able to be added.</returns>
        protected bool TryAddItem(Player player, string itemName)
        {
            if (CustomItem.TryGet(itemName, out CustomItem customItem))
            {
                customItem.Give(player);

                return true;
            }

            if (Enum.TryParse(itemName, out ItemType type))
            {
                if (type.IsAmmo())
                    player.Ammo[type] = 100;
                else
                    player.AddItem(type);

                return true;
            }

            Log.Warn($"{Name}: {nameof(TryAddItem)}: {itemName} is not a valid ItemType or Custom Item name.");

            return false;
        }

        /// <summary>
        /// Gets a random <see cref="Vector3"/> from <see cref="SpawnProperties"/>.
        /// </summary>
        /// <returns>The chosen spawn location.</returns>
        protected Vector3 GetSpawnPosition()
        {
            if (SpawnProperties is null || SpawnProperties.Count() == 0)
                return Vector3.zero;

            if (SpawnProperties.StaticSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.StaticSpawnPoints)
                {
                    int r = Loader.Random.Next(100);
                    if (r <= chance)
                        return pos;
                }
            }

            if (SpawnProperties.DynamicSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.DynamicSpawnPoints)
                {
                    int r = Loader.Random.Next(100);
                    if (r <= chance)
                        return pos;
                }
            }

            if (SpawnProperties.RoleSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in SpawnProperties.RoleSpawnPoints)
                {
                    int r = Loader.Random.Next(100);
                    if (r <= chance)
                        return pos;
                }
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Called when the role is initialized to setup internal events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying += OnInternalDying;
        }

        /// <summary>
        /// Called when the role is destroyed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            foreach (Player player in TrackedPlayers)
                RemoveRole(player);

            Log.Debug($"{Name}: Unloading events.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying -= OnInternalDying;
        }

        /// <summary>
        /// Shows the spawn message to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to show the message to.</param>
        protected virtual void ShowMessage(Player player) => player.ShowHint(string.Format(CustomRoles.Instance.Config.GotRoleHint.Content, Name, Description), CustomRoles.Instance.Config.GotRoleHint.Duration);

        /// <summary>
        /// Called after the role has been added to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the role was added to.</param>
        protected virtual void RoleAdded(Player player)
        {
        }

        /// <summary>
        /// Called 1 frame before the role is removed from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> the role was removed from.</param>
        protected virtual void RoleRemoved(Player player)
        {
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (Check(ev.Player) && (((ev.NewRole == RoleTypeId.Spectator) && !KeepRoleOnDeath) || ((ev.NewRole != RoleTypeId.Spectator) && (ev.NewRole != Role))))
                RemoveRole(ev.Player);
        }

        private void OnInternalDying(DyingEventArgs ev)
        {
            if (Check(ev.Player))
            {
                CustomRoles.Instance.StopRagdollPlayers.Add(ev.Player);
                _ = new Ragdoll(new RagdollData(ev.Player.ReferenceHub, ev.DamageHandler, Role, ev.Player.Position, Quaternion.Euler(ev.Player.Rotation), ev.Player.DisplayNickname, NetworkTime.time), true);
            }
        }
    }
}