void main() {
    // // Declaração de variáveis com tipos corretos
    // int idade = 25;
    // float altura = 1.75;
    // char letra = 'A';
    // string nome = "Carlos";

    // Impressão dos valores
    printf("Idade: %d\n", idade);
    printf("Altura: %f\n", altura);
    printf("Letra: %c\n", letra);
    printf("Nome: %s\n", nome);

    // Testando atribuições inválidas (devem gerar erro)
    idade = "vinte e cinco";  // Erro: String atribuída a int
    altura = "1.80";         // Erro: String atribuída a float
    letra = "AB";            // Erro: Char pode armazenar apenas um caractere
    nome = 30; // Erro: Inteiro atribuído a string
    
    // Impressão dos valores
    printf("Idade: %d\n", idade);
    printf("Altura: %f\n", altura);
    printf("Letra: %c\n", letra);
    printf("Nome: %s\n", nome);

    // Testando operações inválidas
    int resultado = idade + nome;  // Erro: Operação inválida entre int e string
    float soma = altura + letra;   // Erro: Operação inválida entre float e char
}
