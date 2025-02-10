void main() {
    int idade = 25;
    float altura = 1.75;
    char letra = 'A';
    string nome = "Carlos";

    // ❌ Testando atribuições inválidas (Devem gerar erro!)
    idade = "vinte e cinco";  // Erro: String atribuída a int
    altura = "1.80";         // Erro: String atribuída a float
    letra = "AB";            // Erro: Char pode armazenar apenas um caractere
    nome = 30;               // Erro: Inteiro atribuído a string
}
