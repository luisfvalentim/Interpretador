void main() {
    // Declaração de variáveis do tipo string e char
    string nome = "Carlos";
    char inicial = 'C';

    // Impressão das variáveis
    printf("Nome: %s\n", nome);
    printf("Inicial: %c\n", inicial);

    // Modificação da string
    nome = "Ana";
    printf("Novo nome: %s\n", nome);

    // Testando atribuição inválida (deve gerar erro)
    char letra = "AB"; //Erro: Um char pode armazenar apenas um caractere

    // Concatenação de strings
    string sobrenome = "Silva";
    string nomeCompleto = nome + " " + sobrenome;
    printf("Nome completo: %s\n", nomeCompleto);
}

