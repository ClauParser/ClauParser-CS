using System;
using System.Collections.Generic;
using System.Text;

namespace ClauParser_sharp
{
    class ItemType : Type
    {
        public string Data
        {
            set;
            get;
        }

        public ItemType()
        {
            //
        }

        public ItemType(string name, string data)
        {
            this.Name = name;
            this.Data = data;
        }

        public override bool IsItemType()
        {
            return true;
        }
        public override bool IsUserType()
        {
            return false;
        }
    }
}
