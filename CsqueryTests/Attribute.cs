﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Jtc.CsQuery;

namespace CsqueryTests
{
    [TestFixture, Category("Attributes")]
    public class Attribute : CsQueryTest 
    {
        [SetUp]
        public void Init()
        {
            ResetQunit();
        }
        protected void ResetQunit()
        {
            Dom = CsQuery.LoadFile("resources\\jquery-unit-index.htm");
        }
        [Test]
        public void Strings()
        {

            Assert.AreEqual(jQuery("#text1").Attr("type"), "text", "Check for type attribute" );
            Assert.AreEqual(jQuery("#radio1").Attr("type"), "radio", "Check for type attribute");
            Assert.AreEqual(jQuery("#check1").Attr("type"), "checkbox", "Check for type attribute");
            Assert.AreEqual(jQuery("#simon1").Attr("rel"), "bookmark", "Check for rel attribute");
            Assert.AreEqual(jQuery("#text1").Attr("type"), "text", "Check for type attribute");
            Assert.AreEqual(jQuery("#radio1").Attr("type"), "radio", "Check for type attribute");
            Assert.AreEqual(jQuery("#check1").Attr("type"), "checkbox", "Check for type attribute");
            Assert.AreEqual(jQuery("#simon1").Attr("rel"), "bookmark", "Check for rel attribute");
            Assert.AreEqual(jQuery("#google").Attr("title"), "Google!", "Check for title attribute" );
            Assert.AreEqual(jQuery("#mark").Attr("hreflang"), "en", "Check for hreflang attribute" );
            Assert.AreEqual(jQuery("#en").Attr("lang"), "en", "Check for lang attribute" );
            Assert.AreEqual(jQuery("#simon").Attr("class"), "blog link", "Check for class attribute" );
            Assert.AreEqual(jQuery("#name").Attr("name"), "name", "Check for name attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("name"), "action", "Check for name attribute" );
            Assert.IsTrue(jQuery("#form").Attr("action").IndexOf("formaction") >= 0, "Check for action attribute");
            Assert.AreEqual(jQuery("#text1").Attr("value", "t").Attr("value"), "t", "Check setting the value attribute" );
            Assert.AreEqual(jQuery("<div value='t'></div>").Attr("value"), "t", "Check setting custom attr named 'value' on a div" );
            Assert.AreEqual(jQuery("#form").Attr("blah", "blah").Attr("blah"), "blah", "Set non-existant attribute on a form" );
            Assert.AreEqual(jQuery("#foo").Attr("height"), null, "Non existent height attribute should return undefined" );

            //    // [7472] & [3113] (form contains an input with name="action" or name="id")
            var extras = jQuery("<input name='id' name='name' /><input id='target' name='target' />").AppendTo("#testForm");
            Assert.AreEqual(jQuery("#form").Attr("action","newformaction").Attr("action"), "newformaction", "Check that action attribute was changed" );
            Assert.AreEqual(jQuery("#testForm").Attr("target"), null, "Retrieving target does not equal the input with name=target" );
            Assert.AreEqual(jQuery("#testForm").Attr("target", "newTarget").Attr("target"), "newTarget", "Set target successfully on a form" );
            Assert.AreEqual(jQuery("#testForm").RemoveAttr("id").Attr("id"), null, "Retrieving id does not equal the input with name=id after id is removed [#7472]" );
            //    // Bug #3685 (form contains input with name="name")
            Assert.AreEqual(jQuery("#testForm").Attr("name"), null, "Retrieving name does not retrieve input with name=name" );
            //    extras.remove();

            Assert.AreEqual(jQuery("#text1").Attr("maxlength"), "30", "Check for maxlength attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("maxLength"), "30", "Check for maxLength attribute" );
            Assert.AreEqual(jQuery("#area1").Attr("maxLength"), "30", "Check for maxLength attribute" );

            //    // using innerHTML in IE causes href attribute to be serialized to the full path
            jQuery("<a/>").AttrSet("{ 'id': 'tAnchor5', 'href': '#5' }").AppendTo("#qunit-fixture");
            Assert.AreEqual(jQuery("#tAnchor5").Attr("href"), "#5", "Check for non-absolute href (an anchor)" );

            //    // list attribute is readonly by default in browsers that support it
            jQuery("#list-test").Attr("list", "datalist");
            Assert.AreEqual(jQuery("#list-test").Attr("list"), "datalist", "Check setting list attribute" );

            //    // Related to [5574] and [5683]
             var jbody = jQuery("body");
            IDomObject  body = jbody[0];

           Assert.AreEqual( jbody.Attr("foo"), null, "Make sure that a non existent attribute returns undefined" );

            body.SetAttribute("foo", "baz");
            Assert.AreEqual(jbody.Attr("foo"), "baz", "Make sure the dom attribute is retrieved when no expando is found" );

            jbody.Attr("foo","cool");
            Assert.AreEqual(jbody.Attr("foo"), "cool", "Make sure that setting works well when both expando and dom attribute are available" );

           body.RemoveAttribute("foo"); // Cleanup

           var select = new DomElement("select");
            var optgroup = new DomElement("optgroup");
            var option = new DomElement("option");
            // This is not in the original test - nothing is selected by default in this model since it would change the HTML rendered.
            option.SetAttribute("selected");

            Dom.Append(select);

            optgroup.AppendChild( option );
            select.AppendChild( optgroup );


            Assert.AreEqual( jQuery( option ).Attr("selected"), "selected", "Make sure that a single option is selected, even when in an optgroup." );

            var jimg = jQuery("<img style='display:none' width='215' height='53' src='http://static.jquery.com/files/rocker/images/logo_jquery_215x53.gif'/>").AppendTo("body");
            Assert.AreEqual(jimg.Attr("width"), "215", "Retrieve width attribute an an element with display:none." );
            Assert.AreEqual(jimg.Attr("height"), "53", "Retrieve height attribute an an element with display:none." );

            //    // Check for style support
            Assert.IsTrue(0==jQuery("#dl").Attr("style").IndexOf("position"), "Check style attribute getter, also normalize css props to lowercase" );
            Assert.IsTrue(0 == jQuery("#foo").Attr("style", "position:absolute;").Attr("style").IndexOf("position"), "Check style setter");

            //    // Check value on button element (#1954)
            var jbutton = jQuery("<button value='foobar'>text</button>").InsertAfter("#button");
            Assert.AreEqual(jbutton.Attr("value"), "foobar", "Value retrieval on a button does not return innerHTML" );
            Assert.AreEqual(jbutton.Attr("value", "baz").Html(), "text", "Setting the value does not change innerHTML" );

            //    // Attributes with a colon on a table element (#1591)
            Assert.AreEqual(jQuery("#table").Attr("test:attrib"), null, "Retrieving a non-existent attribute on a table with a colon does not throw an error." );
            Assert.AreEqual(jQuery("#table").Attr("test:attrib", "foobar").Attr("test:attrib"), "foobar", "Setting an attribute on a table with a colon does not throw an error." );

           var jform = jQuery("<form class='something'></form>").AppendTo("#qunit-fixture");
           Assert.AreEqual( jform.Attr("class"), "something", "Retrieve the class attribute on a form." );

           var ja = jQuery("<a href='#' onclick='something()'>Click</a>").AppendTo("#qunit-fixture");
           Assert.AreEqual( ja.Attr("onclick"), "something()", "Retrieve ^on attribute without anonymous function wrapper." );

           Assert.IsTrue( jQuery("<div/>").Attr("doesntexist") == null, "Make sure undefined is returned when no attribute is found." );
           Assert.IsTrue( jQuery("<div/>").Attr("title") == null, "Make sure undefined is returned when no attribute is found." );
           Assert.AreEqual( jQuery("<div/>").Attr("title", "something").Attr("title"), "something", "Set the title attribute." );
           Assert.IsTrue( jQuery().Attr("doesntexist") == null, "Make sure undefined is returned when no element is there." );
           Assert.AreEqual( jQuery("<div/>").Attr("value"), null, "An unset value on a div returns undefined." );
           Assert.AreEqual( jQuery("<input/>").Attr("value"), "", "An unset value on an input returns current value." );
        }



        /*
         * Not implemented (XLM)
         */


        //if ( !isLocal ) {
        //    test("attr(String) in XML Files", function() {
        //        expect(3);
        //        stop();
        //        jQuery.get("data/dashboard.xml", function( xml ) {
        //            Assert.AreEqual( jQuery( "locations", xml ).Attr("class"), "foo", "Check class attribute in XML document" );
        //            Assert.AreEqual( jQuery( "location", xml ).Attr("for"), "bar", "Check for attribute in XML document" );
        //            Assert.AreEqual( jQuery( "location", xml ).Attr("checked"), "different", "Check that hooks are not attached in XML document" );
        //            start();
        //        });
        //    });
        //}

        /*
         * Not Implemented
         */

        //test("attr(String, Function)", function() {
        //    expect(2);
        //Assert.AreEqual(jQuery("#text1").Attr("value", function() { return this.id; })[0].value, "text1", "Set value from id" );
        //Assert.AreEqual(jQuery("#text1").Attr("title", function(i) { return i; }).Attr("title"), "0", "Set value with an index");
        //});

        [Test]
        public void Hash()
        {
            bool pass = true;
            jQuery("div").AttrSet("{foo: 'baz', zoo: 'ping'}").Each((IDomElement e) =>
            {
                if ( e.GetAttribute("foo") != "baz" && e.GetAttribute("zoo") != "ping" ) pass = false;
            });
            Assert.IsTrue(pass,"Set Multiple Attributes");
            // no point
            //Assert.AreEqual(jQuery("#text1").Attr({value: function() { return this.id; }})[0].value, "text1", "Set attribute to computed value #1" );
            //Assert.AreEqual(jQuery("#text1").Attr({title: function(i) { return i; }}).Attr("title"), "0", "Set attribute to computed value #2");
        }

        [Test]
        public void Attr_String_Object()
        {
            var div = jQuery("div").Attr("foo", "bar");
            bool fail = false;
            int i;
            for (i = 0; i < div.Length; i++)
            {
                if (div[i].GetAttribute("foo") != "bar")
                {
                    fail = true;
                    break;
                }
            }

            Assert.AreEqual(fail, false, "Set Attribute, the #" + i + " element didn't get the attribute 'foo'");

            //, "Try to set an attribute to nothing" );
            jQuery("#foo").Attr("{ 'width': null }");
            jQuery("#name").Attr("name", "something");



            jQuery("#name").Attr("name", "something");
            //Assert.AreEqual(jQuery("#name").Attr("name"), "something", "Set name attribute" );
            ////    jQuery("#name").Attr("name", null);
            //Assert.AreEqual(jQuery("#name").Attr("name"), undefined, "Remove name attribute" );
            ////    var $input = jQuery("<input>", { name: "something" });
            //Assert.AreEqual($input.Attr("name"), "something", "Check element creation gets/sets the name attribute." );
        }
        [Test]
        public void BooleanAttr()
        {


           jQuery("#check2").Prop("checked", true).Prop("checked", false).Attr("checked", true);
           Assert.AreEqual(document.GetElementById("check2").Checked, true, "Set checked attribute" );
           Assert.AreEqual(jQuery("#check2").Prop("checked"), true, "Set checked attribute" );
           Assert.AreEqual(jQuery("#check2").Attr("checked"), "checked", "Set checked attribute" );
           jQuery("#check2").Attr("checked", false);
            Assert.AreEqual(document.GetElementById("check2").Checked, false, "Set checked attribute" );
           Assert.AreEqual(jQuery("#check2").Prop("checked"), false, "Set checked attribute" );
           Assert.AreEqual(jQuery("#check2").Attr("checked"), null, "Set checked attribute" );
            jQuery("#text1").Attr("readonly", true);
            Assert.AreEqual(document.GetElementById("text1").ReadOnly, true, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Prop("readOnly"), true, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("readonly"), "readonly", "Set readonly attribute" );
                jQuery("#text1").Attr("readonly", false);
            Assert.AreEqual(document.GetElementById("text1").ReadOnly, false, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Prop("readOnly"), false, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("readonly"), null, "Set readonly attribute" );

           jQuery("#check2").Prop("checked", true);
            Assert.AreEqual(document.GetElementById("check2").Checked, true, "Set checked attribute" );
            Assert.AreEqual(jQuery("#check2").Prop("checked"), true, "Set checked attribute" );
            Assert.AreEqual(jQuery("#check2").Attr("checked"), "checked", "Set checked attribute" );
            jQuery("#check2").Prop("checked", false);
            Assert.AreEqual(document.GetElementById("check2").Checked, false, "Set checked attribute" );
            Assert.AreEqual(jQuery("#check2").Prop("checked"), false, "Set checked attribute" );
            Assert.AreEqual(jQuery("#check2").Attr("checked"), null, "Set checked attribute" );

            jQuery("#check2").Attr("checked", "checked");
            Assert.AreEqual( document.GetElementById("check2").Checked, true, "Set checked attribute with 'checked'" );
            Assert.AreEqual( jQuery("#check2").Prop("checked"), true, "Set checked attribute" );
            Assert.AreEqual( jQuery("#check2").Attr("checked"), "checked", "Set checked attribute" );

            jQuery("#text1").Prop("readOnly", true);
            Assert.AreEqual(document.GetElementById("text1").ReadOnly, true, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Prop("readOnly"), true, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("readonly"), "readonly", "Set readonly attribute" );
             jQuery("#text1").Prop("readOnly", false);
            Assert.AreEqual(document.GetElementById("text1").ReadOnly, false, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Prop("readOnly"), false, "Set readonly attribute" );
            Assert.AreEqual(jQuery("#text1").Attr("readonly"), null, "Set readonly attribute" );

            // In original tests, the comparison was to numeric values (not strings). I am not implementing every single
            // attribute as a property - we're just doing everything as strings.

             jQuery("#name").Attr("maxlength", "5");
            Assert.AreEqual(document.GetElementById("name").GetAttribute("maxlength"), "5", "Set maxlength attribute" );
               jQuery("#name").Attr("maxLength", "10");
            Assert.AreEqual(document.GetElementById("name").GetAttribute("maxlength"), "10", "Set maxlength attribute" );

                // HTML5 boolean attributes
             var jtext = jQuery("#text1").AttrSet(@"{
               'autofocus': true,
                 'required': true
            }");
               Assert.AreEqual( jtext.Attr("autofocus"), "autofocus", "Set boolean attributes to the same name" );
              Assert.AreEqual( jtext.Attr("autofocus", false).Attr("autofocus"), null, "Setting autofocus attribute to false removes it" );
              Assert.AreEqual( jtext.Attr("required"), "required", "Set boolean attributes to the same name" );
              Assert.AreEqual(jtext.Attr("required", false).Attr("required"), null, "Setting required attribute to false removes it");

               var jdetails = jQuery("<details open></details>").AppendTo("#qunit-fixture");
                Assert.AreEqual( jdetails.Attr("open"), "open", "open attribute presense indicates true" );
                Assert.AreEqual(jdetails.Attr("open", false).Attr("open"), null, "Setting open attribute to false removes it");
   

            // TODO
            // Data needs to wrap non-objects in an object, and figure thatout again on the return. We should look at how JQ stores it

            Assert.AreEqual(jtext.Attr("data-something", true).Data("something"), true, "Setting data attributes are not affected by boolean settings");
            Assert.AreEqual(jtext.Attr("data-another", false).Data("another"), false, "Setting data attributes are not affected by boolean settings" );
            Assert.AreEqual(jtext.Attr("aria-disabled", false).Attr("aria-disabled"), "false", "Setting aria attributes are not affected by boolean settings");
           
            jtext.RemoveData("something").RemoveData("another").RemoveAttr("aria-disabled");

            jQuery("#foo").Attr("contenteditable", true);
            Assert.AreEqual(jQuery("#foo").Attr("contenteditable"), "true", "Enumerated attributes are set properly" );

            // irrelevant - we skip such nodes. Trying to access the Attributes property directly results in an excepion.
            
            //var attributeNode = document.createAttribute("irrelevant"),
             var   commentNode =new DomComment("some comment");
             var   textNode = new DomText("some text");
             
             IEnumerable<IDomObject> obj = new List<IDomObject> { commentNode,textNode};
            foreach (var elem in obj) {
                var jelem = jQuery( elem );
                jelem.Attr( "nonexisting", "foo" );
                Assert.AreEqual(jelem.Attr("nonexisting"), null, "attr(name, value) works correctly on comment and text nodes (bug #7500)." );
            }

            List<object> objs = new List<object> {document,obj, "#firstp"};
            foreach (var item in objs) {

                var jelem = jQuery(item);
                    Assert.AreEqual ( jelem.Attr("nonexisting"), null, "attr works correctly for non existing attributes (bug #7500)." );
                // Different functionality: we don't really have a "prop" concept except as bound to a rendered attribute. So assigning
                // attributes to non-element items will always do nothing.
                    if (!(jelem[0] is IDomElement))
                    {
                        Assert.AreEqual(jelem.Attr("something", "foo").Attr("something"), null, "attr falls back to prop on unsupported arguments");
                    }
                }

            var table = jQuery("#table").Append("<tr><td>cell</td></tr><tr><td>cell</td><td>cell</td></tr><tr><td>cell</td><td>cell</td></tr>");
             var td = table.Find("td:first");
            td.Attr("rowspan", "2");
            Assert.AreEqual(td[0].Attributes["rowSpan"], "2", "Check rowspan is correctly set");
            td.Attr("colspan", "2");
            Assert.AreEqual(td[0].Attributes["colSpan"], "2", "Check colspan is correctly set");
            table.Attr("cellspacing", "2");
            Assert.AreEqual(table[0].Attributes["cellSpacing"], "2", "Check cellspacing is correctly set");

            Assert.AreEqual(jQuery("#area1").Attr("value"), "foobar", "Value attribute retrieves the property for backwards compatibility.");

            //    // for #1070
              jQuery("#name").Attr("someAttr", "0");
            Assert.AreEqual(jQuery("#name").Attr("someAttr"), "0", "Set attribute to a string of \"0\"");
              jQuery("#name").Attr("someAttr", 0);
            Assert.AreEqual(jQuery("#name").Attr("someAttr"), "0", "Set attribute to the number 0");
               jQuery("#name").Attr("someAttr", 1);
            Assert.AreEqual(jQuery("#name").Attr("someAttr"), "1", "Set attribute to the number 1");

                // using contents will get comments regular, text, and comment nodes
                var j = jQuery("#nonnodes").Contents();

              j.Attr("name", "attrvalue");
            Assert.AreEqual(j.Attr("name"), "attrvalue", "Check node,textnode,comment for attr");
               j.RemoveAttr("name");
        }

        [Test]
        public void TypeTests()
        {
            ResetQunit();
            // Type
            var type = jQuery("#check2").Attr("type");
            var thrown = false;
            try
            {
                jQuery("#check2").Attr("type", "hidden");
            }
            catch
            {
                thrown = true;
            }
            Assert.IsTrue(thrown, "Exception thrown when trying to change type property");
            Assert.AreEqual(type, jQuery("#check2").Attr("type"), "Verify that you can't change the type of an input element");

            var check = new DomElement("input");
            thrown = true;
            try
            {
                jQuery(check).Attr("type", "checkbox");
            }
            catch 
            {
                thrown = false;
            }
            Assert.IsTrue(thrown, "Exception thrown when trying to change type property");
            Assert.AreEqual("checkbox", jQuery(check).Attr("type"), "Verify that you can change the type of an input element that isn't in the DOM");

      
            ResetQunit();
	

            var jcheck = jQuery("<input />");
             thrown = true;
            try {
                jcheck.Attr("type","checkbox");
            } catch {
                thrown = false;
            }
           Assert.IsTrue( thrown, "Exception thrown when trying to change type property" );
            Assert.AreEqual("checkbox", jcheck.Attr("type"), "Verify that you can change the type of an input element that isn't in the DOM" );

            var button = jQuery("#button");
             thrown = false;
            try {
                button.Attr("type","submit");
            } catch {
                thrown = true;
            }
           Assert.IsTrue( thrown, "Exception thrown when trying to change type property" );
         Assert.AreEqual("button", button.Attr("type"), "Verify that you can't change the type of a button element" );

            var jradio = CsQuery.Create("<input>", "{ 'value': 'sup', 'type': 'radio' }").AppendTo("#testForm");
            Assert.AreEqual(jradio.Val(), "sup", "Value is not reset when type is set after value on a radio" );

            // Setting attributes on svg elements (bug #3116)
            var jsvg = jQuery("<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' version='1.1' baseProfile='full' width='200' height='200'>"
                + "<circle cx='200' cy='200' r='150' />"
            + "</svg>").AppendTo("body");
        Assert.AreEqual(jsvg.Attr("cx", 100).Attr("cx"), "100", "Set attribute on svg element" );
        jsvg.Remove();
    
        }

        [Test]
        public void JqueryMethod()
        {

                var jelem = jQuery("<div />");
                var elem = jelem[0];

                // one at a time
                jelem.AttrSet("{html: 'foo'}");
                 Assert.AreEqual(elem.InnerHtml, "foo", "attr(html)");

                jelem.AttrSet("{text: 'bar'}");
                Assert.AreEqual(elem.InnerHtml, "bar", "attr(text)");

                jelem.AttrSet("{css: {color: 'red'}}");
               Assert.IsTrue( Regex.IsMatch(elem.Style["color"], "^(#ff0000|red)$"), "attr(css)");

                jelem.AttrSet("{height: 10}");
                Assert.AreEqual(elem.Style["height"], "10px", "attr(height)");

                // Multiple attributes

                jelem.AttrSet(@"{
                    width:10,
                    css:{ padding-left:1, padding-right:1 }
                }");

                Assert.AreEqual(elem.Style["width"], "10px", "attr({...})");
                Assert.AreEqual(elem.Style["padding-left"], "1px", "attr({...})");
                Assert.AreEqual(elem.Style["padding-right"], "1px", "attr({...})");

        }

        [Test]
        public void TabIndex()
        {


            // elements not natively tabbable
            Assert.AreEqual(jQuery("#listWithTabIndex").Attr("tabindex"), "5", "not natively tabbable, with tabindex set to 0");
            Assert.AreEqual(jQuery("#divWithNoTabIndex").Attr("tabindex"), null, "not natively tabbable, no tabindex set");

            // anchor with href
            // TODO? Apparently 0 should be returned for this? Changeed to null
            // Assert.AreEqual(jQuery("#linkWithNoTabIndex").Attr("tabindex"), "0", "anchor with href, no tabindex set");
            Assert.AreEqual(jQuery("#linkWithTabIndex").Attr("tabindex"), "2", "anchor with href, tabindex set to 2");
            Assert.AreEqual(jQuery("#linkWithNegativeTabIndex").Attr("tabindex"), "-1", "anchor with href, tabindex set to -1");

            // anchor without href
            Assert.AreEqual(jQuery("#linkWithNoHrefWithNoTabIndex").Attr("tabindex"), null, "anchor without href, no tabindex set");
            Assert.AreEqual(jQuery("#linkWithNoHrefWithTabIndex").Attr("tabindex"), "1", "anchor without href, tabindex set to 2");
            Assert.AreEqual(jQuery("#linkWithNoHrefWithNegativeTabIndex").Attr("tabindex"), "-1", "anchor without href, no tabindex set");


            var element = jQuery("#divWithNoTabIndex");
            Assert.AreEqual(element.Attr("tabindex"), null, "start with no tabindex");

            // set a positive string
            element.Attr("tabindex", "1");
            Assert.AreEqual(element.Attr("tabindex"), "1", "set tabindex to 1 (string)");

            // set a zero string
            element.Attr("tabindex", "0");
            Assert.AreEqual(element.Attr("tabindex"), "0", "set tabindex to 0 (string)");

            // set a negative string
            element.Attr("tabindex", "-1");
            Assert.AreEqual(element.Attr("tabindex"), "-1", "set tabindex to -1 (string)");

            // set a positive number
            element.Attr("tabindex", 1);
            Assert.AreEqual(element.Attr("tabindex"), "1", "set tabindex to 1 (number)");

            // set a zero number
            element.Attr("tabindex", 0);
            Assert.AreEqual(element.Attr("tabindex"), "0", "set tabindex to 0 (number)");

            // set a negative number
            element.Attr("tabindex", -1);
            Assert.AreEqual(element.Attr("tabindex"), "-1", "set tabindex to -1 (number)");

            element = jQuery("#linkWithTabIndex");
            Assert.AreEqual(element.Attr("tabindex"), "2", "start with tabindex 2");

            element.Attr("tabindex", -1);
            Assert.AreEqual(element.Attr("tabindex"), "-1", "set negative tabindex");
        }
//[Test]
//public void RemoveAttr()
//{

//Assert.AreEqual(jQuery("#mark").RemoveAttr("class")[0].className, "", "remove class");
//Assert.AreEqual(jQuery("#form").RemoveAttr("id").Attr("id"), null, "Remove id");
//Assert.AreEqual(jQuery("#foo").Attr("style", "position:absolute;").removeAttr("style").Attr("style"), null, "Check removing style attribute");
//Assert.AreEqual(jQuery("#form").Attr("style", "position:absolute;").removeAttr("style").Attr("style"), null, "Check removing style attribute on a form");
//Assert.AreEqual(jQuery("#fx-test-group").Attr("height", "3px").removeAttr("height").css("height"), "1px", "Removing height attribute has no effect on height set with style attribute");

////    jQuery("#check1").removeAttr("checked").Prop("checked", true).removeAttr("checked");
//Assert.AreEqual(document.GetElementById("check1").Checked, false, "removeAttr sets boolean properties to false");
////    jQuery("#text1").Prop("readOnly", true).removeAttr("readonly");
//Assert.AreEqual(document.GetElementById("text1").ReadOnly, false, "removeAttr sets boolean properties to false");
////});
//}


////test("prop(String, Object)", function() {
////    expect(30);

//Assert.AreEqual(jQuery("#text1").Prop("value"), "Test", "Check for value attribute" );
//Assert.AreEqual(jQuery("#text1").Prop("value", "Test2").Prop("defaultValue"), "Test", "Check for defaultValue attribute" );
//Assert.AreEqual(jQuery("#select2").Prop("selectedIndex"), 3, "Check for selectedIndex attribute" );
//Assert.AreEqual(jQuery("#foo").Prop("nodeName").toUpperCase(), "DIV", "Check for nodeName attribute" );
//Assert.AreEqual(jQuery("#foo").Prop("tagName").toUpperCase(), "DIV", "Check for tagName attribute" );
//Assert.AreEqual(jQuery("<option/>").Prop("selected"), false, "Check selected attribute on disconnected element." );

//Assert.AreEqual(jQuery("#listWithTabIndex").Prop("tabindex"), 5, "Check retrieving tabindex" );
////    jQuery("#text1").Prop("readonly", true);
//Assert.AreEqual(document.GetElementById("text1").ReadOnly, true, "Check setting readOnly property with 'readonly'" );
//Assert.AreEqual(jQuery("#label-for").Prop("for"), "action", "Check retrieving htmlFor" );
////    jQuery("#text1").Prop("class", "test");
//Assert.AreEqual(document.GetElementById("text1").className, "test", "Check setting className with 'class'" );
//Assert.AreEqual(jQuery("#text1").Prop("maxlength"), 30, "Check retrieving maxLength" );
////    jQuery("#table").Prop("cellspacing", 1);
//Assert.AreEqual(jQuery("#table").Prop("cellSpacing"), "1", "Check setting and retrieving cellSpacing" );
////    jQuery("#table").Prop("cellpadding", 1);
//Assert.AreEqual(jQuery("#table").Prop("cellPadding"), "1", "Check setting and retrieving cellPadding" );
////    jQuery("#table").Prop("rowspan", 1);
//Assert.AreEqual(jQuery("#table").Prop("rowSpan"), 1, "Check setting and retrieving rowSpan" );
////    jQuery("#table").Prop("colspan", 1);
//Assert.AreEqual(jQuery("#table").Prop("colSpan"), 1, "Check setting and retrieving colSpan" );
////    jQuery("#table").Prop("usemap", 1);
//Assert.AreEqual(jQuery("#table").Prop("useMap"), 1, "Check setting and retrieving useMap" );
////    jQuery("#table").Prop("frameborder", 1);
//Assert.AreEqual(jQuery("#table").Prop("frameBorder"), 1, "Check setting and retrieving frameBorder" );
////    QUnit.reset();

////    var body = document.body, $body = jQuery( body );
////   Assert.IsTrue( $body.prop("nextSibling") === null, "Make sure a null expando returns null" );
////    body.foo = "bar";
//Assert.AreEqual($body.prop("foo"), "bar", "Make sure the expando is preferred over the dom attribute" );
////    body.foo = undefined;
////   Assert.IsTrue( $body.prop("foo") === undefined, "Make sure the expando is preferred over the dom attribute, even if undefined" );

////    var select = document.createElement("select"), optgroup = document.createElement("optgroup"), option = document.createElement("option");
////    optgroup.appendChild( option );
////    select.appendChild( optgroup );

//Assert.AreEqual(jQuery(option).Prop("selected"), true, "Make sure that a single option is selected, even when in an optgroup." );
//Assert.AreEqual(jQuery(document).Prop("nodeName"), "#document", "prop works correctly on document nodes (bug #7451)." );

////    var attributeNode = document.createAttribute("irrelevant"),
////        commentNode = document.createComment("some comment"),
////        textNode = document.createTextNode("some text"),
////        obj = {};
////    jQuery.each( [document, attributeNode, commentNode, textNode, obj, "#firstp"], function( i, ele ) {
////        strictEqual( jQuery(ele).Prop("nonexisting"), undefined, "prop works correctly for non existing attributes (bug #7500)." );
////    });

////    var obj = {};
////    jQuery.each( [document, obj], function( i, ele ) {
////        var $ele = jQuery( ele );
////        $ele.prop( "nonexisting", "foo" );
////        Assert.AreEqual( $ele.prop("nonexisting"), "foo", "prop(name, value) works correctly for non existing attributes (bug #7500)." );
////    });
////    jQuery( document ).removeProp("nonexisting");
////});

////test("removeProp(String)", function() {
////    expect(6);
////    var attributeNode = document.createAttribute("irrelevant"),
////        commentNode = document.createComment("some comment"),
////        textNode = document.createTextNode("some text"),
////        obj = {};

////    strictEqual( jQuery( "#firstp" ).Prop( "nonexisting", "foo" ).removeProp( "nonexisting" )[0].nonexisting, undefined, "removeprop works correctly on DOM element nodes" );

////    jQuery.each( [document, obj], function( i, ele ) {
////        var $ele = jQuery( ele );
////        $ele.prop( "nonexisting", "foo" ).removeProp( "nonexisting" );
////        strictEqual( ele.nonexisting, undefined, "removeProp works correctly on non DOM element nodes (bug #7500)." );
////    });
////    jQuery.each( [commentNode, textNode, attributeNode], function( i, ele ) {
////        var $ele = jQuery( ele );
////        $ele.prop( "nonexisting", "foo" ).removeProp( "nonexisting" );
////        strictEqual( ele.nonexisting, undefined, "removeProp works correctly on non DOM element nodes (bug #7500)." );
////    });
////});

////test("val()", function() {
////    expect(26);

////    document.GetElementById("text1").value = "bla";
//Assert.AreEqual(jQuery("#text1").val(), "bla", "Check for modified value of input element" );

////    QUnit.reset();

//Assert.AreEqual(jQuery("#text1").val(), "Test", "Check for value of input element" );
////    // ticket #1714 this caused a JS error in IE
//Assert.AreEqual(jQuery("#first").val(), "", "Check a paragraph element to see if it has a value" );
////   Assert.IsTrue( jQuery([]).val() === undefined, "Check an empty jQuery object will return undefined from val" );

//Assert.AreEqual(jQuery("#select2").val(), "3", "Call val() on a single=\"single\" select" );

////    same( jQuery("#select3").val(), ["1", "2"], "Call val() on a multiple=\"multiple\" select" );

//Assert.AreEqual(jQuery("#option3c").val(), "2", "Call val() on a option element with value" );

//Assert.AreEqual(jQuery("#option3a").val(), "", "Call val() on a option element with empty value" );

//Assert.AreEqual(jQuery("#option3e").val(), "no value", "Call val() on a option element with no value attribute" );

//Assert.AreEqual(jQuery("#option3a").val(), "", "Call val() on a option element with no value attribute" );

////    jQuery("#select3").val("");
////    same( jQuery("#select3").val(), [""], "Call val() on a multiple=\"multiple\" select" );

////    same( jQuery("#select4").val(), [], "Call val() on multiple=\"multiple\" select with all disabled options" );

////    jQuery("#select4 optgroup").add("#select4 > [disabled]").Attr("disabled", false);
////    same( jQuery("#select4").val(), ["2", "3"], "Call val() on multiple=\"multiple\" select with some disabled options" );

////    jQuery("#select4").Attr("disabled", true);
////    same( jQuery("#select4").val(), ["2", "3"], "Call val() on disabled multiple=\"multiple\" select" );

//Assert.AreEqual(jQuery("#select5").val(), "3", "Check value on ambiguous select." );

////    jQuery("#select5").val(1);
//Assert.AreEqual(jQuery("#select5").val(), "1", "Check value on ambiguous select." );

////    jQuery("#select5").val(3);
//Assert.AreEqual(jQuery("#select5").val(), "3", "Check value on ambiguous select." );

////    var checks = jQuery("<input type='checkbox' name='test' value='1'/><input type='checkbox' name='test' value='2'/><input type='checkbox' name='test' value=''/><input type='checkbox' name='test'/>").appendTo("#form");

////    same( checks.serialize(), "", "Get unchecked values." );

//Assert.AreEqual(checks.eq(3).val(), "on", "Make sure a value of 'on' is provided if none is specified." );

////    checks.val([ "2" ]);
////    same( checks.serialize(), "test=2", "Get a single checked value." );

////    checks.val([ "1", "" ]);
////    same( checks.serialize(), "test=1&test=", "Get multiple checked values." );

////    checks.val([ "", "2" ]);
////    same( checks.serialize(), "test=2&test=", "Get multiple checked values." );

////    checks.val([ "1", "on" ]);
////    same( checks.serialize(), "test=1&test=on", "Get multiple checked values." );

////    checks.remove();

////    var $button = jQuery("<button value='foobar'>text</button>").insertAfter("#button");
//Assert.AreEqual($button.val(), "foobar", "Value retrieval on a button does not return innerHTML" );
//Assert.AreEqual($button.val("baz").html(), "text", "Setting the value does not change innerHTML" );
	
//Assert.AreEqual(jQuery("<option/>").val("test").Attr("value"), "test", "Setting value sets the value attribute" );
////});

////if ( "value" in document.createElement("meter") && 
////            "value" in document.createElement("progress") ) {

////    test("val() respects numbers without exception (Bug #9319)", function() {

////        expect(4);

////        var $meter = jQuery("<meter min='0' max='10' value='5.6'></meter>"),
////            $progress = jQuery("<progress max='10' value='1.5'></progress>");

////        try {
////            Assert.AreEqual( typeof $meter.val(), "number", "meter, returns a number and does not throw exception" );
////            Assert.AreEqual( $meter.val(), $meter[0].value, "meter, api matches host and does not throw exception" );

////            Assert.AreEqual( typeof $progress.val(), "number", "progress, returns a number and does not throw exception" );
////            Assert.AreEqual( $progress.val(), $progress[0].value, "progress, api matches host and does not throw exception" );

////        } catch(e) {}

////        $meter.remove();
////        $progress.remove();
////    });
////}

////var testVal = function(valueObj) {
////    expect(8);

////    QUnit.reset();
////    jQuery("#text1").val(valueObj( "test" ));
//Assert.AreEqual(document.GetElementById("text1").value, "test", "Check for modified (via val(String)) value of input element" );

////    jQuery("#text1").val(valueObj( undefined ));
//Assert.AreEqual(document.GetElementById("text1").value, "", "Check for modified (via val(undefined)) value of input element" );

////    jQuery("#text1").val(valueObj( 67 ));
//Assert.AreEqual(document.GetElementById("text1").value, "67", "Check for modified (via val(Number)) value of input element" );

////    jQuery("#text1").val(valueObj( null ));
//Assert.AreEqual(document.GetElementById("text1").value, "", "Check for modified (via val(null)) value of input element" );

////    var $select1 = jQuery("#select1");
////    $select1.val(valueObj( "3" ));
//Assert.AreEqual($select1.val(), "3", "Check for modified (via val(String)) value of select element" );

////    $select1.val(valueObj( 2 ));
//Assert.AreEqual($select1.val(), "2", "Check for modified (via val(Number)) value of select element" );

////    $select1.append("<option value='4'>four</option>");
////    $select1.val(valueObj( 4 ));
//Assert.AreEqual($select1.val(), "4", "Should be possible to set the val() to a newly created option" );

////    // using contents will get comments regular, text, and comment nodes
////    var j = jQuery("#nonnodes").contents();
////    j.val(valueObj( "asdf" ));
//Assert.AreEqual(j.val(), "asdf", "Check node,textnode,comment with val()" );
////    j.removeAttr("value");
////}

////test("val(String/Number)", function() {
////    testVal(bareObj);
////});

////test("val(Function)", function() {
////    testVal(functionReturningObj);
////});

////test( "val(Array of Numbers) (Bug #7123)", function() {
////    expect(4);
////    jQuery("#form").append("<input type='checkbox' name='arrayTest' value='1' /><input type='checkbox' name='arrayTest' value='2' /><input type='checkbox' name='arrayTest' value='3' checked='checked' /><input type='checkbox' name='arrayTest' value='4' />");
////    var elements = jQuery("input[name=arrayTest]").val([ 1, 2 ]);
////   Assert.IsTrue( elements[0].checked, "First element was checked" );
////   Assert.IsTrue( elements[1].checked, "Second element was checked" );
////   Assert.IsTrue( !elements[2].checked, "Third element was unchecked" );
////   Assert.IsTrue( !elements[3].checked, "Fourth element remained unchecked" );

////    elements.remove();
////});

////test("val(Function) with incoming value", function() {
////    expect(10);

////    QUnit.reset();
////    var oldVal = jQuery("#text1").val();

////    jQuery("#text1").val(function(i, val) {
////        equals( val, oldVal, "Make sure the incoming value is correct." );
////        return "test";
////    });

//Assert.AreEqual(document.GetElementById("text1").value, "test", "Check for modified (via val(String)) value of input element" );

////    oldVal = jQuery("#text1").val();

////    jQuery("#text1").val(function(i, val) {
////        equals( val, oldVal, "Make sure the incoming value is correct." );
////        return 67;
////    });

//Assert.AreEqual(document.GetElementById("text1").value, "67", "Check for modified (via val(Number)) value of input element" );

////    oldVal = jQuery("#select1").val();

////    jQuery("#select1").val(function(i, val) {
////        equals( val, oldVal, "Make sure the incoming value is correct." );
////        return "3";
////    });

//Assert.AreEqual(jQuery("#select1").val(), "3", "Check for modified (via val(String)) value of select element" );

////    oldVal = jQuery("#select1").val();

////    jQuery("#select1").val(function(i, val) {
////        equals( val, oldVal, "Make sure the incoming value is correct." );
////        return 2;
////    });

//Assert.AreEqual(jQuery("#select1").val(), "2", "Check for modified (via val(Number)) value of select element" );

////    jQuery("#select1").append("<option value='4'>four</option>");

////    oldVal = jQuery("#select1").val();

////    jQuery("#select1").val(function(i, val) {
////        equals( val, oldVal, "Make sure the incoming value is correct." );
////        return 4;
////    });

//Assert.AreEqual(jQuery("#select1").val(), "4", "Should be possible to set the val() to a newly created option" );
////});

////// testing if a form.reset() breaks a subsequent call to a select element's .val() (in IE only)
////test("val(select) after form.reset() (Bug #2551)", function() {
////    expect(3);

////    jQuery("<form id='kk' name='kk'><select id='kkk'><option value='cf'>cf</option><option 	value='gf'>gf</option></select></form>").appendTo("#qunit-fixture");

////    jQuery("#kkk").val( "gf" );

////    document.kk.reset();

////    Assert.AreEqual( jQuery("#kkk")[0].value, "cf", "Check value of select after form reset." );
////    Assert.AreEqual( jQuery("#kkk").val(), "cf", "Check value of select after form reset." );

////    // re-verify the multi-select is not broken (after form.reset) by our fix for single-select
////    same( jQuery("#select3").val(), ["1", "2"], "Call val() on a multiple=\"multiple\" select" );

////    jQuery("#kk").remove();
////}); 

////var testAddClass = function(valueObj) {
////    expect(9);

////    var div = jQuery("div");
////    div.addClass( valueObj("test") );
////    var pass = true;
////    for ( var i = 0; i < div.size(); i++ ) {
////        if ( !~div.get(i).className.indexOf("test") ) {
////            pass = false;
////        }
////    }
////   Assert.IsTrue( pass, "Add Class" );

////    // using contents will get regular, text, and comment nodes
////    var j = jQuery("#nonnodes").contents();
////    j.addClass( valueObj("asdf") );
////   Assert.IsTrue( j.hasClass("asdf"), "Check node,textnode,comment for addClass" );

////    div = jQuery("<div/>");

////    div.addClass( valueObj("test") );
//Assert.AreEqual(div.Attr("class"), "test", "Make sure there's no extra whitespace." );

////    div.Attr("class", " foo");
////    div.addClass( valueObj("test") );
//Assert.AreEqual(div.Attr("class"), "foo test", "Make sure there's no extra whitespace." );

////    div.Attr("class", "foo");
////    div.addClass( valueObj("bar baz") );
//Assert.AreEqual(div.Attr("class"), "foo bar baz", "Make sure there isn't too much trimming." );
	
////    div.removeClass();
////    div.addClass( valueObj("foo") ).addClass( valueObj("foo") )
////    Assert.AreEqual( div.Attr("class"), "foo", "Do not add the same class twice in separate calls." );

////    div.addClass( valueObj("fo") );
////    Assert.AreEqual( div.Attr("class"), "foo fo", "Adding a similar class does not get interrupted." );
////    div.removeClass().addClass("wrap2");
////   Assert.IsTrue( div.addClass("wrap").hasClass("wrap"), "Can add similarly named classes");

////    div.removeClass();
////    div.addClass( valueObj("bar bar") );
////    Assert.AreEqual( div.Attr("class"), "bar", "Do not add the same class twice in the same call." );
////};

////test("addClass(String)", function() {
////    testAddClass(bareObj);
////});

////test("addClass(Function)", function() {
////    testAddClass(functionReturningObj);
////});

////test("addClass(Function) with incoming value", function() {
////    expect(45);
////    var div = jQuery("div"), old = div.map(function(){
////        return jQuery(this).Attr("class") || "";
////    });
	
////    div.addClass(function(i, val) {
////        if ( this.id !== "_firebugConsole") {
////            equals( val, old[i], "Make sure the incoming value is correct." );
////            return "test";
////        }
////    });

////    var pass = true;
////    for ( var i = 0; i < div.length; i++ ) {
////        if ( div.get(i).className.indexOf("test") == -1 ) pass = false;
////    }
////   Assert.IsTrue( pass, "Add Class" );
////});

////var testRemoveClass = function(valueObj) {
////    expect(7);

////    var $divs = jQuery("div");

////    $divs.addClass("test").removeClass( valueObj("test") );

////   Assert.IsTrue( !$divs.is(".test"), "Remove Class" );

////    QUnit.reset();
////    $divs = jQuery("div");

////    $divs.addClass("test").addClass("foo").addClass("bar");
////    $divs.removeClass( valueObj("test") ).removeClass( valueObj("bar") ).removeClass( valueObj("foo") );

////   Assert.IsTrue( !$divs.is(".test,.bar,.foo"), "Remove multiple classes" );

////    QUnit.reset();
////    $divs = jQuery("div");

////    // Make sure that a null value doesn't cause problems
////    $divs.eq(0).addClass("test").removeClass( valueObj(null) );
////   Assert.IsTrue( $divs.eq(0).is(".test"), "Null value passed to removeClass" );

////    $divs.eq(0).addClass("test").removeClass( valueObj("") );
////   Assert.IsTrue( $divs.eq(0).is(".test"), "Empty string passed to removeClass" );

////    // using contents will get regular, text, and comment nodes
////    var j = jQuery("#nonnodes").contents();
////    j.removeClass( valueObj("asdf") );
////   Assert.IsTrue( !j.hasClass("asdf"), "Check node,textnode,comment for removeClass" );

////    var div = document.createElement("div");
////    div.className = " test foo ";

////    jQuery(div).removeClass( valueObj("foo") );
//Assert.AreEqual(div.className, "test", "Make sure remaining className is trimmed." );

////    div.className = " test ";

////    jQuery(div).removeClass( valueObj("test") );
//Assert.AreEqual(div.className, "", "Make sure there is nothing left after everything is removed." );
////};

////test("removeClass(String) - simple", function() {
////    testRemoveClass(bareObj);
////});

////test("removeClass(Function) - simple", function() {
////    testRemoveClass(functionReturningObj);
////});

////test("removeClass(Function) with incoming value", function() {
////    expect(45);

////    var $divs = jQuery("div").addClass("test"), old = $divs.map(function(){
////        return jQuery(this).Attr("class");
////    });

////    $divs.removeClass(function(i, val) {
////        if ( this.id !== "_firebugConsole" ) {
////            equals( val, old[i], "Make sure the incoming value is correct." );
////            return "test";
////        }
////    });

////   Assert.IsTrue( !$divs.is(".test"), "Remove Class" );

////    QUnit.reset();
////});

////var testToggleClass = function(valueObj) {
////    expect(17);

////    var e = jQuery("#firstp");
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );
////    e.toggleClass( valueObj("test") );
////   Assert.IsTrue( e.is(".test"), "Assert class present" );
////    e.toggleClass( valueObj("test") );
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    // class name with a boolean
////    e.toggleClass( valueObj("test"), false );
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );
////    e.toggleClass( valueObj("test"), true );
////   Assert.IsTrue( e.is(".test"), "Assert class present" );
////    e.toggleClass( valueObj("test"), false );
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    // multiple class names
////    e.addClass("testA testB");
////   Assert.IsTrue( (e.is(".testA.testB")), "Assert 2 different classes present" );
////    e.toggleClass( valueObj("testB testC") );
////   Assert.IsTrue( (e.is(".testA.testC") && !e.is(".testB")), "Assert 1 class added, 1 class removed, and 1 class kept" );
////    e.toggleClass( valueObj("testA testC") );
////   Assert.IsTrue( (!e.is(".testA") && !e.is(".testB") && !e.is(".testC")), "Assert no class present" );

////    // toggleClass storage
////    e.toggleClass(true);
////   Assert.IsTrue( e[0].className === "", "Assert class is empty (data was empty)" );
////    e.addClass("testD testE");
////   Assert.IsTrue( e.is(".testD.testE"), "Assert class present" );
////    e.toggleClass();
////   Assert.IsTrue( !e.is(".testD.testE"), "Assert class not present" );
////   Assert.IsTrue( jQuery._data(e[0], "__className__") === "testD testE", "Assert data was stored" );
////    e.toggleClass();
////   Assert.IsTrue( e.is(".testD.testE"), "Assert class present (restored from data)" );
////    e.toggleClass(false);
////   Assert.IsTrue( !e.is(".testD.testE"), "Assert class not present" );
////    e.toggleClass(true);
////   Assert.IsTrue( e.is(".testD.testE"), "Assert class present (restored from data)" );
////    e.toggleClass();
////    e.toggleClass(false);
////    e.toggleClass();
////   Assert.IsTrue( e.is(".testD.testE"), "Assert class present (restored from data)" );

////    // Cleanup
////    e.removeClass("testD");
////    jQuery.removeData(e[0], "__className__", true);
////};

////test("toggleClass(String|boolean|undefined[, boolean])", function() {
////    testToggleClass(bareObj);
////});

////test("toggleClass(Function[, boolean])", function() {
////    testToggleClass(functionReturningObj);
////});

////test("toggleClass(Fucntion[, boolean]) with incoming value", function() {
////    expect(14);

////    var e = jQuery("#firstp"), old = e.Attr("class") || "";
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    e.toggleClass(function(i, val) {
////        equals( val, old, "Make sure the incoming value is correct." );
////        return "test";
////    });
////   Assert.IsTrue( e.is(".test"), "Assert class present" );

////    old = e.Attr("class");

////    e.toggleClass(function(i, val) {
////        equals( val, old, "Make sure the incoming value is correct." );
////        return "test";
////    });
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    old = e.Attr("class");

////    // class name with a boolean
////    e.toggleClass(function(i, val, state) {
////        equals( val, old, "Make sure the incoming value is correct." );
////        equals( state, false, "Make sure that the state is passed in." );
////        return "test";
////    }, false );
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    old = e.Attr("class");

////    e.toggleClass(function(i, val, state) {
////        equals( val, old, "Make sure the incoming value is correct." );
////        equals( state, true, "Make sure that the state is passed in." );
////        return "test";
////    }, true );
////   Assert.IsTrue( e.is(".test"), "Assert class present" );

////    old = e.Attr("class");

////    e.toggleClass(function(i, val, state) {
////        equals( val, old, "Make sure the incoming value is correct." );
////        equals( state, false, "Make sure that the state is passed in." );
////        return "test";
////    }, false );
////   Assert.IsTrue( !e.is(".test"), "Assert class not present" );

////    // Cleanup
////    e.removeClass("test");
////    jQuery.removeData(e[0], "__className__", true);
////});

////test("addClass, removeClass, hasClass", function() {
////    expect(17);

////    var jq = jQuery("<p>Hi</p>"), x = jq[0];

////    jq.addClass("hi");
//Assert.AreEqual(x.className, "hi", "Check single added class" );

////    jq.addClass("foo bar");
//Assert.AreEqual(x.className, "hi foo bar", "Check more added classes" );

////    jq.removeClass();
//Assert.AreEqual(x.className, "", "Remove all classes" );

////    jq.addClass("hi foo bar");
////    jq.removeClass("foo");
//Assert.AreEqual(x.className, "hi bar", "Check removal of one class" );

////   Assert.IsTrue( jq.hasClass("hi"), "Check has1" );
////   Assert.IsTrue( jq.hasClass("bar"), "Check has2" );

////    var jq = jQuery("<p class='class1\nclass2\tcla.ss3\n\rclass4'></p>");
////   Assert.IsTrue( jq.hasClass("class1"), "Check hasClass with line feed" );
////   Assert.IsTrue( jq.is(".class1"), "Check is with line feed" );
////   Assert.IsTrue( jq.hasClass("class2"), "Check hasClass with tab" );
////   Assert.IsTrue( jq.is(".class2"), "Check is with tab" );
////   Assert.IsTrue( jq.hasClass("cla.ss3"), "Check hasClass with dot" );
////   Assert.IsTrue( jq.hasClass("class4"), "Check hasClass with carriage return" );
////   Assert.IsTrue( jq.is(".class4"), "Check is with carriage return" );

////    jq.removeClass("class2");
////   Assert.IsTrue( jq.hasClass("class2")==false, "Check the class has been properly removed" );
////    jq.removeClass("cla");
////   Assert.IsTrue( jq.hasClass("cla.ss3"), "Check the dotted class has not been removed" );
////    jq.removeClass("cla.ss3");
////   Assert.IsTrue( jq.hasClass("cla.ss3")==false, "Check the dotted class has been removed" );
////    jq.removeClass("class4");
////   Assert.IsTrue( jq.hasClass("class4")==false, "Check the class has been properly removed" );
////});
    }
}
