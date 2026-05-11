using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace HatTrick.Reflection
{
    public static partial class ReflectionHelper
    {
        #region [extensions]
        public static object ReflectItem(this object source, string expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem(source, expression, throwOnNoItemExists);
        }

        public static T ReflectItem<T>(this object source, string expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem<T>(source, expression, throwOnNoItemExists);
        }
        #endregion

        #region expression [class]
        public static class Expression
        {
            private static readonly ConcurrentDictionary<Type, Dictionary<string, MemberInfo>> _memberCache = new();

            #region reflect item
            public static T ReflectItem<T>(object source, string expression, bool throwOnNoItemExists = true)
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

                int dotIdx = expression.IndexOf('.');
                ReadOnlySpan<char> nameSpan = dotIdx > -1 ? expression.Slice(0, dotIdx) : expression;

                //AlternateLookup is not available on the interface...
                if (source is IDictionary idict)
                {
                    string name = nameSpan.ToString();
                    if (idict.Contains(name))
                    {
                        exists = true;
                        source = idict[name];
                    }
                }

                if (!exists)
                {
                    Type t = source.GetType();

                    if (!_memberCache.TryGetValue(t, out var members))
                    {
                        members = BuildMemberMap(t);
                        _memberCache.TryAdd(t, members);
                    }

#if NET9_0_OR_GREATER
                    if (members.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(nameSpan, out var member))
#else
                    if (members.TryGetValue(nameSpan.ToString(), out var member))
#endif
                    {
                        exists = true;
                        source = member is PropertyInfo p
                            ? p.GetValue(source, null)
                            : ((FieldInfo)member).GetValue(source);
                    }
                }

                if (exists && source != null && dotIdx > -1)
                    source = ReflectItem(source, expression.Slice(++dotIdx, expression.Length - dotIdx), throwOnNoItemExists);

                if (!exists && throwOnNoItemExists)
                    throw new NoItemExistsException($"Argument '{nameof(source)}' of type '{source.GetType().FullName}' does not contain a property, field or dictionary key of: '{nameSpan.ToString()}'");

                return exists ? source : null;
            }

            private static Dictionary<string, MemberInfo> BuildMemberMap(Type t)
            {
                var map = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
                foreach (FieldInfo f in t.GetFields())
                {
                    map[f.Name] = f;
                }

                foreach (PropertyInfo p in t.GetProperties())
                {
                    map[p.Name] = p; // properties shadow fields with the same name
                }

                return map;
            }
            #endregion
        }
        #endregion
    }
}
