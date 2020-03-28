#pragma once

enum Query {
	LOAD, GET_NAME, GET_VALUE, NEXT, CHILD, PARENT, DEL
};

const char* fnClauParserLib(Query query_id, const char* query_data, int* err);