#include <SoftwareSerial.h>

#define LED_VERDE 8
#define LED_VERMELHO 9
#define BUZZER 7

SoftwareSerial lora(10, 11); // RX, TX (LoRa)

void setup() {
  Serial.begin(9600);
  lora.begin(9600);

  pinMode(LED_VERDE, OUTPUT);
  pinMode(LED_VERMELHO, OUTPUT);
  pinMode(BUZZER, OUTPUT);

  Serial.println("Escravo pronto.");
}

void loop() {
  if (lora.available()) {
    String comando = lora.readStringUntil('\n');
    comando.trim();

    Serial.println("Comando recebido: " + comando);

    if (comando == "ABRIR") {
      digitalWrite(LED_VERDE, HIGH);
      delay(3000);
      digitalWrite(LED_VERDE, LOW);
    } 
    else if (comando == "ALERTA") {
      for (int i = 0; i < 20; i++) {
        digitalWrite(LED_VERMELHO, HIGH);
        digitalWrite(BUZZER, HIGH);
        delay(300);
        digitalWrite(LED_VERMELHO, LOW);
        digitalWrite(BUZZER, LOW);
        delay(300);
      }
    }
  }
}
