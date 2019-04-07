//#include "SoftwareSerial.h"
#include "SerialCommand.h"
#include "SoftwareSerial.h"
//#include "MPU6050.h"

#define LED_PIN 13
#define PRESCALER 8
#define CMP 999

unsigned long deltaTime = 0;
double interuptPeriod = 0;
bool ledState = false;
bool printing = false;
bool interruptPrinting = false;
//MPU6050 mpu;




SerialCommand sCmd;
bool reset = true;




unsigned long lastRevolTime = 0;
unsigned long revolSpeed = 0;
unsigned long zeroSpeedThreshold = 1000;
unsigned long zeroSpeedTime = 0;

void rotHandler();
void speedHandler();


void setup()
{

  printing = false;
  interruptPrinting = false;
  interuptPeriod = 1. / (16000000. / ((double)PRESCALER * ((double)CMP + 1.)));
  pinMode(LED_PIN, OUTPUT);
  pinMode(2,INPUT_PULLUP);
  
  //mpu.ConnectMPU();
  //mpu.CalibrateMPU();
  SetUpTimerInterrupt();


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
	//mpu.UpdateMPU(deltaTime * interuptPeriod);
  deltaTime = 0;

  //Serial.println(analogRead(A0));

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

 if(digitalRead(12) == 1)
 
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
  Serial.flush  ();
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


void SetUpTimerInterrupt()
{
  cli();//stop interrupts

      //set timer1 interrupt at 1Hz
  TCCR1A = 0;// set entire TCCR1A register to 0
  TCCR1B = 0;// same for TCCR1B
  TCNT1 = 0;//initialize counter value to 0
        // set compare match register for 1hz increments
  OCR1A = CMP;///15624;// = (16*10^6) / (1*1024) - 1 (must be <65536)
        // turn on CTC mode
  TCCR1B |= (1 << WGM12);

  TCCR1B |= (1 << CS01);
  // enable timer compare interrupt
  TIMSK1 |= (1 << OCIE1A);

  sei();//allow interrupts


}

ISR(TIMER1_COMPA_vect)
{
  deltaTime += 1;
  interruptPrinting = true;
  ledState = !ledState;
  digitalWrite(13, ledState);
}
