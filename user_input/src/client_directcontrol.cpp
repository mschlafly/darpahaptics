
// This ros node recieved strings with coordinate through a TCP socket and
// publishes them to ros topic /input using custom message user_input/input_array

// https://www.binarytides.com/socket-programming-c-linux-tutorial/ used for making TCP socket

// For setting up in ros - make sure to source!
// source devel/setup.bash
// echo $ROS_PACKAGE_PATH

// For compiling not in ros
//$ gcc client.c -o client
//$ ./client

#define PORT 8888
//#define IPv4 "192.168.1.3"
#define IPv4 "192.168.1.3"

#define MAXLEN 100

#include<stdio.h>
#include<string.h>	//strlen
#include<sys/socket.h>
#include<arpa/inet.h>	//inet_addr
#include<unistd.h>
#include<string.h>


#include "ros/ros.h"
#include "user_input/input_location.h"
#include "user_input/person_position.h"
#include "signal.h"

// Define varables for looping
char previous_message[MAXLEN];
char message_from_server[MAXLEN];
int flag = 0;
int count = 0;

// Define variables used for reformatting strings into two arrays
int flag_exclaim = 0;
int flag_comma = 0;
char coordinate;
char temp_char[6];
int temp_number;
int i, i_temp, i_char;

// Set values to 0 to prepare for the start of input
int num_values_in_message_x=0;
int num_values_in_message_y=0;
int comma_index = 0;
int done = 0;

using namespace std;
// Define global variables for postition of person_location
string xloc = "";
string yloc = "";
string th = "";
string location_string = "?";
char location_chararray[MAXLEN];

int gridsize_unity = 30;

void chatterCallback(const user_input::person_position::ConstPtr& msg)
{
  printf("Updating the person's position\n");
  xloc = to_string(msg->xpos);
  yloc = to_string(msg->ypos);
  th = to_string(msg->theta);
  //yloc = msg->ypos;
  //th = msg->theta;
  //location_string = "";
  location_string = xloc + "," + yloc + "," + th + "," + "!";
  //printf("location_string %s \n",location_string);
}

int main(int argc , char *argv[])
{
// Set up ros node
  ros::init(argc, argv, "client_directcontrol");
  ros::NodeHandle n;
  ros::Rate rate(10.0);

	// Set up publisher
  ros::Publisher pub = n.advertise<user_input::input_location>("input_directcontrol", 1000);
  user_input::input_location send_loc;
  // Send empty array so that the topic can be found even if no inputs have been made
  send_loc.xinput.clear();
  send_loc.yinput.clear();
  send_loc.datalen = 0;
  pub.publish(send_loc);
  ros::spinOnce();

  // Set up Subscriber
  ros::Subscriber sub = n.subscribe("person_location", 1000, chatterCallback);

    // Uncomment to publish a "person_location" message
    // ros::Publisher pub2 = n.advertise<user_input::person_position>("person_location", 1000);
    // user_input::person_position trythis;
    // trythis.xpos = 4;
    // trythis.ypos = 4;
    // trythis.theta = 4;
    // pub2.publish(trythis);
    // printf("Published \n");
    // ros::spinOnce();

// Setting up TCP socket
	int socket_desc;
	struct sockaddr_in server;
	char *message, server_reply[MAXLEN];

	//Create socket
	socket_desc = socket(AF_INET , SOCK_STREAM , 0);
	if (socket_desc == -1)
	{
		printf("Could not create socket");
	}

	server.sin_addr.s_addr = inet_addr(IPv4);
	server.sin_family = AF_INET;
	server.sin_port = htons(PORT);

	//Connect to remote server
	if (connect(socket_desc , (struct sockaddr *)&server , sizeof(server)) < 0)
	{
		puts("connect error");
		return 1;
	}
	puts("Connected to Windows Server");

// Testing connection
	//Send data for test
	message = "Testing connection";
	if( send(socket_desc , message , strlen(message) , 0) < 0)
	{
		puts("Send failed");
		return 1;
	}
	puts("Data Send");
	//Receive a reply from the server
	if( recv(socket_desc, server_reply , MAXLEN , 0) < 0)
	{
		puts("recv failed");
	}
	puts("Reply received");
	puts("Test completed");

  // int count = 0;
  while (ros::ok())
  {
    // Keep track of iterations
	  // count=count+1;

    //puts("New loop");
    // Recieve message from server
		if( recv(socket_desc, message_from_server , MAXLEN , 0) < 0)
		{
			puts("recv failed");
		}

    // Print message
		puts(message_from_server);
		//printf("\n");

    if ((message_from_server[0]=='D') &&
        (message_from_server[1]=='o') &&
        (message_from_server[2]=='n') &&
        (message_from_server[3]=='e')) {
          // If the message is 'Done', don't do anything.
        } else {

        // Strings follow format "number of drone,x-location,y-location!"
        //printf("New string containing coordinates \n");
        flag_exclaim = 0; // flag for exclaimation mark
        flag_comma = 0; // flag for comma
        num_comma = 0; // keep track of the number of commas
        comma_index = 0; // The string starts with the first number so the imaginary comma is at location -1

        // Iterate through the characters in latest message from server
        for (i=0;i<MAXLEN;i++)
    		{
          if ((int)message_from_server[i]==33) {
            flag_exclaim = 1;
            printf("String ends \n");
          }
          if ((int)message_from_server[i]==44) {
            flag_comma = 1;
            flag_comma = flag_comma + 1;
          }
          // If reached the end of a number and the string has not been terminated, store value
          if (flag_exclaim==0 && flag_comma ==1) {
            // Store characters between commas into temporary string and convert to number
            i_temp=0;
            if (comma_index!=0)
              comma_index=comma_index+1;
            for (i_char=comma_index; i_char<i; i_char++) {
              // it cuts off the first number of the first entree
              temp_char[i_temp]=message_from_server[i_char];
              //printf("%d \n",(int)message_from_server[i2]);
              i_temp++;
            }
            temp_char[i_temp]='.'; // atoi recognized a '.' as indicating the end of a number
            temp_number = atoi(temp_char); // from std package

            // Store number into send_loc message
            if (num_comma == 1) {
              send_loc.dronenumber = temp_number
            } else if (num_comma == 2) {
              if (temp_number<0){
                send_loc.xpos = 0;
              } else if (temp_number>29) {
                send_loc.xpos = 29;
              } else {
                send_loc.xpos = temp_number
              }
            } else if (num_comma == 3) {
              if (temp_number<0){
                send_loc.ypos = 0;
              } else if (temp_number>29) {
                send_loc.ypos = 29;
              } else {
                send_loc.ypos = temp_number
              }
            }

            comma_index=i; // Store latest comma location for use later
            flag_comma = 0; // reset flag
          }
    }
      pub.publish(send_loc);
      printf("Input coordinates published \n");
      ros::spinOnce(); // Standard line to allow services
      sleep(1);

      // Publish the empty array
      send_loc.dronenumber = 0;
      send_loc.xpos = 0;
      send_loc.ypos = 0;
      pub.publish(send_loc);
      printf("Empty array published \n");
      ros::spinOnce(); // Standard line to allow services
      sleep(1);
		}

    // Reply to server (Later I imaging sending the location of the person here
        // and editing the code to continue looping at a set rate)
    strcpy(location_chararray,location_string.c_str());
    int stringlen = location_string.length();
    // if ((stringlen==0)  || (stringlen==1))
    // {
    //   stringlen=5;
    //   printf("string length is zero");
    // }
		//puts("Sending reply");
    //printf("location_chararray len %d \n",strlen(location_chararray));
    if ((strlen(location_chararray)==0) || (strlen(location_chararray)==1))
    {
       location_chararray[0]='?';
       location_chararray[1]='n';
       location_chararray[2]='o';
       stringlen=5;
       //printf("location set");
    }
		//message = "Recieved";
		if( send(socket_desc , location_chararray , stringlen , 0) < 0)
		{
			puts("Send failed");
			return 1;
		}
		//puts("Replyed!");
    ros::spinOnce();
    rate.sleep();
    //sleep(1);

	} // Continue looping until ctrl c is pressed
	return 0;
}
