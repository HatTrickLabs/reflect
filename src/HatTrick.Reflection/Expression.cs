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
            #region const
            public const int MaxStackDepth = 16;
            #endregion

            #region internals
            private static Dictionary<(Type root, string expression), Func<object, object>> _helpers;
            private static Dictionary<(Type root, string expression), Func<object, object>>.AlternateLookup<AlternateLookupKey> _helperAltLookup;
            #endregion

            #region ctors
            static Expression()
            {
                _helpers = new Dictionary<(Type root, string expression), Func<object, object>>(new AlternateLookupComparer());
                _helperAltLookup = _helpers.GetAlternateLookup<AlternateLookupKey>();
            }
            #endregion

            #region register helper
            public static void RegisterHelper<T>(ReadOnlySpan<char> expression, Func<T, object> helper)
            {
                if (expression.IsEmpty)
                    throw new ArgumentException("argument cannot be empty.", nameof(expression));

                if (helper is null)
                    throw new ArgumentNullException(nameof(helper));

                _helpers.Add((typeof(T), expression.ToString()), o => helper((T)o));
            }
            #endregion

            #region reflect item
            public static T ReflectItem<T>(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                return (T)ReflectItem(source, expression, throwOnNoItemExists);
            }

            public static object ReflectItem(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                return ReflectItemRecursive(source, expression, 1, throwOnNoItemExists);
            }

            private static object ReflectItemRecursive(object source, ReadOnlySpan<char> expression, int depth, bool throwOnNoItemExists)
            {
                if (depth > MaxStackDepth)
                    throw new RecursionStackDepthException($"Recursion depth overflow...recursion depth cannot exceed {MaxStackDepth}");

                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                if (expression.IsEmpty)
                    throw new ArgumentException($"{nameof(ReadOnlySpan<char>)} cannot be empty.", nameof(expression));

                var exists = false;

                Type type = source.GetType();
                if (_helperAltLookup.TryGetValue(new AlternateLookupKey(type, expression), out var accessor))
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
                    PropertyInfo p = type.GetProperty(name);
                    if (p != null)
                    {
                        exists = true;
                        source = p.GetValue(source, null);
                    }
                    else //check for a field
                    {
                        FieldInfo f = type.GetField(name);
                        if (f != null)
                        {
                            exists = true;
                            source = f.GetValue(source);
                        }
                    }
                }

                if (exists && source != null && dotIdx > -1)//should do recursive call...
                    source = ReflectItemRecursive(
                        source, 
                        expression.Slice(++dotIdx, expression.Length - dotIdx), 
                        ++depth, 
                        throwOnNoItemExists
                    );

                if (!exists && throwOnNoItemExists)
                    throw new NoItemExistsException($"Argument '{nameof(source)}' of type '{source.GetType().FullName}' does not contain a property, field or dictionary key of: '{name}'");

                return exists ? source : null;
            }
            #endregion
        }
        #endregion
    }
}
