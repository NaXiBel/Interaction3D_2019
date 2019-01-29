#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <map>
#include <sstream>
#include <thread>
#include <vector>
#define BUFLEN 4096
#ifdef _WIN32
	#include <winsock2.h>
	#include "stdafx.h"
	#pragma comment(lib, "ws2_32.lib")
	typedef int socklen_t;
#else
 	#include <arpa/inet.h>
	#include <netinet/in.h>
	#include <sys/socket.h>
	#include <sys/types.h>
	#include <unistd.h>
	typedef int SOCKET;
    typedef struct sockaddr_in SOCKADDR_IN;
    typedef struct sockaddr SOCKADDR;
    #define INVALID_SOCKET -1
    #define SOCKET_ERROR -1
    #define closesocket(s) close(s)
#endif

typedef struct s_BSpline {
	int tailleTab;
	std::vector<float> tabX;
	std::vector<float> tabY;
	std::vector<float> tabZ;
} BSpline;

class Server {
	private :
		int port;
		static unsigned int nbUser;
		static unsigned int tokenId;
		socklen_t m_csinsize;
		#ifdef __WIN32__
			WSADATA WSAData;
		#endif
		SOCKET serverSocket;
		SOCKADDR_IN serverInfo;
		static std::map<unsigned int, SOCKET> sessions;
		static BSpline spline;
		static std::vector<float> tabUserX;
		static std::vector<float> tabUserY;
		static std::vector<float> tabUserZ;

	public :
		Server(const int & _port);
		static void sendAll(std::string buffer);
		static void listenUser(unsigned int _id);
		static void disconnectUser(unsigned int _id);
		static void sendUser(unsigned int _id, std::string buffer);
		static void releaseToken(unsigned int _id);
		static std::string getSummary(void);
		static void parseArgs(std::string _s, std::vector<std::string> &_v);
		static void parseTab(std::string _s, std::vector<std::string> &_v);
};
