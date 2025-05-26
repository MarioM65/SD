#define LED_VERDE 8
#define LED_VERMELHO 9
#define BUZZER 7

void setup() {
  Serial.begin(9600);
  pinMode(LED_VERDE, OUTPUT);
  pinMode(LED_VERMELHO, OUTPUT);
  pinMode(BUZZER, OUTPUT);
}

void loop() {
  if (Serial.available()) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();

    if (comando == "ABRIR") {
      digitalWrite(LED_VERDE, HIGH);
      delay(3000);
      digitalWrite(LED_VERDE, LOW);
    } 
    else if (comando == "ALERTA") {
      for (int i = 0; i < 5; i++) {
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
