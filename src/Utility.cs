using System;
using System.Collections.Generic;
using System.Text;

namespace ClauParser_sharp
{
    class Utility
    {
        public enum TYPE
        {
            LEFT = 1, // 01
            RIGHT = 2, // 10
            ASSIGN = 3, // 11
            NOTHING
        };
        public static bool IsWhitespace(in char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                case '\v':
                case '\f':
                    return true;
            }
            return false;
        }
    
        public static int Equal(in long x, in long y)
        {
            if (x == y)
            {
                return 0;
            }
            return -1;
        }
        public static int Equal(in Utility.TYPE x, in Utility.TYPE y)
        {
            if (x == y)
            {
                return 0;
            }
            return -1;
        }
        public static long Get(in long position, in long length, in char ch) {
			long x = (position << 32) + (length << 3) + 0;

			if (length != 1) {
				return x;
			}

			if (LoadDataOption.Left == ch) {
				x += 2; // 010
			}
			else if (LoadDataOption.Right == ch) {
				x += 4; // 100
			}
			else if (LoadDataOption.Assignment == ch) {
				x += 6;
			}

			return x;
		}

        public static int GetIdx(in long x)
        {
            return (int) ((x >> 32) & 0xFFFFFFFF);
        }
        public static int GetLength(in long x)
        {
            return (int) ((x & 0xFFFFFFF8) >> 3);
        }
        public static TYPE GetType(in long x)
        {
            int val =  (int)((x & 6) >> 1);
            switch (val)
            {
                case 1:
                    return TYPE.LEFT;
                case 2:
                    return TYPE.RIGHT;
                case 3:
                    return TYPE.ASSIGN;
            }
            return TYPE.NOTHING;
        }
        public static bool IsToken2(in long x)
        {
            return (x & 1) == 1;
        }
    }
}
