using System;
using System.Collections.Generic;
using System.Text;

namespace ClauParser_sharp
{
    class Type
    {
        public string Name
        {
            set;
            get;
        }

        public Type ()
        {
            Name = new string("");
        }

        public virtual bool IsItemType()
        {
            return false;
        }
        public virtual bool IsUserType()
        {
            return false;
        }
    }
}
