//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xaml;

namespace Microsoft.Samples.Activities.Designer.ViewStateCleaner
{
    public class ViewStateCleaningWriter : XamlWriter
    {
        public ViewStateCleaningWriter(XamlWriter innerWriter)
        {
            this.InnerWriter = innerWriter;
            this.MemberStack = new Stack<XamlMember>();
        }

        XamlWriter InnerWriter {get; set; }
        Stack<XamlMember> MemberStack {get; set; }
     
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (InnerWriter != null)
                {
                    ((IDisposable)InnerWriter).Dispose();
                    InnerWriter = null;
                }

                MemberStack.Clear();
            }

            base.Dispose(disposing);
        }

        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return InnerWriter.SchemaContext;
            }
        }

        public override void WriteEndMember()
        {
            XamlMember xamlMember = MemberStack.Pop();
            if (m_attachedPropertyDepth > 0)
            {
                if (IsDesignerAttachedProperty(xamlMember))
                {
                    m_attachedPropertyDepth--;
                }
                return;
            }

            InnerWriter.WriteEndMember();
        }

        public override void WriteEndObject()
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            } 
            
            InnerWriter.WriteEndObject();
        }

        public override void WriteGetObject()
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteGetObject();
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteNamespace(namespaceDeclaration);
        }

        public override void WriteStartMember(XamlMember xamlMember)
        {
            MemberStack.Push(xamlMember);
            if (IsDesignerAttachedProperty(xamlMember))
            {
                m_attachedPropertyDepth++;
            }

            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteStartMember(xamlMember);
        }

        public override void WriteStartObject(XamlType type)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteStartObject(type);
        }

        public override void WriteValue(Object value)
        {
            if (m_attachedPropertyDepth > 0)
            {
                return;
            }

            InnerWriter.WriteValue(value);
        }

        static Boolean IsDesignerAttachedProperty(XamlMember xamlMember)
        {
            return xamlMember.IsAttachable &&
                   xamlMember.PreferredXamlNamespace.Equals(c_sapNamespaceURI, StringComparison.OrdinalIgnoreCase);
        }

        private Int32 m_attachedPropertyDepth = 0;
        const String c_sapNamespaceURI = "http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation";
    }
}