﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery
{
    public class DomAttributes: IDictionary<string, string>, IEnumerable<KeyValuePair<string, string>>
    {
        protected Dictionary<string, string> Attributes = new Dictionary<string, string>();
        //public override string ToString()
        //{
        //    string attr = String.Empty;
        //    foreach (var kvp in Attributes)
        //    {
        //        attr += (style == String.Empty ? String.Empty : " ") + kvp.Key + ": " + kvp.Value + ";";
        //    }
        //    return style;
        //}

        public bool Remove(string name)
        {
            return Attributes.Remove(name);
        }
        public void Add(string name, string value)
        {
            this[name] = value;
        }
        public DomAttributes Clone()
        {
            DomAttributes clone = new DomAttributes();
            clone.Attributes = new Dictionary<string, string>();
            foreach (var kvp in Attributes)
            {
                clone.Attributes[kvp.Key] = kvp.Value;
            }
            return clone;
        }
        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name, value);
            }
        }

        protected string Get(string name)
        {
            string value;
            if (Attributes.TryGetValue(name.ToLower(), out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        protected void Set(string name, string value)
        {
            //string parsedVal;
            name = name.Trim().ToLower();

            Attributes[name] = value;
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public bool ContainsKey(string key)
        {
            return Attributes.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return Attributes.Keys; }
        }

        public bool TryGetValue(string key, out string value)
        {
            return Attributes.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return Attributes.Values; }
        }


        public void Clear()
        {
            Attributes.Clear();
        }

        //public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        //{
        //    array = new KeyValuePair<string, string>[Styles.Count];
        //    int index = 0;
        //    foreach (var kvp in Styles)
        //    {
        //        array[index++] = kvp;
        //    }
        //}

        public int Count
        {
            get { return Attributes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }


        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ContainsKey(item.Key) && Attributes[item.Key] == item.Value;
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            array = new KeyValuePair<string, string>[Attributes.Count];
            int index = 0;
            foreach (var kvp in Attributes)
            {
                array[index++] = kvp;
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            if (ContainsKey(item.Key) && Attributes[item.Key] == item.Value)
            {
                return Remove(item.Key);
            }
            else
            {
                return false;
            }
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
