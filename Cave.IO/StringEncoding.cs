#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;

namespace Cave.IO
{
    /// <summary>
    /// Provides supported string encodings
    /// </summary>
    public enum StringEncoding
    {
        /// <summary>Character set not defined</summary>
        Undefined = 0,

        /// <summary>
        /// 7 Bit per Character
        /// </summary>
        ASCII = 1,

        /// <summary>
        /// 8 Bit per Character Unicode
        /// </summary>
        UTF8 = 2,

        /// <summary>
        /// Little Ending 16 Bit per Character Unicode
        /// </summary>
        UTF16 = 3,

        /// <summary>
        /// Little Ending 32 Bit per Character Unicode
        /// </summary>
        UTF32 = 4,

        /// <summary>ISO/IEC 8859-2:1999, Information technology — 8-bit single-byte coded graphic character sets — Part 2: Latin alphabet No. 1</summary>
        [Obsolete]
        ISO_8859_1 = 88591,

        /// <summary>ISO/IEC 8859-2:1999, Information technology — 8-bit single-byte coded graphic character sets — Part 2: Latin alphabet No. 2</summary>
        [Obsolete]
        ISO_8859_2 = 88592,
	}
}
