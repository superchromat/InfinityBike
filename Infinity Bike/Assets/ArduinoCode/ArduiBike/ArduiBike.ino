#include "SoftwareSerial.h"
//#include "SerialCommand.h"

SerialCommand sCmd;
bool isOnROT = false;
bool isOnSPEED = false;
bool reset = true;


unsigned long lastRevolTime =  0; 
//unsigned long timeROT = 0;
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
	sCmd.addCommand("READY", areYouReady);

	pinMode(11, OUTPUT);
	digitalWrite(11, LOW);
}

void loop()
{	
	if (Serial.available() > 0)
	{
		isOnROT = !isOnROT;
		digitalWrite(11, isOnROT);
	}

	while(Serial.available() > 0)
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
  unsigned long sinceLastRevol =  millis()-lastRevolTime;
  if (sinceLastRevol < zeroSpeedThreshold) {
    Serial.println(revolSpeed);
  }
  else {
    Serial.println(0); 
  }
}//

void allHandler()
{

	rotHandler();
	speedHandler();
}

void areYouReady()
{	
	Serial.println("READY");
	digitalWrite(11, HIGH);

}	

void rpm_fun() {
  unsigned long revolTime = millis(); 
  unsigned long deltaTime = revolTime-lastRevolTime;
  
    
  
  revolSpeed = 20000/deltaTime; 
  lastRevolTime = revolTime;
  
}



