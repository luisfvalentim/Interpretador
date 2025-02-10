void printf(string format, ...);
void scanf(string format, ...);
void puts(string text);
void gets(string &input);

void testPrintf() {
    printf("Número inteiro: %d\n", 42);
    printf("Número float: %f\n", 3.1415);
    printf("Caractere: %c\n", 'A');
    printf("Texto: %s\n", "Olá, Mundo!");
}

void testPuts() {
    puts("Esta é uma string.");
    puts("Outra linha de texto.");
}

void testScanf() {
    int idade;
    float altura;
    char inicial;
    string nome;

    printf("Digite sua idade: ");
    scanf("%d", idade);
    printf("Digite sua altura: ");
    scanf("%f", altura);
    printf("Digite sua inicial: ");
    scanf("%c", inicial);
    printf("Digite seu nome: ");
    gets(nome);

    printf("Nome: %s, Idade: %d, Altura: %f, Inicial: %c\n", nome, idade, altura, inicial);
}

void testGets() {
    string frase;
    puts("Digite uma frase:");
    gets(frase);
    printf("Você digitou: %s\n", frase);
}

void main() {
    puts("🔹 Testando printf:");
    testPrintf();

    puts("\n🔹 Testando puts:");
    testPuts();

    puts("\n🔹 Testando scanf:");
    testScanf();

    puts("\n🔹 Testando gets:");
    testGets();
}
