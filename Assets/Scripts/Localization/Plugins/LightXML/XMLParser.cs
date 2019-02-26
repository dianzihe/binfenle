using UnityEngine;
using System.Collections;

public class XMLParser 
{
    /*
     * UnityScript Lightweight XML Parser
     * by Fraser McCormick (unityscripts@roguishness.com)
     * http://twitter.com/flimgoblin
     * http://www.roguishness.com/unity/
     *
     * You may use this script under the terms of either the MIT License 
     * or the Gnu Lesser General Public License (LGPL) Version 3. 
     * See:
     * http://www.roguishness.com/unity/lgpl-3.0ff-standalone.html
     * http://www.roguishness.com/unity/gpl-3.0ff-standalone.html
     * or
     * http://www.roguishness.com/unity/MIT-license.txt
     */




    /* Usage:
     * parser=new XMLParser();
     * FIXME_VAR_TYPE node=parser.Parse("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
     * 
     * Nodes are Boo.Lang.Hash values with text content in "_text" field, other attributes
     * in "@attribute" and any child nodes listed in an array of their nodename.
     * 
     * any XML meta tags <? .. ?> are ignored as are comments <!-- ... -->
     * any CDATA is bundled into the "_text" attribute of its containing node.
     *
     * e.g. the above XML is parsed to:
     * node={ "example": 
     *			[ 
     *			   { "_text":"", 
     *				  "value": [ { "_text":"Foobar", "@type":"String"}, {"_text":"3", "@type":"Int"}]
     *			   } 
     *			],
     *		  "_text":""
     *     }
     *		  
     */

   

        private char LT = "<"[0];
        private char GT = ">"[0];
        private char SPACE = " "[0];
        private char QUOTE = "\""[0];
        private char SLASH = "/"[0];
        private char QMARK = "?"[0];
        private char EQUALS = "="[0];
        private char EXCLAMATION = "!"[0];
        private char DASH = "-"[0];
        //private char SQL="["[0];
        private char SQR = "]"[0];

        public object Parse(string content)
        {

            XMLNode rootNode = new XMLNode();
            rootNode["_text"] = "";

            //string nodeContents = "";

            bool inElement = false;
            bool collectNodeName = false;
            bool collectAttributeName = false;
            bool collectAttributeValue = false;
            bool quoted = false;
            string attName = "";
            string attValue = "";
            string nodeName = "";
            string textValue = "";

            bool inMetaTag = false;
            bool inComment = false;
            bool inCDATA = false;

            XMLNodeList parents = new XMLNodeList();

            XMLNode currentNode = rootNode;
            for (int i = 0; i < content.Length; i++)
            {

                char c = content[i];
                char cn = 'x';
                char cnn = 'x';
                char cp = 'x';
                if ((i + 1) < content.Length) cn = content[i + 1];
                if ((i + 2) < content.Length) cnn = content[i + 2];
                if (i > 0) cp = content[i - 1];

                if (inMetaTag)
                {
                    if (c == QMARK && cn == GT)
                    {
                        inMetaTag = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LT && cn == QMARK)
                    {
                        inMetaTag = true;
                        continue;
                    }
                }

                if (inComment)
                {
                    if (cp == DASH && c == DASH && cn == GT)
                    {
                        inComment = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LT && cn == EXCLAMATION)
                    {

                        if (content.Length > i + 9 && content.Substring(i, 9) == "<![CDATA[")
                        {
                            inCDATA = true;
                            i += 8;
                        }
                        else
                        {
                            inComment = true;
                        }
                        continue;
                    }
                }

                if (inCDATA)
                {
                    if (c == SQR && cn == SQR && cnn == GT)
                    {
                        inCDATA = false;
                        i += 2;
                        continue;
                    }
                    textValue += c;
                    continue;
                }


                if (inElement)
                {
                    if (collectNodeName)
                    {
                        if (c == SPACE)
                        {
                            collectNodeName = false;
                        }
                        else if (c == GT)
                        {
                            collectNodeName = false;
                            inElement = false;
                        }



                        if (!collectNodeName && nodeName.Length > 0)
                        {
                            if (nodeName[0] == SLASH)
                            {
                                // close tag
                                if (textValue.Length > 0)
                                {
                                    currentNode["_text"] += textValue;
                                }

                                textValue = "";
                                nodeName = "";
                                currentNode = (XMLNode)parents[(int)(parents.Count - 1)];
                                parents.RemoveAt((parents.Count-1));
                            }
                            else
                            {

                                if (textValue.Length > 0)
                                {
                                    currentNode["_text"] += textValue;
                                }
                                textValue = "";
                                XMLNode newNode = new XMLNode();
                                newNode["_text"] = "";
                                newNode["_name"] = nodeName;

                                if (currentNode[nodeName]==null)
                                {
                                    currentNode[nodeName] = new XMLNodeList();
                                }
                                ArrayList a = (ArrayList)currentNode[nodeName];
                                a.Add(newNode);
                                parents.Add(currentNode);
                                currentNode = newNode;
                                nodeName = "";
                            }
                        }
                        else
                        {
                            nodeName += c;
                        }
                    }
                    else
                    {

                        if (!quoted && c == SLASH && cn == GT)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName!=null)
                            {
                                if (attValue!=null)
                                {
                                    currentNode["@" + attName] = attValue;
                                }
                                else
                                {
                                    currentNode["@" + attName] = true;
                                }
                            }

                            i++;
                            currentNode = (XMLNode)parents[(int)(parents.Count - 1)];
                            parents.RemoveAt((parents.Count - 1));
                            attName = "";
                            attValue = "";
                        }
                        else if (!quoted && c == GT)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName!=null)
                            {
                                currentNode["@" + attName] = attValue;
                            }

                            attName = "";
                            attValue = "";
                        }
                        else
                        {
                            if (collectAttributeName)
                            {
                                if (c == SPACE || c == EQUALS)
                                {
                                    collectAttributeName = false;
                                    collectAttributeValue = true;
                                }
                                else
                                {
                                    attName += c;
                                }
                            }
                            else if (collectAttributeValue)
                            {
                                if (c == QUOTE)
                                {
                                    if (quoted)
                                    {
                                        collectAttributeValue = false;
                                        currentNode["@" + attName] = attValue;
                                        attValue = "";
                                        attName = "";
                                        quoted = false;
                                    }
                                    else
                                    {
                                        quoted = true;
                                    }
                                }
                                else
                                {
                                    if (quoted)
                                    {
                                        attValue += c;
                                    }
                                    else
                                    {
                                        if (c == SPACE)
                                        {
                                            collectAttributeValue = false;
                                            currentNode["@" + attName] = attValue;
                                            attValue = "";
                                            attName = "";
                                        }
                                    }
                                }
                            }
                            else if (c == SPACE)
                            {

                            }
                            else
                            {
                                collectAttributeName = true;
                                attName = "" + c;
                                attValue = "";
                                quoted = false;
                            }
                        }
                    }
                }
                else
                {
                    if (c == LT)
                    {
                        inElement = true;
                        collectNodeName = true;
                    }
                    else
                    {
                        textValue += c;
                    }

                }

            }

            return rootNode;
        }

    }

