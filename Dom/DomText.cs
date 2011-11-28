﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Defines an interface for elements whose defintion (not innerhtml) contain non-tag or attribute formed data
    /// </summary>

    public interface IDomText : IDomObject
    {
        
    }


    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    public class DomText : DomObject<DomText>, IDomText
    {
        public DomText()
        {

        }

        public DomText(string nodeValue)
            : base()
        {
            NodeValue = nodeValue;
        }


        public override NodeType NodeType
        {
            get { return NodeType.TEXT_NODE; }
        }
        private int textIndex=-1;
        // for use during initial construction from char array
        public void SetTextIndex(IDomRoot dom, int index)
        {
            textIndex = index;
            // create a hard reference to the DOM from which we are mapping our string data. Otherwise if this
            // is moved to another dom, it will break
            stringRef = dom;
        }
        protected string RawText
        {
            get
            {
                return textIndex >= 0 ?
                    stringRef.GetTokenizedString(textIndex)
                        : unboundText;
            }
            set
            {
                unboundText =value;
                textIndex = -1;
            }
        }
        IDomRoot stringRef = null;
        string unboundText;
        public override string InnerText
        {
            get
            {
                return HttpUtility.HtmlDecode(RawText);
            }
            set
            {
                RawText = HttpUtility.HtmlEncode(value);
            }
        }
        public override string NodeValue
        {
            get
            {
                return InnerText;
            }
            set
            {
                InnerText = value;
            }
        }

        public override string InnerHTML
        {
            get
            {
                return RawText;
            }
            set
            {
                RawText = value;
            }
        }
        public override string Render()
        {
            return InnerHTML;
        }
        public override DomText Clone()
        {
            DomText domText = base.Clone();
            domText.textIndex = textIndex;
            domText.unboundText = unboundText;
            domText.stringRef = stringRef;
            return domText;
        }


        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }
        public override bool HasChildren
        {
            get { return false; }
        }
        public override bool Complete
        {
            get { 
                //return !String.IsNullOrEmpty(Text); 
                return textIndex >=0;
            }
        }
        public override string ToString()
        {
            return NodeValue;
        }

    }
}
