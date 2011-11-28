﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using System.Diagnostics;

namespace Jtc.CsQuery
{
    public class DomElementFactory
    {
        public DomElementFactory(IDomRoot document)
        {
            Document = document;
        }
        public IDomRoot Document { get; set; }
        protected char[] BaseHtml;
        protected int EndPos
        {
            get
            {
                if (_EndPos == -1)
                {
                    _EndPos = BaseHtml.Length - 1;
                }
                return _EndPos;
            }
        } protected int _EndPos = -1;
        protected void SetBaseHtml(char[] baseHtml)
        {
            _EndPos = -1;
            BaseHtml = baseHtml;

        }
        /// <summary>
        /// No literals allowed
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IEnumerable<IDomElement> CreateElements(string html)
        {
            foreach (IDomObject obj in CreateObjectsImpl(html.ToCharArray(), false))
            {
                yield return (IDomElement)obj;
            }
        }
        /// <summary>
        /// returns a single element, any html is discarded after that
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IDomElement CreateElement(string html)
        {
            SetBaseHtml(html.ToCharArray());
            return (IDomElement)Parse(false).First();
        }
        /// <summary>
        /// Returns a list of elements created by parsing the string. If allowLiterals is false, any literal text that is not
        /// inside a tag will be wrapped in span tags.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> CreateObjects(string html)
        {
            isBound = false;
            return CreateObjectsImpl(html.ToCharArray(), true);
        }
        public IEnumerable<IDomObject> CreateObjects(char[] html)
        {
            isBound = false;
            return CreateObjectsImpl(html, true);
        }
        public IEnumerable<IDomObject> CreateObjects()
        {
            if (Document == null)
            {
                throw new Exception("This method requires Document be set");
            }
            isBound = true;

            return CreateObjectsImpl(Document.SourceHtml,true);
        }
        //public IEnumerable<IDomObject> CreateObjects(string html, IDomRoot document)
        //{
        //    Document = document;
        //    return CreateObjectsImpl(html.ToCharArray(), true);
        //}
        //public IEnumerable<IDomObject> CreateObjects(char[] html, IDomRoot document)
        //{
        //    Document = document;
        //    return CreateObjectsImpl(html, true);
        //}
        protected IEnumerable<IDomObject> CreateObjectsImpl(char[] html, bool allowLiterals)
        {

            SetBaseHtml(html);
            return Parse(allowLiterals);
        }
        protected class IterationData
        {
            public IterationData Parent;
            public IDomObject Object;
          
            public IDomElement Element
            {
                get
                {
                    return (IDomElement)Object;
                }
            }
            // when true, the contents will be treated as text until the next close tag
            public bool ReadTextOnly {
                get
                {
                    return _ReadTextOnly;
                }
                set{
                    _ReadTextOnly = value;
                    if (value)
                    {
                        Step = -1;
                    }
                }
            } protected bool _ReadTextOnly= false;
            public int Pos;
            public int Step = 0;
            public bool Finished;
            public bool AllowLiterals;
            public bool Invalid = false;
            public int HtmlStart = 0;
            /// <summary>
            /// Use this to prepare the iterator object to continue finding siblings. It retains the parent.
            /// </summary>
            public void Reset()
            {
                Step = 0;
                HtmlStart = Pos;
                ReadTextOnly = false;
                Object = null;
            }
            public void Reset(int pos)
            {
                Pos = pos;
                Reset();
            }
        }

        //protected CsQuery Owner;
        protected bool isBound;
        /// <summary>
        /// When CsQuery is provided, an initial indexing context can be used
        /// </summary>
        /// <param name="csq"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> Parse(bool allowLiterals)
        {
    
            int pos=0;
            Stack<IterationData> stack = new Stack<IterationData>();

            while (pos <= EndPos)
            {
                IterationData current = new IterationData();
                current.AllowLiterals = allowLiterals;
                current.Reset(pos);
                stack.Push(current);

                while (stack.Count != 0)
                {

                    current = stack.Pop();
                    //Debug.Assert(current.Object == null);

                    while (!current.Finished && current.Pos <= EndPos)
                    {
                        char c = BaseHtml[current.Pos];
                        switch (current.Step)
                        {
                                // special case for textareas & scripts (things which can't have HTML children)
                            case -1:
                                if (c == '<')
                                {
                                    // read ahead to see if a close tag
                                    int endPos =  Array.IndexOf<char>(BaseHtml,'>',current.Pos);
                                    if (endPos>0) {
                                        string tag = BaseHtml.SubstringBetween(current.Pos+1,endPos).ToLower();
                                        if (tag.Substring(1)==current.Parent.Element.NodeName)
                                        {
                                            current.Step=1;
                                            current.ReadTextOnly = false;
                                            break;
                                        }
                                    }
                                }
                                current.Pos++;
                                break;
                            case 0:
                                if (c == '<')
                                {
                                    
                                    // found a tag-- it could be a close tag, or a new HTML tag
                                    current.Step = 1;
                                }
                                else
                                {
                                    current.Pos++;
                                }
                                break;
                            case 1:
                                if (current.Pos > current.HtmlStart)
                                {
                                    IDomObject literal = GetLiteral(current);
                                    if (literal != null)
                                    {
                                        yield return literal;
                                    }
 
                                    continue;
                                }

                                int tagStartPos = current.Pos;
                                string newTag;
                                
                                newTag = GetTagOpener(current);
                                
                                string newTagLower = newTag.ToLower();
                                
                                // when Element exists, it's because a previous iteration created it: it's our parent
                                string parentTag = String.Empty;
                                if (current.Parent != null)
                                {
                                    parentTag = current.Parent.Element.NodeName.ToLower();
                                }

                                if (newTag == String.Empty)
                                {
                                    // It's a tag closer. Make sure it's the right one.
                                    current.Pos = tagStartPos + 1;
                                    //Debug.Assert(curPos != 1504);
                                    string closeTag = GetCloseTag(current);
                                   // Debug.Assert(closeTag != "ul");
                                    // Ignore empty tags, or closing tags found when no parent is open


                                    bool isProperClose = closeTag.ToLower() == parentTag;
                                    if (closeTag == String.Empty)
                                    {
                                        // ignore empty tags
                                        continue;
                                    }
                                    else
                                    {
                                        // locate match for this closer up the heirarchy
                                        IterationData actualParent =null;
                                        
                                        if (!isProperClose)
                                        {
                                            actualParent = current.Parent;
                                            while (actualParent != null && actualParent.Element.NodeName.ToLower() != closeTag.ToLower())
                                            {
                                                actualParent = actualParent.Parent;
                                            }
                                        }
                                        // if no matching close tag was found up the tree, ignore it
                                        // otherwise always close this and repeat at the same position until the match is found
                                        if (!isProperClose && actualParent == null)
                                        {
                                            current.Invalid = true;
                                            continue;
                                        }
                                    }
                                   // element is closed 
                                    
                                    if (current.Parent.Parent == null)
                                    {
                                        yield return current.Parent.Element;
                                    }
                                    current.Finished = true;
                                    if (isProperClose)
                                    {
                                        current.Parent.Reset(current.Pos);
                                    }
                                    else
                                    {
                                        current.Parent.Reset(tagStartPos);
                                    }
                                    // already been returned before we added the children
                                    continue;
                                }
                                // Before we keep going see if this is an implicit close
                                if (parentTag != String.Empty)
                                {
                                    if (TagHasImplicitClose(parentTag,newTag)
                                        && parentTag == newTag)
                                    {
                                        // same tag for a repeater like li occcurred - treat like a close tag
                                        if (current.Parent.Parent == null)
                                        {
                                            yield return current.Parent.Element;
                                        }
                                        current.Parent.Reset(tagStartPos);
                                        current.Finished = true;

                                        continue;
                                    }
                                }
                                // seems to be a new tag. Parse it

                                
                                IDomSpecialElement specialElement = null;
                                
                                if (newTagLower[0] == '!')
                                {
                                    if (newTagLower.StartsWith("!doctype"))
                                    {
                                        specialElement = new DomDocumentType();
                                        current.Object = specialElement;
                                    }
                                    else if (newTagLower.StartsWith("![cdata["))
                                    {
                                        specialElement = new DomCData();
                                        current.Object = specialElement;
                                        current.Pos = tagStartPos + 9;
                                    }
                                    else 
                                    {
                                        specialElement = new DomComment();
                                        current.Object = specialElement;
                                        if (newTagLower.StartsWith("!--"))
                                        {
                                            ((DomComment)specialElement).IsQuoted = true;
                                            current.Pos = tagStartPos + 4;
                                        } else {
                                            current.Pos = tagStartPos+1;
                                        }
                                    }
                                }
                                else
                                {
                                    current.Object = new DomElement(newTag);
                                    
                                    if (!current.Element.InnerHtmlAllowed && current.Element.InnerTextAllowed)
                                    {
                                        current.ReadTextOnly = true;
                                    }
                                }
                                
                                // Check for informational tag types
                                
                               // Debug.Assert(newTag != "p");
                                if (current.Object is IDomSpecialElement)
                                {
                                    string endTag = (current.Object is IDomComment && ((IDomComment)current.Object).IsQuoted) ? "-->" : ">";

                                    int tagEndPos = BaseHtml.Seek(endTag, current.Pos);
                                    if (tagEndPos <0)
                                    {
                                        // if a tag is unclosed entirely, then just find a new line.
                                        tagEndPos = BaseHtml.Seek(System.Environment.NewLine, current.Pos);
                                    }
                                    if (tagEndPos < 0)
                                    {
                                        // Never closed, no newline - junk, treat it like such
                                        tagEndPos = EndPos;
                                    }

                                    specialElement.NonAttributeData = BaseHtml.SubstringBetween(current.Pos, tagEndPos);
                                    current.Pos = tagEndPos;
                                }
                                else
                                {
                                    // Parse attribute data
                                    while (current.Pos <= EndPos)
                                    {
                                        if (!GetTagAttribute(current)) break;
                                    }
                                }

                                bool hasChildren = MoveOutsideTag(current);

                                // tricky part: if there are children, push ourselves back on the stack and start with a new object
                                // from this position. The children will add themselves as they are created, avoiding recursion.
                                // When the close tag is found, the parent will be yielded if it's a root element.
                                // I think there's a slightly better way to do this, capturing all the yield logic at the end of the
                                // stack but it works for now.

                                // For some reason I cannot get my head around a way to perform these logical steps with fewer conditional statements
                                // They must be performed in this order.

                                if (current.Parent != null)
                                {
                                    current.Parent.Element.AppendChild(current.Object);
                                } else if (!hasChildren) {
                                    yield return current.Object;
                                }

                                if (!hasChildren)
                                {
                                    current.Reset();
                                    continue;
                                }


                                stack.Push(current);
                                //Debug.Assert(current.Object == null);

                                IterationData subItem = new IterationData();
                                subItem.Parent = current;
                                subItem.AllowLiterals = true;
                                subItem.Reset(current.Pos);
                                subItem.ReadTextOnly = current.ReadTextOnly;
                                current = subItem;
                                break;

                        }
                    }
                    // Catchall for unclosed tags -- if there's an "unfinished" carrier here, it's because  top-level tag was unclosed.
                    if (!current.Finished)
                    {

                        if (current.Parent != null)
                        {
                            
                            if (current.Parent.Parent == null)
                            {
                                yield return current.Parent.Element;
                            }
                            current.Parent.Reset(current.Pos);
                            current.Finished = true;
                        }
                       
                    }
                }
                /// Check for any straggling text - typically the case for non-dom-bound data.
                if (!current.Finished && current.Pos > current.HtmlStart)
                {
                    IDomObject literal = GetLiteral(current);
                    if (literal != null)
                    {
                        yield return literal;
                    }
                }

                //yield return current.Element;
                pos = current.Pos;
            }

        }

        protected IDomObject GetLiteral(IterationData current)
        {
            // There's plain text -return it as a literal.
            
            IDomObject textObj = null;
            DomText lit;
            if (current.Invalid) {
                lit = new DomInvalidElement();
            } else {
                lit = new DomText();
            }
            //lit.Text = text;
            if (isBound)
            {
                lit.SetTextIndex(Document, Document.TokenizeString(current.HtmlStart, current.Pos - current.HtmlStart));
            }
            else
            {
                string text = BaseHtml.SubstringBetween(current.HtmlStart, current.Pos);
                lit.InnerText = text;
            }
             
            if (!current.AllowLiterals)
            {
                IDomElement wrapper = new DomElement("span");
                wrapper.AppendChild(lit);
                textObj = wrapper;
            }
            else
            {
                textObj = lit;
            }

            if (current.Parent != null)
            {
                current.Parent.Element.AppendChild(textObj);
                current.Reset();
                return null;
            }
            else
            {
                current.Finished = true;
                return textObj;
            }
        }
        /// <summary>
        /// Move pointer to the first character after the end of this tag. Returns True if there are children.
        /// </summary>
        /// <returns></returns>
        protected bool MoveOutsideTag(IterationData current)
        {
            bool finished = false;
            bool inner = false;
            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                if (c == '>')
                {
                    if (BaseHtml[current.Pos - 1] == '/')
                    {
                        inner = false;
                    }
                    else
                    {
                        inner = current.Object.InnerHtmlAllowed || current.Object.InnerTextAllowed;
                    }
                    finished = true;
                    current.HtmlStart = current.Pos + 1;
                }
                current.Pos++;
            }
            return inner;
        }

        protected string GetCloseTag(IterationData current)
        {
            bool finished = false;
            int step = 0;
            int nameStart = 0;
            string name = String.Empty;
            char c;
            while (!finished && current.Pos <= EndPos)
            {
                c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0:
                        if (validNameStartCharacters.Contains(c))
                        {
                            nameStart = current.Pos;
                            step = 1;
                        }
                        current.Pos++;
                        break;
                    case 1:
                        if (!validNameCharacters.Contains(c))
                        {
                            name = BaseHtml.SubstringBetween(nameStart, current.Pos);
                            step = 2;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 2:
                        if (c == '>')
                        {
                            finished = true;
                        }
                        current.Pos++;
                        break;
                }
            }
            return name;
        }
        protected bool GetTagAttribute(IterationData current)
        {
            bool finished = false;
            int step = 0;
            string aName = null;
            string aValue = null;
            int nameStart = -1;
            int valStart = -1;
            bool isQuoted = false;
            char quoteChar = ' ';

            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0: // find name
                        if (validNameStartCharacters.Contains(c))
                        {
                            step = 1;
                            nameStart = current.Pos;
                            current.Pos++;
                        }
                        else if (isTagChar(c))
                        {
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }

                        break;
                    case 1:
                        if (!validNameCharacterSet.Contains(c))
                        {
                            step = 2;
                            aName = BaseHtml.SubstringBetween(nameStart, current.Pos);
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 2: // find value
                        if (c == '=')
                        {
                            step = 3;
                            current.Pos++;
                        }
                        else if (c != ' ')
                        {
                            // anything else means new attribute
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 3: // find quote start
                        if (c=='\\' || c=='>')
                        {
                            // early ending tag
                            finished = true;
                            //step = 4;
                            //valStart = current.Pos;
                            //current.Pos++;
                        }
                        else if (c == ' ')
                        {
                            current.Pos++;
                        }
                        else
                        {
                            if (c == '"' || c == '\'')
                            {
                                isQuoted = true;
                                valStart = current.Pos+1;
                                current.Pos++;
                                quoteChar = c;
                            } else {
                                valStart = current.Pos;
                            }
                            
                            step = 4;
                        
                            // any non-whitespace is part of the attribute   
                        }
                        break;
                    case 4: // parse the attribute until whitespace or closing quote
                        if ((isQuoted && c == quoteChar) || 
                            (!isQuoted && (c==' ' || c=='/' || c=='>')))
                        {
                            aValue = BaseHtml.SubstringBetween(valStart, current.Pos);
                            if (isQuoted)
                            {
                                isQuoted = false;
                                current.Pos++;
                            }
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                }
            }
            if (aName != null)
            {
                current.Element.SetAttribute(aName, aValue);
                return true;
            }
            else
            {
                return false;
            }
        }
        protected string GetOpenText(IterationData current)
        {
            int pos = Array.IndexOf<char>(BaseHtml,'<', current.Pos);
            if (pos > current.Pos)
            {
                int startPos = current.Pos;
                current.Pos = pos;
                return BaseHtml.SubstringBetween(startPos, pos);
            }
            else if (pos == -1)
            {
                int oldPos = current.Pos;
                current.Pos = BaseHtml.Length;
                return BaseHtml.SubstringBetween(oldPos, current.Pos);
            }
            else
            {
                return String.Empty;
            }
        }
       
        protected string GetTagOpener(IterationData current)
        {
            bool finished = false;
            int step = 0;
            int tagStart = -1;

            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0:
                        if (c == '<')
                        {
                            tagStart = current.Pos + 1;
                            step = 1;
                        }
                        current.Pos++;
                        break;
                    case 1:
                        if (c == ' ')
                        {
                            current.Pos++;
                        }
                        else
                        {
                            step = 2;
                        }
                        break;
                    case 2:
                        if (c == '/' || c == ' ' || c == '>')
                        {
                            return BaseHtml.SubstringBetween(tagStart, current.Pos).Trim();
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                }

            }
            return String.Empty;
        }
        const string validNameStartCharacters = "abcdefghijklmnopqrstuvwxyzABCEDFGHIJKLMNOPQRSTUVWXYZ0123456789_:";
        const string validNameCharacters = validNameStartCharacters + ".-";

        protected HashSet<char> validNameStartCharacterSet = new HashSet<char>(validNameStartCharacters.ToArray());
        protected HashSet<char> validNameCharacterSet = new HashSet<char>(validNameCharacters.ToArray());

        protected bool isTagChar(char c)
        {
            return (c == '<' || c == '>' || c == '/');
        }
        // Some tags have inner HTML but are often not closed properly. There are two possible situations. A tag may not have a nested instance of itself, and therefore any
        // recurrence of that tag implies the previous one is closed. Other tag closings are simply optional, but are not repeater tags (e.g. body, html). These should be handled
        // automatically by the logic that bubbles any closing tag to its parent if it doesn't match the current tag. The exception is <head> which technically does not require
        // a close, but we would not expect to find another close tag
        // Complete list of optional closing tags: -</HTML>- </HEAD> -</BODY> -</P> -</DT> -</DD> -</LI> -</OPTION> -</THEAD> </TH> </TBODY> </TR> </TD> </TFOOT> </COLGROUP>

        // body, html don't matter, they will be closed by the document end.
        // 
        protected bool TagHasImplicitClose(string tag, string newTag)
        {
            switch (tag)
            {
                case "li":
                case "option":
                case "p":
                case "tr":
                case "td":
                case "th":

                    // simple case: repeater-like tags should be closed by another occurence of itself
                    return tag == newTag;
                case "head":
                    return (newTag == "body");
                case "dt":
                    return tag == newTag || newTag == "dd";
                case "colgroup":
                    return tag == newTag || newTag == "tr";
                default:
                    return false;

            }
        }
    }
}
