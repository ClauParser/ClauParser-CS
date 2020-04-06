using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClauParser_sharp
{
    class UserType : Type
    {
        private UserType parent = null;
        private List<Type> typeList = null;


        public UserType()
        {
            typeList = new List<Type>();
        }
        public UserType(string name)
        {
            this.Name = name;
            typeList = new List<Type>();
        }
        public UserType GetParent()
        {
            return parent;
        }
        public void SetParent(UserType ut)
        {
            parent = ut;
        }
        public Type GetList(int idx)
        {
            return typeList[idx];
        }
        public int GetListSize()
        {
            return typeList.Count;
        }
        public void SetNull(int idx)
        {
            typeList[idx] = null;
        }

        public void Remove()
        {
            typeList.Clear();
        }
        public void AddItemType(ItemType it)
        {
            typeList.Add(it);
        }
        public void AddItemType(string buffer, int position1, int length1,
            int position2, int length2)
        {
            string name = buffer.Substring(position1, length1);
            string data = buffer.Substring(position2, length2);

            typeList.Add(new ItemType(name, data));
        }

        public void AddUserTypeItem(UserType ut)
        {
            typeList.Add(ut);
            ut.parent = this;
        }
        public Type GetLastType()
        {
            if (typeList.Count >= 1)
            {
                return typeList[typeList.Count - 1];
            }
            return null;
        }
        public override bool IsItemType() 
        {
            return false;
        }
        public override bool IsUserType()
        {
            return true;
        }

        private void Save1(StreamWriter stream, in UserType ut, in int depth = 0)
        {

            for (int i = 0; i < ut.GetListSize(); ++i)
            {
                //std::cout << "ItemList" << endl;
                if (ut.GetList(i).IsItemType())
                {
                    String temp = new String("");

                    for (int k = 0; k < depth; ++k)
                    {
                        temp += "\t";
                    }

                    if (ut.GetList(i).Name.Length > 0)
                    {
                        temp += ut.GetList(i).Name;
                        temp += " = ";
                    }
                    temp += ((ItemType)(ut.GetList(i))).Data;
                    if (i != ut.GetListSize() - 1)
                    {
                        temp += " ";
                    }
                    stream.Write(temp);
                }
                else
                {
                    stream.Write("\n");
                    // std::cout << "UserTypeList" << endl;
                    for (int k = 0; k < depth; ++k)
                    {
                        stream.Write("\t");
                    }

                    if (ut.GetList(i).Name.Length > 0)
                    {
                        stream.Write(ut.GetList(i).Name + " = ");
                    }

                    stream.Write("{\n");

                    Save1(stream, (UserType)(ut.GetList(i)), depth + 1);
                    stream.Write("\n");

                    for (int k = 0; k < depth; ++k)
                    {
                        stream.Write("\t");
                    }
                    stream.Write("}");
                    if (i != ut.GetListSize() - 1)
                    {
                        stream.Write("\n");
                    }
                }
            }
        }
        /// save2 - for more speed loading data!?
        public void Save1(StreamWriter stream, int depth = 0)
        {
            Save1(stream, this, depth);
        }
    }
}
