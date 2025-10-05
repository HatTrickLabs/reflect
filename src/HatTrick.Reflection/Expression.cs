using System;
using System.Collections;
using System.Reflection;
using System.Text;

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
            #region reflect item
            public static T ReflectItem<T>(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                return (T)ReflectItem(source, expression, throwOnNoItemExists);
            }

            public static object ReflectItem(object source, ReadOnlySpan<char> expression, bool throwOnNoItemExists = true)
            {
                if (source == null) { throw new ArgumentNullException(nameof(source)); }
                if (expression.IsEmpty) { throw new ArgumentException($"{nameof(ReadOnlySpan<char>)} cannot be empty.", nameof(expression)); }

                object o = source;

                var itemExists = false;

                int memberAccessorIdx = expression.IndexOf('.');
                ReadOnlySpan<char> thisExpression = (memberAccessorIdx > -1) ? expression.Slice(0, memberAccessorIdx) : expression;

                //attempt dictionary lookup
                if (source is IDictionary idict)
                {
                    string expr = thisExpression.ToString();
                    if (idict.Contains(expr))
                    {
                        itemExists = true;
                        o = idict[expr];
                    }
                }
                
                if (!itemExists)//check for a property
                {
                    Type t = o.GetType();
                    string expr = thisExpression.ToString();
                    PropertyInfo p = t.GetProperty(expr);

                    if (p != null)
                    {
                        itemExists = true;
                        o = p.GetValue(o, null);
                    }
                    else //check for a field
                    {
                        FieldInfo f = t.GetField(expr);

                        if (f != null)
                        {
                            itemExists = true;
                            o = f.GetValue(o);
                        }
                    }
                }

                if (itemExists && o != null && memberAccessorIdx > -1)
                {
                    //recursive call...
                    o = ReflectItem(o, expression.Slice(++memberAccessorIdx, expression.Length - memberAccessorIdx), throwOnNoItemExists);
                }

                if (!itemExists && throwOnNoItemExists)
                {
                    string expr = thisExpression.Length > 128 ? (expression.Slice(0, 125).ToString() + "...") : thisExpression.ToString();
                    throw new NoItemExistsException($"Argument '{nameof(source)}' of type '{source.GetType().FullName}' does not contain a property, field or dictionary key of: '{expr}'");
                }

                return itemExists ? o : null;
            }
            #endregion
        }
        #endregion
    }
}
