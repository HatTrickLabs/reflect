using System;
using System.Collections;
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

                string name = (dotIdx > -1) 
                    ? expression.Slice(0, dotIdx) .ToString()
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
                    Type t = source.GetType();
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
    }
}
