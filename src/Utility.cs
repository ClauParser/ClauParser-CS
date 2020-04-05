using System;
using System.Collections.Generic;
using System.Text;

namespace ClauParser_sharp
{
    class Utility
    {
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
        public static long GetLength(in long x)
        {
            return (x & 0xFFFFFFF8) >> 3;
        }
        public static long GetType(in long x)
        {
            return (x & 6) >> 1;
        }
        public static bool IsToken2(in long x)
        {
            return (x & 1) == 1;
        }
    }
}
