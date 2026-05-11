using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HatTrick.Reflection
{
    public static partial class ReflectionHelper
    {
        #region [extensions]
        public static object ReflectItem(this object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem(source, expression, throwOnNoItemExists);
        }

        public static T ReflectItem<T>(this object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem<T>(source, expression, throwOnNoItemExists);
        }
        #endregion

        #region expression [class]
        public static class Expression
        {
            private static Dictionary<(Type root, string expression), Func<object, object>> _cheats;
            private static Dictionary<(Type root, string expression), Func<object, object>>.AlternateLookup<TypeSpanKey> _cheatLookup;

            static Expression()
            {
                _cheats = new Dictionary<(Type root, string expression), Func<object, object>>(32, new TypeSpanComparer());
                _cheatLookup = _cheats.GetAlternateLookup<TypeSpanKey>();
            }

            #region register cheat
            public static void RegisterCheat(Type type, ReadOnlySpan<char> expression, Func<object, object> cheat)
            {
                if (type is null)
                    throw new ArgumentNullException(nameof(type));

                if (expression.IsEmpty)
                    throw new ArgumentException("argument cannot be empty.", nameof(expression));

                if (cheat is null)
                    throw new ArgumentNullException(nameof(cheat));

                _cheats.Add((type, expression.ToString()), cheat);
            }

            public static void RegisterCheat<T>(ReadOnlySpan<char> expression, Func<T, object> cheat)
            {
                if (expression.IsEmpty)
                    throw new ArgumentException("argument cannot be empty.", nameof(expression));

                if (cheat is null)
                    throw new ArgumentNullException(nameof(cheat));

                _cheats.Add((typeof(T), expression.ToString()), o => cheat((T)o));
            }
            #endregion

            #region reflect item
            public static T ReflectItem<T>(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                return (T)ReflectItem(source, expression, throwOnNoItemExists);
            }

            public static object ReflectItem(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                if (expression.IsEmpty)
                    throw new ArgumentException($"{nameof(ReadOnlySpan<char>)} cannot be empty.", nameof(expression));

                var exists = false;

                Type t = source.GetType();
                if (_cheatLookup.TryGetValue(new TypeSpanKey(t, expression), out var accessor))
                    return accessor(source);

                int dotIdx = expression.IndexOf('.');

                string name = (dotIdx > -1)
                    ? expression.Slice(0, dotIdx).ToString()
                    : expression.ToString();

                //attempt dictionary lookup
                if (source is IDictionary idict)
                {
                    if (idict.Contains(name))
                    {
                        exists = true;
                        source = idict[name];
                    }
                }

                if (!exists)
                {
                    //check for a property
                    PropertyInfo p = t.GetProperty(name);
                    if (p != null)
                    {
                        exists = true;
                        source = p.GetValue(source, null);
                    }
                    else //check for a field
                    {
                        FieldInfo f = t.GetField(name);
                        if (f != null)
                        {
                            exists = true;
                            source = f.GetValue(source);
                        }
                    }
                }

                if (exists && source != null && dotIdx > -1)
                {
                    //recursive call...
                    source = ReflectItem(source, expression.Slice(++dotIdx, expression.Length - dotIdx), throwOnNoItemExists);
                }

                if (!exists && throwOnNoItemExists)
                    throw new NoItemExistsException($"Argument '{nameof(source)}' of type '{source.GetType().FullName}' does not contain a property, field or dictionary key of: '{name}'");

                return exists ? source : null;
            }
            #endregion
        }
        #endregion

        public readonly ref struct TypeSpanKey
        {
            public readonly Type Type;
            public readonly ReadOnlySpan<char> Name;

            public TypeSpanKey(Type type, ReadOnlySpan<char> name)
            {
                Type = type;
                Name = name;
            }
        }

        // 2. Implement the comparer using the custom ref struct
        public sealed class TypeSpanComparer :
            IEqualityComparer<(Type Type, string Name)>,
            IAlternateEqualityComparer<TypeSpanKey, (Type Type, string Name)>
        {
            public bool Equals((Type Type, string Name) x, (Type Type, string Name) y) =>
                x.Type == y.Type && x.Name == y.Name;

            public int GetHashCode((Type Type, string Name) obj) =>
                HashCode.Combine(obj.Type, obj.Name);

            // Alternate Lookup methods using the custom ref struct
            public bool Equals(TypeSpanKey alternate, (Type Type, string Name) other) =>
                alternate.Type == other.Type && alternate.Name.SequenceEqual(other.Name);

            public int GetHashCode(TypeSpanKey alternate) =>
                HashCode.Combine(alternate.Type, string.GetHashCode(alternate.Name));

            public (Type Type, string Name) Create(TypeSpanKey alternate) =>
                (alternate.Type, alternate.Name.ToString());
        }

    }
}
