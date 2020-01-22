// DIRECT CONTROL

//// This program reads a text file containing a string coordinates and
//// sends the string in managable sections over a TCP socket  
//// Meant to be used on a windows OS sending to a linux OS
//
//// The server side of the TCP socket was created with help from here
//// https://gist.github.com/hostilefork/f7cae3dc33e7416f2dd25a402857b6c6
////
//
//tested
#undef UNICODE

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>

// need to link with ws2_32.lib
#pragma comment (lib, "ws2_32.lib")

#define DEFAULT_BUFLEN 100 // Max length of string to send over TCP socket
#define MAX_STRING 10000 // Maximum length of text file string 
 //The number of characters to save to determine if a new string has been opened
#define MAX_STRING_SAVE_PREV 50  
#define MAX_STRING_SAVE_PREV_PERSON 50  

 //Set socket-specific values
#define DEFAULT_PORT "8888"
// #define DEFAULT_IPv4 "192.168.10.181"
#define DEFAULT_IPv4 "192.168.1.3"

// For reading from file
using namespace std;
#include <string>
#include <iostream>
#include <fstream>
// Set up file for reading 
//string docPath_touch = "C:/Users/brandon/Desktop/Tanvas/DARPA_hapticsandtouchdetection/touch_locations.txt";
string docPath_touch = "C:/Users/numur/Downloads/touch_locations.txt";
string docPath_person = "C:\\Users\\numur\\Desktop\\Tanvas\\darpahaptics\\HapticDisplay\\person_position.txt";
int filefetch_wait = 500; // Milliseconds between trying to open file

// Global
char recvbuf_person_prev[MAX_STRING_SAVE_PREV_PERSON];

void printtofile(char recvbuf_person[DEFAULT_BUFLEN]) {

	//printf("New string: %s \n\n", recvbuf_person);
	bool personpos_equal; // boolean to record whether recvbuf_person==recvbuf_person_prev
	int i = 0;

	personpos_equal = (recvbuf_person_prev[i] == recvbuf_person[i]);
	while (personpos_equal && (i < MAX_STRING_SAVE_PREV_PERSON)) {
		personpos_equal = (personpos_equal && (recvbuf_person_prev[i] == recvbuf_person[i]));
		i++;
	}

	// If string is new, 
	if (personpos_equal || (recvbuf_person[0] == '?')) {
		// Don't do anything
	}
	else {
		// Write to file
		string person_location_string = "";
		for (i = 0; i < DEFAULT_BUFLEN; i++) {
			person_location_string = person_location_string + recvbuf_person[i];
			if (recvbuf_person[i] == '!') {
				break;
			}
		}
		//person_location_string.copy(recvbuf_person, endhere + 1);

		int flag2 = 0; // For determining if the file has been opened properly
		fstream file_personlocation;
		do {
			file_personlocation.open(docPath_person);
			// Check is file has been opened properly, if not wait 
			// (important because other program is opening and writing to this file)
			if (file_personlocation.is_open())
			{
				flag2 = 1;
				//printf("File opened \n");
			}
			else {
				Sleep(filefetch_wait);
				printf("failed to open \n");
				flag2 = 1;
			}
		} while (flag2 == 0);
		file_personlocation << person_location_string;
		file_personlocation.close();
		cout << "New person position" << person_location_string << "\n";

		// Save as new previous string
		for (i = 0; i < MAX_STRING_SAVE_PREV_PERSON; i++) {
			recvbuf_person_prev[i] = recvbuf_person[i];
		}
	}
}

int __cdecl main(void)
{
	// Variables

		// Set up file for reading 
	ifstream file_touchlocations;
	string string_of_coordinates; // Variable to read from file
	string string_of_coordinates_prev; // Previous read to check to see if they are the same
	bool strings_equal; // boolean to record whether string_of_coordinates==string_of_coordinates_prev
	int len_string_of_coordinates;

	// Initialize character arrays for handling and sending strings
	char recvbuf[MAX_STRING];
	char recvbuf_person[DEFAULT_BUFLEN];
	char recvbuf_prev[MAX_STRING_SAVE_PREV];
	char recvbuf_section[DEFAULT_BUFLEN];
	int recvbuflen = DEFAULT_BUFLEN;

	// Setting up Socket
		// Setting up TCP socket
	WSADATA wsaData;
	// Variables for TCP socket
	int iSendResult, iResult;

	SOCKET ListenSocket = INVALID_SOCKET;
	SOCKET ClientSocket = INVALID_SOCKET;

	struct addrinfo* result = NULL;
	struct addrinfo hints;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed with error: %d\n", iResult);
		return 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;

	// Resolve the server address and port
	iResult = getaddrinfo(DEFAULT_IPv4, DEFAULT_PORT, &hints, &result);
	printf("%d\n", iResult);
	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		return 1;
	}

	// Create a SOCKET for connecting to server
	ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (ListenSocket == INVALID_SOCKET) {
		printf("socket failed with error: %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		WSACleanup();
		return 1;
	}


	// Setup the TCP listening socket
	iResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) {
		printf("bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	freeaddrinfo(result);
	//printf("%c %d %d", result->ai_addr->sa_data[5], result->ai_addrlen, result->ai_protocol);


	iResult = listen(ListenSocket, SOMAXCONN);
	if (iResult == SOCKET_ERROR) {
		printf("listen failed with error: %d\n", WSAGetLastError());
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	// Accept a client socket
	ClientSocket = accept(ListenSocket, NULL, NULL);
	printf("Connected to Linux Client \n");
	if (ClientSocket == INVALID_SOCKET) {
		printf("accept failed with error: %d\n", WSAGetLastError());
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	// No longer need server socket
	closesocket(ListenSocket);


	// Test Connection
	printf("Testing connection \n");
	int flag = 0;
	// Receive and send test message
	do {

		iResult = recv(ClientSocket, recvbuf, recvbuflen, 0);
		if (iResult > 0) {
			printf("Bytes received: %d\n", iResult);

			// Echo the buffer back to the sender
			iSendResult = send(ClientSocket, recvbuf, iResult, 0);
			if (iSendResult == SOCKET_ERROR) {
				printf("send failed with error: %d\n", WSAGetLastError());
				closesocket(ClientSocket);
				WSACleanup();
				return 1;
			}
			printf("Bytes sent: %d\n", iSendResult);
			flag = 1;
		}

	} while (flag == 0);
	printf("Test Completed\n");

	// Main loop
		// Variables for seperating and sending string in sections
	int flag2;
	int i, i_temp, i_start, i_end = 0, comma_index = 0, end_loop_here;
	bool endhere = false;
	int recvbuf_1len;
	bool done_message_sent = true;
	// Send string messages to linux machine
	flag = 0; // For while loop
	do {
		// Read file
		flag2 = 0; // For determining if the file has been opened properly
		do {
			file_touchlocations.open(docPath_touch);
			// Check is file has been opened properly, if not wait 
			// (important because other program is opening and writing to this file)
			if (file_touchlocations.is_open())
			{
				flag2 = 1;
				//printf("File opened \n");
			}
			else {
				Sleep(filefetch_wait);
				printf("failed to open \n");
			}
		} while (flag2 == 0);
		getline(file_touchlocations, string_of_coordinates);
		file_touchlocations.close();
		// Reformatting for printing output to console
		//LPCSTR sw = string_of_coordinates.c_str();
		//OutputDebugString(sw);
		//OutputDebugStringW(L"\n");

		// Format to char for 
		string_of_coordinates.copy(recvbuf, string_of_coordinates.size() + 1);
		recvbuf[string_of_coordinates.size()] = '\0'; // terminate char
		recvbuflen = string_of_coordinates.size();

		// Check is string is equal to previous
		i = 0;
		if (recvbuflen == 0) {
			strings_equal = 1;
			printf("recvbuflen is 0 \n");
		}
		else {
			strings_equal = (recvbuf_prev[i] == recvbuf[i]);
			while (strings_equal && (i < MAX_STRING_SAVE_PREV)) {
				strings_equal = (strings_equal && (recvbuf_prev[i] == recvbuf[i]));
				i++;
				//printf("%d \n ", strings_equal);
			}
		}

		// If string is new, 
		if (strings_equal) {
			// Don't do anything
		}
		else {
			printf("New string \n");
			//printf("%d\n", recvbuflen);

			// Save as new previous string
			for (i = 0; i < MAX_STRING_SAVE_PREV; i++) {
				recvbuf_prev[i] = recvbuf[i];
			}

			if ((recvbuflen + 1) > DEFAULT_BUFLEN) {
				printf("Error: String is too large to send.\n");
			}
			else {
				// Send string to client
				iSendResult = send(ClientSocket, recvbuf, (recvbuflen + 1), 0);
				if (iSendResult == SOCKET_ERROR) {
					printf("send failed with error: %d\n", WSAGetLastError());
					closesocket(ClientSocket);
					WSACleanup();
					return 1;
				}
				// Get reply
				iResult = recv(ClientSocket, recvbuf_person, DEFAULT_BUFLEN, 0);
				if (iResult <= 0) {
					flag = 1;
				}
				printf("reply recieved \n");
				printtofile(recvbuf_person);
			}
		}

		iSendResult = send(ClientSocket, "Done", 5, 0);
		if (iSendResult == SOCKET_ERROR) {
			printf("send failed with error: %d\n", WSAGetLastError());
			closesocket(ClientSocket);
			WSACleanup();
			return 1;
		}

		//printf("Bytes sent: %d\n", iSendResult);
		// Get reply
		//printf("recieving \n");
		iResult = recv(ClientSocket, recvbuf_person, DEFAULT_BUFLEN, 0);
		if (iResult <= 0) {
			flag = 1;
		}
		//printf("%s \n", recvbuf_person);
		printtofile(recvbuf_person);
		//printf("reply to done message recieved \n");
		//done_message_sent = 1;

	} while (flag == 0);

	printf("Closing Socket\n");
	closesocket(ClientSocket);
	WSACleanup();

	// shutdown the connection since we're done
	iResult = shutdown(ClientSocket, SD_SEND);
	if (iResult == SOCKET_ERROR) {
		printf("shutdown failed with error: %d\n", WSAGetLastError());
		closesocket(ClientSocket);
		WSACleanup();
		return 1;
	}

	// cleanup
	closesocket(ClientSocket);
	WSACleanup();

	return 0;
}