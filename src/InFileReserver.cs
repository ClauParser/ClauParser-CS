using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace ClauParser_sharp
{
	class InFileReserver
	{
		class _Scanning {
			private string text;
			private int num;
			private long length;
			private long[] token_arr;
			public long _token_arr_size;

			public _Scanning(string _text, in int _num, in long _length, long[] _token_arr)
			{
				text = _text;
				num = _num;
				length = _length;
				token_arr = _token_arr;
			}

			public void Run() {

				long token_arr_size = 0;

				{

					int token_first = 0;
					int token_last = -1;

					int token_arr_count = 0;

					for (int i = 0; i < length; ++i) {
						char ch = text[num + i];

						switch (ch) {
							case '\"':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}

								token_first = i;
								token_last = i;

								token_first = i + 1;
								token_last = i + 1;

								{//
									token_arr[num + token_arr_count] = 1;
									token_arr[num + token_arr_count] += Utility.Get(i + num, 1, ch);
									token_arr_count++;
								}
								break;
							case '\\':
								{//
									token_arr[num + token_arr_count] = 1;
									token_arr[num + token_arr_count] += Utility.Get(i + num, 1, ch);
									token_arr_count++;
								}
								break;
							case '\n':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i + 1;
								token_last = i + 1;

								{//
									token_arr[num + token_arr_count] = 1;
									token_arr[num + token_arr_count] += Utility.Get(i + num, 1, ch);
									token_arr_count++;
								}
								break;
							case '\0':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i + 1;
								token_last = i + 1;

								{//
									token_arr[num + token_arr_count] = 1;
									token_arr[num + token_arr_count] += Utility.Get(i + num, 1, ch);
									token_arr_count++;
								}
								break;
							case '#':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i + 1;
								token_last = i + 1;

								{//
									token_arr[num + token_arr_count] = 1;
									token_arr[num + token_arr_count] += Utility.Get(i + num, 1, ch);
									token_arr_count++;
								}

								break;
							case ' ':
							case '\t':
							case '\r':
							case '\v':
							case '\f':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i + 1;
								token_last = i + 1;

								break;
							case '{':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}

								token_first = i;
								token_last = i;

								token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
								token_arr_count++;

								token_first = i + 1;
								token_last = i + 1;
								break;
							case '}':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i;
								token_last = i;

								token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
								token_arr_count++;

								token_first = i + 1;
								token_last = i + 1;
								break;
							case '=':
								token_last = i - 1;
								if (token_last - token_first + 1 > 0) {
									token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
									token_arr_count++;
								}
								token_first = i;
								token_last = i;

								token_arr[num + token_arr_count] = Utility.Get(token_first + num, token_last - token_first + 1, text[num + token_first]);
								token_arr_count++;

								token_first = i + 1;
								token_last = i + 1;
								break;
						}
					}

					if (length - 1 - token_first + 1 > 0) {
						token_arr[num + token_arr_count] = Utility.Get(token_first + num, length - 1 - token_first + 1, text[num + token_first]);
						token_arr_count++;
					}
					token_arr_size = token_arr_count;
				}

				{
					_token_arr_size = token_arr_size;
				}
			}
		}

		private static void ScanningNew(string text, in int length, in int thr_num,
			out long[] _token_arr, out long _token_arr_size)
		{
			Thread[] thr = new Thread[thr_num];

			int[] start = new int[thr_num];
			int[] last = new int[thr_num];

			{
				start[0] = 0;

				for (int i = 1; i < thr_num; ++i)
				{
					start[i] = length / thr_num * i;

					for (int x = start[i]; x <= length; ++x)
					{
						if (Utility.IsWhitespace(text[x]) || '\0' == text[x] ||
							LoadDataOption.Left == text[x] || LoadDataOption.Right == text[x] || LoadDataOption.Assignment == text[x])
						{
							start[i] = x;
							break;
						}
					}
				}
				for (int i = 0; i < thr_num - 1; ++i)
				{
					last[i] = start[i + 1];
					for (int x = last[i]; x <= length; ++x)
					{
						if (Utility.IsWhitespace(text[x]) || '\0' == text[x] ||
							LoadDataOption.Left == text[x] || LoadDataOption.Right == text[x] || LoadDataOption.Assignment == text[x])
						{
							last[i] = x;
							break;
						}
					}
				}
				last[thr_num - 1] = length;
			}
			long real_token_arr_count = 0;

			long[] tokens = new long[length + 1];
			long token_count = 0;

			long[] token_arr_size = new long[thr_num];

			_Scanning[] first_scanning = new _Scanning[thr_num];

			for (int i = 0; i < thr_num; ++i)
			{
				first_scanning[i] = new _Scanning(text, start[i], last[i] - start[i], tokens);
				thr[i] = new Thread(first_scanning[i].Run);
				thr[i].Start();
			}

			for (int i = 0; i < thr_num; ++i)
			{
				thr[i].Join();
			}

			for (int i = 0; i < thr_num; ++i)
			{
				token_arr_size[i] = first_scanning[i]._token_arr_size;
			}

			{
				for (int i = 0; i < thr_num; ++i)
				{
					for (long j = 0; j < token_arr_size[i]; ++j)
					{
						tokens[token_count] = tokens[start[i] + j];
						token_count++;
					}
				}
			}

			int state = 0;
			long qouted_start = -1;
			long slush_start = -1;

			for (int i = 0; i < token_count; ++i)
			{
				long len = Utility.GetLength(tokens[i]);
				char ch = text[Utility.GetIdx(tokens[i])];
				long idx = Utility.GetIdx(tokens[i]);
				bool isToken2 = Utility.IsToken2(tokens[i]);

				if (isToken2)
				{
					if (0 == state && '\"' == ch)
					{
						state = 1;
						qouted_start = i;
					}
					else if (0 == state && LoadDataOption.LineComment == ch)
					{
						state = 2;
					}
					else if (1 == state && '\\' == ch)
					{
						state = 3;
						slush_start = idx;
					}
					else if (1 == state && '\"' == ch)
					{
						state = 0;

						{
							int _idx = Utility.GetIdx(tokens[qouted_start]);
							long _len = Utility.GetLength(tokens[qouted_start]);

							_len = Utility.GetIdx(tokens[i]) - _idx + 1;

							tokens[real_token_arr_count] = Utility.Get(_idx, _len, text[_idx]);
							real_token_arr_count++;
						}
					}
					else if (3 == state)
					{
						if (idx != slush_start + 1)
						{
							--i;
						}
						state = 1;
					}
					else if (2 == state && ('\n' == ch))
					{
						state = 0;
					}
				}
				else if (0 == state && !('\n' == ch))
				{ // '\\' case?
					tokens[real_token_arr_count] = tokens[i];
					real_token_arr_count++;
				}
			}

			if (2 == state)
			{
				state = 0;
			}

			{
				if (0 != state)
				{
					Console.Out.WriteLine("[ERRROR] state [", state, "] is not zero \n");
				}
			}


			{
				_token_arr = tokens;
				_token_arr_size = real_token_arr_count;
			}
		}


		private static void Scanning(string text, in long length,
			out long[] _token_arr, out long _token_arr_size)
		{

			long[] token_arr = new long[length + 1];
			long token_arr_size = 0;

			{
				int state = 0;

				int token_first = 0;
				int token_last = -1;

				int token_arr_count = 0;

				for (int i = 0; i <= length; ++i)
				{
					char ch = text[i];

					if (0 == state)
					{
						if (LoadDataOption.LineComment == ch)
						{
							token_last = i - 1;
							if (token_last - token_first + 1 > 0)
							{
								token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
								token_arr_count++;
							}

							state = 3;
						}
						else if ('\"' == ch)
						{
							state = 1;
						}
						else if (Utility.IsWhitespace(ch) || '\0' == ch)
						{
							token_last = i - 1;
							if (token_last - token_first + 1 > 0)
							{
								token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
								token_arr_count++;
							}
							token_first = i + 1;
							token_last = i + 1;
						}
						else if (LoadDataOption.Left == ch)
						{
							token_last = i - 1;
							if (token_last - token_first + 1 > 0)
							{
								token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
								token_arr_count++;
							}

							token_first = i;
							token_last = i;

							token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
							token_arr_count++;

							token_first = i + 1;
							token_last = i + 1;
						}
						else if (LoadDataOption.Right == ch)
						{
							token_last = i - 1;
							if (token_last - token_first + 1 > 0)
							{
								token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
								token_arr_count++;
							}
							token_first = i;
							token_last = i;

							token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
							token_arr_count++;

							token_first = i + 1;
							token_last = i + 1;

						}
						else if (LoadDataOption.Assignment == ch)
						{
							token_last = i - 1;
							if (token_last - token_first + 1 > 0)
							{
								token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
								token_arr_count++;
							}
							token_first = i;
							token_last = i;

							token_arr[token_arr_count] = Utility.Get(token_first, token_last - token_first + 1, text[token_first]);
							token_arr_count++;

							token_first = i + 1;
							token_last = i + 1;
						}
					}
					else if (1 == state)
					{
						if ('\\' == ch)
						{
							state = 2;
						}
						else if ('\"' == ch)
						{
							state = 0;
						}
					}
					else if (2 == state)
					{
						state = 1;
					}
					else if (3 == state)
					{
						if ('\n' == ch || '\0' == ch)
						{
							state = 0;

							token_first = i + 1;
							token_last = i + 1;
						}
					}
				}

				token_arr_size = token_arr_count;
				if (state == 3)
				{
					state = 0;
				}

				if (0 != state)
				{
					Console.Out.WriteLine("[", state, "] state is not zero.\n");
				}
			}

			{
				_token_arr = token_arr;
				_token_arr_size = token_arr_size;
			}
		}


		static KeyValuePair<bool, int> Scan(in String fileName, int thr_num,
			out string _buffer, out long _buffer_len, out long[] _token_arr, out long _token_arr_len)
		{
			if (!File.Exists(fileName))
			{
				_buffer = null;
				_buffer_len = 0;
				_token_arr = null;
				_token_arr_len = 0;
				return new KeyValuePair<bool, int>(false, 0);
			}

			string buffer;
			int file_length = 0;

			{
				buffer = File.ReadAllText(fileName);
				file_length = buffer.Length;

				{
					//int a = clock();
					long[] token_arr;
					long token_arr_size;

					if (thr_num == 1)
					{
						Scanning(buffer, file_length, out token_arr, out token_arr_size);
					}
					else
					{
						ScanningNew(buffer, file_length, thr_num, out token_arr, out token_arr_size);
					}

					//int b = clock();
					//	cout << b - a << "ms\n";
					_buffer = buffer;
					_token_arr = token_arr;
					_token_arr_len = token_arr_size;
					_buffer_len = file_length;
				}
			}

			return new KeyValuePair<bool, int>(true, 1);
		}

		private String file;
		public InFileReserver(String _fileName)
		{
			file = _fileName;
		}
		public bool Run(int thr_num, out string buffer, out long buffer_len, out long[] token_arr, out long token_arr_len)
		{
			bool x = Scan(file, thr_num, out buffer, out buffer_len, out token_arr, out token_arr_len).Value > 0;
			return x;
		}
	};
}

