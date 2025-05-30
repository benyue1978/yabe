﻿/**************************************************************************
*                           MIT License
* 
* Copyright (C) 2015 Frederic Chaxel <fchaxel@free.fr>
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
* the following conditions:
*
* The above copyright notice and this permission notice shall be included
* in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.BACnet;

namespace BaCSharp
{
    public class AnalogInput<T> : AnalogObject<T>
    {
        public uint m_PROP_RELIABILITY;
        [BaCSharpType(BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED)]
        public virtual uint PROP_RELIABILITY
        {
            get { return  m_PROP_RELIABILITY; }
        }

        public AnalogInput(int ObjId, String ObjName, String Description, T InitialValue, BacnetUnitsId Unit)
            : base(new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, (uint)ObjId), ObjName, Description, InitialValue, Unit)
        {
            m_PRESENT_VALUE_ReadOnly = true;
        }
        public AnalogInput(BacnetObjectId ObjId, String ObjName, String Description, T InitialValue, BacnetUnitsId Unit)
            : base(ObjId, ObjName, Description, InitialValue, Unit)
        {
            m_PRESENT_VALUE_ReadOnly = true;
        }
        public AnalogInput() { }
    }
}
