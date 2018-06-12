//#include "SoftwareSerial.h"
#include "SerialCommand.h"
#include "SoftwareSerial.h"

SerialCommand sCmd;
bool reset = true;


unsigned long lastRevolTime = 0;
unsigned long revolSpeed = 0;
unsigned long zeroSpeedThreshold = 750;
unsigned long zeroSpeedTime = 0;

void rotHandler();
void speedHandler();


void setup()
{
	zeroSpeedTime = millis();
	Serial.begin(9600);
	attachInterrupt(0, rpm_fun, RISING);

	sCmd.addCommand("ROT", rotHandler);
	sCmd.addCommand("SPEED", speedHandler);
	sCmd.addCommand("ALL", allHandler);
	sCmd.addCommand("READY", StartCommunication);
	sCmd.addCommand("DONE", StopCommunication);

	pinMode(11, OUTPUT);
	pinMode(12, INPUT);
	digitalWrite(LED_BUILTIN, LOW);
}



void loop()
{	
	
	while (Serial.available() > 0)
	{
    Serial.flush  ();
		sCmd.readSerial();
	}

	unsigned long currMillis = millis();
	if ((currMillis - zeroSpeedTime) > zeroSpeedThreshold)
	{	
		zeroSpeedTime = currMillis;
		revolSpeed >>= 1;
	}	

  if(  digitalRead(12) == true)
  {
    revolSpeed = 80;  
  }


}

void rotHandler()
{
	Serial.println(analogRead(A0));
  Serial.flush  ();
}
void speedHandler()
{
	Serial.println(revolSpeed);
  Serial.flush  ();
}

void allHandler()
{	
	rotHandler();
	speedHandler();
}	

void StartCommunication()
{	
  
	Serial.println("READY");
 // Serial.flush  ();
  digitalWrite(11, HIGH);
}	

void StopCommunication()
{ 
	digitalWrite(11, LOW);

}



void rpm_fun() {
	unsigned long revolTime = millis();
	unsigned long deltaTime = revolTime - lastRevolTime;
	
	revolSpeed = 20000 / deltaTime;
	lastRevolTime = revolTime;
	zeroSpeedTime = millis();

}
