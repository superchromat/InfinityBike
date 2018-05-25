
#include "SoftwareSerial.h"
#include "SerialCommand.h"


SerialCommand sCmd;
bool reset = true;


unsigned long timeROT = 0;
unsigned long timeSPEED = 0;

unsigned long lastRevolTime = 0;
unsigned long revolSpeed = 0;
unsigned long zeroSpeedThreshold = 3000;

void rotHandler();
void speedHandler();


void setup()
{
	Serial.begin(9600);
	attachInterrupt(0, rpm_fun, RISING);


	sCmd.addCommand("ROT", rotHandler);
	sCmd.addCommand("SPEED", speedHandler);
	sCmd.addCommand("ALL", allHandler);
	sCmd.addCommand("READY", StartCommunication);
	sCmd.addCommand("DONE", StopCommunication);

	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, LOW);
	
}

void loop()
{	
	while (Serial.available() > 0)
	{
		sCmd.readSerial();
	}
	

}

void rotHandler()
{
	Serial.println(analogRead(A0));
}
void speedHandler()
{
	Serial.println(analogRead(A1));
}

void allHandler()
{

	Serial.println(analogRead(A0));
	Serial.println(analogRead(A1));
}

void StartCommunication()
{	
	Serial.flush();
	delay(1);

	Serial.println("READY");
	digitalWrite(LED_BUILTIN, HIGH);
}	
void StopCommunication()
{

	Serial.flush();
	delay(1);

	Serial.println("DONE");
	digitalWrite(LED_BUILTIN, LOW);
}



void rpm_fun() {
	unsigned long revolTime = millis();
	unsigned long deltaTime = revolTime - lastRevolTime;



	revolSpeed = 20000 / deltaTime;
	lastRevolTime = revolTime;

}
