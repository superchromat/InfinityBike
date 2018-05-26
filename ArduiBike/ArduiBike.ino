#include "SerialCommand.h"

SerialCommand sCmd;
bool reset = true;


unsigned long lastRevolTime = 0;
unsigned long revolSpeed = 0;
unsigned long zeroSpeedThreshold = 3000;
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

	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, LOW);
}



void loop()
{	
	
	while (Serial.available() > 0)
	{
		sCmd.readSerial();
	}

	unsigned long currMillis = millis();
	if ((currMillis - zeroSpeedTime) > zeroSpeedThreshold)
	{
		zeroSpeedTime = currMillis;
		revolSpeed >>= 1;
	}


}

void rotHandler()
{
	Serial.println(analogRead(A0));
}
void speedHandler()
{
	//Serial.println(revolSpeed);
	Serial.println(analogRead(A1));
}

void allHandler()
{	
	rotHandler();
	speedHandler();
}	

void StartCommunication()
{	
	Serial.end();
	digitalWrite(LED_BUILTIN, HIGH);

	Serial.begin(9600);
	Serial.println("READY");
}	

void StopCommunication()
{
	Serial.println("DONE");
	digitalWrite(LED_BUILTIN, LOW);
}



void rpm_fun() {
	unsigned long revolTime = millis();
	unsigned long deltaTime = revolTime - lastRevolTime;
	
	revolSpeed = 20000 / deltaTime;
	lastRevolTime = revolTime;
	zeroSpeedTime = millis();

}
