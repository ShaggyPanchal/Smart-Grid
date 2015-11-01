#include <ESP8266WiFi.h>
#include <EasyTransfer.h>
#include <ESP8266WebServer.h>
#include <WiFiClient.h> 
/* Arguments sizes and Timeout constants */
#define AutoReset_Timeout    500        // If no device connects for this milliseconds, ESP will reset 'Arguments'
#define Argument_Size        6          // Number of arguments to be fetch and store in global 'Arguments'
#define SensorBuffer_Size    100        // Max number of characters to be sent back to remote device
ESP8266WebServer MyServer(80);
WiFiClient MyClient;

const char *SG_SsID = "SG_395002";    // SsID should not be more than 15 chars
const char *SG_Password = "12345678";

const char *AP_SsID = "CLSYS";    // SsID should not be more than 15 chars
const char *AP_Password = "abcd1234";

EasyTransfer ETin, ETout;

struct SEND_DATA_STRUCTURE
{
  unsigned long Arguments[Argument_Size];
};

/* 'RECEIVE_DATA_STRUCTURE' MUST BE IDENTICAL ON GATEWAY SIDE (WITH NAME OF 'SEND_DATA_STRUCTURE') */
struct RECEIVE_DATA_STRUCTURE
{
  char ReplyBuffer_100[SensorBuffer_Size];
  byte MODE;
};

SEND_DATA_STRUCTURE OutgoingData;
RECEIVE_DATA_STRUCTURE IncomingData;


uint8_t C1;
unsigned long LastAccessTime;  // To reset 'Arguments' if no connection was made in last AutoReset_Timeout milliseconds.



/* Handles index or root requests */
void HandleRoot()
{
  MyServer.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate", false);
  MyServer.sendHeader("Pragma", "no-cache", false);
  MyServer.sendHeader("Expires", "1000", false);
  MyServer.send(200, "text/plain", "https://github.com/AnuragVasanwala/Smart-Grid/");
}

/* Handles maneuver requests */
void HandleManeuver()
{
  /* Parse arguments and store it to Arguments byte array */
  for (C1 = 0; (C1 < Argument_Size) && (C1 < MyServer.args()); C1++)
  {
    /* Convert string argument value to equivalent byte */
    OutgoingData.Arguments[C1] = (unsigned long)(MyServer.arg(C1).toInt());
  }

  /* Prepare html headers to instruct remote device to not to cache page */
  MyServer.sendHeader("Cache-Control", "no-cache, no-store, must-revalidate", false);
  MyServer.sendHeader("Pragma", "no-cache", false);
  MyServer.sendHeader("Expires", "0", false);

  /* Send data to remote device */
  MyServer.send(200, "text/plain", IncomingData.ReplyBuffer_100);

  LastAccessTime = millis();
}

void setup()
{
  /* Wait for a one and half second to make ESP stable */
  delay(1500);

  Serial.begin(1024);

  WiFi.mode(WIFI_STA);
  WiFi.begin(AP_SsID, AP_Password);

  /////////////////////////////////////////////////////////////////////
  // WiFi.mode(WIFI_AP_STA);
  // WiFi.softAP(SG_SsID, SG_Password);


  MyServer.on("/DeviceInfo", HandleRoot);

  MyServer.on("/DeviceController", HandleManeuver);

  MyServer.begin();


  ETin.begin(details(IncomingData), &Serial);
  ETout.begin(details(OutgoingData), &Serial);
}

IPAddress server(192, 168, 1, 103);  // TCPlsr

void loop()
{
  MyServer.handleClient();

  if (ETin.receiveData())
  {
    if (IncomingData.MODE == 1)
    {
      if (MyClient.connect(server, 8080))
      {
        MyClient.print(IncomingData.ReplyBuffer_100);

        while (MyClient.available()) {
          String line = MyClient.readStringUntil('\r');
        }
      }
    }

    ETout.sendData();
  }
  delay(500);
}