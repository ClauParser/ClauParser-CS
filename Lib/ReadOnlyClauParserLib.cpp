// ReadOnlyCaluParserLib.cpp : 정적 라이브러리를 위한 함수를 정의합니다.
//

#include "pch.h"
#include "framework.h"

// TODO: 라이브러리 함수의 예제입니다.
#include "readOnly_clau_parser.h"

namespace wiz {
	Node* MemoryPool::Get() {
		if (size > 0 && count < size) {
			count++;
			return &arr[count - 1];
		}
		else {
			// in real depth?  <= 10 ?
			// static Node[10] and list<Node*> ?
			count++; // for number check.
			else_list.push_back(new Node());
			return else_list.back();
		}
	}
	MemoryPool::~MemoryPool() {
		//
	}

	void MemoryPool::Clear() { // maybe just one called....
		if (arr) {
			delete[] arr;
			arr = nullptr;
		}
		for (Node* x : else_list) {
			delete x;
		}
	}
}


static std::string result;
static wiz::Node global;
static std::vector<wiz::Node*> nested_pos;
static bool firstTime = false;
static char* buffer = nullptr;
static std::vector<wiz::MemoryPool> pool;

const char* fnClauParserLib(int query_id, const char* query_data, int* err)
{
	*err = 0; // default.
	result.clear();

	if (query_id == 0) {
		//if (0 == strncmp(query, "LOAD", 4)) {
		global = wiz::Node();
		nested_pos = std::vector<wiz::Node*>();
		bool success = wiz::LoadData::LoadDataFromFile(query_data, &global, &buffer, &pool, 0, 0);
		if (!success) {
			*err = -2;

			return "LOAD_FAIL";
		}
		nested_pos.push_back(global.child);
		firstTime = true;
	}
	else if (query_id == 1) {
		//else if (0 == strncmp(query, "GET_", 4)) {
		auto x = nested_pos.back();

		if (nullptr == x) {
			*err = -3;
			return "END_DATA";
		}

		//if (0 == strncmp(select, "NAME", 4)) {
		result = std::string(buffer + wiz::Utility::GetIdx(x->name), wiz::Utility::GetLength(x->name));
		//}
	}
	else if (query_id == 2) {
		auto x = nested_pos.back();

		if (x == nullptr) {
			*err = -3;
			return "END_DATA";
		}

		//else if (0 == strncmp(select, "VALUE", 4)) {
		if (x->value != 0) {
			result = std::string(buffer + wiz::Utility::GetIdx(x->value), wiz::Utility::GetLength(x->value));
		}
		else {
			result = "";
		}
		//}
	}
	else if (query_id == 3) {
		//else if (0 == strncmp(query, "NEXT", 4)) {
		if (firstTime) {
			firstTime = false;
			return "NEXT SUCCESS";
		}

		if (nested_pos.size() == 1 && nested_pos.back() == nullptr) {
			*err = 4;
			return "NEXT END";
		}

		// DOWN		
		if (nested_pos.back() != nullptr
			&& 0 == nested_pos.back()->value) {
			nested_pos.push_back(nested_pos.back()->child);
		}
		// UP
		else if (nested_pos.back() == nullptr) {
			{
				nested_pos.pop_back();
				if (nested_pos.empty()) {
					*err = -5;
					return "NEXT FAIL";
				}
			}

			if (nested_pos.back() != nullptr) {
				nested_pos.back() = nested_pos.back()->next;
				*err = 1;
			}
			else {
				*err = 2;
			}
		}
		// RIGHT
		else {
			nested_pos.back() = nested_pos.back()->next;
		}
		return "NEXT SUCCESS";
	}
	else if (query_id == 4) {
		//else if (0 == strncmp(query, "DEL_", 4)) {
		global = wiz::Node();
		nested_pos = std::vector<wiz::Node*>();
		if (buffer) {
			delete[] buffer;
		}
		if (pool.empty() == false) {
			for (auto& x : pool) {
				x.Clear();
			}
			pool = std::vector<wiz::MemoryPool>();
		}

		return "DEL_ SUCCESS";
	}


	return result.c_str();
}

