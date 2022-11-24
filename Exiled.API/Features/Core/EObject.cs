// -----------------------------------------------------------------------
// <copyright file="EObject.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// The base class of all Exiled objects.
    /// </summary>
    public abstract class EObject : TypeCastObject<EObject>
    {
        private static readonly Dictionary<Type, List<string>> RegisteredTypesValue = new();
        private bool destroyedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EObject"/> class.
        /// </summary>
        protected EObject()
            : base()
        {
            IsEditable = true;
            InternalObjects.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        protected EObject(GameObject gameObject = null)
            : this()
        {
            if (gameObject)
                Base = gameObject;
        }

        /// <summary>
        /// Gets all the registered <see cref="EObject"/> types.
        /// </summary>
        public static IReadOnlyDictionary<Type, List<string>> RegisteredTypes => RegisteredTypesValue;

        /// <summary>
        /// Gets or sets the base <see cref="GameObject"/>.
        /// </summary>
        public GameObject Base { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="EObject"/> instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tag of the <see cref="EObject"/> instance.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EObject"/> values can be edited.
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Gets all the active <see cref="EObject"/> instances.
        /// </summary>
        internal static List<EObject> InternalObjects { get; } = new();

        /// <summary>
        /// Gets a <see cref="Type"/> from a given type name.
        /// </summary>
        /// <param name="typeName">The type name to look for.</param>
        /// <returns>A <see cref="Type"/> matching the type name or <see langword="null"/> if not found.</returns>
        public static Type GetUObjectTypeByName(string typeName)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Name != typeName || type.BaseType != typeof(EObject))
                    continue;

                return type;
            }

            return null;
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="name">The name of the registered type.</param>
        /// <returns>The registered <see cref="Type"/>.</returns>
        public static Type RegisterUObjectType<T>(string name)
            where T : EObject
        {
            Type matching = GetUObjectTypeFromRegisteredTypes<T>(name);
            if (matching is not null)
                return matching;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Name != typeof(T).Name)
                    continue;

                if (RegisteredTypesValue[t] is not null)
                {
                    RegisteredTypesValue[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name };
                    RegisteredTypesValue.Add(t, values);
                }

                return typeof(T);
            }

            throw new NullReferenceException($"Couldn't find a defined EObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="name">The name of the registered type.</param>
        /// <returns>The registered <see cref="Type"/>.</returns>
        public static Type RegisterUObjectType(Type type, string name)
        {
            Type matching = GetUObjectTypeFromRegisteredTypes(type, name);
            if (matching is not null)
                return matching;

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(
                item =>
                    item.BaseType == typeof(EObject) || item.IsSubclassOf(typeof(EObject))))
            {
                if (t.Name != type.Name)
                    continue;

                if (RegisteredTypesValue.ContainsKey(t))
                {
                    RegisteredTypesValue[t].Add(name);
                }
                else
                {
                    List<string> values = new() { name };
                    RegisteredTypesValue.Add(t, values);
                }

                return t;
            }

            throw new NullReferenceException($"Couldn't find a defined EObject type for {name}");
        }

        /// <summary>
        /// Registers the specified <see cref="EObject"/> type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterUObjectType(Type type) => RegisteredTypesValue.Remove(type);

        /// <summary>
        /// Unregisters the specified <see cref="EObject"/> type.
        /// </summary>
        /// <param name="name">The name of the type to unregister.</param>
        /// <returns><see langword="true"/> if the type was unregistered successfully; otherwise, <see langword="false"/>.</returns>
        public static bool UnregisterUObjectType(string name)
        {
            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Value.Contains(name))
                    continue;

                RegisteredTypesValue.Remove(kvp.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the most accurate <see cref="Type"/> matching the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="ignoreAbstractTypes">A value indicating whether abstract types should be ignored.</param>
        /// <returns>The <see cref="Type"/> with the name that matches the given name.</returns>
        public static Type FindUObjectDefinedTypeByName(string name, bool ignoreAbstractTypes = true)
        {
            Type[] assemblyTypes = ignoreAbstractTypes
                ? Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => !t.IsAbstract).ToArray()
                : Assembly.GetExecutingAssembly().GetTypes();
            List<int> matches = new();
            matches.AddRange(
                assemblyTypes.Select(
                    type =>
                        LevenshteinDistance(type.Name, name)));
            return assemblyTypes[matches.IndexOf(matches.Min())];
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type is not the same as the specified type.</exception>
        public static Type GetUObjectTypeFromRegisteredTypes<T>()
            where T : EObject
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != typeof(T))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetUObjectTypeFromRegisteredTypes<T>(string name)
            where T : EObject
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != typeof(T) || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type is not the same as the specified type.</exception>
        public static Type GetUObjectTypeFromRegisteredTypes(Type type)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != type)
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetUObjectTypeFromRegisteredTypes(Type type, string name)
        {
            Type t = null;

            foreach (KeyValuePair<Type, List<string>> kvp in RegisteredTypesValue)
            {
                if (kvp.Key != type || !kvp.Value.Contains(name))
                    continue;

                t = kvp.Key;
                break;
            }

            return t;
        }

        /// <summary>
        /// Gets a <see cref="EObject"/> type from all the registered types.
        /// </summary>
        /// <param name="name">The name of the type to look for.</param>
        /// <returns>The matching <see cref="Type"/>.</returns>
        /// <exception cref="NullReferenceException">Occurs when the requested type's name is not the same as the specified name.</exception>
        public static Type GetUObjectTypeFromRegisteredTypesByName(string name) => RegisteredTypesValue.FirstOrDefault(kvp => kvp.Value.Contains(name)).Key;

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return Activator.CreateInstance(type, flags, null, parameters, null) is not EObject outer ? null : outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type)
            where T : EObject => CreateDefaultSubobject(type, null).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>()
            where T : EObject => CreateDefaultSubobject(typeof(T), null).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type to cast.</typeparam>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(params object[] parameters)
            where T : EObject => CreateDefaultSubobject(typeof(T), parameters).Cast<T>();

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name)
            where T : EObject
        {
            if (CreateDefaultSubobject<T>() is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer.Cast<T>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="EObject"/> type.</typeparam>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(GameObject gameObject, string name, params object[] parameters)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(type);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <typeparam name="T">The cast <see cref="EObject"/> type.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static T CreateDefaultSubobject<T>(Type type, GameObject gameObject, string name, params object[] parameters)
            where T : EObject
        {
            object newObj = CreateDefaultSubobject<T>(type, parameters);
            if (newObj is not T outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, GameObject gameObject, string name)
        {
            if (CreateDefaultSubobject(type) is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="gameObject"><inheritdoc cref="Base"/></param>
        /// <param name="name">The name to be given to the new <see cref="EObject"/> instance.</param>
        /// <param name="parameters">The parameters to initialize the object.</param>
        /// <returns>The new <see cref="EObject"/> instance.</returns>
        public static EObject CreateDefaultSubobject(Type type, GameObject gameObject, string name, params object[] parameters)
        {
            if (CreateDefaultSubobject(type, parameters) is not EObject outer)
                return null;

            outer.Base = gameObject;
            outer.Name = name;
            return outer;
        }

        /// <summary>
        /// Destroys all the active <see cref="EObject"/> instances.
        /// </summary>
        public static void DestroyAllObjects()
        {
            List<EObject> objects = ListPool<EObject>.Shared.Rent(InternalObjects);
            foreach (EObject @object in objects)
                @object.Destroy();

            objects.Clear();
            ListPool<EObject>.Shared.Return(objects);
        }

        /// <summary>
        /// Destroys all the active <typeparamref name="T"/> <see cref="EObject"/> instances.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        public static void DestroyAllObjectsOfType<T>()
            where T : EObject
        {
            List<EObject> objects = ListPool<EObject>.Shared.Rent(InternalObjects);
            foreach (EObject @object in objects)
            {
                if (@object.Cast(out T obj))
                    obj.Destroy();
            }

            objects.Clear();
            ListPool<EObject>.Shared.Return(objects);
        }

        /// <summary>
        /// Finds the active <see cref="EObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>The corresponding active <typeparamref name="T"/> <see cref="EObject"/>.</returns>
        public static T FindActiveObjectOfType<T>(Func<EObject, bool> predicate)
            where T : EObject
        {
            foreach (EObject @object in InternalObjects.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<EObject, bool> predicate)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>()
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(string name)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && (obj.Name == name))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Type type)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects.Where(obj => obj.GetType() == type))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> filtered based on a predicate.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="type">The <see cref="EObject"/> type.</param>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsOfType<T>(Type type, Func<EObject, bool> predicate)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects
                .Where(obj => obj.GetType() == type)
                .Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/> with the specified <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">The tag to look for.</param>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <returns>A <typeparamref name="T"/>[] containing all the matching results.</returns>
        public static T[] FindActiveObjectsWithTagOfType<T>(string tag)
            where T : EObject
        {
            List<T> objects = new();
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && obj.Tag.ToLower().Contains(tag))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the elements that satify the condition.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<object, bool> predicate)
            where T : EObject
        {
            List<T> objects = new();

            foreach (EObject @object in InternalObjects.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Finds all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <typeparamref name="T"/>[] containing all the elements that satify the condition.</returns>
        public static T[] FindActiveObjectsOfType<T>(Func<T, bool> predicate)
            where T : EObject
        {
            IEnumerable<T> iterator = InternalObjects.Where(obj => obj.Cast(out T _)).Select(obj => obj.Cast<T>());
            List<T> objects = new();

            foreach (EObject @object in iterator.Where(predicate))
            {
                if (@object.Cast(out T obj))
                    objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Destroys all the active <see cref="EObject"/> instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        public static void DestroyActiveObjectsOfType<T>()
            where T : EObject
        {
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj))
                    obj.Destroy();
            }
        }

        /// <summary>
        /// Destroys an active <see cref="EObject"/> instance of type <typeparamref name="T"/> given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> type to look for.</typeparam>
        /// <param name="gameObject">The <see cref="GameObject"/> belonging to the <see cref="EObject"/> instance to be destroyed.</param>
        /// <returns><see langword="true"/> if the object was destroyed; otherwise, <see langword="false"/>.</returns>
        public static bool DestroyActiveObject<T>(GameObject gameObject)
            where T : EObject
        {
            foreach (EObject @object in InternalObjects)
            {
                if (@object.Cast(out T obj) && (obj.Base == gameObject))
                {
                    obj.Destroy();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Destroys an active <see cref="EObject"/> instance given the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="gameObject">The <see cref="GameObject"/> belonging to the object.</param>
        /// <returns><see langword="true"/> if the object was destroyed; otherwise, <see langword="false"/>.</returns>
        public static bool DestroyActiveObject(Type type, GameObject gameObject)
        {
            foreach (EObject @object in InternalObjects)
            {
                if ((@object.GetType() == type) && (@object.Base == gameObject))
                {
                    @object.Destroy();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the most appropriate entry.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="name">The name to pair.</param>
        /// <param name="source">The source on which iterate on.</param>
        /// <returns>The corresponding entry or <see langword="default"/> if not found.</returns>
        public static T FindMostAppropriateEntry<T>(string name, IEnumerable<T> source)
        {
            List<int> matches = new();
            matches.AddRange(
                source.Select(
                    type =>
                        LevenshteinDistance(type.GetType().Name, name)));
            return source.ElementAt(matches.IndexOf(matches.Min()));
        }

        /// <summary>
        /// Destroys the current <see cref="EObject"/> instance.
        /// </summary>
        public void Destroy()
        {
            Destroy(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Destroy()"/>
        protected virtual void Destroy(bool destroying)
        {
            if (!destroyedValue)
            {
                if (destroying)
                {
                    OnBeginDestroy();
                    InternalObjects.Remove(this);
                }

                OnDestroyed();
                destroyedValue = true;
            }
        }

        /// <summary>
        /// Fired before the current <see cref="EObject"/> instance is destroyed.
        /// </summary>
        protected virtual void OnBeginDestroy()
        {
        }

        /// <summary>
        /// Fired when the current <see cref="EObject"/> instance has been explicitly destroyed.
        /// </summary>
        protected virtual void OnDestroyed()
        {
        }

        private static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            if (source.Length > target.Length)
                (source, target) = (target, source);

            int m = target.Length;
            int n = source.Length;
            int[,] distance = new int[2, m + 1];

            for (int j = 1; j <= m; j++)
                distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(
                        Math.Min(
                            distance[previousRow, j] + 1,
                            distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
        }
    }
}