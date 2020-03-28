// ClauParserLib.cpp : 정적 라이브러리를 위한 함수를 정의합니다.
//

// main function.
// query style.- function param_num(0,1,2,..) param0 param1 param2 ...
// 
// LOAD //
// NEXT //
// GET_ //
// DEL_ // delete
//
//

#include "pch.h"
#include "framework.h"
#include "ClauParserLib.h"
#include "clau_parser.h"

class wrap
{
public:
	wiz::UserType* first = nullptr;
	long long second = -1; // ilist idx
	long long third = -1; // itemList idx
	long long forth = -1; // userTypeList idx
public:
	wrap() { }
	wrap(wiz::UserType* first, long long second, long long third, long long forth) 
		: first(first), second(second), third(third), forth(forth)
	{	}
};

static std::string result;
static wiz::UserType global;
static std::vector<wrap> nested_pos;


const char* fnClauParserLib(Query query_id, const char* query_data, int* err)
{
	*err = 0; // default.
	result.clear();

	if (query_id == LOAD) {
		//if (0 == strncmp(query, "LOAD", 4)) {
		global = wiz::UserType();
		nested_pos = std::vector<wrap>();
		bool success = wiz::LoadData::LoadDataFromFile(query_data, global, 0, 0);
		if (!success) {
			*err = -2;

			return "LOAD_FAIL";
		}
		nested_pos.push_back({ &global, 0, 0, 0 });
	}
	else if (query_id == GET_NAME) {
		//else if (0 == strncmp(query, "GET_", 4)) {
		auto x = nested_pos.back();

		if (x.first->GetIListSize() <= x.second) {
			*err = -3;
			return "END_DATA";
		}

		//if (0 == strncmp(select, "NAME", 4)) {
		if (x.first->IsItemList(x.second)) {
			result = x.first->GetItemList(x.third).GetName();
		}
		else {
			result = x.first->GetUserTypeList(x.forth)->GetName();
		}
		//}
	}
	else if (query_id == GET_VALUE) {
		auto x = nested_pos.back();

		if (x.first->GetIListSize() <= x.second) {
			*err = -4;
			return "END_DATA";
		}

		//else if (0 == strncmp(select, "VALUE", 4)) {
		if (x.first->IsItemList(x.second)) {
			result = x.first->GetItemList(x.third).Get();
		}
		else {
			result = "";
		}
		//}
	}
	else if (query_id == NEXT) {
		//else if (0 == strncmp(query, "NEXT", 4)) {
		if (nested_pos.back().second + 1 < nested_pos.back().first->GetIListSize()) {
			nested_pos.back().second++;

			if (nested_pos.back().first->IsItemList(nested_pos.back().second - 1)) {
				nested_pos.back().third++;
			}
			else {
				nested_pos.back().forth++;
			}
			
			return "NEXT SUCCESS";
		}
		else {
			*err = -5;
			return "NEXT FAIL";
		}
	}
	else if (query_id == CHILD) {
		// DOWN		
		if (nested_pos.back().second >= 0 && nested_pos.back().second < nested_pos.back().first->GetIListSize()
			&& nested_pos.back().first->IsUserTypeList(nested_pos.back().second)) {
			nested_pos.push_back({ nested_pos.back().first->GetUserTypeList(nested_pos.back().forth), 0, 0, 0 });
			return "CHILD SUCCESS";
		}
		else {
			*err = -6;
			return "CHILD FAIL";

		}
	}
	else if (query_id == PARENT) {
		// UP
		if (nested_pos.back().first->GetIListSize() <= nested_pos.back().second + 1) {
			nested_pos.pop_back();
			if (nested_pos.empty()) {
				*err = -7;
				return "PARENT FAIL";
			}
		}

		return "PARENT SUCCESS";
	}
	else if (query_id == DEL) {
		//else if (0 == strncmp(query, "DEL_", 4)) {
		global = wiz::UserType();
		nested_pos = std::vector<wrap>();
		return "DEL_ SUCCESS";
	}


	return result.c_str();
}

