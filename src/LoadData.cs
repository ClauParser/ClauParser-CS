using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;

namespace ClauParser_sharp
{
	class LoadData
	{
		private static long check_syntax_error1(long str, out int err)
		{
			err = 0;
			long len = Utility.GetLength(str);
			Utility.TYPE type = (Utility.TYPE)Utility.GetType(str);

			if (1 == len && (type == Utility.TYPE.LEFT || type == Utility.TYPE.RIGHT ||
				type == Utility.TYPE.ASSIGN))
			{
				err = -4; // exit()?
			}
			return str;
		}
		private static int Merge(UserType next, UserType ut, UserType ut_next)
		{
			//check!!
			while (ut.GetListSize() >= 1 && !ut.GetList(0).IsItemType()
				&& (((Type)(ut.GetList(0))).Name == "#"))
			{
				ut = (UserType)ut.GetList(0);
			}

			while (true)
			{
				UserType _ut = ut;
				UserType _next = next;


				if (ut_next != null && _ut == ut_next)
				{
					ut_next = _next;
				}

				for (int i = 0; i < _ut.GetListSize(); ++i)
				{
					if (!_ut.GetList(i).IsItemType())
					{
						if (((UserType)(_ut.GetList(i))).Name == "#")
						{
							((Type)_ut.GetList(i)).Name = "";
						}
						else
						{
							{
								_next.AddUserTypeItem((UserType)_ut.GetList(i));
								_ut.SetNull(i);
							}
						}
					}
					else
					{
						_next.AddItemType((ItemType)_ut.GetList(i));
						_ut.SetNull(i);
					}
				}

				_ut.Remove();

				ut = ut.GetParent();
				next = next.GetParent();


				if (next != null && ut != null)
				{
					//
				}
				else
				{
					// right_depth > left_depth
					if (next == null && ut != null)
					{
						return -1;
					}
					else if (next == null && ut != null)
					{
						return 1;
					}

					return 0;
				}
			}
		}

		class Parsing
		{
			string buffer;
			long[] token_arr;
			int token_start;
			int token_arr_len;
			int start_state;
			int last_state;
			public UserType _global;
			public UserType next;
			public int err;

			public Parsing(in string buffer, in long[] token_arr, in int token_start, in int token_arr_len,
			in int start_state, in int last_state)
			{
				this.buffer = buffer;
				this.token_arr = token_arr;
				this.token_arr_len = token_arr_len;
				this.start_state = start_state;
				this.last_state = last_state;
				this.token_start = token_start;
			}
			public void Run()
			{
				next = null;
				err = 0;

				List<long> varVec = new List<long>();
				List<long> valVec = new List<long>();


				if (token_arr_len <= 0)
				{
					next = null;
					_global = null;
					err = -1;
					return;
				}


				UserType global = new UserType();

				int state = start_state;
				int braceNum = 0;
				List<UserType> nestedUT = new List<UserType>();

				long var = 0, val = 0;

				nestedUT.Add(global);


				int count = 0;
				int x = 0;
				int x_next = 0;

				for (long i = 0; i < token_arr_len; ++i)
				{
					x = x_next;
					{
						x_next = x + 1;
					}
					if (count > 0)
					{
						count--;
						continue;
					}
					int len = Utility.GetLength(token_arr[token_start + i]);

					switch (state)
					{
						case 0:
							{
								// Left 1
								if (len == 1 && (-1 != Utility.Equal(Utility.TYPE.LEFT, Utility.GetType(token_arr[token_start + i]))))
								{
									if (varVec.Count != 0)
									{
										for (int idx = 0; idx < varVec.Count; ++idx)
										{
											nestedUT[braceNum].AddItemType(buffer, Utility.GetIdx(varVec[idx]), Utility.GetLength(varVec[idx]),
												Utility.GetIdx(valVec[idx]), Utility.GetLength(valVec[idx]));
										}

										varVec.Clear();
										valVec.Clear();
									}

									UserType temp = new UserType();

									nestedUT[braceNum].AddUserTypeItem(temp);
									UserType pTemp = (UserType)nestedUT[braceNum].GetLastType();

									braceNum++;

									/// new nestedUT
									if (nestedUT.Count == braceNum)
									{ /// changed 2014.01.23..
										nestedUT.Add(null);
									}

									/// initial new nestedUT.
									nestedUT[braceNum] = pTemp;
									///

									state = 0;
								}
								// Right 2
								else if (len == 1 && (-1 != Utility.Equal(Utility.TYPE.RIGHT, Utility.GetType(token_arr[token_start + i]))))
								{
									state = 0;

									if (varVec.Count > 0)
									{

										{
											for (int idx = 0; idx < varVec.Count; ++idx)
											{
												nestedUT[braceNum].AddItemType(buffer, Utility.GetIdx(varVec[idx]), Utility.GetLength(varVec[idx]),
													Utility.GetIdx(valVec[idx]), Utility.GetLength(valVec[idx]));
											}
										}

										varVec.Clear();
										valVec.Clear();
									}

									if (braceNum == 0)
									{
										UserType ut = new UserType();
										ut.AddUserTypeItem(new UserType("#")); // json . "var_name" = val  // clautext, # is line comment delimiter.
										UserType pTemp = (UserType)ut.GetLastType();

										int max = nestedUT[braceNum].GetListSize();
										for (int idx = 0; idx < max; ++idx)
										{
											if (!nestedUT[braceNum].GetList(idx).IsItemType())
											{
												((UserType)ut.GetList(0)).AddUserTypeItem((UserType)nestedUT[braceNum].GetList(idx));
											}
											else
											{
												((UserType)ut.GetList(0)).AddItemType(((ItemType)(nestedUT[braceNum].GetList(idx))));
											}
										}

										nestedUT[braceNum].Remove();
										nestedUT[braceNum].AddUserTypeItem(((UserType)ut.GetList(0)));

										braceNum++;
									}

									{
										if (braceNum < nestedUT.Count)
										{
											nestedUT[braceNum] = null;
										}
										braceNum--;
									}
								}
								else
								{
									if (x < token_arr_len - 1)
									{
										int _len = Utility.GetLength(token_arr[token_start + i + 1]);
										// EQ 3
										if (_len == 1 && -1 != Utility.Equal(Utility.TYPE.ASSIGN, Utility.GetType(token_arr[token_start + i + 1])))
										{
											var = token_arr[token_start + i];

											state = 1;

											{
												count = 1;
											}
										}
										else
										{
											// var1
											if (x <= token_arr_len - 1)
											{

												val = token_arr[token_start + i];

												varVec.Add(check_syntax_error1(var, out err));
												valVec.Add(check_syntax_error1(val, out err));

												val = 0;

												state = 0;

											}
										}
									}
									else
									{
										// var1
										if (x <= token_arr_len - 1)
										{
											val = token_arr[token_start + i];
											varVec.Add(check_syntax_error1(var, out err));
											valVec.Add(check_syntax_error1(val, out err));
											val = 0;

											state = 0;
										}
									}
								}
							}
							break;
						case 1:
							{
								// LEFT 1
								if (len == 1 && (-1 != Utility.Equal(Utility.TYPE.LEFT, Utility.GetType(token_arr[token_start + i]))))
								{

									for (int idx = 0; idx < varVec.Count; ++idx)
									{
										nestedUT[braceNum].AddItemType(buffer, Utility.GetIdx(varVec[idx]), Utility.GetLength(varVec[idx]),
											Utility.GetIdx(valVec[idx]), Utility.GetLength(valVec[idx]));
									}


									varVec.Clear();
									valVec.Clear();

									///
									{
										nestedUT[braceNum].AddUserTypeItem(new UserType(buffer.Substring(Utility.GetIdx(var), Utility.GetLength(var))));
										UserType pTemp = (UserType)nestedUT[braceNum].GetLastType();


										var = 0;
										braceNum++;

										/// new nestedUT
										if (nestedUT.Count == braceNum)
										{
											nestedUT.Add(null);
										}

										/// initial new nestedUT.
										nestedUT[braceNum] = pTemp;
									}
									///
									state = 0;
								}
								else
								{
									if (x <= token_arr_len - 1)
									{
										val = token_arr[token_start + i];

										varVec.Add(check_syntax_error1(var, out err));
										valVec.Add(check_syntax_error1(val, out err));
										var = 0; val = 0;

										state = 0;
									}
								}
							}
							break;
						default:
							// syntax err!!
							err = -1;
							_global = null;
							return; // throw "syntax error "
					}
				}

				next = nestedUT[braceNum];

				if (varVec.Count > 0)
				{
					for (int idx = 0; idx < varVec.Count; ++idx)
					{
						nestedUT[braceNum].AddItemType(buffer, Utility.GetIdx(varVec[idx]), Utility.GetLength(varVec[idx]),
							Utility.GetIdx(valVec[idx]), Utility.GetLength(valVec[idx]));
					}


					varVec.Clear();
					valVec.Clear();
				}

				if (state != last_state)
				{
					err = -2;
					_global = null;
					return;
					// throw std::string("error final state is not last_state!  : ") + toStr(state);
				}
				if (x > token_arr_len)
				{
					err = -3;
					_global = null;
					return;
					//throw std::string("error x > buffer + buffer_len: ");
				}
				_global = global;
				err = 0;
				return;
			}
		}


		static int FindDivisionPlace(string buffer, long[] token_arr, int start, int last)
		{
			for (int a = last; a >= start; --a)
			{
				int len = Utility.GetLength(token_arr[a]);
				Utility.TYPE val = Utility.GetType(token_arr[a]);


				if (len == 1 && (-1 != Utility.Equal(Utility.TYPE.RIGHT, val)))
				{ // right
					return a;
				}

				bool pass = false;
				if (len == 1 && (-1 != Utility.Equal(Utility.TYPE.LEFT, val)))
				{ // left
					return a;
				}
				else if (len == 1 && -1 != Utility.Equal(Utility.TYPE.ASSIGN, val))
				{ // assignment
				  //
					pass = true;
				}

				if (a < last && pass == false)
				{
					int len2 = Utility.GetLength(token_arr[a + 1]);
					Utility.TYPE val2 = Utility.GetType(token_arr[a + 1]);

					if (!(len2 == 1 && -1 != Utility.Equal(Utility.TYPE.ASSIGN, val2))) // assignment
					{ // NOT
						return a;
					}
				}
			}
			return -1;
		}

		static bool PreLoadData(InFileReserver reserver, out UserType global, int lex_thr_num, int parse_num) // first, strVec.empty() must be true!!
		{
			int pivot_num = parse_num - 1;
			string buffer = null;
			long[] token_arr = null;
			int buffer_total_len;
			int token_arr_len = 0;

			{
				bool success = reserver.Run(lex_thr_num, out buffer, out buffer_total_len, out token_arr, out token_arr_len);

				//	std::cout << b - a << "ms\n";

				//	{
				//		for (long i = 0; i < token_arr_len; ++i) {
				//			std::string(buffer + GetIdx(token_arr[i]), GetLength(token_arr[i]));
				//				if (0 == GetIdx(token_arr[i])) {
				//				std::cout << "chk";
				//			}
				//		}
				//	}

				if (!success)
				{
					global = null;
					return false;
				}
				if (token_arr_len <= 0)
				{
					global = null;
					return true;
				}
			}

			UserType before_next = null;
			UserType _global = new UserType();

			if (token_arr_len < 10000)
			{
				int err = 0;

				Parsing parsing = new Parsing(buffer, token_arr, 0, token_arr_len, 0, 0);

				parsing.Run();
				_global = parsing._global;
				err = parsing.err;


				{
					switch (err)
					{
						case 0:
							break;
						case -1:
						case -4:
							Console.WriteLine("Syntax Error");
							break;
						case -2:
							Console.WriteLine("error final state is not last_state!");
							break;
						case -3:
							Console.WriteLine("error x > buffer + buffer_len:");
							break;
						default:
							Console.WriteLine("unknown parser error");
							break;
					}
				}

				global = _global;

				return true;
			}

			{
				HashSet<int> _pivots = new HashSet<int>();
				List<int> pivots = new List<int>();
				int num = token_arr_len; //

				if (pivot_num > 0)
				{
					List<int> pivot = new List<int>();

					for (int i = 0; i < pivot_num; ++i)
					{
						pivot.Add(FindDivisionPlace(buffer, token_arr, (num / (pivot_num + 1)) * (i), (num / (pivot_num + 1)) * (i + 1) - 1));
					}

					for (int i = 0; i < pivot.Count; ++i)
					{
						if (pivot[i] != -1)
						{
							_pivots.Add(pivot[i]);
						}
					}

					foreach (int x in _pivots)
					{
						pivots.Add(x);
					}
				}

				UserType[] next = new UserType[pivots.Count + 1];

				{
					UserType[] __global = new UserType[pivots.Count + 1];
					Parsing[] parsing = new Parsing[pivots.Count + 1];
					Thread[] thr = new Thread[pivots.Count + 1];
					int[] err = new int[pivots.Count + 1];
					{
						int idx = pivots.Count == 0 ? num - 1 : pivots[0];
						int _token_arr_len = idx - 0 + 1;
						parsing[0] = new Parsing(buffer, token_arr, 0, _token_arr_len, 0, 0);
						parsing[0].err = 0;
						parsing[0].next = null;
						parsing[0]._global = null;

						thr[0] = new Thread(parsing[0].Run);
						thr[0].Start();
					}

					for (int i = 1; i < pivots.Count; ++i)
					{
						int _token_arr_len = pivots[i] - (pivots[i - 1] + 1) + 1;

						parsing[i] = new Parsing(buffer, token_arr, pivots[i - 1] + 1, _token_arr_len, 0, 0);

						thr[i] = new Thread(parsing[i].Run);
						thr[i].Start();
					}

					if (pivots.Count >= 1)
					{
						int _token_arr_len = num - 1 - (pivots[pivots.Count - 1] + 1) + 1;

						parsing[pivots.Count] = new Parsing(buffer, token_arr, pivots[pivots.Count - 1] + 1, _token_arr_len, 0, 0);

						thr[pivots.Count] = new Thread(parsing[pivots.Count].Run);
						thr[pivots.Count].Start();
					}

					// wait
					for (int i = 0; i < thr.Length; ++i)
					{
						thr[i].Join();
					}

					for (int i = 0; i < thr.Length; ++i)
					{
						__global[i] = parsing[i]._global;
						err[i] = parsing[i].err;
						next[i] = parsing[i].next;
					}

					for (int i = 0; i < err.Length; ++i)
					{
						switch (err[i])
						{
							case 0:
								break;
							case -1:
							case -4:
								Console.WriteLine("Syntax Error");
								break;
							case -2:
								Console.WriteLine("error final state is not last_state!");
								break;
							case -3:
								Console.WriteLine("error x > buffer + buffer_len:");
								break;
							default:
								Console.WriteLine("unknown parser error");
								break;
						}
					}

					// Merge
					if (__global[0].GetListSize() > 0 && ((Type)__global[0].GetList(0)).Name == "#")
					{
						Console.WriteLine("not valid file1");
						global = null;
						return false;
					}
					if (next[next.Length - 1].GetParent() != null)
					{
						Console.WriteLine("not valid file2");
						global = null;
						return false;
					}

					int err2 = Merge(_global, __global[0], next[0]);
					if (-1 == err2 || (pivots.Count == 0 && 1 == err2))
					{
						Console.WriteLine("not valid file3");
						global = null;
						return false;
					}

					for (int i = 1; i < pivots.Count + 1; ++i)
					{
						// linearly merge and error check...
						int err3 = Merge(next[i - 1], __global[i], next[i]);
						if (-1 == err3)
						{
							Console.WriteLine("not valid file4");
							global = null;
							return false;
						}
						else if (i == pivots.Count && 1 == err3)
						{
							Console.WriteLine("not valid file5");
							global = null;
							return false;
						}
					}


					before_next = next[next.Length - 1];
				}
			}

			global = _global;

			return true;

		}

		public static bool LoadDataFromFile(string fileName, out UserType global, int lex_thr_num, int parse_thr_num) /// global should be empty
		{
			if (lex_thr_num <= 0)
			{
				lex_thr_num = Process.GetCurrentProcess().Threads.Count;
			}
			if (lex_thr_num <= 0)
			{
				lex_thr_num = 1;
			}

			if (parse_thr_num <= 0)
			{
				parse_thr_num = Process.GetCurrentProcess().Threads.Count;
			}
			if (parse_thr_num <= 0)
			{
				parse_thr_num = 1;
			}

			UserType globalTemp;

			InFileReserver ifReserver = new InFileReserver(fileName);

			// cf) empty file..
			if (false == PreLoadData(ifReserver, out globalTemp, lex_thr_num, parse_thr_num))
			{
				global = null;
				return false; // return true?
			}

			global = globalTemp;

			return true;
		}
		public static bool LoadWizDB(out UserType global, string fileName, int thr_num)
		{
			UserType globalTemp = new UserType("global");

			// Scan + Parse 
			if (false == LoadDataFromFile(fileName, out globalTemp, thr_num, thr_num)) { global = null;  return false; }
			//std::cout << "LoadData End" << "\n";

			global = globalTemp;
			return true;
		}
		// SaveQuery
		public static bool SaveWizDB(UserType global, string fileName)
		{

			if (fileName.Length == 0) { return false; }

			using (StreamWriter outFile = new StreamWriter(fileName))
			{
				/// saveFile
				global.Save1(outFile); // cf) friend

			}
			return true;
		}
	}
}

