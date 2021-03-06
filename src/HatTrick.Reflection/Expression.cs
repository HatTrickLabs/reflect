﻿using System;
using System.Collections;
using System.Reflection;

namespace HatTrick.Reflection
{
    public static partial class ReflectionHelper
    {
        #region extension
        public static object ReflectItem(this object source, string expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem(source, expression, throwOnNoItemExists);
        }

        public static T ReflectItem<T>(this object source, string expression, bool throwOnNoItemExists = true)
        {
            return Expression.ReflectItem<T>(source, expression, throwOnNoItemExists);
        }
        #endregion
        public static class Expression
        {
            #region reflect item
            public static T ReflectItem<T>(object source, string expression, bool throwOnNoItemExists = true)
            {
                return (T)ReflectItem(source, expression, throwOnNoItemExists);
            }

            public static object ReflectItem(object source, string expression, bool throwOnNoItemExists = true)
            {
                if (source == null) { throw new ArgumentNullException(nameof(source)); }
                if (expression == null) { throw new ArgumentNullException(nameof(expression)); }

                object o = source;

                var itemExists = false;

                int memberAccessorIdx = expression.IndexOf('.');
                string thisExpression = (memberAccessorIdx > -1) ? expression.Substring(0, memberAccessorIdx) : expression;

                //attempt dictionary lookup
                IDictionary idict;
                if ((idict = source as IDictionary) != null)
                {
                    if (idict.Contains(thisExpression))
                    {
                        itemExists = true;
                        o = idict[thisExpression];
                    }
                }
                
                if (!itemExists)//check for a property
                {
                    Type t = o.GetType();

                    PropertyInfo p = t.GetProperty(thisExpression);

                    if (p != null)
                    {
                        itemExists = true;
                        o = p.GetValue(o, null);
                    }
                    else //check for a field
                    {
                        FieldInfo f = t.GetField(thisExpression);

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
                    o = ReflectItem(o, expression.Substring(++memberAccessorIdx, expression.Length - memberAccessorIdx), throwOnNoItemExists);
                }

                if (!itemExists && throwOnNoItemExists)
                {
                    throw new NoItemExistsException($"Item does not exist on source object. Item: {expression}, Bound Type: {source.GetType()}");
                }

                return itemExists ? o : null;
            }
            #endregion
        }
    }
}
