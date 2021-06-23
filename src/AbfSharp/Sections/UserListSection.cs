using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class UserListSection : Section
    {
        public UserListSection(BinaryReader reader) : base(reader, 172)
        {
        }
    }
}
